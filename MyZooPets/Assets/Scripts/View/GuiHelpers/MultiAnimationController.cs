using UnityEngine;
using System.Collections;

public class MultiAnimationController : MonoBehaviour {
	
	public Animation[] animationList;
	public bool isDebug;
	
	public void PlayAnimations(){
		foreach(Animation animation in animationList){
			animation.Play();
		}
	}
	
	void OnGUI(){
		if(isDebug){
			if(GUI.Button(new Rect(100, 100, 100, 100), "Play Animations")){
				PlayAnimations();
			}
		}
	}
}
