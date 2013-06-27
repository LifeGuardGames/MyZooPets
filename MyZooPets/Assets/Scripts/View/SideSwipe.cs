using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

// ================================================================================================
/*
    SideSwipe:

    What it does:

    To use SideSwipe:
        1) Attach SideSwipe to a GameObject
        2) Attach FlickGesture (from TouchScript.Gestures)
            !) Make sure to set the FlickGesture script's direction
               from "Any" to "Horizontal"!
        3) Add a function (using +=, not =) to OnSwipe (in another script).


    What this does:
*/
// ================================================================================================

public delegate void SwipeEventHandler();

public class SideSwipe : MonoBehaviour {
    public event SwipeEventHandler OnSwipe;

    private FlickGesture flickGesture;

    void Start()
    {
        flickGesture = GetComponent<FlickGesture>();
        flickGesture.StateChanged += HandleStateChanged;
        OnSwipe += Swiped;
    }
    void HandleStateChanged(object sender, TouchScript.Events.GestureStateChangeEventArgs e)
    {
        if (e.State == Gesture.GestureState.Recognized)
        {
            if (OnSwipe != null) OnSwipe();
        }
    }

    void Swiped(){
        // print("flicked");
        print(flickGesture.ScreenFlickVector.x);

        if (flickGesture.ScreenFlickVector.x > 0){
            // flick was from left to right

        }
        else {
            // flick was from right to left

        }

    }
}