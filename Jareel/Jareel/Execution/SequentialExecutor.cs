
using System.Collections.Generic;
using System.Linq;
using Jareel.Utility;

namespace Jareel
{
    /// <summary>
    /// Container used to execute master controller updates on a single thread.
    /// </summary>
    public class SequentialExecutor
    {
        #region Fields

        /// <summary>
        /// The controller managed by this executor
        /// </summary>
        private MasterController m_controller;

		/// <summary>
		/// All time travelers currently watching
		/// </summary>
		private List<TimeTraveler> m_timeTravelers;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new sequential executor
        /// </summary>
        /// <param name="controller">The master controller this executor manages</param>
        public SequentialExecutor(MasterController controller)
        {
            m_controller = controller;
			m_timeTravelers = new List<TimeTraveler>();

            // Execute once to make sure all states are initialized
            ForceExecute();
        }

		#endregion

		#region Execution

		/// <summary>
		/// Performs a single execution of each execution chain in
		/// the controller. This will only cause a change if at least
		/// one event has triggered a change in the state
		/// </summary>
		public void Execute()
        {
			if (m_controller.Events.EventRegistered || m_timeTravelers.Any(p => p.PlayRegistered)) {
				ForceExecute();

				m_timeTravelers.ForEach(p => p.PlayRegistered = false);
				m_controller.Events.ResetEventRegistration();
			}
        }

		/// <summary>
		/// This will force the master controller to update, regardless of
		/// whether an event has occurred or not
		/// </summary>
		internal void ForceExecute()
		{
			foreach (var chain in m_controller.Chains) {
				chain.Execute();
			}

			foreach (var traveler in m_timeTravelers.Where(p => p.Recording)) {
				traveler.CaptureNow();
			}
		}

		#endregion

		#region Time Travelers

		/// <summary>
		/// Spawns and returns a new time traveler. If startImmediately is true, the time
		/// traveler will be set to receive captures immediately and will be started with
		/// the current state of the master controller. Otherwise, the time traveler
		/// will need to be activated via ActivateTimeTraveler to begin capturing
		/// </summary>
		/// <param name="activateImmediately">If true, activates the new traveler immediately</param>
		/// <returns>The spawned time traveler</returns>
		public TimeTraveler SpawnTimeTraveler(bool activateImmediately=true)
		{
			var timeTraveler = new TimeTraveler(m_controller);
			m_timeTravelers.Add(timeTraveler);

			if (activateImmediately) {
				timeTraveler.StartRecording();
			}
			return timeTraveler;
		}

		#endregion
	}
}
