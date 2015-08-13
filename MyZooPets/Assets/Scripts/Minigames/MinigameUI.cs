using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//---------------------------------------------------
// MinigameUI
// This is strictly a view controller, for displaying
// information to the player.
//---------------------------------------------------

public class MinigameUI : MonoBehaviour{
	// popups
	public MinigamePopup popupOpening;
	public MinigamePopup popupGameOver;
	public MinigamePopup popupPaused;
	
	// player score
	public UILabel labelScore;
	
	// player lives
	public UILabel labelLives;
	
	// hashes
	private Hashtable hashLabels;
	private Dictionary<MinigamePopups, MinigamePopup> hashPopups;

	void Awake(){
		// set up the hash of labels
		hashLabels = new Hashtable();
		hashLabels[MinigameLabels.Score] = labelScore;
		hashLabels[MinigameLabels.Lives] = labelLives;
		
		// set up hash of popups
		hashPopups = new Dictionary<MinigamePopups, MinigamePopup>();
		hashPopups[MinigamePopups.Opening] = popupOpening;
		hashPopups[MinigamePopups.Pause] = popupPaused;
		hashPopups[MinigamePopups.GameOver] = popupGameOver;
	}
	
	//---------------------------------------------------
	// SetLabel()
	// Updates the label with the incoming type to the
	// incoming string.
	//---------------------------------------------------	
	public void SetLabel(MinigameLabels eLabel, string strText){
		UILabel label = (UILabel)hashLabels[eLabel];	
		
		// minigame may not have label (I'm looking at you, runner)
		if(label == null)
			return;
		
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
	public void TogglePopup(MinigamePopups ePopup, bool bShow){
		MinigamePopup popup = hashPopups[ePopup];
		
		// only do the toggle if appropriate
		if((bShow && popup.IsShowing() == false) || (!bShow && popup.IsShowing() == true))
			popup.Toggle(bShow);
	}
	
	//---------------------------------------------------
	// IsPopupShowing()
	//---------------------------------------------------
	public bool IsPopupShowing(MinigamePopups ePopup){
		MinigamePopup popup = hashPopups[ePopup];
		bool bShowing = popup.IsShowing();
		return bShowing;
	}
}
