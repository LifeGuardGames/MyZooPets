/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//-----------------------------------------
// Starts the floating tween and alpha tween
// self destructs when it's completed
//-----------------------------------------
public class FloatyController : MonoBehaviour {
    public Vector3 floatingUpPos;
    public float floatingTime;
    public List<FadeTweener> alphaScripts = new List<FadeTweener>(); //all the alpha script in this floaty prefab

	void Start () {
        FloatUp();
        foreach(FadeTweener alphaScript in alphaScripts){
			if(alphaScript.gameObject.activeSelf){	// NOTE: Floaty stats will bypass this funtion call
            	alphaScript.FadeText();
			}
        }
	}

    private void FloatUp(){
		// Lean tween doesn't have a move by, so what we really want to do is move the object to its current position + the floating vector
		Vector3 vTarget = gameObject.transform.localPosition;
		vTarget += floatingUpPos;
		
        LeanTween.moveLocal(gameObject, vTarget, floatingTime).setOnComplete(SelfDestruct);
    }

    private void SelfDestruct(){
        Destroy(gameObject);
    }
}
*/