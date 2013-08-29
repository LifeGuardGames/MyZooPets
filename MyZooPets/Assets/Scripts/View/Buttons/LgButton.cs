using UnityEngine;
using System.Collections;

//---------------------------------------------------
// LgButton (Lifeguard Button)
// Generic button class that other buttons derive from.
// This class handles high level input restrictions
// and makes sure that the button can process a click.
//---------------------------------------------------

public class LgButton : MonoBehaviour {
	
	// is this button a sprite (2D)?  if it is, it is clicked a little differently than a 3d object
	public bool bSprite;
	
	public string strAnalytics;	// string key for analytics on this button
	protected string GetAnalyticsKey()
	{
		return strAnalytics;
	}	
	
    void Start(){
		
		// this works for 3D -- 2D uses OnPressed
        TapItem tapItem = GetComponent<TapItem>();
        if (tapItem != null){
            tapItem.OnTap += ButtonClicked;
        }
    }	
	
	//---------------------------------------------------
	// OnPress()
	// 2D sprite buttons will receive this event, which
	// will click the button.  At the moment 3D objects
	// also happen to receive this event, but it's possible
	// they won't in the future, so this is for 2D only.
	//---------------------------------------------------	
	void OnPress( bool bPress ) {
		if ( bPress && bSprite )
			ButtonClicked();
	}
	
	//---------------------------------------------------
	// ButtonClicked()
	// When the button is actually clicked/pressed.
	//---------------------------------------------------	
	public void ButtonClicked ()
	{
		// if the button is being pressed and it is okay to respond...
		if ( ClickManager.Instance.CanRespondToTap() ) {
			
			// if there is an analytic event on this button, let's process that
			string strAnalytics = GetAnalyticsKey();
			if ( strAnalytics != null )
				GA.API.Design.NewEvent( strAnalytics );			
			
			// process the click
			ProcessClick();
		}
	}
	
	//---------------------------------------------------
	// ProcessClick()
	// Children should implement this.  This function will
	// only be called if the button is allowed to process
	// the click (i.e., UI is not locked, etc).
	//---------------------------------------------------	
	protected virtual void ProcessClick() {
		Debug.Log("Children should implement ProcessClick!");	
	}
}
