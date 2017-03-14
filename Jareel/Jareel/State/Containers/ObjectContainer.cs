using System.Collections.Generic;

namespace Jareel
{
    /// <summary>
    /// Container used to hold complex objects. Object containers create new
    /// state converters
    /// </summary>
    internal class ObjectContainer : StateDataContainer
    {
        /// <summary>
        /// Converter used to export/import data into the state object
        /// </summary>
        private StateConverter m_converter;

        /// <summary>
        /// Converts the complex object into a boxed dictionary
        /// </summary>
        public override object Value
        {
            get
            {
                return m_converter.DataMap;
            }

            set
            {
                if (value is Dictionary<string, object>) {
                    m_converter.PopulateState((Dictionary<string, object>)value);
                }
            }
        }

        /// <summary>
        /// Creates a new state object container with a given name and state
        /// </summary>
        /// <param name="name">The export name of the object</param>
        /// <param name="state">The state object</param>
        public ObjectContainer(string name, bool persistent, StateObject state) : base(name, persistent)
        {
            m_converter = new StateConverter(state);
        }
    }
}
