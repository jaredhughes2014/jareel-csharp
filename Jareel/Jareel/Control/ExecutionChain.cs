using System.Collections.Generic;
using System;

namespace Jareel
{
    /// <summary>
    /// Class designed to handle the chaining logic of execution chains. This class
    /// does not handle the execution of system controllers
    /// </summary>
    public abstract class AbstractExecutionChain
    {
        /// <summary>
        /// The number of chains this chain is waiting on to begin executing
        /// </summary>
        internal List<AbstractExecutionChain> Parents { get; private set; }

        /// <summary>
        /// The chains that are children of this chain
        /// </summary>
        internal List<AbstractExecutionChain> Children { get; private set; }

        /// <summary>
        /// The abstract controller that executes at this execution chain
        /// </summary>
        public AbstractController AbstractController { get; set; }

        /// <summary>
        /// The number of parent chains that have executed
        /// </summary>
        private int m_executed;

        /// <summary>
        /// Initializes this chain
        /// </summary>
        public AbstractExecutionChain()
        {
            Children = new List<AbstractExecutionChain>();
            Parents = new List<AbstractExecutionChain>();
        }

        /// <summary>
        /// Resets this chain to its initial state
        /// </summary>
        public void Reset()
        {
            m_executed = 0;
        }

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
        /// Adds a child to this chain. The child also registers this chain
        /// as a parent
        /// </summary>
        /// <param name="child">The child execution chain</param>
        public void AddChild(AbstractExecutionChain child)
        {
            Children.Add(child);
            child.Parents.Add(this);
        }

        /// <summary>
        /// Notifies all children that this chain has executed
        /// </summary>
        public virtual void Execute()
        {
            foreach (var child in Children) {
                child.NotifyReady();
            }
        }
    }

    /// <summary>
    /// Extension of the abstract execution chain designed to control the execution
    /// of system controllers.
    /// </summary>
    public class ExecutionChain<S, C> : AbstractExecutionChain where S : State, new() where C : StateController<S>, new()
    {
        /// <summary>
        /// The controller held within this chain
        /// </summary>
        public C Controller
        {
            get { return (C)AbstractController; }
            set { AbstractController = value; }
        }

        /// <summary>
        /// The adapters this chain 
        /// </summary>
        private List<AbstractStateAdapter> m_adapters;

        /// <summary>
        /// Creates a new execution chain. This will automatically generate a starting
        /// state and a controler for this execution chain
        /// </summary>
        public ExecutionChain()
        {
            Controller = new C();
            Controller.AbstractState = new S();

            m_adapters = new List<AbstractStateAdapter>();
        }

        /// <summary>
        /// Adds an adapter function to a child execution chain. When this chain finishes
        /// execution, this adapter function will be executed before the child is able to
        /// execute
        /// </summary>
        /// <typeparam name="T">State type of the child chain</typeparam>
        /// <typeparam name="D">Controller type of the child chain</typeparam>
        /// <param name="child">Execution chain that is dependent on this chain to execute</param>
        /// <param name="adapter">Function called to perform the adaptation</param>
        internal void AddAdapter<T, D>(ExecutionChain<T, D> child, Action<S, T> adapter)
            where T : State, new() where D : StateController<T>, new()
        {
            AddChild(child);
            m_adapters.Add(new StateAdapter<S, T>(Controller, child.Controller, adapter));
        }

        /// <summary>
        /// Executes the control update for this chain, performs all state adaptations
        /// for child execution chains, then notifies those chains that this parent has
        /// terminated
        /// </summary>
        public override void Execute()
        {
            Controller.Update();
            foreach (var adapter in m_adapters) {
                adapter.Adapt();
            }

            base.Execute();
        }

        /// <summary>
        /// Creates a new execution chain that is dependent on the given parent chain. This new chain
        /// will wait for the given parent chain to execute before it will execute, and will use the
        /// given action to adapt the parent chain's state data into its own state.
        /// 
        /// This is a convenience function for creating a new chain (without spawning one within this
        /// controller) and joining it with the parent.
        /// </summary>
        /// <typeparam name="T">State type of the child controller</typeparam>
        /// <typeparam name="D">Controller type of the child chain</typeparam>
        /// <param name="adapter">Function called to adapt the parent chain's state to the child state</param>
        /// <returns>New execution chain operating with a controller of type D</returns>
        internal ExecutionChain<T, D> Branch<T, D>(Action<S, T> adapter)
            where T : State, new() where D : StateController<T>, new()
        {
            var child = new ExecutionChain<T, D>();
            return Join(child, adapter);
        }

        /// <summary>
        /// Joins an execution chain onto another chain. This is identical to Branch, except it uses
        /// an already existing chain as the child instead of creating a new chain.
        /// 
        /// This should never be called on a chain that was spawned as a starting chain. This function
        /// is a way to add additional dependencies to a chain that was branched from an existing chain
        /// </summary>
        /// <typeparam name="T">State type of the child controller</typeparam>
        /// <typeparam name="D">Controller type of the child chain</typeparam>
        /// <param name="child">Chain being added to the parent as a child</param>
        /// <param name="adapter">Function called to adapt the parent chain's state to the child state</param>
        /// <returns>New execution chain operating with a controller of type D</returns>
        internal ExecutionChain<T, D> Join<T, D>(ExecutionChain<T, D> child, Action<S, T> adapter)
            where T : State, new() where D : StateController<T>, new()
        {
            AddAdapter(child, adapter);
            return child;
        }
    }
}