using System;

namespace Jareel
{
    /// <summary>
    /// Attribute applied to a method used to handle events. The event triggering
    /// the listener is type agnostic and uses a strategy called type-proximity
    /// prioritization explained below:
    /// 
    /// Each argument in the listener will be executed with the first argument of matching type
    /// If no arguments exist that satisfy the argument type of an argument, the listener will not execute
    /// If this listener does not execute and is strict, an exception will occur
    /// 
    /// Non-strict events are useful for having variants of an event. For example, if you only want an
    /// event to handle integer inputs, one event listener could process the integer input. Another
    /// listener could handle strings to give an error message when a
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =false)]
    public class EventListenerAttribute : Attribute
    {
        /// <summary>
        /// The object used as an event
        /// </summary>
        public object Event { get; private set; }

        /// <summary>
        /// Event listener attribute with a given event object
        /// </summary>
        /// <param name="ev">The event that will trigger this listener</param>
        public EventListenerAttribute(object ev)
        {
            Event = ev;
        }
    }
}
