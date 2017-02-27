using System;
using System.Collections.Generic;
using System.Linq;

namespace Jareel
{
    /// <summary>
    /// Supplements the partial argument priority system used by event
    /// listeners. This facilitates the structure by conveniently mapping
    /// argument types to priority queues
    /// </summary>
    internal class EventArgSet
    {
        #region Fields

        /// <summary>
        /// Maps arguments to their types
        /// </summary>
        private Dictionary<Type, Queue<object>> m_args;

        /// <summary>
        /// If true, this event must be executed successfully
        /// or an exception will be raised
        /// </summary>
        public bool Strict { get; private set; }

        /// <summary>
        /// The event executed with this arg set
        /// </summary>
        public object Event { get; private set; }

        #endregion

        #region Setup

        /// <summary>
        /// Creates a new EventArgSet. This will automatically filter
        /// the set of arguments provided by their type
        /// </summary>
        /// <param name="args">Set of arguments to filter by type</param>
        /// <param name="ev">The event fired with this argset</param>
        /// <param name="strict">If true, this event must be successfully executed by every listener</param>
        public EventArgSet(bool strict, object ev, object[] args)
        {
            Strict = strict;
            Event = ev;
            m_args = new Dictionary<Type, Queue<object>>();

            foreach (var arg in args) {

                var type = arg.GetType();
                if (m_args.ContainsKey(type)) {
                    m_args[type].Enqueue(arg);
                }                 
                else {
                    var queue = new Queue<object>();
                    queue.Enqueue(arg);
                    m_args.Add(type, queue);
                }
            }
        }

        /// <summary>
        /// Makes a copy of this event arg set
        /// </summary>
        /// <returns>An independent copy of this event argument set</returns>
        public EventArgSet Copy()
        {
            EventArgSet copy = new EventArgSet(Strict, Event, new object[] { });
            copy.m_args = m_args.ToDictionary(p => p.Key, q => new Queue<object>(q.Value));
            return copy;
        }

        #endregion

        /// <summary>
        /// Gets an argument of the given type. Returns null if no
        /// argument exists with the given type.
        /// </summary>
        /// <param name="t">The type of argument to get</param>
        /// <returns>The next argument of type t, or null if no arg exists</returns>
        public object GetArg(Type t)
        {
            if (m_args.ContainsKey(t)) {
                var queue = m_args[t];
                return queue.Count > 0 ? queue.Dequeue() : null;
            }
            return null;
        }
    }
}
