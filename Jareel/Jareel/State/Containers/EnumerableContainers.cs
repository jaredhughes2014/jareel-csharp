using System;
using System.Collections.Generic;
using System.Linq;

namespace Jareel
{
    /// <summary>
    /// Contains an enumerable of integers.
    /// </summary>
    internal class IntegerEnumerableContainer : StateDataContainer
    {
        #region Properties

        /// <summary>
        /// The value this container interacts with
        /// </summary>
        private IEnumerable<int> m_value;

        /// <summary>
        /// When set, the enumerable is rebuilt completely. When accessed, a copy of the enumerable
        /// is returned to preserve the actual value
        /// </summary>
        public override object Value 
        {
            get => m_value.Select(p => p).ToArray();
            set => m_value = m_value.Where(p => false).Union((IEnumerable<int>)value);
        }

        #endregion

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="name">The export name of the state data</param>
        /// <param name="persistent">If true, this data should be included in standard exports</param>
        /// <param name="value">The enumerable this container interacts with</param>
        public IntegerEnumerableContainer(string name, bool persistent, IEnumerable<int> value) : base(name, persistent)
        {
            m_value = value;
        }
    }

    /// <summary>
    /// Container for an enumerable of floating point numbers
    /// </summary>
    internal class FloatEnumerableContainer : StateDataContainer
    {
        #region Properties

        /// <summary>
        /// The value this container interacts with
        /// </summary>
        private IEnumerable<float> m_value;

        /// <summary>
        /// When set, the enumerable is rebuilt completely. When accessed, a copy of the enumerable
        /// is returned to preserve the actual value
        /// </summary>
        public override object Value {
            get => m_value.Select(p => p).ToArray();
            set => m_value = m_value.Where(p => false).Union((IEnumerable<float>)value);
        }

        #endregion

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="name">The export name of the state data</param>
        /// <param name="persistent">If true, this data should be included in standard exports</param>
        /// <param name="value">The enumerable this container interacts with</param>
        public FloatEnumerableContainer(string name, bool persistent, IEnumerable<float> value) : base(name, persistent)
        {
            m_value = value;
        }
    }

    /// <summary>
    /// Container for an enumerable of boolean values
    /// </summary>
    internal class BooleanEnumerableContainer : StateDataContainer
    {
        #region Properties

        /// <summary>
        /// The value this container interacts with
        /// </summary>
        private IEnumerable<bool> m_value;

        /// <summary>
        /// When set, the enumerable is rebuilt completely. When accessed, a copy of the enumerable
        /// is returned to preserve the actual value
        /// </summary>
        public override object Value {
            get => m_value.Select(p => p).ToArray();
            set => m_value = m_value.Where(p => false).Union((IEnumerable<bool>)value);
        }

        #endregion

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="name">The export name of the state data</param>
        /// <param name="persistent">If true, this data should be included in standard exports</param>
        /// <param name="value">The enumerable this container interacts with</param>
        public BooleanEnumerableContainer(string name, bool persistent, IEnumerable<bool> value) : base(name, persistent)
        {
            m_value = value;
        }
    }

    /// <summary>
    /// Container for an enumerable of string values
    /// </summary>
    internal class StringEnumerableContainer : StateDataContainer
    {
        #region Properties

        /// <summary>
        /// The value this container interacts with
        /// </summary>
        private IEnumerable<string> m_value;

        /// <summary>
        /// When set, the enumerable is rebuilt completely. When accessed, a copy of the enumerable
        /// is returned to preserve the actual value
        /// </summary>
        public override object Value {
            get => m_value.Select(p => p).ToArray();
            set => m_value = m_value.Where(p => false).Union((IEnumerable<string>)value);
        }

        #endregion

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="name">The export name of the state data</param>
        /// <param name="persistent">If true, this data should be included in standard exports</param>
        /// <param name="value">The enumerable this container interacts with</param>
        public StringEnumerableContainer(string name, bool persistent, IEnumerable<string> value) : base(name, persistent)
        {
            m_value = value;
        }
    }
}
