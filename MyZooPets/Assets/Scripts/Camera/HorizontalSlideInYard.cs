using UnityEngine;
using System.Collections;

public class HorizontalSlideInYard : UserNavigation {

    float currentXPos;
    public float slideIncrement = 40f;
    public bool inverse = false;
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

    protected override bool IsRightArrowEnabled(){
        return true;
    }

    protected override bool IsLeftArrowEnabled(){
        return true;
    }

    void FinishedSlide(){
        lockSlide = false;
        currentXPos = transform.position.x;
    }


}
