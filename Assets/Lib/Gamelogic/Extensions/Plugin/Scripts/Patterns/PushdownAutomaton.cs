// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System.Collections.Generic;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// This class is a state machine that has the ability to remember previous states
	/// and transition back to them (FIFO).
	/// </summary>
	/// <typeparam name="TLabel">The type of state labels.</typeparam>
	[Version(1, 4)]
	public class PushdownAutomaton<TLabel> : StateMachine<TLabel>
	{
		#region Private Fields

		private readonly Stack<TLabel> stateHistory;

		#endregion

		#region Constructors

		public PushdownAutomaton()
		{
			stateHistory = new Stack<TLabel>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Pushes the current state onto the stack, and transitions to the next state.
		/// </summary>
		/// <param name="nextState"></param>
		public void Push(TLabel nextState)
		{
			stateHistory.Push(CurrentState);
			
			CurrentState = nextState;
		}

		/// <summary>
		/// Pops a state from the stack and switches to it.
		/// </summary>
		public void Pop()
		{
			if (stateHistory.Count > 0)
			{
				CurrentState = stateHistory.Pop();
			}
		}

		#endregion
	}
}