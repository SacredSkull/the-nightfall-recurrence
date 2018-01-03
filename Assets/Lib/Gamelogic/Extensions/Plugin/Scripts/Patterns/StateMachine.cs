// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System.Collections.Generic;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// A lightweight state machine.
	/// </summary>
	/// <remarks>
	/// 	<para>To use it: </para>
	/// 	<list type="bullet">
	/// 		<item>
	/// 			<description>Define your own label. Enums are probably the best
	/// choice.</description>
	/// 		</item>
	/// 		<item>
	/// 			<description>Construct a new state machine, typically in a
	/// MonoBehaviour's Start method.</description>
	/// 		</item>
	/// 		<item>
	/// 			<description>Add the various states with the appropriate delegates.
	/// </description>
	/// 		</item>
	/// 		<item>
	/// 			<description>Call the state machine's Update method from the
	/// MonoBehaviour's Update method.</description>
	/// 		</item>
	/// 		<item>
	/// 			<description>Set the CurrentState property on the state machine to transition.
	/// (You can eitther set it from one of the state delegates, or from anywhere else.
	/// </description>
	/// 		</item>
	/// 	</list>
	/// 	<para>When a state is changed, the OnStop on existing state is called, then the
	/// OnStart of the new state, and from there on OnUpdate of the new state each time
	/// the update is called.</para>
	/// </remarks>
	/// <typeparam name="TLabel">The label type of this state machine. Enums are common,
	/// but strings or int are other possibilities.</typeparam>
	[Version(1)]
	public class StateMachine<TLabel>
	{
		#region  Types

		private class State
		{
		    #region Public Fields

		    public readonly TLabel label;
		    public readonly System.Action onStart;
		    public readonly System.Action onStop;
		    public readonly System.Action onUpdate;

		    #endregion

		    #region Constructors

		    public State(TLabel label, System.Action onStart, System.Action onUpdate, System.Action onStop)
			{
				this.onStart = onStart;
				this.onUpdate = onUpdate;
				this.onStop = onStop;
				this.label = label;
			}

		    #endregion
		}

		#endregion

		#region Private Fields

		private readonly Dictionary<TLabel, State> stateDictionary;
		private State currentState;

		#endregion

		#region  Properties

		/// <summary>
		/// Returns the label of the current state.
		/// </summary>
		public TLabel CurrentState
		{
			get { return currentState.label; }

			[Version(1, 2)]
			set { ChangeState(value); }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new StateMachine.
		/// </summary>
		public StateMachine()
		{
			stateDictionary = new Dictionary<TLabel, State>();
		}

		#endregion

		#region Unity Callbacks

		/// <summary>
		/// This method should be called every frame. 
		/// </summary>
		public void Update()
		{
			if (currentState != null && currentState.onUpdate != null)
			{
				currentState.onUpdate();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a state, and the delegates that should run 
		/// when the state starts, stops, 
		/// and when the state machine is updated.
		/// 
		/// Any delegate can be null, and wont be executed.
		/// </summary>
		/// <param name="label">The name of the state to add.</param>
		/// <param name="onStart">The action performed when the state is entered.</param>
		/// <param name="onUpdate">The action performed when the state machine is updated in the given state.</param>
		/// <param name="onStop">The action performed when the state machine is left.</param>
		public void AddState(TLabel label, System.Action onStart, System.Action onUpdate, System.Action onStop)
		{
			stateDictionary[label] = new State(label, onStart, onUpdate, onStop);
		}

		public void AddState(TLabel label, System.Action onStart, System.Action onUpdate)
		{
			AddState(label, onStart, onUpdate, null);
		}

		public void AddState(TLabel label, System.Action onStart)
		{
			AddState(label, onStart, null);
		}

		public void AddState(TLabel label)
		{
			AddState(label, null);
		}

		/// <summary>
		/// Adds a sub state machine for the given state.
		///
		/// The sub state machine need not be updated, as long as this state machine
		/// is being updated.
		/// </summary>
		/// <typeparam name="TSubstateLabel">The type of the submachine.</typeparam>
		/// <param name="label">The name of the state to add.</param>
		/// <param name="subMachine">The submachine that will run during the given state.</param>
		/// <param name="subMachineStartState">The starting state of the submachine.</param>
		[Version(1, 4)]
		public void AddState<TSubstateLabel>(TLabel label, StateMachine<TSubstateLabel> subMachine,
			TSubstateLabel subMachineStartState)
		{
			AddState(
				label,
				() => subMachine.ChangeState(subMachineStartState),
				subMachine.Update);
		}

		/// <summary>
		/// This method should be called every frame. 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return CurrentState.ToString();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Changes the state from the existing one to the state with the given label.
		/// 
		/// It is legal (and useful) to transition to the same state, in which case the 
		/// current state's onStop action is called, the onstart ation is called, and the
		/// state keeps on updating as before. The behviour is exactly the same as switching to
		/// a new state.
		/// </summary>
		/// <param name="newState"></param>
		private void ChangeState(TLabel newState)
		{
			if (currentState != null && currentState.onStop != null)
			{
				currentState.onStop();
			}

			currentState = stateDictionary[newState];

			if (currentState.onStart != null)
			{
				currentState.onStart();
			}
		}

		#endregion
	}
}