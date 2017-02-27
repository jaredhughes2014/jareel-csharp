
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

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new sequential executor
        /// </summary>
        /// <param name="controller">The master controller this executor manages</param>
        public SequentialExecutor(MasterController controller)
        {
            m_controller = controller;
        }

        #endregion

        /// <summary>
        /// Performs a single execution of each execution chain in
        /// the controller.
        /// </summary>
        public void Execute()
        {
            foreach (var chain in m_controller.Chains) {
                chain.Execute();
            }
        }
    }
}
