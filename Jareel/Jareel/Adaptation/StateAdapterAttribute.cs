using System;

namespace Jareel
{
    /// <summary>
    /// This attribute must decorate a method intended to be used as
    /// a state adapter in order to automatically initialize state
    /// adaptation
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class StateAdapterAttribute : Attribute
    {
    }
}
