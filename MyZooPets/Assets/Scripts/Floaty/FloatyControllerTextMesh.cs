﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//-----------------------------------------
// Starts the floating tween and alpha tween
// self destructs when it's completed
//-----------------------------------------
public class FloatyControllerTextMesh : MonoBehaviour{
	public Vector3 floatingUpPos;
	public float floatingTime;
	public LeanTweenType floatingEase = LeanTweenType.easeOutQuad;
	public LeanTweenType colorEase = LeanTweenType.easeOutQuad;
	public TextMesh textMesh;
	public string rendererLayer = "AboveSprite1"; // TODO doesnt work

	private Color textColor;

	void Start(){

		// Check for text mesh
		if(textMesh == null){
			textMesh = GetComponent<TextMesh>();
			if(textMesh == null){
				Debug.LogError("No text mesh specified");
			}
		}
		textColor = textMesh.color;
		textMesh.GetComponent<Renderer>().sortingLayerName = rendererLayer;

		FloatUp();
	}

	private void FloatUp(){
		// Lean tween doesn't have a move by, so what we really want to do is move the object to its current position + the floating vector
		Vector3 vTarget = gameObject.transform.localPosition;
		vTarget += floatingUpPos;
		
		LeanTween.moveLocal(gameObject, vTarget, floatingTime)
			.setEase(floatingEase)
			.setOnComplete(SelfDestruct);
		
		LeanTween.value(gameObject, UpdateValueCallback, 1f, 0, floatingTime)
			.setEase(colorEase);
	}

	void UpdateValueCallback(float val){
		textColor = new Color(textColor.r, textColor.g, textColor.b, val);
		textMesh.color = textColor;
	}

	private void SelfDestruct(){
		Destroy(gameObject);
	}
}
