using UnityEngine;
using System.Collections;

#pragma warning disable 0414

//---------------------------------------------------
// BlinkButton
// Attach and init this script to cause a button to
// blink between to different sprites.
//---------------------------------------------------

public class BlinkButton : BlinkSprite {
	// hover and pressed sprites -- cached because they have to be nulled and then re-set
	private UIImageButton button;
	private string strHover;
	private string strPressed;
	
	//---------------------------------------------------
	// Init()
	// Sets up the button to be able to blink.
	//---------------------------------------------------	
	public void Init( UIImageButton button, string strImagePulse, float fPulseTime ) {
		// store hover and pressed states
		this.button = button;
		strPressed = button.pressedSprite;
		strHover = button.hoverSprite;
		
		// call super to start the blinking
		bool bBlink = base.Init( button.target, strImagePulse, fPulseTime );
		
		if ( bBlink ) {
			// if the button is going to blink, null the hover and pressed states so they don't interfere
			button.hoverSprite = null;
			button.pressedSprite = null;			
		}
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------	
	protected override void OnDestroy() {
		// re-set the hover and pressed states since the button will no longer be blinking
		button.pressedSprite = strPressed;
		button.hoverSprite = strHover;
		
		base.OnDestroy();
	}
}
