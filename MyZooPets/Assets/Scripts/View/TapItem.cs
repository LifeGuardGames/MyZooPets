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

	Optional:
		4) Assign any effects to OnStart and OnFinish.
			(OnFinish does not necessarily mean the tap was executed properly; use OnTap for this)

	What this does:
		OnTap will be called when a tap on the object is detected.
*/
// ================================================================================================

public delegate void TapEventHandler();

public class TapItem : MonoBehaviour {
	public event TapEventHandler OnTap;
	public event TapEventHandler OnStart;
	public event TapEventHandler OnFinish;
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
			//couldn't this be done with tapGesture.ScreenPosition?
		}
		if (e.State == Gesture.GestureState.Began){
        	if (OnStart != null) OnStart();
		}
		else if (e.State == Gesture.GestureState.Recognized)
		{
        	if (OnTap != null) OnTap(); 
        	if (OnFinish != null) OnFinish();
		}
		else if (e.State == Gesture.GestureState.Ended ||
				 e.State == Gesture.GestureState.Cancelled ||
				 e.State == Gesture.GestureState.Failed)
		{
        	if (OnFinish != null) OnFinish();
		}
	}
}