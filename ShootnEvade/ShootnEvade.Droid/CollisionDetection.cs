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

namespace ShootnEvade.Droid
{
    class CollisionDetection
    {
        private static CollisionDetection sInstance = null;
        private bool mIsDetectingBounds;
        // Was based on the game loop architecture; now use it for control of when it is allowed to detect
        private bool mIsDetectingTouch;
        private int mClockId;
        private Clock mClock;
        //array list 
        //private ArrayList list = new ArrayList(); event handler in C#
        private ArrayList<OnOutOfBounds> mBoundsListener;
        private ArrayList<OnCollide> mCollideListener;

        private ArrayList<GameObject> mLostObjs;


        public static CollisionDetection getInstance()
        {
            if (sInstance == null)
            {
                sInstance = new CollisionDetection();
            }
            return sInstance;
        }

        private CollisionDetection()
        {
            mIsDetectingBounds = false;
            mIsDetectingTouch = false;
            //make Clock singleton
            //mClock = Clock.getInstance();
            mClockId = -1;

            mLostObjs = new ArrayList<>();
            mCollideListener = new ArrayList<>();
            mBoundsListener = new ArrayList<>();
        }

        // Is no longer called repeatedly
        public void detectCollisions()
        {

            // First time; start clock.
            if (mClockId == -1)
            {
                mClockId = mClock.watchClock();
                mIsDetectingTouch = true;
                mIsDetectingBounds = true;

                // Not bounds tested yet and within time range.
            }
            else if (mClock.getTimePassed(mClockId) < MainActivity.MPF && mIsDetectingBounds)
            {
                checkOutOfBounds();

                // Frame is over; restart isDetectings
            }
            else if (mClock.getTimePassed(mClockId) >= GameActivity.MPF)
            {
                mClock.startClock(mClockId);
                mIsDetectingTouch = true;
                mIsDetectingBounds = true;
            }
        }

        public void setBoundsListener(OnOutOfBounds listener) { mBoundsListener.add(listener); }

        public void removeBoundsListener(OnOutOfBounds listener) { mBoundsListener.remove(listener); }

        public void setCollideListener(OnCollide listener) { mCollideListener.add(listener); }

        public void removeCollideListener(OnCollide listener) { mCollideListener.remove(listener); }

     
    public void onTouchEvent(int motionEventX, int motionEventY)
        {

           
           
                    
                    

                        try
                        {
                            Log.Debug("TOUCH DETECTING", "Obtained lock");
                            //this is the objects
                            //Rect outRect = new Rect();

                            int[] location = new int[2];

                            // Check for user touch events
                            for (Iterator<GameObject> iterator = GameObjectManager.getInstance(null, null).getGameObjectList().iterator(); iterator.hasNext();)
                            {
                                if (iterator.hasNext())
                                {
                                    GameObject gameObject = iterator.next();

                                    Log.d("TOUCH DETECTING", "Checking Game Object");
                                    // Gets the bounds with points as if view was attached to the upper left corner
                                    gameObject.getDrawingRect(outRect);
                                    // Grab the supposed top let position of the view on the screen (the y point is not equal to the motion event raw y or normal y)
                                    gameObject.getLocationOnScreen(location);

                                    // Adjust the bounds to match the true location of the view.
                                    outRect.offset(location[0], location[1]);

                                    if (outRect.contains(motionEventX, motionEventY))
                                    {
                                        Log.d("TOUCH DETECTING", "BOOM " + gameObject.getId());

                                        switch (gameObject.getState())
                                        {
                                            case ACTIVE:
                                                for (OnCollide listener : mCollideListener)
                                                {
                                                    listener.onCollide(gameObject);
                                                }
                                                break;
                                        }
                                    }
                                    // This is here because without it, the detection does not work
                                    outRect.setEmpty();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            e.printStackTrace();
                        }
                        finally
                        {
                            GameObjectManager.getInstance(null, null).getGameObjectListLock().unlock();
                            Log.d("BOUNDS DETECTING", "Unlocked");
                        }
                    
              
        }

        /**
         * Checks for "Active" GameObjects that have moved off screen. "Passive" and "Terminated" Objects are not considered.
         */
        public void checkOutOfBounds()
        {

            Log.d("BOUNDS DETECTING", "DETECTOR BOUNDS CHECK");
            if (mClockId != -1)
            {
                Log.d("BOUNDS DETECTING", "Good clock");
                if (mIsDetectingBounds && mClock.getTimePassed(mClockId) > GameActivity.MPF)
                {
                    Log.d("BOUNDS DETECTING", "isDetecting && timepassed");
                    if (GameObjectManager.getInstance(null, null).getGameObjectListLock().tryLock())
                    {
                        try
                        {
                            Log.d("BOUNDS DETECTING", "Obtained lock");
                            Rect outRect = new Rect();
                            int[] location = new int[2];

                            for (Iterator<GameObject> iterator = GameObjectManager.getInstance(null, null).getGameObjectList().iterator(); iterator.hasNext();)
                            {
                                if (iterator.hasNext())
                                {
                                    Log.d("BOUNDS DETECTING", "Grabbing Game Object");
                                    GameObject gameObject = iterator.next();
                                    switch (gameObject.getState())
                                    {
                                        case ACTIVE:
                                            Log.d("BOUNDS DETECTING", "Active Game Object");
                                            // Gets the bounds with points as if view was attached to the upper left corner
                                            gameObject.getDrawingRect(outRect);
                                            // Grab the supposed top let position of the view on the screen (the y point is not equal to the motion event raw y or normal y)
                                            gameObject.getLocationOnScreen(location);

                                            // Adjust the bounds to match the true location of the view.
                                            outRect.offset(location[0], location[1]);

                                            // STOPS the bounds tester from removing objects that are below the screen but have not been laucnhed yet
                                            if (((ObjectAnimator)gameObject.getAnimatorSet().getChildAnimations().get(0)).getCurrentPlayTime() > 1000)
                                            {
                                                if (outRect.left > GameActivity.sWindowDimensions.x || outRect.top > GameActivity.sWindowDimensions.y)
                                                {
                                                    Log.d("BOUNDS DETECTING", "Boom");
                                                    mLostObjs.add(gameObject);
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            for (OnOutOfBounds listener : mBoundsListener)
                            {
                                for (GameObject g : mLostObjs)
                                {
                                    Log.d("BOUNDS DETECTING", "Listeners are notified");
                                    listener.onOutOfBounds(g);
                                }
                            }
                            mLostObjs.clear();
                        }
                        catch (Exception e)
                        {
                            e.printStackTrace();
                        }
                        finally
                        {
                            GameObjectManager.getInstance(null, null).getGameObjectListLock().unlock();
                            Log.d("BOUNDS DETECTING", "Unlocked");
                        }
                    }
                }
            }
        }
    }
}