using UnityEngine;
using System.Collections;

//---------------------------------------------------
// MinigameUI
// This is strictly a view controller, for displaying
// information to the player.
//---------------------------------------------------

public class MinigameUI : MonoBehaviour {
	// popups
	public GameObject goOpening;
	public GameObject goGameOver;
	public GameObject goPaused;
	
	// player score
	public UILabel labelScore;
	
	// player lives
	public UILabel labelLives;
	
	// hashes
	private Hashtable hashLabels;
	private Hashtable hashPopups;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
	void Start() {
		// set up the hash of labels
		hashLabels = new Hashtable();
		hashLabels[MinigameLabels.Score] = labelScore;
		hashLabels[MinigameLabels.Lives] = labelLives;
		
		// set up hash of popups
		hashPopups = new Hashtable();
		hashPopups[MinigamePopups.Opening] = goOpening;
		hashPopups[MinigamePopups.Pause] = goPaused;
		hashPopups[MinigamePopups.GameOver] = goGameOver;
	}
	
	//---------------------------------------------------
	// SetLabel()
	// Updates the label with the incoming type to the
	// incoming string.
	//---------------------------------------------------	
	public void SetLabel( MinigameLabels eLabel, string strText ) {
		UILabel label = (UILabel) hashLabels[eLabel];	

		// update text
		label.text = strText;
		
		// Animate if any
		AnimationControl anim = label.transform.parent.gameObject.GetComponent<AnimationControl>();
		if(anim != null){
			anim.Stop();
			anim.Play();
		}		
	}	
	
	//---------------------------------------------------
	// TogglePopup()
	// Shows or hides a UI popup.
	//---------------------------------------------------		
	public void TogglePopup( MinigamePopups ePopup, bool bShow ) {
		GameObject goUI = (GameObject) hashPopups[ePopup];
		
		PositionTweenToggle script = goUI.GetComponent<PositionTweenToggle>();
		if ( bShow ) {
			script.Show();
			
			// lock clicks
			ClickManager.Instance.ClickLock();
			ClickManager.SetActiveGUIModeLock( true );
		}
		else {
			script.Hide();
			
			// clicks are ok
			ClickManager.Instance.ReleaseClickLock();
			ClickManager.SetActiveGUIModeLock( false );
		}
	}
	
	//---------------------------------------------------
	// IsPopupShowing()
	//---------------------------------------------------	
	public bool IsPopupShowing( MinigamePopups ePopup ) {
		GameObject goUI = (GameObject) hashPopups[ePopup];
		bool bShowing = goUI.GetComponent<PositionTweenToggle>().IsShowing;
		
		return bShowing;
	}
}
