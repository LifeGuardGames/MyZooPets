using UnityEngine;
using System.Collections;

//---------------------------------------------------
// LgButton (Lifeguard Button)
// Generic button class that other buttons derive from.
// This class handles high level input restrictions
// and makes sure that the button can process a click.
//---------------------------------------------------

public class LgButton : MonoBehaviour {
	
	public string strAnalytics;	// string key for analytics on this button
	protected string GetAnalyticsKey()
	{
		return strAnalytics;
	}	
	
	//---------------------------------------------------
	// OnPress
	// When the button is actually clicked/pressed.
	//---------------------------------------------------	
	void OnPress (bool isPressed)
	{
		// if the button is being pressed and it is okay to respond...
		if ( isPressed && ClickManager.Instance.CanRespondToTap() ) {
			
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
