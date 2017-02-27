using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Jareel
{
    /// <summary>
    /// Utility class designed to support asynchronous event handling. When
    /// an event is fired, the effects of that event are not immediate. This
    /// utility class is designed to hold an event argument set until the
    /// holding controller is ready to execute the events
    /// </summary>
    internal class EventListener
    {
        /// <summary>
        /// Stores all incoming events
        /// </summary>
        private Queue<EventArgSet> m_events;

        /// <summary>
        /// Used to execute events when called to do so
        /// </summary>
        private EventExecutor m_executor;

        /// <summary>
        /// Creates a new EventListener
        /// </summary>
        /// <param name="controller">Controller which handles events this listens for</param>
        /// <param name="method">The method executed by the event</param>
        public EventListener(AbstractController controller, MethodInfo method)
        {
            m_events = new Queue<EventArgSet>();
            m_executor = new EventExecutor(controller, method);
        }

        /// <summary>
        /// Queues an event to be executed
        /// </summary>
        /// <param name="ev">The event to execute</param>
        public void QueueEvent(EventArgSet ev)
        {
            m_events.Enqueue(ev);
        }

        /// <summary>
        /// Executes all events in the order they arrived at this listener
        /// </summary>
        /// <returns>True if any events were executed</returns>
        public bool Execute()
        {
            bool executed = m_events.Count > 0;
            while (m_events.Count > 0) {
                var ev = m_events.Dequeue();

                if (!m_executor.Execute(ev) && ev.Strict) {
                    string listenerName = m_executor.Listener.Name;
                    string controllerName = m_executor.Controller.GetType().Name;
                    string argTypes = BuildTypesString(m_executor.ArgTypes);

                    throw new ArgumentException(string.Format("Failed to execute {0} on controller {1} with event {2}. Expected argument types: {3}",
                                                              listenerName,
                                                              controllerName,
                                                              ev.Event.ToString(),
                                                              argTypes));
                }
            }

            return executed;
        }

        /// <summary>
        /// Convenience function for building a debug string combining multiple types into
        /// a single string.
        /// </summary>
        /// <param name="argTypes">Types to be built into a debug string</param>
        /// <returns>Single string listing the types in order</returns>
        private string BuildTypesString(Type[] argTypes)
        {
            if (argTypes.Length == 0) return "None";

            StringBuilder sb = new StringBuilder();
            sb.Append(argTypes[0].Name);
            
            for (int i = 1; i < argTypes.Length; ++i) {
                sb.Append(", ").Append(argTypes[i].Name);
            }

            return sb.ToString();
        }
    }
}
