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
	/// Base object for the application. Every object visble to the user has three properties: Lives (how many hits an object takes before destruction), 
	/// Move (how and where to move on the screen) and Response (how to respond in event of collision)
	/// </summary>
    public class Object : Android.Widget.ImageView, ICollisionResponse
	{
        private int movementSpeed = 33;
        private int posX = 0;
        private int posY = 0;

        private int lives { get; set; }

        public Object(Context context) : base(context)
        { }

        //this would inherit from the AI system in determining the speed of the object or characters.
        public virtual void speed(int speeds)
        {
            movementSpeed = speeds;
        }

        //move and response would inherite from the main function since its everything and tell
        //the function the movement and responce time.
        public virtual void move(int positionX, int positionY)
        {
            posX = positionX;
            posY = positionY;
        }

		public void CollisionResponse(Object o)
		{
			// Repond to object collision
		}
	}
}