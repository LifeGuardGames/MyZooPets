using UnityEngine;
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
	// the text label for the popup
	public UILabel label;
	
	// sprite bg
	public UISlicedSprite bg;
	
	// arbitrary border to make the bg look a little nicer
	public float fBorder;
	
	// where the popup should appear
	private Vector3 vLoc;
	
	public void Init( string strLabelText ) {
		label.text = strLabelText;
	}
	
	void Start() {
		Vector3 vSize = label.relativeSize;
		//Debug.Log("Okay, here we go...");
		//Debug.Log("Relative size: " + vSize);
		
		Vector3 textScale = label.transform.localScale;
		//Vector3 offset = label.transform.localPosition;	// not sure what this was supposed to do...
		//Debug.Log("And the local scale of the text is " + textScale);
		
		vSize.x *= textScale.x;
		vSize.y *= textScale.y;
		vSize.x += fBorder;
		vSize.y += fBorder;
		//vSize.x += bg.border.x + bg.border.z + ( offset.x - bg.border.x) * 2f;
		//vSize.y += bg.border.y + bg.border.w + (-offset.y - bg.border.y) * 2f;
		vSize.z = 1f;

		bg.transform.localScale = vSize;		
	}
}
