using UnityEngine;
using System.Collections;

public class DragAnimator : MonoBehaviour{
	float distance = 50;

	// Use this for initialization
	void Start(){
	
	}
	
	// Update is called once per frame
	void Update(){
	
	}

	void OnDrag(DragGesture gesture){
		//Debug.DrawLine(gesture.StartPosition,gesture.Position, Color.red, 1f);
		//Debug.Log("DRAWING"+gesture.StartPosition+gesture.Position);
		//Debug.Log(gesture.StartSelection);
		Vector3 delta = gesture.Position - gesture.StartPosition;
		Debug.Log(delta.magnitude + ":" + Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg);
	}
}
