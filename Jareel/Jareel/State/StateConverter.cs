using System;
using System.Collections.Generic;
using System.Linq;

namespace Jareel
{
    /// <summary>
    /// Process class designed to efficiently extract state
    /// data from a state object
    /// </summary>
    internal class StateConverter
    {
        #region Properties

        /// <summary>
        /// The state object processed by this processor
        /// </summary>
        private StateObject ProcessedState { get; set; }

        /// <summary>
        /// Contains the reflected metadata used to import/export state data
        /// </summary>
        internal List<StateDataContainer> DataContainers { get; private set; }

        /// <summary>
        /// The export name of the state container
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// Converts the data in this state container into a dictionary. The
        /// names in the dictionary are export names
        /// </summary>
        public Dictionary<string, object> DataMap
        {
            get
            {
                return DataContainers.Where(p => p.Persistent).ToDictionary(p => p.Name, q => q.Value);
            }
        }

        /// <summary>
        /// The entire data map. This will include non-persistent data and is mostly for debug purposes.
        /// This exported map can still be used to hydrate the state
        /// </summary>
        public Dictionary<string, object> CompleteDataMap
        {
            get
            {
                return DataContainers.ToDictionary(p => p.Name, q => q.Value);
            }
        }

        /// <summary>
        /// The set of object currently contained in this state convertor
        /// </summary>
        public object[] StateData { get { return DataContainers.Select(p => p.Value).ToArray(); } }

        #endregion

        /// <summary>
        /// Creates a new StateProcessor using the given state object
        /// </summary>
        /// <param name="state">State which is processed by this processor</param>
        public StateConverter(StateObject state)
        {
            ProcessedState = state;

            // Only extract names from top-level states and not complex properties
            ContainerName = (state is State) ? ExtractStateName(state) : "";
            ExtractStateProperties();
        }

        /// <summary>
        /// Populates the data in this state converter with data from the given raw data dictionary
        /// </summary>
        /// <param name="rawData">Parsed raw state data</param>
        public void PopulateState(Dictionary<string, object> rawData)
        {
            foreach (var container in DataContainers) {
                container.Value = rawData[container.Name];
            }
        }

        #region Processing

        /// <summary>
        /// Extracts the state name from this state object using its StateContainer attribute
        /// </summary>
        /// <param name="state">State instance marked with a StateContainer attribute</param>
        /// <returns>The name of the state container</returns>
        private static string ExtractStateName(StateObject state)
        {
            var stateType = state.GetType();
            var container = stateType.GetCustomAttributes(typeof(StateContainerAttribute), true).FirstOrDefault() as StateContainerAttribute;

            if (container == null) {
                throw new ArgumentException(string.Format("State of type {0} is not marked as a state container", stateType.Name));
            }

            return string.IsNullOrEmpty(container.Name) ? stateType.Name : container.Name;
        }

        /// <summary>
        /// Extracts all StateData attributes from the state and organizes them
        /// for quick, efficient imports/exports
        /// </summary>
        private void ExtractStateProperties()
        {
            DataContainers = new List<StateDataContainer>();
            var properties = ProcessedState.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(StateDataAttribute)));

            foreach (var property in properties) {
                var attrib = (StateDataAttribute)property.GetCustomAttributes(typeof(StateDataAttribute), true).First();

                string name = string.IsNullOrEmpty(attrib.Name) ? property.Name : attrib.Name;
                DataContainers.Add(StateDataContainer.GetStateContainer(name, property, ProcessedState));
            }
        }

        #endregion
    }
}
