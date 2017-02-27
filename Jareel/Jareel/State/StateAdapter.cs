using System;

namespace Jareel
{
    /// <summary>
    /// Base class for all state adapters. Used to define type-agnostic behaviors
    /// for the top-level system
    /// </summary>
    internal class AbstractStateAdapter
    {
        /// <summary>
        /// Performs the adaptation
        /// </summary>
        public virtual void Adapt() { }
    }

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
    internal class StateAdapter<T, V> : AbstractStateAdapter where T : State where V : State
    {
        #region Fields

        /// <summary>
        /// Controller which generates the state used for adaptation
        /// </summary>
        private StateController<T> _sourceController;

        /// <summary>
        /// Controller making use of the adapted state
        /// </summary>
        private StateController<V> _targetController;

        /// <summary>
        /// Callback executed to perform the adaptation
        /// </summary>
        private Action<T, V> _adaptationCallback;

        #endregion

        /// <summary>
        /// Creates a new StateAdapter with a source controller, target controller, and adapter action
        /// </summary>
        /// <param name="source">Controller whose state is the source of adaptation</param>
        /// <param name="target">Controller whose state is the target of adaptation</param>
        /// <param name="adapter">Function used to perform the adaptation</param>
        public StateAdapter(StateController<T> source, StateController<V> target, Action<T, V> adapter)
        {
            _sourceController = source;
            _targetController = target;
            _adaptationCallback = adapter;
        }

        /// <summary>
        /// Performs the adaptation
        /// </summary>
        public override void Adapt()
        {
            _adaptationCallback(_sourceController.CloneState(), _targetController.State);
            _targetController.Adapted = true;
        }
    }
}
