namespace Jareel
{
    /// <summary>
    /// State objects are complex objects that can be serialized/deserialized
    /// as part of a system's state. Just like top-level states, state objects
    /// must be decorated with a StateContainer attribute and properties must
    /// be decorated with a StateData attribute. If a state object is used as
    /// a nested state, the name value of the state container attribute will
    /// be ignored in favor of the state data attribute name
    /// </summary>
    public class StateObject
    {
        /// <summary>
        /// State objects must have a default constructor to
        /// be built
        /// </summary>
        public StateObject()
        {
        }
    }
}
