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
using Android.Util;
using Android.Graphics;

namespace ShootnEvade.Droid
{
	/// <summary>
	/// Responsible for checking collisions of all game objects known to the State's gameObject list. 
	/// Currently the class will notify all parties involved in a collision and the object can choose to ignore it.
	/// We will want to add an event to notify other classes of specific collisions. Such notificatio nmost likely be critically needed by the AI system
	/// </summary>
    class CollisionDetection : IClockTick
    {
		private static CollisionDetection _instance;

		private CollisionDetection()
		{
			Clock.Instance.AddSubscriber(this, 25);
		}

		public static CollisionDetection Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CollisionDetection();
				}
				return _instance;
			}
		}

		public void ClockTick25()
		{
			// Begin checking for collisions
			CheckCollisions();
		}

		/// <summary>
		/// Gathers state information, determines which objects have collided and notifies
		/// </summary>
		private void CheckCollisions()
		{
			// TODO: Will most likey need a lock for the state list
			// If two object collide, call Object.CollisionResponse(object_that_this_object_collided_with)

			// TODO: Replace with the state list
			List<Object> list = new List<Object>();

			Rect first = new Rect();
			Rect second = new Rect();

			for (int i = 0; i < list.Count; i++)
			{
				first.Left = list[i].Left;
				first.Right = list[i].Right;
				first.Top = list[i].Top;
				first.Bottom = list[i].Bottom;

				for (int j = i + 1; j < list.Count; j++)
				{
					second.Left = list[j].Left;
					second.Right = list[j].Right;
					second.Top = list[j].Top;
					second.Bottom = list[j].Bottom;

					if (Rect.Intersects(first, second))
					{
						list[i].CollisionResponse(list[j]);
						list[j].CollisionResponse(list[i]);
					}
				}
			}
		}
	}
}