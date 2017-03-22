using System;
using System.Collections.Generic;
using System.Linq;

namespace Jareel
{
    /// <summary>
    /// Contains an enumerable of integers.
    /// </summary>
    internal class IntegerListContainer : StateDataContainer
    {
        #region Properties

        /// <summary>
        /// Method used to get the state's current value
        /// </summary>
        private Func<List<int>> m_getMethod;

        /// <summary>
        /// Method used to set the state's current value
        /// </summary>
        private Action<List<int>> m_setMethod;

        /// <summary>
        /// When set, the enumerable is rebuilt completely. When accessed, a copy of the enumerable
        /// is returned to preserve the actual value
        /// </summary>
        public override object Value 
        {
            get => m_getMethod().Select(p => p).ToArray();
            set {
				m_setMethod(((object[])value).Cast<int>().ToList());
			}
        }

        #endregion

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="name">The export name of the state data</param>
        /// <param name="persistent">If true, this data should be included in standard exports</param>
        public IntegerListContainer(string name, bool persistent, Action<List<int>> setMethod, Func<List<int>> getMethod) : base(name, persistent)
        {
            m_getMethod = getMethod;
            m_setMethod = setMethod;
        }
    }

    /// <summary>
    /// Contains an enumerable of floating point numbers.
    /// </summary>
    internal class FloatListContainer : StateDataContainer
    {
        #region Properties

        /// <summary>
        /// Method used to get the state's current value
        /// </summary>
        private Func<List<float>> m_getMethod;

        /// <summary>
        /// Method used to set the state's current value
        /// </summary>
        private Action<List<float>> m_setMethod;

        /// <summary>
        /// When set, the enumerable is rebuilt completely. When accessed, a copy of the enumerable
        /// is returned to preserve the actual value
        /// </summary>
        public override object Value {
            get => m_getMethod().Select(p => p).ToArray();
            set {
				m_setMethod(((object[])value).Cast<float>().ToList());
			}
        }

        #endregion

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="name">The export name of the state data</param>
        /// <param name="persistent">If true, this data should be included in standard exports</param>
        public FloatListContainer(string name, bool persistent, Action<List<float>> setMethod, Func<List<float>> getMethod) : base(name, persistent)
        {
            m_getMethod = getMethod;
            m_setMethod = setMethod;
        }
    }

    /// <summary>
    /// Contains an enumerable of booleans.
    /// </summary>
    internal class BooleanListContainer : StateDataContainer
    {
        #region Properties

        /// <summary>
        /// Method used to get the state's current value
        /// </summary>
        private Func<List<bool>> m_getMethod;

        /// <summary>
        /// Method used to set the state's current value
        /// </summary>
        private Action<List<bool>> m_setMethod;

        /// <summary>
        /// When set, the enumerable is rebuilt completely. When accessed, a copy of the enumerable
        /// is returned to preserve the actual value
        /// </summary>
        public override object Value {
            get => m_getMethod().Select(p => p).ToArray();
            set {
				m_setMethod(((object[])value).Cast<bool>().ToList());
			}
        }

        #endregion

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="name">The export name of the state data</param>
        /// <param name="persistent">If true, this data should be included in standard exports</param>
        public BooleanListContainer(string name, bool persistent, Action<List<bool>> setMethod, Func<List<bool>> getMethod) : base(name, persistent)
        {
            m_getMethod = getMethod;
            m_setMethod = setMethod;
        }
    }

    /// <summary>
    /// Contains an enumerable of strings.
    /// </summary>
    internal class StringListContainer : StateDataContainer
    {
        #region Properties

        /// <summary>
        /// Method used to get the state's current value
        /// </summary>
        private Func<List<string>> m_getMethod;

        /// <summary>
        /// Method used to set the state's current value
        /// </summary>
        private Action<List<string>> m_setMethod;

        /// <summary>
        /// When set, the enumerable is rebuilt completely. When accessed, a copy of the enumerable
        /// is returned to preserve the actual value
        /// </summary>
        public override object Value {
            get => m_getMethod().Select(p => p).ToArray();
			set {
				m_setMethod(((object[])value).Cast<string>().ToList());
			}
		}

        #endregion

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="name">The export name of the state data</param>
        /// <param name="persistent">If true, this data should be included in standard exports</param>
        public StringListContainer(string name, bool persistent, Action<List<string>> setMethod, Func<List<string>> getMethod) : base(name, persistent)
        {
            m_getMethod = getMethod;
            m_setMethod = setMethod;
        }
    }

	/// <summary>
	/// Contains a list of StateObjects
	/// </summary>
	internal class ObjectListContainer : StateDataContainer
	{
		/// <summary>
		/// The method used to retrieve objects from the list
		/// </summary>
		private Func<List<StateObject>> m_getMethod;

		/// <summary>
		/// Interchange between the actual state data value and its serializable value
		/// </summary>
		public override object Value 
		{
			get {
				var list = m_getMethod();

				if (list != null) {
					return m_getMethod().Select(p => new StateConverter(p)).Select(p => p.DataMap).ToArray();
				}
				else return null;
			}
			set {
				if (m_getMethod == null) return;

				var stateType = m_getMethod().GetType().GetGenericArguments()[0];
				var objects = m_getMethod();
				objects.Clear();

				foreach (var dataMap in ((object[])value).Cast<Dictionary<string, object>>()) {
					var newState = (StateObject)Activator.CreateInstance(stateType);
					new StateConverter(newState).PopulateState(dataMap);

					objects.Add(newState);
				}
			}
		}

		/// <summary>
		/// Creates a new container
		/// </summary>
		/// <param name="name">The export name of the value</param>
		/// <param name="persistent">If true, this data should be included in standard exports</param>
		/// <param name="setMethod">The method used to set the value</param>
		/// <param name="getMethod">The method used to retrieve the value</param>
		public ObjectListContainer(string name, bool persistent, Func<List<StateObject>> getMethod) : base(name, persistent)
		{
			m_getMethod = getMethod;
		}
	}
}
