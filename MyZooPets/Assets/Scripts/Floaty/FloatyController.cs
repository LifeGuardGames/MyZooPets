using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//-----------------------------------------
// Starts the floating tween and alpha tween
// self destructs when it's completed
//-----------------------------------------
public class FloatyController : MonoBehaviour {
    public Vector3 floatingUpPos;
    public float floatingTime;
    public List<NGUIAlphaTween> alphaScripts = new List<NGUIAlphaTween>(); //all the alpha script in this floaty prefab

	void Start () {
        FloatUp();
        foreach(NGUIAlphaTween alphaScript in alphaScripts){
			if(alphaScript.gameObject.activeSelf){	// NOTE: Floaty stats will bypass this funtion call
            	alphaScript.StartAlphaTween();
			}
        }
	}

    private void FloatUp(){
        Hashtable optional = new Hashtable();
        optional.Add("onCompleteTarget", gameObject);
        optional.Add("onComplete", "SelfDestruct");
        LeanTween.moveLocal(gameObject, floatingUpPos, floatingTime, optional);
    }

    private void SelfDestruct(){
        Destroy(gameObject);
    }
}
