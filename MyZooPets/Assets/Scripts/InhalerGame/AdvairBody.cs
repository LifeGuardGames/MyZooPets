using UnityEngine;
using System.Collections;

/*
	Advair part that rotates (Advair Step 1).

	This listens to the user's touch input, and rotates the part to follow the user's touch.

	If the part reaches its target rotation (ie. finalPosition), the step is completed, and it stays there.
	If it doesn't, it snaps back to its original position when the touch is released.
*/
public class AdvairBody : AdvairDragToRotate{
    protected override void Awake(){
        base.Awake();
        finalPosition = new Vector3(0, 0, 151);
        advairStepID = 1;
        maskLayer = 1 << 8;
    }	
}