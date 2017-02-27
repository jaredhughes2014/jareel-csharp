using System;

namespace Jareel
{
    /// <summary>
    /// Specialized container used for accessing strings
    /// </summary>
    internal class StateStringContainer : StateDataContainer
    {
        /// <summary>
        /// The method used to get the data from this container
        /// </summary>
        private Func<string> m_getMethod;

        /// <summary>
        /// The method used to set the data in this container
        /// </summary>
        private Action<string> m_setMethod;

        /// <summary>
        /// Overriden to mask string conversions to and from this container
        /// </summary>
        public override object Value
        {
            get
            {
                return m_getMethod();
            }

            set
            {
                if (value is string) {
                    m_setMethod((string)value);
                }
                else {
                    throw new ArgumentException("Argument is not a string");
                }
            }
        }

        /// <summary>
        /// Creates a new state data container specialized to handle strings
        /// </summary>
        /// <param name="name">The name of the data in this container</param>
        /// <param name="getMethod">Method used to get data from this container</param>
        /// <param name="setMethod">Method used to set data in this container</param>
        public StateStringContainer(string name, Func<string> getMethod, Action<string> setMethod) : base(name)
        {
            m_getMethod = getMethod;
            m_setMethod = setMethod;
        }
    }

    /// <summary>
    /// Specialized container used for accessing integers
    /// </summary>
    internal class StateIntegerContainer : StateDataContainer
    {
        /// <summary>
        /// The method used to get the data from this container
        /// </summary>
        private Func<int> m_getMethod;

        /// <summary>
        /// The method used to set the data in this container
        /// </summary>
        private Action<int> m_setMethod;

        /// <summary>
        /// Overriden to mask string conversions to and from this container
        /// </summary>
        public override object Value
        {
            get
            {
                return m_getMethod();
            }

            set
            {
                if (value is int) {
                    m_setMethod((int)value);
                }
                else {
                    throw new ArgumentException("Argument is not an integer");
                }
            }
        }

        /// <summary>
        /// Creates a new state data container specialized to handle integers
        /// </summary>
        /// <param name="name">The name of the data in this container</param>
        /// <param name="getMethod">Method used to get data from this container</param>
        /// <param name="setMethod">Method used to set data in this container</param>
        public StateIntegerContainer(string name, Func<int> getMethod, Action<int> setMethod) : base(name)
        {
            m_getMethod = getMethod;
            m_setMethod = setMethod;
        }
    }

    /// <summary>
    /// Specialized container used for accessing floating point numbers
    /// </summary>
    internal class StateFloatContainer : StateDataContainer
    {
        /// <summary>
        /// The method used to get the data from this container
        /// </summary>
        private Func<float> m_getMethod;

        /// <summary>
        /// The method used to set the data in this container
        /// </summary>
        private Action<float> m_setMethod;

        /// <summary>
        /// Overriden to mask string conversions to and from this container
        /// </summary>
        public override object Value
        {
            get
            {
                return m_getMethod();
            }

            set
            {
                if (value is float) {
                    m_setMethod((float)value);
                }
                else {
                    throw new ArgumentException("Argument is not a float");
                }
            }
        }

        /// <summary>
        /// Creates a new state data container specialized to handle floating point numbers
        /// </summary>
        /// <param name="name">The name of the data in this container</param>
        /// <param name="getMethod">Method used to get data from this container</param>
        /// <param name="setMethod">Method used to set data in this container</param>
        public StateFloatContainer(string name, Func<float> getMethod, Action<float> setMethod) : base(name)
        {
            m_getMethod = getMethod;
            m_setMethod = setMethod;
        }
    }

    /// <summary>
    /// Specialized container used for accessing boolean values
    /// </summary>
    internal class StateBooleanContainer : StateDataContainer
    {
        /// <summary>
        /// The method used to get the data from this container
        /// </summary>
        private Func<bool> m_getMethod;

        /// <summary>
        /// The method used to set the data in this container
        /// </summary>
        private Action<bool> m_setMethod;

        /// <summary>
        /// Overriden to mask string conversions to and from this container
        /// </summary>
        public override object Value
        {
            get
            {
                return m_getMethod();
            }

            set
            {
                if (value is bool) {
                    m_setMethod((bool)value);
                }
                else {
                    throw new ArgumentException("Argument is not a boolean");
                }
            }
        }

        /// <summary>
        /// Creates a new state data container specialized to handle boolean values
        /// </summary>
        /// <param name="name">The name of the data in this container</param>
        /// <param name="getMethod">Method used to get data from this container</param>
        /// <param name="setMethod">Method used to set data in this container</param>
        public StateBooleanContainer(string name, Func<bool> getMethod, Action<bool> setMethod) : base(name)
        {
            m_getMethod = getMethod;
            m_setMethod = setMethod;
        }
    }
}
