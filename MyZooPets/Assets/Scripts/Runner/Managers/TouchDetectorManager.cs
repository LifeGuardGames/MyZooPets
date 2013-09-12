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

        if(direction == FingerGestures.SwipeDirection.Up){
            foreach(MonoBehaviour currentScript in ListeningScripts)
                currentScript.SendMessage("onSwipeUp", SendMessageOptions.DontRequireReceiver);
        }else{
            foreach(MonoBehaviour currentScript in ListeningScripts)
                currentScript.SendMessage("onSwipeDown", SendMessageOptions.DontRequireReceiver);
        }

    }	
	// // Update is called once per frame
	// void Update () {
 //        // Check keys
 //        if (Input.GetKeyDown(KeyCode.T)) {
 //            Debug.Log("Triggering fade");
 //            ParallaxingBackgroundManager bgm = GameObject.Find("ParallaxingBGManager").GetComponent<ParallaxingBackgroundManager>();
 //            bgm.TransitionToGroup("Test2");
 //        }
 //        if (Input.GetKeyDown(KeyCode.J)) {
 //            RunnerGameManager.GetInstance().PlayerRunner.TriggerSlowdown(2.0f);
 //        }
 //        if (Input.GetKeyDown(KeyCode.R)) {
 //            RunnerGameManager.GetInstance().ResetGame();
 //        }

 //        // Check touches
 //        bool bIsTouching = Input.touchCount > 0;
	// 	if (bIsTouching) {
 //            Touch firstTouch =  Input.touches[0];
 //            TouchPhase currentTouchPhase = firstTouch.phase;

 //            switch (currentTouchPhase) {
 //                case TouchPhase.Began:
 //                    mbSwipingStraight = false;
 //                    mStartTouchPosition = firstTouch.position;
 //                    break;

 //                case TouchPhase.Moved: {
 //                    if (!mbSwipingStraight) {
 //                        float verticalSwipeLength = firstTouch.position.y - mStartTouchPosition.y;
 //                        if (Mathf.Abs(verticalSwipeLength) > SwipeDistance) {
 //                            foreach (MonoBehaviour currentScript in ListeningScripts) {
 //                                if (verticalSwipeLength >= 0)
 //                                    currentScript.SendMessage("onSwipeUp", SendMessageOptions.DontRequireReceiver);
 //                                else
 //                                    currentScript.SendMessage("onSwipeDown", SendMessageOptions.DontRequireReceiver);
 //                            }
 //                            mbSwipingStraight = true;
 //                        }
 //                    }
 //                    break;
 //                }

 //                case TouchPhase.Ended: {
 //                    if (!mbSwipingStraight) {
 //                        foreach (MonoBehaviour currentScript in ListeningScripts) {
 //                            currentScript.SendMessage("onSwipeUp", SendMessageOptions.DontRequireReceiver);
 //                        }
 //                    }
 //                    break;
 //                }
 //            }
	// 	}
	
}
