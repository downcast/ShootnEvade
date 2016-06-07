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
	interface ICollisionResponse
	{
		/// <summary>
		/// Called when an object collides another
		/// </summary>
		/// <param name="o">Object that was collided with</param>
		void CollisionResponse(Object o);
	}
}