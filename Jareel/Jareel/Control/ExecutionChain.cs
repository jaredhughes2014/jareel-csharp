using System.Collections.Generic;
using System;

namespace Jareel
{
    /// <summary>
    /// Class designed to handle the chaining logic of execution chains. This class
    /// does not handle the execution of system controllers
    /// </summary>
    public class ExecutionChain
    {
        #region Properties

        /// <summary>
        /// The number of chains this chain is waiting on to begin executing
        /// </summary>
        internal List<ExecutionChain> Parents { get; private set; }

        /// <summary>
        /// The chains that are children of this chain
        /// </summary>
        internal List<ExecutionChain> Children { get; private set; }

        /// <summary>
        /// The abstract controller that executes at this execution chain
        /// </summary>
        internal AbstractController AbstractController { get; set; }

        /// <summary>
        /// The adapters this segment of the chain executes
        /// </summary>
        private List<StateAdapter> m_adapters;

        /// <summary>
        /// The number of parent chains that have executed
        /// </summary>
        private int m_executed;

        #endregion

        #region Setup

        /// <summary>
        /// Initializes this chain
        /// </summary>
        /// <param name="controller">The controller handled by this chain segment</param>
        internal ExecutionChain(AbstractController controller)
        {
            Children = new List<ExecutionChain>();
            Parents = new List<ExecutionChain>();

            m_adapters = new List<StateAdapter>();
            AbstractController = controller;
        }

        /// <summary>
        /// Resets this chain to its initial state
        /// </summary>
        internal void Reset()
        {
            m_executed = 0;
        }

        /// <summary>
        /// Adds a child to this chain. The child also registers this chain
        /// as a parent
        /// </summary>
        /// <param name="child">The child execution chain</param>
        internal void AddChild(ExecutionChain child, Action<State, State> adapter)
        {
            Children.Add(child);
            child.Parents.Add(this);

            m_adapters.Add(new StateAdapter(AbstractController, child.AbstractController, adapter));
        }

        #endregion

        #region Execution

        /// <summary>
        /// Called by a parent chain when it has finished executing. If
        /// all parents have finished executing, this chain will begin
        /// executing
        /// </summary>
        internal void NotifyReady()
        {
            m_executed += 1;
            if (m_executed >= Parents.Count) {
                Execute();
                Reset();
            }
        }

        /// <summary>
        /// Notifies all children that this chain has executed
        /// </summary>
        internal void Execute()
        {
            if (AbstractController.Update()) {
                foreach (var adapter in m_adapters) {
                    adapter.Adapt();
                }
            }

            foreach (var child in Children) {
                child.NotifyReady();
            }
        }

        #endregion
    }
}