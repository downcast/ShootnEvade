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

namespace ShootnEvade.Droid
{
	/// <summary>
	/// Classes that wish to be notified every 25 millisecond will implement callback
	/// </summary>
	interface IClockTick
	{
		void ClockTick25();
	}
}