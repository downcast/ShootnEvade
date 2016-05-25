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
    public class Object : Android.Widget.ImageView
    {
        private int lives { get; set; }

        public Object(Context context) : base(context)
        { }

        public virtual void move()
        { }

        public virtual void response()
        { }
    }
}