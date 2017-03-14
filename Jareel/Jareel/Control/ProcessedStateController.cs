
namespace Jareel
{
    /// <summary>
    /// Extension of the standard system controller which allows a processed state to
    /// be generated. When building state driven systems, the idea is that states only
    /// contain data which can be imported exported to the system. However, sometimes
    /// a system must process pure data to make it into a usable for.
    /// 
    /// ProcessedStateControllers give this functionality by allowing the user to define
    /// a second state type. Unlike StateControllers, multiple processed state controllers
    /// are allowed to use the same processed state type.
    /// 
    /// This state is intended to be a by-product of the actual state. To enforce this, the
    /// processed state will not persist between updates. Each time the actual state is changed,
    /// a new processed state will be created and populated. The processed state can be viewed
    /// as the output of a function which accepts the standard state as an input
    /// </summary>
    /// <typeparam name="T">The standard state</typeparam>
    /// <typeparam name="P">The processed state</typeparam>
    public abstract class ProcessedStateController<T, P> : StateController<T> where T : State, new() where P : State, new()
    {
        /// <summary>
        /// The state generated as a function of the standard state
        /// </summary>
        protected P ProcessedState { get; private set; }

        /// <summary>
        /// Overridden to include the processed state update 
        /// </summary>
        internal override bool Update()
        {
            if (ProcessEvents()) {
                ProcessedState = new P();
                ProcessState(State, ProcessedState);

                OnStateChange();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Function called to generate the data in the processed state. The processed
        /// state has already been created at this point; this function is only designed
        /// to set data in the processed state after it has been created.
        /// 
        /// A new processed state is created each time the primary state is updated. This
        /// will be executed before OnUpdate so the processed state will be up to date
        /// each time OnUpdate is executed
        /// </summary>
        /// <param name="state">The primary state. This contains pure state data</param>
        /// <param name="processed">The processed state. This was just created</param>
        protected abstract void ProcessState(T state, P processed);
    }
}
