using System;

namespace Jareel
{
    /// <summary>
    /// Attribute applied to properties within a state object. This
    /// defines the export name of an individual item of data as well
    /// as defining whether it should be exported or not.
    /// 
    /// All state properties must include tihs attribute to insure that
    /// the controller knows when the state container has been modified.
    /// If this attribute is not applied, the system controller will never
    /// update
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StateDataAttribute : Attribute
    {
        /// <summary>
        /// The export name of this data attribute. Unused if this data property
        /// is not exported
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// If true, the property marked by this attribute will be exported
        /// during a state export
        /// </summary>
        public bool Persistent { get; private set; }

        /// <summary>
        /// Name constructor. If an attribute is named, it is assumed that the data is persistent
        /// </summary>
        /// <param name="name">The export name of the marked property</param>
        public StateDataAttribute(string name)
        {
            Name = name;
            Persistent = true;
        }
        
        /// <summary>
        /// Inherent name attribute which defines if the data should be persistent. If
        /// this is persistent, the name of the property will also be the export name
        /// </summary>
        /// <param name="persistent">If true, this data is serializable</param>
        public StateDataAttribute(bool persistent)
        {
            Name = "";
            Persistent = persistent;
        }

        /// <summary>
        /// Default constructor. The marked property will not be exported if this property
        /// is used
        /// </summary>
        public StateDataAttribute() : this(false) { }
    }
}
