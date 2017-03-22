
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public static StateDataContainer GetStateContainer(string name, bool persistent, PropertyInfo prop, StateObject container)
        {
            var type = prop.PropertyType;

            if (type == typeof(string)) {
                return new StateStringContainer(name, persistent, ConvertGetMethod<string>(prop, container), ConvertSetMethod<string>(prop, container));
            }
            else if (type == typeof(int)) {
                return new StateIntegerContainer(name, persistent, ConvertGetMethod<int>(prop, container), ConvertSetMethod<int>(prop, container));
            }
            else if (type == typeof(float)) {
                return new StateFloatContainer(name, persistent, ConvertGetMethod<float>(prop, container), ConvertSetMethod<float>(prop, container));
            }
            else if (type == typeof(bool)) {
                return new StateBooleanContainer(name, persistent, ConvertGetMethod<bool>(prop, container), ConvertSetMethod<bool>(prop, container));
            }
            else if (type.IsSubclassOf(typeof(StateObject))) {
                object value = prop.GetValue(container, null);
                return new ObjectContainer(name, persistent, (StateObject)value);
            }
            else if (type.GetInterfaces().Contains(typeof(IList)) && type.IsGenericType) {
                var generics = type.GetGenericArguments();

                if (generics.Length == 1) {
                    return ConvertEnumerable(name, persistent, prop, container, generics[0]);
                }
                else {
                    throw new ArgumentException("Jareel only supports generic enumerables with 1 generic type");
                }
            }
            else {
                throw new ArgumentException("Unsupported state data type: " + type.Name);
            }
        }

        /// <summary>
        /// Converts an enumerable type into an appropriate enumerable container
        /// </summary>
        /// <param name="name">The export name of the data</param>
        /// <param name="persistent">If true, this data should be included in standard exports</param>
        /// <param name="value">The enumerable to contain</param>
        /// <param name="childType">The type of the elements in the enumerable</param>
        /// <returns></returns>
        private static StateDataContainer ConvertEnumerable(string name, bool persistent, PropertyInfo prop, StateObject container, Type childType)
        {
			if (childType == typeof(string)) {
				return new StringListContainer(name, persistent, ConvertSetMethod<List<string>>(prop, container), ConvertGetMethod<List<string>>(prop, container));
			}
			else if (childType == typeof(int)) {
				return new IntegerListContainer(name, persistent, ConvertSetMethod<List<int>>(prop, container), ConvertGetMethod<List<int>>(prop, container));
			}
			else if (childType == typeof(float)) {
				return new FloatListContainer(name, persistent, ConvertSetMethod<List<float>>(prop, container), ConvertGetMethod<List<float>>(prop, container));
			}
			else if (childType == typeof(bool)) {
				return new BooleanListContainer(name, persistent, ConvertSetMethod<List<bool>>(prop, container), ConvertGetMethod<List<bool>>(prop, container));
			}

			//TODO This is very ugly and should be cleaned up
			else if (childType.IsSubclassOf(typeof(StateObject))) {
				var getter = ConvertGetMethod<IEnumerable>(prop, container);

				Func<List<StateObject>> getMethod = () => {
					var newList = new List<StateObject>();
					foreach (object data in getter()) {
						newList.Add((StateObject)data);
					}
					return newList;
				};

				return new ObjectListContainer(name, persistent, getMethod);
			}
			else {
				throw new ArgumentException("Unsupported enumerable data type: " + childType.Name);
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
			var getMethod = prop.GetGetMethod();
            return (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), container, getMethod);
        }
    }
}
