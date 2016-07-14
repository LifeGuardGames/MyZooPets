using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Lg inhaler animation event handler.
/// This class listens to Animation Events send out by the inhaler animations.
/// Then it sends out the appropriate C# events.
/// </summary>
public class LgInhalerAnimationEventHandler : MonoBehaviour {
	public static EventHandler<EventArgs> BreatheOutEndEvent;
	public static EventHandler<EventArgs> BreatheInEndEvent;
	public static EventHandler<EventArgs> InhalerHappy1EndEvent;


	public void InhalerAnimationEvent(string eventName){
		Debug.Log("ANIMATE");
		switch(eventName){
		case "BreatheOut":
			if(BreatheOutEndEvent != null)
				BreatheOutEndEvent(this, EventArgs.Empty);
			break;
		case "BreatheIn":
			if(BreatheInEndEvent != null)
				BreatheInEndEvent(this, EventArgs.Empty);
			break;
		case "InhalerHappy1":
			if(InhalerHappy1EndEvent != null)
				InhalerHappy1EndEvent(this, EventArgs.Empty);
			break;
		}
	}
}
