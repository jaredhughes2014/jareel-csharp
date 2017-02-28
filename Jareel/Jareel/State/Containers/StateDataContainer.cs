
using System;
using System.Reflection;

namespace Jareel
{
    /// <summary>
    /// Utility class used to store the metadata necessary to export and import an attribute
    /// in a state structure
    /// </summary>
    internal abstract class StateDataContainer
    {
        /// <summary>
        /// Export name of the data
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Each type of data container needs to define a boxed value
        /// accesor and a boxed value setter
        /// </summary>
        public abstract object Value { get; set; }

        /// <summary>
        /// If true, this container holds persistent data (which should always be serialized)
        /// </summary>
        public bool Persistent { get; private set; }

        /// <summary>
        /// Creates a new StateDataContainer
        /// </summary>
        /// <param name="name">The export name of the data</param>
        /// <param name="persistent">If true, this container will always be included in a state export</param>
        public StateDataContainer(string name, bool persistent)
        {
            Name = name;
            Persistent = persistent;
        }

        /// <summary>
        /// Converts a reflection-based property into a state converter
        /// </summary>
        /// <param name="prop">The reflected property to convert from</param>
        /// <param name="name">The export name of the container</param>
        /// <param name="container">The state object that contains this state</param>
        /// <returns>State data container used to access and modify the given property</returns>
        public static StateDataContainer GetStateContainer(string name, PropertyInfo prop, StateObject container)
        {
            var type = prop.PropertyType;

            if (type == typeof(string)) {
                return new StateStringContainer(name, ConvertGetMethod<string>(prop, container), ConvertSetMethod<string>(prop, container));
            }
            else if (type == typeof(int)) {
                return new StateIntegerContainer(name, ConvertGetMethod<int>(prop, container), ConvertSetMethod<int>(prop, container));
            }
            else if (type == typeof(float)) {
                return new StateFloatContainer(name, ConvertGetMethod<float>(prop, container), ConvertSetMethod<float>(prop, container));
            }
            else if (type == typeof(bool)) {
                return new StateBooleanContainer(name, ConvertGetMethod<bool>(prop, container), ConvertSetMethod<bool>(prop, container));
            }
            else if (type.IsSubclassOf(typeof(StateObject))) {
                object value = prop.GetValue(container, null);
                return new ObjectContainer(name, (StateObject)value);
            }
            else {
                throw new ArgumentException("Unsupported state data type: " + type.Name);
            }
        }

        /// <summary>
        /// Converts the given property and source state into a delegate which can be
        /// used to set the data in the given property
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="prop">Reflected property info</param>
        /// <param name="container">The state containing the given property</param>
        /// <returns>An action delegate used to set data in the property</returns>
        private static Action<T> ConvertSetMethod<T>(PropertyInfo prop, StateObject container)
        {
            return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), container, prop.GetSetMethod());
        }

        /// <summary>
        /// Converts the given property from the given container into a delegate type
        /// that allows accessing the data in the given property
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="prop">The property from reflection</param>
        /// <param name="container">The state which contains the given property</param>
        /// <returns>A function delegate which can be used to retrieve data from this property</returns>
        private static Func<T> ConvertGetMethod<T>(PropertyInfo prop, StateObject container)
        {
            return (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), container, prop.GetGetMethod());
        }
    }
}
