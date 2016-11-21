/* Sean Duane
 * TouchDetectorManager.cs
 * 8:26:2013   14:39
 * Description:
 * Handles all input.
 * Right now that' just touches, so you can dig into the TouchPhase switch in Update()
 * Whenever a swipe happens, we let every script that is "lestening to us" know by sending an approprait
 * message to them.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchDetectorManager : MonoBehaviour {
	public List<MonoBehaviour> ListeningScripts = new List<MonoBehaviour>();

	void OnSwipe(SwipeGesture gesture) {
		// Approximate swipe direction
		FingerGestures.SwipeDirection direction = gesture.Direction; 
		if (direction == FingerGestures.SwipeDirection.Down) {
			foreach (MonoBehaviour currentScript in ListeningScripts)
				currentScript.SendMessage("onSwipeDown", SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnTap(TapGesture gesture) {
		foreach (MonoBehaviour currentScript in ListeningScripts)
			currentScript.SendMessage("onTap", SendMessageOptions.DontRequireReceiver);
	}
}
