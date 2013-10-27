using UnityEngine;
using System.Collections;

/// <summary>
/// Degrad alpha tween.
/// Loops the alpha up and down for TK2D sprites
/// </summary>
public class DegradAlphaTween : MonoBehaviour {
	
	public tk2dSprite[] spriteList;
	public bool isloop;
	public float cycleTime = 1f;
	public float toValue = 1f;
	public float fromValue = 0f;
	
	private bool forward = true;
	private bool on = true;
	private float lastTime;
	
	void Update(){
		if(on){
			// Stupid hack to fix lerp issue, brainfarting alot so not sure how to use correctly -s
			float time = (Time.time / cycleTime) % 1;
			
			// Tweening has as reached ending point
			if(lastTime > time){	// Check to see the time as reset as per modulus
				if(isloop){
					forward = !forward;
					
					// Switch the to and from values
					float aux = toValue;
					toValue = fromValue;
					fromValue = aux;
				}
				else{
					Off();
				}
			}

			foreach(tk2dSprite sprite in spriteList){
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(fromValue, toValue, time));
			}
			
			lastTime = time;
		}
	}
	
	public void On(){
		on = true;
	}
	
	public void Off(){
		on = false;
	}
}
