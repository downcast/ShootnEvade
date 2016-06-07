using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System.Threading;
using System.Diagnostics;

namespace ShootnEvade.Droid
{
	/// <summary>
	/// Provides time keeping services for the application. It is used for keeping track of game time and real time 
	/// and is responsible for notifying subscribers of time related events. It does not guarentee precise notification times. It will notify on the smallest interval given by the subscribers in future
	/// 
	/// The class utilizes another Thread to keep track of how much time has passed. 
	/// It uses a high precision timer to account to possible discrepancies when Thread is put to sleep for efficiency.
	/// 
	/// Typical use case is for subscriber to get instance of clock, call addSubscriber and the last subscriber should call startclock. In future versions, this flow will be updated
	/// </summary>
	class Clock
	{
		private static Clock _instance;
		private volatile List<ClockSubscriber> Subscribers;         // TODO: Will need a object lock if multiple people are modifying the list
		private volatile bool _isRunning;							// We probably need to a way to end a thread, hence the isRunning; however, the clock will stop when the app stops, so we dont need to manualy sto the clock
		public bool IsRunning
		{
			get { return _isRunning; }
			set { _isRunning = value; }
		}

		private Clock()
		{
			Subscribers = new List<ClockSubscriber>();
			IsRunning = false;
		}

		public static Clock Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new Clock();
				}
				return _instance;
			}
		}

		/// <summary>
		/// Register to listen for notification on certain interval
		/// </summary>
		/// <param name="sub">Class who will recieve the callback when interval is reached</param>
		/// <param name="interval">Time in millisecond to wait before notifying sub</param>
		public void AddSubscriber(IClockTick sub, int interval)
		{
			this.Subscribers.Add(new ClockSubscriber(sub, interval));
		}

		public void StartClock()
		{
			if (!_isRunning)
			{
				_isRunning = true;
				Thread thread = new Thread(EvaluateElaspedTime);
				thread.IsBackground = true;                         // This will let the thread die when the application exits
				thread.Start();
			}
		}

		private void EvaluateElaspedTime()
		{
			// Assumes all subscribers are registered and none will be added later
			// Start the clock for each subscriber
			foreach (var item in Subscribers) { item.Watch.Start(); }

			while (_isRunning)
			{
				Thread.Sleep(25);
				foreach (var item in Subscribers)
				{
					if(item.Watch.ElapsedMilliseconds > item.Interval)
					{
						// Notify subscriber and restart watch
						item.Watch.Restart();
						item.Callback.ClockTick25();
					}
				}
			}
		}

		private class ClockSubscriber
		{
			public IClockTick Callback { get; set; }
			public Stopwatch Watch { get; }
			public int Interval { get; set; }

			public ClockSubscriber(IClockTick call, int interval)
			{
				this.Callback = call;
				this.Interval = interval;
				Watch = new Stopwatch();
			}
		}
	}
}