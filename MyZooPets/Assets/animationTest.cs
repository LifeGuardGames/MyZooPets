using UnityEngine;
using System.Collections;

public class animationTest : MonoBehaviour {
	
	tk2dSpriteAnimator animator;
	
	tk2dSpriteAnimation lib1;
	
	// Use this for initialization
	void Start () {
		animator = GetComponent<tk2dSpriteAnimator>();
		lib1 = animator.Library;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(10f, 10f, 100f, 100f), "Test1")){
			animator.Play("HappySadTransition");
		}
		
		if(GUI.Button(new Rect(110f, 10f, 100f, 100f), "Test2")){
			animator.Play("SadIdle");
		}
		
		if(GUI.Button(new Rect(210f, 10f, 100f, 100f), "Test3")){
			animator.Play("HappyIdle");
		}
	}
}
