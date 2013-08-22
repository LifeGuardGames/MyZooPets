using UnityEngine;
using System.Collections;

/*
	Advair switch (Advair Step 2).

	This listens to the user's touch input, and rotates the part to follow the user's touch.
*/
public class AdvairSwitch : AdvairDragToRotate{
    protected override void Awake(){
        base.Awake();
        finalPosition = new Vector3(0, 0, 330);
        advairStepID = 2;
        maskLayer = 1 << 9;
    }	
}