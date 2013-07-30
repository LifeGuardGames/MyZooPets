using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

//Handles any touch gestures on the pets head
public class Head : MonoBehaviour {
    private FlickGesture flickGesture;
    private TapGesture tapGesture;

	// Use this for initialization
	void Start () {
        flickGesture = GetComponent<FlickGesture>();
        flickGesture.StateChanged += FlickHandleStateChanged;
        tapGesture = GetComponent<TapGesture>();
        tapGesture.StateChanged += TapHandleStateChange;
	}

    private void TapHandleStateChange(object sender, TouchScript.Events.GestureStateChangeEventArgs e){
        if(e.State == Gesture.GestureState.Recognized){
            PetSpeech.Instance.Talk("tickle!!");
        }
    }

    private void FlickHandleStateChanged(object sender, TouchScript.Events.GestureStateChangeEventArgs e){
        if (e.State == Gesture.GestureState.Recognized){
            float angle = Mathf.Rad2Deg * Mathf.Atan2(flickGesture.ScreenFlickVector.x, flickGesture.ScreenFlickVector.y);

            angle = (360 + angle - 45) % 360;
            if (angle < 90) { //swipe right
                PetSpeech.Instance.Talk("nice move");
            } else if (angle < 180) { //swipe down
                PetSpeech.Instance.Talk("aww yeah");
            } else if (angle < 270) { //swipe left
                PetSpeech.Instance.Talk("keep that moving");
            } else { //swipe up
                PetSpeech.Instance.Talk("aint nobody got time for that");
            }
        }
    }
}
