using UnityEngine;
using System.Collections;

/// <summary>
/// Window controller.
/// This script is attached to the window object to control the display contents in the window.
/// </summary>
public class WindowController : MonoBehaviour {

	public tk2dSprite windowObject; // The sun or the moon
	public tk2dSprite windowBackground; // The background sprite, for changing color
	
	public void setTime(bool isDaytime){
		if(isDaytime){
			// Set the sun sprite
			windowObject.SetSprite("windowSun");
			
			// Change the sky to bright blue
			windowBackground.color = new Color(107f/255f, 230f/255f, 1f, 1f);
		}
		else{
			// Set the moon sprite
			windowObject.SetSprite("windowMoon");
			
			// Change the sky to dark blue
			windowBackground.color = new Color(16f/255f, 29f/255f, 79f/255f, 1f);
		}
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(10f, 10f, 10f, 10f), "1")){
			setTime(true);
		}
		
		if(GUI.Button(new Rect(30f, 10f, 10f, 10f), "2")){
			setTime (false);
		}
	}
}
