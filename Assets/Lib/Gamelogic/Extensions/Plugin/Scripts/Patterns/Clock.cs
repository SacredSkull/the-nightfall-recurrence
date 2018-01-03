// Copyright Gamelogic (c) http://www.gamelogic.co.za

using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// To use this clock, instantiate it, call Reset with the right time value, and call Update it each frame.
	/// 
	/// Any class that wants to be notified of events need to implement the IClockListener interface,
	/// and subscribe to events using the AddListener method. A listener can be removed with the RemoveListener
	/// event.
	/// 
	/// Clocks can be paused independently of Time.timeScale using the Pause method (and started again using Unpause).
	/// </summary>
	[Version(1, 2)]
	public class Clock
	{
		#region Private Fields

		private float time;
		private int timeInSeconds;

		public event System.Action OnClockExpired;
		public event System.Action OnSecondsChanged; 

		#endregion

		#region  Properties

		public bool IsPaused
		{
			get; private set;
		}

		public bool IsDone
		{
			get; private set;
		}

		public float Time
		{
			get
			{
				return time;
			}
		}

		public int TimeInSeconds
		{
			get
			{
				return timeInSeconds;
			}
		}

		#endregion

		#region Constructors

		public Clock()
		{
			//listeners = new List<IClockListener>();
			IsPaused = true;
			Reset(0);
		}

		#endregion

		#region Unity Callbacks

		public void Update(float deltaTime)
		{
			if (IsPaused) return;
			if (IsDone) return;
		
			time -= deltaTime;

			CheckIfTimeInSecondsChanged();
			CheckIfClockExpired();
		}

		#endregion

		#region Public Methods

		public void AddTime(float timeIncrement)
		{
			time += timeIncrement;

			CheckIfClockExpired();
		}

		public void Reset(float startTime)
		{
			time = startTime;
			IsDone = false;
			CheckIfTimeInSecondsChanged();
		}

		public void Unpause()
		{
			IsPaused = false;
		}

		public void Pause()
		{
			IsPaused = true;
		}

		#endregion

		#region Private Methods

		private void CheckIfTimeInSecondsChanged()
		{
			var newTimeInSeconds = (int)time;

			if (newTimeInSeconds == timeInSeconds) return;
		
			timeInSeconds = newTimeInSeconds;

			if (OnSecondsChanged != null)
			{
				OnSecondsChanged();
			}
		}

		private void CheckIfClockExpired()
		{
			if (time <= 0)
			{
				time = 0;
				IsDone = true;

				if (OnClockExpired != null)
				{
					OnClockExpired();
				}
			}
		}

		#endregion
	}
}