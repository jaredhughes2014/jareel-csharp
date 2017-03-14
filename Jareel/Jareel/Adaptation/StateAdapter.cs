using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jareel
{
    /// <summary>
    /// Used to perform a state transition. This adapter is executed at the end
    /// of one controller's execution cycle.
    /// 
    /// It is recommended not to copy data directly from one persistent state into
    /// another. If data is needed directly from one state to another, use a
    /// processed state instead, as processed states are not stoed in serialized
    /// states
    /// </summary>
    /// <typeparam name="T">The initial state this is adapting from</typeparam>
    /// <typeparam name="V">This state this is adapting to</typeparam>
    internal class StateAdapter
    {
        #region Fields

        /// <summary>
        /// The controller which provides a state to modify the target
        /// </summary>
        private AbstractController _sourceController;

        /// <summary>
        /// The controller whose state is modified by the adapter
        /// </summary>
        private AbstractController _targetController;

        /// <summary>
        /// Callback executed to perform the adaptation
        /// </summary>
        private Action<State, State> _adaptationCallback;

        #endregion

        #region Setup

        /// <summary>
        /// Creates a new StateAdapter with a source controller, target controller, and adapter action
        /// </summary>
        /// <param name="source">Controller whose state is the source of adaptation</param>
        /// <param name="target">Controller whose state is the target of adaptation</param>
        /// <param name="adapter">Function used to perform the adaptation</param>
        public StateAdapter(AbstractController source, AbstractController target, Action<State, State> adapter)
        {
            _sourceController = source;
            _targetController = target;
            _adaptationCallback = adapter;
        }

        /// <summary>
        /// Extracts all state adapter functions from the given controller
        /// </summary>
        /// <param name="controller">The controller to extract state adapters from</param>
        /// <returns>All functions marked with a StateAdapterAttribute which accepts two state objects</returns>
        public static Action<State, State>[] ExtractStateAdapters(AbstractController controller)
        {
            return controller.GetType()
                             .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                             .Where(p => Attribute.IsDefined(p, typeof(StateAdapterAttribute)))
                             .Where(p => ValidateAdapterParameters(p))
                             .Select(p => (Action<State, State>)Delegate.CreateDelegate(typeof(Action<State, State>), controller, p))
                             .ToArray();
        }

        /// <summary>
        /// Validates that the given method info accepts two arguments which are subtypes of State
        /// </summary>
        /// <param name="adapter">The metho info of the method being used as a state adapter</param>
        /// <returns>True if the method is properly configured to be a listener</returns>
        private static bool ValidateAdapterParameters(MethodInfo adapter)
        {
            var parameters = adapter.GetParameters();

            return parameters.Length == 2 &&
                   parameters.Select(p => p.ParameterType)
                             .All(p => p.IsSubclassOf(typeof(State)));
        }

        #endregion

        #region Adaptation

        /// <summary>
        /// Performs a state adaptation
        /// </summary>
        public void Adapt()
        {
            _adaptationCallback(_sourceController.AbstractCloneState(), _targetController.AbstractState);
            _targetController.Dirty = true;
        }

        #endregion
    }
}
