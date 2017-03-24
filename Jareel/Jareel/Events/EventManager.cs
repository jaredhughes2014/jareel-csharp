using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jareel
{
    /// <summary>
    /// Utility class used to store event listeners and execute events.
    /// </summary>
    public class EventManager
    {
        #region Events

        /// <summary>
        /// Event fired every time an event is executed.
        /// </summary>
        /// <param name="ev">The event object exeucted</param>
        /// <param name="args">The arguments provided with the event</param>
        internal delegate void OnEventExecutedHandler(object ev, object[] args);
        internal event OnEventExecutedHandler OnEventExecuted;

        #endregion

        #region Fields

        /// <summary>
        /// Mapping structure for mapping event types to event values to a list
        /// of listeners. Each executor in a list is mapped to the same event value
        /// </summary>
        private Dictionary<Type, Dictionary<object, List<EventListener>>> m_eventMap;

		/// <summary>
		/// If true, at least one event has been registered since the last reset
		/// </summary>
		internal bool EventRegistered { get; private set; }

        #endregion

        #region Setup

        /// <summary>
        /// Creates a new event manager
        /// </summary>
        internal EventManager()
        {
            m_eventMap = new Dictionary<Type, Dictionary<object, List<EventListener>>>();
        }

        #endregion

        #region Registration

        /// <summary>
        /// Registers an event listener for the given value
        /// </summary>
        /// <param name="value">The event value</param>
        /// <param name="controller">The controller this listener comes from</param>
        /// <param name="method">The listener to invoke on event execution</param>
        internal EventListener RegisterListener(object value, AbstractController controller, MethodInfo method)
        {
            var listener = new EventListener(controller, method);
            GetListenerList(value, true).Add(listener);
            return listener;
        }

        /// <summary>
        /// Cycles through the event map to find the executor list corresponding to
        /// the given event value. This will create a new executor list if create is
        /// true and the value is not found, otherwise it will throw an ArgumentException
        /// </summary>
        /// <param name="value">The event value</param>
        /// <param name="create">If true, create an executor list if not found</param>
        /// <returns>The executor list, if one is found</returns>
        private List<EventListener> GetListenerList(object value, bool create)
        {
            var eventType = value.GetType();

            if (m_eventMap.ContainsKey(eventType)) {

                //List found
                if (m_eventMap[eventType].ContainsKey(value)) {
                    return m_eventMap[eventType][value];
                }

                //Create a new list
                else if (create) {
                    var list = new List<EventListener>();
                    m_eventMap[eventType].Add(value, list);
                    return list;
                }

                //Value not found
                else {
                    throw new ArgumentException("No listeners mapped for event of value: " + value.ToString());
                }
            }

            //Create new map and list
            else if (create) {
                m_eventMap.Add(eventType, new Dictionary<object, List<EventListener>>());
                return GetListenerList(value, true);
            }

            //Type not found
            else {
                throw new ArgumentException("No listeners mapped for events of type: " + eventType.Name);
            }
        }

        #endregion

        #region Execution

        /// <summary>
        /// Executes an event. A standard event execution will execute any listeners
        /// that are properly configured to execute events with the given argument
        /// types. Any listeners that require arguments not provided will be skipped
        /// </summary>
        /// <param name="ev">The event being executed</param>
        /// <param name="args">The arguments provided with the event</param>
        public void Execute(object ev, params object[] args)
        {
            ExecutionSequence(false, ev, args);
        }

        /// <summary>
        /// Executes an event. A strict event is an event where every listener must
        /// execute, or else an exception will be raised.
        /// </summary>
        /// <param name="ev">The event object</param>
        /// <param name="args">The arguments provided with the event</param>
        public void ExecuteStrict(object ev, params object[] args)
        {
            ExecutionSequence(true, ev, args);
        }

        /// <summary>
        /// Performs the actual event execution
        /// </summary>
        /// <param name="strict">True if this is a strict event</param>
        /// <param name="ev">The event value</param>
        /// <param name="args">The arguments provided with the event</param>
        private void ExecutionSequence(bool strict, object ev, object[] args)
        {
            var argSet = new EventArgSet(strict, ev, args);
            OnEventExecuted?.Invoke(ev, args);

            foreach (var listener in GetListenerList(ev, false)) {
                listener.QueueEvent(argSet.Copy());
            }

			EventRegistered = true;
        }

		/// <summary>
		/// Resets the event registration.
		/// </summary>
		internal void ResetEventRegistration()
		{
			EventRegistered = false;
		}

        #endregion
    }
}
