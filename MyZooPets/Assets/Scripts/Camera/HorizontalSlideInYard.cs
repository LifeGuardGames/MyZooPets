using UnityEngine;
using System.Collections;

public class HorizontalSlideInYard : UserNavigation {

    float currentXPos;
    public float slideIncrement = 40f;
    Hashtable optional = new Hashtable();
    bool lockSlide = false;

    float minLeft;
    float maxRight;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        currentXPos = transform.position.x;
        optional.Add("onComplete", "FinishedSlide");

        // Init limits to yard navigation
        minLeft = -slideIncrement;
        maxRight = slideIncrement;
    }

    public override void ToTheRight(){
        if (IsRightArrowEnabled()){
            if (ClickManager.CanRespondToTap()){
                lockSlide = true;
                currentXPos += slideIncrement;
                LeanTween.moveX(gameObject, currentXPos, 1.0f, optional);
            }
        }
    }

    public override void ToTheLeft(){
        if (IsLeftArrowEnabled()){
            if (ClickManager.CanRespondToTap()){
                lockSlide = true;
                currentXPos -= slideIncrement;
                LeanTween.moveX(gameObject, currentXPos, 1.0f, optional);
            }
        }
    }
    protected override bool IsRightArrowEnabled(){
        return (!lockSlide && currentXPos < maxRight);
    }

    protected override bool IsLeftArrowEnabled(){
        return (!lockSlide && currentXPos > minLeft);
    }

    void FinishedSlide(){
        lockSlide = false;
        currentXPos = transform.position.x;
    }


}
