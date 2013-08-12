using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchDetectorManager : MonoBehaviour {

    public float MaxSwipeAngleDegrees = 45.0f;
    public List<MonoBehaviour> ListeningScripts = new List<MonoBehaviour>();

    private bool mbSwipingStraight;
    private Vector2 mStartTouchPosition;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        bool bIsTouching = Input.touchCount > 0;
		if (bIsTouching) {
            Touch firstTouch =  Input.touches[0];
            TouchPhase currentTouchPhase = firstTouch.phase;

            switch (currentTouchPhase)
            {
                case TouchPhase.Began:
                    mbSwipingStraight = true;
					mStartTouchPosition = firstTouch.position;
                    break;

                case TouchPhase.Ended:
                {
                    if (mbSwipingStraight)
                    {
                        // How angled was the swipe compared to what we tolerate?
                        Vector2 swipeVector = firstTouch.position - mStartTouchPosition;
                        float swipeAngle = Vector2.Angle(Vector2.up, swipeVector);
                        if (swipeAngle < MaxSwipeAngleDegrees)
                        {
                            foreach (MonoBehaviour currentScript in ListeningScripts)
                            {
                                currentScript.SendMessage("onSwipeUp");
                            }
                            break;
                        }

                        swipeAngle = Vector2.Angle(-Vector2.up, swipeVector);
                        if (swipeAngle < MaxSwipeAngleDegrees)
                        {
                            foreach (MonoBehaviour currentScript in ListeningScripts)
                            {
                                currentScript.SendMessage("onSwipeDown");
                            }
                            break;
                        }
                    }
                    break;
                }
            }
		}
	
	}

}
