using UnityEngine;
using System.Collections;

public class HorizontalSlideInYard : MonoBehaviour {

    float currentXPos;
    public float slideIncrement = 40f;
    bool inverse = true;
    Hashtable optional = new Hashtable();
    bool lockSlide = false;
    // Use this for initialization
    void Start () {
        currentXPos = transform.position.x;
        optional.Add("onComplete", "FinishedSlide");
        if (inverse){
            slideIncrement = - slideIncrement;
        }

        // Init swipe listener.
        SwipeDetection.OnSwipeDetected += OnSwipeDetected;
    }

    public void SlideRight(){
        if (!lockSlide){
            // if (ClickManager.CanRespondToTap()){
                lockSlide = true;
                currentXPos += slideIncrement;
                LeanTween.moveX(gameObject, currentXPos, 1.0f, optional);
            // }
        }
    }

    public void SlideLeft(){
        if (!lockSlide){
            // if (ClickManager.CanRespondToTap()){
                lockSlide = true;
                currentXPos -= slideIncrement;
                LeanTween.moveX(gameObject, currentXPos, 1.0f, optional);
            // }
        }
    }

    void FinishedSlide(){
        lockSlide = false;
        currentXPos = transform.position.x;
    }

    // function to pass to Swipe Listener
    void OnSwipeDetected(Swipe s){
        switch (s){
            // case Swipe.Up:
            // print("Swipe.Up");
            // break;

            // case Swipe.Down:
            // print("Swipe.Down");
            // break;

            case Swipe.Left:
            print("Swipe.Left");
            SlideLeft();
            break;

            case Swipe.Right:
            print("Swipe.Right");
            SlideRight();
            break;
        }
    }
}
