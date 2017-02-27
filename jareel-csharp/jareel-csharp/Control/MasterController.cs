using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace Jareel
{
    /// <summary>
    /// Top-level controller used to organize the flow of events in the application
    /// </summary>
    public abstract class MasterController
    {
        #region Fields

        /// <summary>
        /// Set of all controllers in this system. Each controller must be distinct (failure
        /// to do so will result in a runtime exception), so this will always be a distinct
        /// set of controllers
        /// </summary>
        private AbstractController[] Controllers
        {
            get
            {
                var controllers = new List<AbstractController>();
                foreach (var chain in Chains) {
                    RecursiveControllerExtraction(chain, ref controllers);
                }
                return controllers.ToArray();
            }
        }

        /// <summary>
        /// Recursive helper function for extracting all controllers from each execution chain. Because
        /// chains can have multiple children, it's necessary to recurse through each child and extract
        /// its children as well. This equates to a depth-first search in a B-tree
        /// </summary>
        /// <param name="chain">The chain to recursively add all children from</param>
        /// <param name="accum">Reference list used to store each child controller</param>
        private void RecursiveControllerExtraction(AbstractExecutionChain chain, ref List<AbstractController> accum)
        {
            accum.Add(chain.AbstractController);
            foreach (var child in chain.Children) {
                RecursiveControllerExtraction(child, ref accum);
            }
        }

        /// <summary>
        /// Stores all execution chains defined for this controller
        /// </summary>
        private List<AbstractExecutionChain> m_chains;
        internal List<AbstractExecutionChain> Chains { get { return m_chains; } }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the master controller
        /// </summary>
        public MasterController()
        {
            m_chains = new List<AbstractExecutionChain>();
            BuildChains();
            VerifyStateUniqueness();
        }

        /// <summary>
        /// Overload to build your execution chains
        /// </summary>
        protected abstract void BuildChains();

        /// <summary>
        /// Verifies that there exists only one controller for each type of state
        /// in this controller. Throws an exception if this condition fails
        /// </summary>
        private void VerifyStateUniqueness()
        {
            var dict = new Dictionary<Type, int>();

            foreach (var controller in Controllers) {
                Type type = controller.AbstractState.GetType();

                if (dict.ContainsKey(type)) {
                    dict[type] += 1;
                }
                else {
                    dict.Add(type, 1);
                }
            }

            StringBuilder err = new StringBuilder();
            err.Append("More the one controller exists for the following types:").Append('\n');
            bool errExists = false;

            foreach (var kv in dict) {
                if (kv.Value > 1) {
                    errExists = true;
                    err.Append(string.Format("{0}({1})", kv.Key.Name, kv.Value)).Append('\n');
                }
            }

            if (errExists) {
                throw new Exception(err.ToString());
            }
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Exports all states into a JSON object string. Each controller state
        /// is packaged into a separate object
        /// </summary>
        /// <returns>JSON object string containing all persistent state data</returns>
        public string ExportStates()
        {
            var packaged = Controllers.ToDictionary(p => p.StateName,
                                                    q => (object)q.DataMap);
            return JsonWriter.Write(packaged);
        }

        /// <summary>
        /// Imports data from a previously exported state object string. All states in this
        /// system will be populated with data from the previously exported states
        /// </summary>
        /// <param name="exportString">JSON string previously exported from this system</param>
        public void ImportState(string exportString)
        {
            var data = (Dictionary<string, object>)JsonReader.Read(exportString);

            foreach (var controller in Controllers) {
                controller.ImportState((Dictionary<string, object>)data[controller.StateName]);
            }
        }

        #endregion

        #region Execution Chains

        /// <summary>
        /// Creates a new execution chain which operates with a controller and state of the types
        /// specified
        /// </summary>
        /// <typeparam name="S">The state type. Required due to generic limitations with C#</typeparam>
        /// <typeparam name="C">The controller type. The controller must use state type S</typeparam>
        /// <returns>The executio>n chain created</returns>
        public ExecutionChain<S, C> StartChain<S, C>() 
            where S : State, new() where C : StateController<S>, new()
        {
            var chain = new ExecutionChain<S, C>();
            m_chains.Add(chain);
            return chain;
        }

        #endregion

        #region Subscription

        /// <summary>
        /// Spawns a state subscriber for state of type T. State types must be unique
        /// within a master controller, so this is guaranteed to subscribe to the correct
        /// state type
        /// </summary>
        /// <typeparam name="T">The state type</typeparam>
        /// <returns>Subscriber to the state of type T</returns>
        public StateSubscriber<T> SpawnSubscriber<T>() where T : State
        {
            foreach (var controller in Controllers) {
                if (controller.AbstractState.GetType() == typeof(T)) {
                    return (StateSubscriber<T>)controller.SpawnSubscriber();
                }
            }
            throw new ArgumentException(string.Format("No controllers for state of type {0}", typeof(T).Name));
        }

        /// <summary>
        /// Removes a subscriber from this controller. If you are no longer need
        /// a state subscriber you should always call this to reduce the number
        /// of state clones performed
        /// </summary>
        /// <param name="subscriber">The subscriber to unsubscribe</param>
        public void DisconnectSubscriber(AbstractStateSubscriber subscriber)
        {
            // This has no effect if the subscriber is not subscribed
            foreach (var controller in Controllers) {
                controller.RemoveSubscriber(subscriber);
            }
        }

        #endregion
    }
}
