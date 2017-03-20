using System;
using System.Linq;

namespace Jareel
{
    /// <summary>
    /// State subscribers are requested from master controllers
    /// to provide access to state data outside of a state controller.
    /// Note that the state data provided is a copy of the controlled state
    /// and changes made to the copy will not affect the original state
    /// </summary>
    internal class AbstractStateSubscriber
    {
        #region Properties

        /// <summary>
        /// The top-level state type. This is not used by the user
        /// </summary>
        private State m_abstractState;
        internal State AbstractState
        {
            get {
                Updated = false;
                return m_abstractState;
            }
            set {
                Updated = true;
                m_abstractState = value;
            }
        }

        /// <summary>
        /// Unique identifier used to access/remove state subscribers
        /// </summary>
        internal string ID { get; private set; }

        /// <summary>
        /// If true, this subscriber has been updated since the last time
        /// the state was accessed
        /// </summary>
        public bool Updated { get; set; }

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
    /// Manages multiple abstract state subscribers within a single instance.
    /// </summary>
    internal class MultipleStateSubscriber
    {
        #region Fields

        /// <summary>
        /// All of the subscribers held by this multiple state subscriber
        /// </summary>
        internal AbstractStateSubscriber[] Subscribers { get; private set; }

        /// <summary>
        /// Returns true if any of the subscribed states have updated
        /// </summary>
        public bool Updated { get { return Subscribers.Any(p => p.Updated); } }

        #endregion

        #region Construction

        /// <summary>
        /// Creates a new multiple state subscriber
        /// </summary>
        /// <param name="subscribers">The subscribers held by this object</param>
        public MultipleStateSubscriber(params AbstractStateSubscriber[] subscribers)
        {
            Subscribers = subscribers;
        }

        #endregion

        #region Access

        /// <summary>
        /// Gets the state subscribed to at the given index
        /// </summary>
        /// <param name="index">The index of the subscriber</param>
        /// <returns>The subscribed state at the given index</returns>
        public State GetState(int index)
        {
            return Subscribers[index].AbstractState;
        }

        #endregion
    }

    /// <summary>
    /// Provides access to a state managed in a master controller. Each time a
    /// state controller finishes processing changes made to a state, this subscriber
    /// will receive a copy of the final state
    /// </summary>
    /// <typeparam name="T">The type of state accessed by this subscriber</typeparam>
    public abstract class StateSubscriber
    {
        /// <summary>
        /// Contains each individual state subscriber
        /// </summary>
        internal MultipleStateSubscriber Subscribers { get; private set; }

        /// <summary>
        /// Creates a new state subscriber
        /// </summary>
        /// <param name="subscribers">All of the state subscribers used by this subscriber</param>
        internal StateSubscriber(MultipleStateSubscriber subscribers)
        {
            Subscribers = subscribers;
        }
    }

    /// <summary>
    /// Multiple state subscriber which subscribers to one type of state
    /// </summary>
    /// <typeparam name="A">The first state type</typeparam>
    public class StateSubscriber<A> : StateSubscriber
        where A : State, new()
    {
        /// <summary>
        /// The first state this subscribes to
        /// </summary>
        public A State1 { get { return (A)Subscribers.GetState(0); } }

        /// <summary>
        /// Constructs a new state subscriber which subscribers to one type of state
        /// </summary>
        /// <param name="subscribers">Subscriber for the one state type</param>
        internal StateSubscriber(MultipleStateSubscriber subscribers) : base(subscribers) { }
    }

    /// <summary>
    /// Multiple state subscriber which subscribers to two distinct types of states
    /// </summary>
    /// <typeparam name="A">The first state type</typeparam>
    /// <typeparam name="B">The second state type</typeparam>
    public class StateSubscriber<A, B> : StateSubscriber
        where A : State, new()
        where B : State, new()
    {
        /// <summary>
        /// The first state this subscribes to
        /// </summary>
        public A State1 { get { return (A)Subscribers.GetState(0); } }

        /// <summary>
        /// The second state this subscribes to
        /// </summary>
        public B State2 { get { return (B)Subscribers.GetState(1); } }

        /// <summary>
        /// Constructs a new state subscriber which subscribers to two types of states
        /// </summary>
        /// <param name="subscribers">Subscribers for each of the two state types</param>
        internal StateSubscriber(MultipleStateSubscriber subscribers) : base(subscribers) { }
    }

    /// <summary>
    /// Multiple state subscriber which subscribers to three distinct types of states
    /// </summary>
    /// <typeparam name="A">The first state type</typeparam>
    /// <typeparam name="B">The second state type</typeparam>
    /// <typeparam name="C">The third state type</typeparam>
    public class StateSubscriber<A, B, C> : StateSubscriber
        where A : State, new()
        where B : State, new()
        where C : State, new()
    {
        /// <summary>
        /// The first state this subscribes to
        /// </summary>
        public A State1 { get { return (A)Subscribers.GetState(0); } }

        /// <summary>
        /// The second state this subscribes to
        /// </summary>
        public B State2 { get { return (B)Subscribers.GetState(1); } }

        /// <summary>
        /// The third state this subscribes to
        /// </summary>
        public C State3 { get { return (C)Subscribers.GetState(2); } }

        /// <summary>
        /// Constructs a new state subscriber which subscribers to three types of states
        /// </summary>
        /// <param name="subscribers">Subscribers for each of the three state types</param>
        internal StateSubscriber(MultipleStateSubscriber subscribers) : base(subscribers) { }
    }

    /// <summary>
    /// Multiple state subscriber which subscribers to four distinct types of states
    /// </summary>
    /// <typeparam name="A">The first state type</typeparam>
    /// <typeparam name="B">The second state type</typeparam>
    /// <typeparam name="C">The third state type</typeparam>
    /// <typeparam name="D">The fourth state type</typeparam>
    public class StateSubscriber<A, B, C, D> : StateSubscriber
        where A : State, new()
        where B : State, new()
        where C : State, new()
        where D : State, new()
    {
        /// <summary>
        /// The first state this subscribes to
        /// </summary>
        public A State1 { get { return (A)Subscribers.GetState(0); } }

        /// <summary>
        /// The second state this subscribes to
        /// </summary>
        public B State2 { get { return (B)Subscribers.GetState(1); } }

        /// <summary>
        /// The third state this subscribes to
        /// </summary>
        public C State3 { get { return (C)Subscribers.GetState(2); } }

        /// <summary>
        /// The fourth state this subscribes to
        /// </summary>
        public D State4 { get { return (D)Subscribers.GetState(3); } }

        /// <summary>
        /// Constructs a new state subscriber which subscribers to four types of states
        /// </summary>
        /// <param name="subscribers">Subscribers for each of the four state types</param>
        internal StateSubscriber(MultipleStateSubscriber subscribers) : base(subscribers) { }
    }
}
