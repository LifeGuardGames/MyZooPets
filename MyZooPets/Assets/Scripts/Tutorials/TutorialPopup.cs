﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// TutorialPopup
// This is a more advanced type of message that pops
// up while the user is playing the tutorials, and the
// sprite bg will shrink or expand to encompass all
// the text.
// DISCLAIMER: I'm not totally confident in this code.
// Most of it as taken from the NGUI forum, and I kind
// of cobbled the rest together.  Hopefully this works
// in the long term.
//---------------------------------------------------

public class TutorialPopup : MonoBehaviour {
	public UILabel label; // the text label for the popup
	public UISprite sprite;
	public LgButtonMessage button1;
	public UISlicedSprite bg; // sprite bg
	public float fBorder = -1; // arbitrary border to make the bg look a little nicer
	public delegate void Callback();
	
	private Callback Button1Callback;
	private bool shrinkBgToFitText;
	
	void Start() {
		if(shrinkBgToFitText){
			Vector3 vSize = label.relativeSize;
			//Debug.Log("Okay, here we go...");
			//Debug.Log("Relative size: " + vSize);
			
			Vector3 textScale = label.transform.localScale;
			//Vector3 offset = label.transform.localPosition;	// not sure what this was supposed to do...
			//Debug.Log("And the local scale of the text is " + textScale);

			if(fBorder > 0){
				float sizeX = vSize.x * textScale.x;
				float sizeY = vSize.y * textScale.y;
				
				sizeX += fBorder;
				sizeY += fBorder;

				float sizeZ = 1f;

				bg.transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
			}
		}
	}

	public void Init(Hashtable option) {
		// label.text = strLabelText;
		if(option.ContainsKey(TutorialPopupFields.Message)){
			label.text = (string) option[TutorialPopupFields.Message];
		}

		if(option.ContainsKey(TutorialPopupFields.SpriteAtlas)){
			string atlastName = (string) option[TutorialPopupFields.SpriteAtlas];
			GameObject atlasObject = (GameObject) Resources.Load(atlastName);
			sprite.atlas = atlasObject.GetComponent<UIAtlas>();
		}

		if(option.ContainsKey(TutorialPopupFields.SpriteName)){
			sprite.spriteName = (string) option[TutorialPopupFields.SpriteName];
			sprite.MakePixelPerfect();	
		}

		if(option.ContainsKey(TutorialPopupFields.Button1Callback)){
			Button1Callback = (Callback) option[TutorialPopupFields.Button1Callback];
		}

		if(option.ContainsKey(TutorialPopupFields.ShrinkBgToFitText)){
			shrinkBgToFitText = (bool) option[TutorialPopupFields.ShrinkBgToFitText];
		}
	}

	public void Button1Action(){
		if(Button1Callback != null)
			Button1Callback();
	}
	
}
