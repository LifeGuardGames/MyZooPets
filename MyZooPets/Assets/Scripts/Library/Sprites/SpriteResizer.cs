using UnityEngine;
using System.Collections;

//---------------------------------------------------
// SpriteResizer
// This script is placed on a game object that has
// a UI Sprite, and will resize that sprite based on
// the value of the public variable.
//---------------------------------------------------	

public class SpriteResizer : MonoBehaviour {
	// public variable holding the size to be resized to
	public string strConstant;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Start () {
		// get the sprite
		UISprite sprite = gameObject.GetComponent<UISprite>();
		if ( sprite == null ) {
			Debug.Log( "Sprite resizer on an object that does not have a sprite.", gameObject );
			return;
		}
		
		// get the size that we want to resize the sprite to
		int nSizeTo = Constants.GetConstant<int>( strConstant );
		
		// then make the sprite pixel perfect so we can easily get the width and height
		sprite.MakePixelPerfect();
		
		// get width and height
		float fPerfectHeight = gameObject.transform.localScale.y;
		float fPerfectWidth = gameObject.transform.localScale.x;
		
		// this is purposefully a little inefficient code-wise because I found this to be a little complicated
		if ( fPerfectHeight > fPerfectWidth ) {
			// if the perfect height is > perfect width, we want to scale the width based on the ratio
			float fRatio = (float) nSizeTo / (float) fPerfectHeight;
			float fWidth = fPerfectWidth * fRatio;
			Vector3 vScale = new Vector3( fWidth, nSizeTo, 0 );
			gameObject.transform.localScale = vScale;
		}
		else {
			// otherwise, the exact opposite: scale height based on the ratio of the desired size to perfect width
			float fRatio = (float) nSizeTo / (float) fPerfectWidth;
			float fHeight = fPerfectHeight * fRatio;
			Vector3 vScale = new Vector3( nSizeTo, fHeight, 0 );
			gameObject.transform.localScale = vScale;
		}
	}
}
