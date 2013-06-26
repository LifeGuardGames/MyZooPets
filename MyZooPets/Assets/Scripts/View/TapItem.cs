using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class TapItem : MonoBehaviour {

	public delegate void OnTapCallback();
	public OnTapCallback OnTap;

	void Start()
	{
		GetComponent<TapGesture>().StateChanged += HandleStateChanged;
	}

	void HandleStateChanged(object sender, TouchScript.Events.GestureStateChangeEventArgs e)
	{
		if (e.State == Gesture.GestureState.Recognized)
		{
			OnTap();
		}
	}
}