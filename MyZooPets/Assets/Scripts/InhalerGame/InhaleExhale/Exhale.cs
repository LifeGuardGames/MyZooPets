using UnityEngine;
using System;
using System.Collections;

/*
    Handles exhale (swipe down) action
*/
public class Exhale : SwipeToInhaleExhale {
    protected override void Start(){
        base.Start();
        gameStepID = 3;
    }
}
