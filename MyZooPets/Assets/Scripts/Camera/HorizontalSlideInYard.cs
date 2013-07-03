using UnityEngine;
using System.Collections;

public class HorizontalSlideInYard : UserNavigation {

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

    public override void ToTheRight(){
        if (!lockSlide){
            // if (ClickManager.CanRespondToTap()){
                lockSlide = true;
                currentXPos += slideIncrement;
                LeanTween.moveX(gameObject, currentXPos, 1.0f, optional);
            // }
        }
    }

    public override void ToTheLeft(){
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


}
