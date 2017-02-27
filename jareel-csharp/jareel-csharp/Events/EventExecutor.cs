using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Jareel
{
    /// <summary>
    /// Wrapper class used to hold an event listener. An event executor
    /// is designed to determine which event arguments to execute using
    /// a proximal priority system:
    /// 
    /// For each listener argument, the earliest matching type argument is used
    /// Each argument is only applied to one event argument
    /// 
    /// If any arguments in the listener cannot be matched to arguments provided
    /// by the event, this executor will be skipped. If the user executed a strict
    /// event, a runtime exception will occur.
    /// 
    /// If an event has no arguments, it will always be executed
    /// </summary>
    internal class EventExecutor
    {
        #region Properties

        /// <summary>
        /// The type of each argument in the listener.
        /// </summary>
        public Type[] ArgTypes { get; private set; }

        /// <summary>
        /// Reflection listener used to execute an event
        /// </summary>
        public MethodInfo Listener { get; private set; }
        
        /// <summary>
        /// The controller containing the event listener
        /// </summary>
        public AbstractController Controller { get; private set; }

        #endregion

        #region Setup

        /// <summary>
        /// Creates a new event executor
        /// </summary>
        /// <param name="controller">The controller containing the event listener</param>
        /// <param name="listener">The method executed using the event</param>
        public EventExecutor(AbstractController controller, MethodInfo listener)
        {
            Listener = listener;
            Controller = controller;
            ArgTypes = Listener.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        /// <summary>
        /// Executes this event executor. Returns true if the event execution was successful,
        /// or false if the given arg set is missing any values required for this event listener
        /// to execute
        /// </summary>
        /// <param name="args">ArgSet containing all arguments the event was executed with</param>
        /// <returns>True if the event listener executed successfully, or false otherwise</returns>
        public bool Execute(EventArgSet args)
        {
            var values = ArgTypes.Select(p => args.GetArg(p)).ToArray();
            if (values.Contains(null)) {
                return false;
            }
            else if (values.Length == 0) {
                Listener.Invoke(Controller, null);
            }
            else {
                Listener.Invoke(Controller, values);
            }
            return true;
        }

        #endregion
    }
}
