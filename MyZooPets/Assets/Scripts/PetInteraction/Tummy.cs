using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class Tummy : MonoBehaviour {
    private FlickGesture flickGesture;
    private TapGesture tapGesture;

    // Use this for initialization
    void Start () {
        flickGesture = GetComponent<FlickGesture>();
        flickGesture.StateChanged += FlickHandleStateChanged;
        tapGesture = GetComponent<TapGesture>();
        tapGesture.StateChanged += TapHandleStateChange;
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    private void TapHandleStateChange(object sender, TouchScript.Events.GestureStateChangeEventArgs e){
        if(e.State == Gesture.GestureState.Recognized){
            SpeechBubbleControllerTK2D.Instance.Talk("that's my tummy");
        }
    }

    private void FlickHandleStateChanged(object sender, TouchScript.Events.GestureStateChangeEventArgs e){
        if (e.State == Gesture.GestureState.Recognized){
            float angle = Mathf.Rad2Deg * Mathf.Atan2(flickGesture.ScreenFlickVector.x, flickGesture.ScreenFlickVector.y);

            angle = (360 + angle - 45) % 360;
            if (angle < 90) { //swipe right
                SpeechBubbleControllerTK2D.Instance.Talk("hehehe");
            } else if (angle < 180) { //swipe down
                SpeechBubbleControllerTK2D.Instance.Talk("boooo");
            } else if (angle < 270) { //swipe left
                SpeechBubbleControllerTK2D.Instance.Talk("I like to move it move it");
            } else { //swipe up
                SpeechBubbleControllerTK2D.Instance.Talk("having fun?");
            }
        }
    }
}
