using UnityEngine;
using System;
using System.Collections;

public class TutorialSwipeListener : MonoBehaviour{
    public EventHandler<EventArgs> OnTutorialSwiped;

    void OnSwipe(SwipeGesture gesture){
       FingerGestures.SwipeDirection direction = gesture.Direction; 

       if(direction == FingerGestures.SwipeDirection.Left){
            PanToMoveCamera scriptPan = CameraManager.Instance.PanScript;
            scriptPan.TutorialSwipeLeft();

            if(OnTutorialSwiped != null)
                OnTutorialSwiped(this, EventArgs.Empty);
       }
    }
}