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
		#region Priorities

		/// <summary>
		/// The object used as an event
		/// </summary>
		public object Event { get; private set; }

		/// <summary>
		/// The priority of this attribute. Lower priorites will always execute first, and listeners
		/// with no priority will always execute after those with priorities
		/// </summary>
		public int Priority { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Event listener attribute with a given event object and a given priority. Lower priorities will
		/// always be executed first
		/// </summary>
		/// <param name="ev">The event that will trigger this listener</param>
		/// <param name="priority">The priority of this listener. Lower priorities will be executed first</param>
		public EventListenerAttribute(object ev, int priority)
        {
            Event = ev;
			Priority = priority;
        }

		/// <summary>
		/// Creates an event listener attribute with a given event object and no defined priority. This will
		/// always execute after event listeners with on the same controller defined priorities 
		/// </summary>
		/// <param name="ev">The event that will trigger this listener</param>
		public EventListenerAttribute(object ev) : this(ev, int.MaxValue)
		{
		}

		#endregion
	}
}
