using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonChangeEggColor
// Button that changes the egg color on the intro
// screen.
//---------------------------------------------------

public class ButtonChangeEggColor : LgButton{
	
	// sprite name for the egg
	public string strSprite;
	
	// pet color name for the egg
	public string strColor;
	
	protected override void _Awake(){
		buttonSound = "introChangeColor";	
	}
	
	protected override void ProcessClick(){
		//CustomizationUIManager.Instance.ChangeEggColor(strSprite, strColor);
	}
}
