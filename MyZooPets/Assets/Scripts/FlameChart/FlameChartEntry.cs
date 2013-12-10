using UnityEngine;
using System.Collections;
using System;

//---------------------------------------------------
// FlameChartEntry
// This is a UI element on the fire chart board that
// represents a fire skill and the path to get to it.
//---------------------------------------------------

public class FlameChartEntry : MonoBehaviour {
	// the ID of the skill this entry represents
	public string strSkillID;
	
	// the sprite icon for this flame
	public UISprite spriteIcon;
	
	// list of path sprites for this particular entry
	public UISprite[] arraySpritePaths;
	
	// the skill for this entry
	private Skill skill;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Start() {		
		// listen for level up messages
		HUDAnimator.OnLevelUp += LevelUp;
		
		// we need the fire skill to set some visuals
		skill = FlameChartLogic.Instance.GetSkillData( strSkillID );
		
		if ( skill != null ) {
			// set the texture based on id
			string strSprite = skill.TextureName;
			spriteIcon.spriteName = strSprite;
			
			// now set the paths for this entry based on the unlock status of the skill
			SetPathColors();
			
			// set the lock UI if the skill is locked
			bool bUnlocked = skill.IsUnlocked;
			if ( bUnlocked == false ) {
				int nLevel = skill.UnlockLevel;	
				LevelLockObject.CreateLock( spriteIcon.gameObject.GetParent(), nLevel, true, "LevelLockUI_Default" );
			}
		}
	}
	
	//---------------------------------------------------
	// SetPathColors()
	// Sets the path colors for this entry bases on
	// the lock status of the skill.
	//---------------------------------------------------	
	private void SetPathColors() {
		bool bUnlocked = skill.IsUnlocked;
		string strColor = bUnlocked ? "FlameChartColor_Unlocked" : "FlameChartColor_Locked";
		Color color = Constants.GetConstant<Color>( strColor );
		for ( int i = 0; i < arraySpritePaths.Length; ++i ) {
			UISprite spritePath = arraySpritePaths[i];
			spritePath.color = color;
		}		
	}
	
	//---------------------------------------------------
	// LevelUp()
	//---------------------------------------------------		
	private void LevelUp(object senders, EventArgs args){
		// we got a level up event, so just set the colors in case anything has changed
        SetPathColors();
	}		
}
