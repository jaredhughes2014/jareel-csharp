
using System.Collections.Generic;
using System.Linq;

namespace Jareel
{
    /// <summary>
    /// Base class for all system controllers.
    /// </summary>
    public abstract class AbstractController
    {
        #region Properties

        /// <summary>
        /// Type-agnostic container of the system's current state
        /// </summary>
        private State m_abstractState;
        internal State AbstractState
        {
            get { return m_abstractState; }
            set
            {
                m_abstractState = value;
                StateConverter = new StateConverter(value);
            }
        }

        /// <summary>
        /// Manages the storing and execution of events
        /// </summary>
        private EventManager m_events;
        public EventManager Events { get { return m_events; } }

        /// <summary>
        /// Used for state serialization and content checking
        /// </summary>
        private StateConverter StateConverter { get; set; }

        /// <summary>
        /// Used for event extraction and execution
        /// </summary>
        internal ListenerConverter Listeners { get; set; }

        /// <summary>
        /// All subscribers
        /// </summary>
        protected List<AbstractStateSubscriber> Subscribers { get; private set; }

        /// <summary>
        /// Set true when the state has changed and an update is needed
        /// </summary>
        internal bool Dirty { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Base constructor for all system controllers. This is used to
        /// store state data at a high level
        /// </summary>
        protected AbstractController()
        {
            Subscribers = new List<AbstractStateSubscriber>();
        }

        /// <summary>
        /// Initializes all controller data for the controller.
        /// </summary>
        /// <param name="events">Used to manage event execution and listening</param>
        /// <param name="state">The state used by this controller</param>
        internal void InitializeControllerData(EventManager events, State state)
        {
            m_events = events;
            AbstractState = state;
            Listeners = new ListenerConverter(this, m_events);
        }

        #endregion

        #region Serialization

        /// <summary>
        /// The export name of the state controlled by this controller
        /// </summary>
        internal string StateName { get { return StateConverter.ContainerName; } }

        /// <summary>
        /// Exported data contained in this controller's state
        /// </summary>
        internal Dictionary<string, object> DataMap { get { return StateConverter.DataMap; } }

        /// <summary>
        /// Imports deserialized JSON data into this state controller
        /// </summary>
        /// <param name="data">Dictionary representing a deserialized JSON object</param>
        internal void ImportState(Dictionary<string, object> data)
        {
            StateConverter.PopulateState(data);
            Dirty = true;
        }

        #endregion

        #region State Updates

        /// <summary>
        /// Executes this system's state update. This should only be called by the master controller
        /// Returns true if a state change results from this update
        /// </summary>
        internal virtual bool Update()
        {
            if (ProcessEvents() || Dirty) {
                Dirty = false;
                UpdateSubscribers();
                OnStateChange();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Executed to perform all pending updates to the state. Returns true
        /// if any events were processed
        /// </summary>
        /// <returns>True if at least one event was executed</returns>
        protected bool ProcessEvents()
        {
            return Listeners.ExecuteAll();
        }

        /// <summary>
        /// This is a place for the user to perform their update logic
        /// </summary>
        protected virtual void OnStateChange() { }

        #endregion

        #region Cloning

        /// <summary>
        /// Creates a copy of the state contained in this controller.
        /// </summary>
        /// <returns>A clone of the state controlled by this controller</returns>
        internal abstract State AbstractCloneState();

        #endregion

        #region Subscription

        /// <summary>
        /// Updates each state subscriber with a copy of this controller's state
        /// </summary>
        internal void UpdateSubscribers()
        {
            foreach (var subscriber in Subscribers) {
                subscriber.AbstractState = AbstractCloneState();
            }
        }

        /// <summary>
        /// Removes the given subscriber from this state controller
        /// </summary>
        /// <param name="subscriber">Subscriber to remove</param>
        internal void RemoveSubscriber(AbstractStateSubscriber subscriber)
        {
            Subscribers = Subscribers.Where(p => p.ID != subscriber.ID).ToList();
        }

        /// <summary>
        /// Spawns a subscriber to this controller's state
        /// </summary>
        /// <returns>State subscriber that will receive updates to this controller's state</returns>
        internal AbstractStateSubscriber SpawnSubscriber()
        {
            var sub = CreateSubscriber();
            Subscribers.Add(sub);

            sub.AbstractState = AbstractCloneState();
            return sub;
        }

        /// <summary>
        /// Type-specific state subscribers must define how a subscriber is created
        /// </summary>
        /// <returns>Abstract state subscriber to add to this controller</returns>
        protected abstract AbstractStateSubscriber CreateSubscriber();

        #endregion
    }

    /// <summary>
    /// Controls an individual system state container
    /// </summary>
    /// <typeparam name="T">The state used by this controller</typeparam>
    public abstract class StateController<T> : AbstractController where T : State
    {
        /// <summary>
        /// The current state of this controller
        /// </summary>
        public T State { get { return (T)AbstractState; } }

        /// <summary>
        /// Reflection-based constructor. This must be
        /// called to insure the controller has a state
        /// to operate with
        /// </summary>
        public StateController() : base()
        {
        }

        /// <summary>
        /// Overriden to defer to a type-specific state clone. This
        /// insures that a proper clone is performed to reduce bugs
        /// </summary>
        /// <returns>Clone of the state managed by this controller</returns>
        internal override State AbstractCloneState()
        {
            return CloneState();
        }

        /// <summary>
        /// Creates a type-specific state subscriber
        /// </summary>
        /// <returns>State subscriber specified to this state type</returns>
        protected override AbstractStateSubscriber CreateSubscriber()
        {
            return new StateSubscriber<T>();
        }

        /// <summary>
        /// Override to define how to create a clone of a state object
        /// </summary>
        /// <returns>A clone of the state object</returns>
        public abstract T CloneState();
    }
}
