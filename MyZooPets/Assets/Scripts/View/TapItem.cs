using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

// ================================================================================================
/*
	TapItem:

	What it does:
		Recognizes a tap on a GameObject, and only a tap.
		Hence, there is not need to worry about other actions being mistakenly treated as a tap.
		Eg. "Slide-ins" (MouseUp on the GameObject, but MouseDown happened outside).

	To use TapItem:
		1) Attach TapItem to a GameObject
		2) Attach TapGesture (from TouchScript.Gestures)
		3) Add a function (using +=, not =) to OnTap (in another script).

	What this does:
		OnTap will be called when a tap on the object is detected.
*/
// ================================================================================================

public delegate void TapEventHandler();

public class TapItem : MonoBehaviour {
	public event TapEventHandler OnTap;
	public Vector2 lastTapPosition;

	private TapGesture tapGesture;

	void Start()
	{
		tapGesture = GetComponent<TapGesture>();
		tapGesture.StateChanged += HandleStateChanged;
	}

	void HandleStateChanged(object sender, TouchScript.Events.GestureStateChangeEventArgs e)
	{
		if (tapGesture.ActiveTouches.Count > 0){
			lastTapPosition = tapGesture.ActiveTouches[0].Position;
		}
		if (e.State == Gesture.GestureState.Recognized)
		{
        	if (OnTap != null) OnTap();
		}
	}
}