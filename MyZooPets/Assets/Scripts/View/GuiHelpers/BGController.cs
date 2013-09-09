﻿using UnityEngine;
using System.Collections;

public class BGController : Singleton<BGController> {
	
	public Material blue;
	public Material black;
	public Camera camera;	// For far clipping optimization when bg is shown;
	private float startFarClip;
	
	void Start(){
		startFarClip = camera.farClipPlane;
	}

	public void Show(string material){
		AssignMaterial(material);
		
		TweenToggle toggle = GetComponent<TweenToggle>();
		toggle.ShowFunctionName = "DecreaseCameraClip";	// Assigning callback
		toggle.Show();
	}
	
	public void AssignMaterial(string material){
		if(material == "blue")
			renderer.material = blue;
		if(material == "black")
			renderer.material = black;
	}
	
	public void Hide(){
		RestoreCameraClip();
		GetComponent<TweenToggle>().Hide();
	}
	
	// Callback of BG Tween
	public void DecreaseCameraClip(){
		camera.farClipPlane = 5;
	}
	
	public void RestoreCameraClip(){
		camera.farClipPlane = startFarClip;
	}
}