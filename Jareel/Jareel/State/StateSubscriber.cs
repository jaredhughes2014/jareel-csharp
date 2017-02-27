using System;

namespace Jareel
{
    /// <summary>
    /// State subscribers are requested from master controllers
    /// to provide access to state data outside of a state controller.
    /// Note that the state data provided is a copy of the controlled state
    /// and changes made to the copy will not affect the original state
    /// </summary>
    public class AbstractStateSubscriber
    {
        #region Properties

        /// <summary>
        /// The top-level state type. This is not used by the user
        /// </summary>
        internal State AbstractState { get; set; }

        /// <summary>
        /// Unique identifier used to access/remove state subscribers
        /// </summary>
        internal string ID { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Creates a new AbstractStateSubscriber
        /// </summary>
        public AbstractStateSubscriber()
        {
            ID = Guid.NewGuid().ToString();
        }

        #endregion
    }

    /// <summary>
    /// Provides access to a state managed in a master controller. Each time a
    /// state controller finishes processing changes made to a state, this subscriber
    /// will receive a copy of the final state
    /// </summary>
    /// <typeparam name="T">The type of state accessed by this subscriber</typeparam>
    public class StateSubscriber<T> : AbstractStateSubscriber where T : State
    {
        /// <summary>
        /// The state this is subscribed to. This will always be a copy of the latest state
        /// </summary>
        public T State { get { return (T)AbstractState; } internal set { AbstractState = value; } }
    }
}
