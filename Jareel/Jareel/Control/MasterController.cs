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
        private List<AbstractController> m_controllers;

        /// <summary>
        /// Stores all execution chains defined for this controller
        /// </summary>
        private List<ExecutionChain> m_chains;
        internal List<ExecutionChain> Chains { get { return m_chains; } }

        /// <summary>
        /// Manages the registration and execution of events
        /// </summary>
        private EventManager m_events;
        internal EventManager Events { get { return m_events; } }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the master controller
        /// </summary>
        public MasterController()
        {
            m_chains = new List<ExecutionChain>();
            m_events = new EventManager();
            m_controllers = new List<AbstractController>();

            UseControllers();
            VerifyStateUniqueness();
            BuildChains();
        }

        /// <summary>
        /// Verifies that there exists only one controller for each type of state
        /// in this controller. Throws an exception if this condition fails
        /// </summary>
        private void VerifyStateUniqueness()
        {
            var typeMap = new Dictionary<Type, int>();

            foreach (var controller in m_controllers) {
                Type type = controller.AbstractState.GetType();

                if (typeMap.ContainsKey(type)) {
                    typeMap[type] += 1;
                }
                else {
                    typeMap.Add(type, 1);
                }
            }

            ValidateTypeMap(typeMap);
        }

        /// <summary>
        /// Validates that the type map generated from the registered controller types
        /// are unique. Throws an exception if they are not.
        /// 
        /// Each state type should be used exactly once
        /// </summary>
        /// <param name="typeMap">Dictionary mapping state types to the number of controllers handling them</param>
        private void ValidateTypeMap(Dictionary<Type, int> typeMap)
        {
            StringBuilder err = new StringBuilder();
            err.Append("More the one controller exists for the following types:").Append('\n');
            bool errExists = false;

            foreach (string message in typeMap.Where(p => p.Value > 1)
                                              .Select(p => string.Format("{0}({1})", p.Key.Name, p.Value))) {
                err.Append(message).Append('\n');
                errExists = true;
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
            var packaged = m_controllers.ToDictionary(p => p.StateName,
                                                    q => (object)q.DataMap);
            return Json.Write(packaged);
        }

        /// <summary>
        /// Imports data from a previously exported state object string. All states in this
        /// system will be populated with data from the previously exported states
        /// </summary>
        /// <param name="exportString">JSON string previously exported from this system</param>
        public void ImportState(string exportString)
        {
            var data = (Dictionary<string, object>)Json.Read(exportString);

            foreach (var controller in m_controllers) {
                controller.ImportState((Dictionary<string, object>)data[controller.StateName]);
            }
        }

        #endregion

        #region Execution Chains

        /// <summary>
        /// Overload to call your Use() functions. Every controller type you plan
        /// on using must be used here
        /// </summary>
        protected abstract void UseControllers();

        /// <summary>
        /// Declares a state and controller type that will be used
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="C"></typeparam>
        protected void Use<S, C>() where S : State, new() where C : StateController<S>, new()
        {
            C controller = new C();
            controller.InitializeControllerData(Events, new S());

            m_controllers.Add(controller);
        }

        /// <summary>
        /// Extracts all state adapters from every used controller. Once all chains have
        /// been extracted, builds execution chains
        /// </summary>
        private void BuildChains()
        {
            var stateTypes = m_controllers.ToDictionary(p => p.AbstractState.GetType(), q => q);
            var adapters = m_controllers.ToDictionary(p => p, q => StateAdapter.ExtractStateAdapters(q));

            var chains = adapters.OrderBy(p => p.Value.Length)
                                 .Select(p => p.Key)
                                 .ToDictionary(p => p, q => new ExecutionChain(q));
            
            foreach (var kv in adapters) {
                foreach (var adapter in kv.Value) {
                    var parent = chains[stateTypes[adapter.GetParameters().First().ParameterType]];
                    parent.AddChild(chains[kv.Key], adapter);
                }
            }

            // Only chains that do not require adapters are used at the top
            m_chains = adapters.Where(p => p.Value.Length == 0)
                               .Select(p => chains[p.Key])
                               .ToList();
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
            foreach (var controller in m_controllers) {
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
            foreach (var controller in m_controllers) {
                controller.RemoveSubscriber(subscriber);
            }
        }

        #endregion
    }
}
