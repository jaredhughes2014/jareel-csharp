using System;

namespace Jareel
{
    /// <summary>
    /// Attribute meant to be applied to a State object to define the
    /// export name of a state container
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StateContainerAttribute : Attribute
    {
        /// <summary>
        /// The export name of this state container
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a state container attribute with a user-defined name.
        /// When the state is exported, this is the name that will be
        /// given to the state object
        /// </summary>
        /// <param name="name">The export name of the state container</param>
        public StateContainerAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Creates a state container attribute without a user-defined
        /// name. This will infer the name of the container from the type
        /// itself
        /// </summary>
        public StateContainerAttribute() : this("")
        {

        }
    }
}
