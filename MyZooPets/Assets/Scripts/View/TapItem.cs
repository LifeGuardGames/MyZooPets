using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

// ================================================================================================
/*
	What it does:
		Recognizes a tap on a GameObject, and only a tap.
		Hence, there is not need to worry about other actions being mistakenly treated as a tap.
		Eg. "Slide-ins" (MouseUp on the GameObject, but MouseDown happened outside).

	To use TapItem:
		1) Attach TapItem to a GameObject
		2) Attach TapGesture (from TouchScript.Gestures)
		3) Add a function (using +=, not =) to OnTap (in another script).
*/
// ================================================================================================

public delegate void TapEventHandler();

public class TapItem : MonoBehaviour {

	public event TapEventHandler OnTap;

    protected virtual void HandleTap() {
        if (OnTap != null) OnTap();
    }

	void Start()
	{
		GetComponent<TapGesture>().StateChanged += HandleStateChanged;
	}

	void HandleStateChanged(object sender, TouchScript.Events.GestureStateChangeEventArgs e)
	{
		if (e.State == Gesture.GestureState.Recognized)
		{
			HandleTap();
		}
	}
}