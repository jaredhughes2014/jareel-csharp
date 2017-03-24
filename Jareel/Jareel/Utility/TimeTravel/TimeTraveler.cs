using System.Collections.Generic;

namespace Jareel.Utility
{
	/// <summary>
	/// A time traveler is a utility object which can be used to record changes to
	/// a master controller and playback changes to that controller.
	/// 
	/// TimeTravelers also allow imports and exports of captured states for future use.
	/// 
	/// Note that the time traveler has the potential to become very large in memory if
	/// recording a large number of large states. Use with caution
	/// </summary>
	public class TimeTraveler
	{
		#region Fields

		/// <summary>
		/// All states captured by this time traveler
		/// </summary>
		private List<string> m_captures;
		public string[] CapturedStates { get { return m_captures.ToArray(); } }

		/// <summary>
		/// The master controller this time traveler modifies
		/// </summary>
		private MasterController m_master;

		/// <summary>
		/// The next state to play in this time traveler
		/// </summary>
		private int m_nextState;
		public int StateIndex { get { return m_nextState; } }

		/// <summary>
		/// If true, this time traveler has updated the state
		/// </summary>
		internal bool PlayRegistered { get; set; }

		/// <summary>
		/// If true, this should receive a new state every time the given
		/// controller is updated by the executor this traveler spawned from
		/// </summary>
		internal bool Recording { get; set; }

		#endregion

		#region Initialization

		/// <summary>
		/// Creates a new TimeTraveler
		/// </summary>
		internal TimeTraveler(MasterController master)
		{
			m_captures = new List<string>();
			m_master = master;
		}

		#endregion

		#region Captures

		/// <summary>
		/// Begins receiving state updates
		/// </summary>
		public void StartRecording()
		{
			Recording = true;
			CaptureNow();
		}

		/// <summary>
		/// Ends receiving state updates
		/// </summary>
		public void StopRecording()
		{
			Recording = false;
		}

		/// <summary>
		/// Commands this time traveler to capture the master controller's current state
		/// </summary>
		internal void CaptureNow()
		{
			m_captures.Add(m_master.ExportDebugState());
		}

		/// <summary>
		/// Clears all time traveling data from this traveler
		/// </summary>
		public void ClearCaptures()
		{
			m_captures.Clear();
		}

		#endregion

		#region Playback

		/// <summary>
		/// Forces the master controller to import the next state in the playback. Returns
		/// false if there are no more states to play.
		/// 
		/// This change will not be visible until the next time your executor updates
		/// </summary>
		/// <returns>True if a state was imported successfully, false otherwise</returns>
		public bool PlayNextState()
		{
			if (m_nextState < m_captures.Count) {
				m_master.ImportState(m_captures[m_nextState]);
				m_nextState += 1;
				PlayRegistered = true;

				return true;
			}
			return false;
		}

		/// <summary>
		/// Starts the time traveler back at its first state. Does not change the master controller
		/// </summary>
		public void ResetPlayback()
		{
			m_nextState = 0;
		}

		#endregion

		#region Import/Export

		/// <summary>
		/// Exports all of the exported data strings into a JSON array. Note that each of the strings
		/// in the JSON array are still strings
		/// </summary>
		/// <returns>JSON-formatted export string from this time traveler</returns>
		public string Export()
		{
			return Json.Write(m_captures);
		}

		/// <summary>
		/// Imports a previously exported JSON string from this time traveler. Attempting to import an
		/// export string from an incorrect time traveler may not break the time traveler but may break
		/// the master controller when attempting to play back
		/// </summary>
		/// <param name="exportString">Export string originating from a time traveler recording the same master controller</param>
		public void Import(string exportString)
		{
			ClearCaptures();
			m_captures.AddRange(Json.ReadArray<string>(exportString));
		}

		#endregion
	}
}
