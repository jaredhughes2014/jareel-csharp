using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Jareel
{
    /// <summary>
    /// Utility class for extracting and executing event listeners
    /// </summary>
    internal class ListenerConverter
    {
        #region Fields

        /// <summary>
        /// List of event listeners extracted from the controller
        /// </summary>
        private List<EventListener> m_listeners;

        #endregion

        /// <summary>
        /// Creates a new listener converter. This extracts all event listeners
        /// from the controller and prepares them for execution
        /// </summary>
        /// <param name="controller">The controller being converted to a listener set</param>
        public ListenerConverter(AbstractController controller)
        {
            m_listeners = new List<EventListener>();
            var methods = controller.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                              .Where(p => Attribute.IsDefined(p, typeof(EventListenerAttribute)));

            foreach (var method in methods) {
                var attrib = GetListener(method);
                m_listeners.Add(Events.RegisterListener(attrib.Event, controller, method));
            }
        }

        /// <summary>
        /// Gets the listener attribute off of a reflected method
        /// </summary>
        /// <param name="method">Reflected method which is designated as an event listener</param>
        /// <returns>The event listener attribute</returns>
        private EventListenerAttribute GetListener(MethodInfo method)
        {
            return (EventListenerAttribute)method.GetCustomAttributes(typeof(EventListenerAttribute), true).First();
        }

        /// <summary>
        /// Executes all queued events. Each listener will process events in the order they were received
        /// </summary>
        /// <returns>True if any events were executed, false otherwise</returns>
        public bool ExecuteAll()
        {
            bool executed = false;
            foreach (var listener in m_listeners) {
                executed = listener.Execute() || executed;
            }
            return executed;
        }
    }
}
