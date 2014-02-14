using UnityEngine;
using System.Collections;

//---------------------------------------------------
// BlinkSprite
// Attach and init this script on a sprite to cause
// it to blink between two different sprites.
//---------------------------------------------------

public class BlinkSprite : MonoBehaviour {
	// the sprite name that this sprite will blink TO
	private string strImageBlink;
	
	// the sprite name of the original image
	private string strImageOriginal;
	
	// the sprite that should be blinking
	UISprite sprite;
	
	//---------------------------------------------------
	// Init()
	// Sets up the sprite to blink, and does so if the
	// requirements are met.
	//---------------------------------------------------	
	public bool Init( UISprite sprite, string strImageBlink, float fBlinkTime ) {
		// save incoming variables
		this.strImageBlink = strImageBlink;
		this.strImageOriginal = sprite.spriteName;
		this.sprite = sprite;
		
		if ( sprite == null ) {
			fBlinkTime = 0;
			Debug.LogError("Attempting to blink something that does not have a UISprite(" + gameObject.name + ")", gameObject);
		}
		
		// we don't want to blink anything if there is an invalid blink time
		bool bBlink = fBlinkTime > 0;
		
		if ( bBlink )			
			InvokeRepeating("Blink", 1, fBlinkTime);	// let the blinking begin...probably the only time I'll EVER use invoke
		
		return bBlink;
	}
	
	//---------------------------------------------------
	// Blink()
	// Blinks the button to the sprite it is not using.
	//---------------------------------------------------	
	private void Blink() {
		if ( sprite.spriteName == strImageBlink )
			sprite.spriteName = strImageOriginal;
		else
			sprite.spriteName = strImageBlink;
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------		
	protected virtual void OnDestroy() {
		// on destroy, make sure to set the image to the original
		sprite.spriteName = strImageOriginal;
	}
}
