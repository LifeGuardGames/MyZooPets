using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Room GUI animator.
/// This is the middleman proxy class that listens to the statLogic to display animation for roomGUI.
/// 
/// this animator will also have a series of events. These events will be raised
/// when special things are happening to the changing stats. Client classes will need
/// to provide listeners to handle these events. 
/// For example, Event will be raised when the level up progress bar reached the leveling up point
/// If any other class wants to display UI elements when this event happens, a listener will need to
/// be provided.
/// </summary>

public class RoomGUIAnimator : MonoBehaviour {
	
	public int dataPoints, dataStars, dataHealth, dataMood, dataHunger;
	public int displayPoints, displayStars, displayHealth, displayMood, displayHunger;
	public int nextLevelPoints; //the minimum requirement for next level up
	
	//================Events================
	//call when the pet levels up. used this to level up UI components
    public delegate void OnLevelUpEventHandlers(object sender, EventArgs e);
    public static OnLevelUpEventHandlers OnLevelUp;

    //========================================

	private Level lastLevel; //pet's last level
	private RoomGUI roomgui;
	private bool IconSwitch = true;
	private int leantweencurrent;
	Hashtable optionalGrow = new Hashtable();
    Hashtable optionalShrink = new Hashtable();
	private bool IconHealthSwitch;
	private bool IconMoodSwitch;
	private bool IconFoodSwitch;
	
	public void Init()
	{
		dataPoints = DataManager.Points;
		dataStars = DataManager.Stars;
		dataHealth = DataManager.Health;
		dataMood = DataManager.Mood;
		dataHunger = DataManager.Hunger;
		
		displayPoints = DataManager.Points;
		displayStars = DataManager.Stars;
		displayHealth = DataManager.Health;
		displayMood = DataManager.Mood;
		displayHunger = DataManager.Hunger;

		lastLevel = DataManager.CurrentLevel;
		nextLevelPoints = LevelUpLogic.NextLevelPoints();
		roomgui = GameObject.Find("RoomGUI").GetComponent<RoomGUI>();
		
		optionalGrow.Add("onCompleteTarget", gameObject);	
		optionalShrink.Add("onCompleteTarget", gameObject);
    	optionalGrow.Add("onComplete", "Shrink");
        optionalShrink.Add("onComplete", "Grow");
	}
	
    public void Grow(){
//		if(IconHealthSwitch)
    	leantweencurrent = LeanTween.scale(roomgui.healthIconRect,new Vector2(80,80),0.2f, optionalGrow);
//    	if(IconMoodSwitch)
    	leantweencurrent = LeanTween.scale(roomgui.moodIconRect,new Vector2(80,80),0.2f, optionalGrow);
//    	if(IconFoodSwitch)
    	leantweencurrent = LeanTween.scale(roomgui.foodIconRect,new Vector2(80,80),0.2f, optionalGrow);
    }

    public void Shrink(){
//    	if(IconHealthSwitch)
    	leantweencurrent = LeanTween.scale(roomgui.healthIconRect,new Vector2(60,60),0.2f, optionalShrink);
//    	if(IconMoodSwitch)
    	leantweencurrent = LeanTween.scale(roomgui.moodIconRect,new Vector2(60,60),0.2f, optionalShrink);
//    	if(IconFoodSwitch)
    	leantweencurrent = LeanTween.scale(roomgui.foodIconRect,new Vector2(60,60),0.2f, optionalShrink);
    }
    
	void FixedUpdate(){
		if(!LoadDataLogic.IsDataLoaded) return;

		//Points 
		if(dataPoints != DataManager.Points){
			if(displayPoints < DataManager.Points){ //animate 
				if(displayPoints + 3 <= DataManager.Points){
					displayPoints += 3;
				}
				else{
					displayPoints += DataManager.Points - displayPoints;
				}
			}
			else{ //animation done
				dataPoints = DataManager.Points;	
			}
		}else{ //animation is done and dataPoints is now == to DataManager.Points
			if(!lastLevel.Equals(DataManager.CurrentLevel)){ //check if points have went beyond level up requirements
				nextLevelPoints = LevelUpLogic.NextLevelPoints(); //update the nxt level points if pet has leveled up
				DataManager.ResetPoints(); //reset points back to 0
				displayPoints = DataManager.Points; //display 0 in RoomGUI

				lastLevel = DataManager.CurrentLevel;
				//notify NotificationUIManager
				if(OnLevelUp != null) OnLevelUp(this, EventArgs.Empty);
			}
		}

		//Stars
		if(dataStars != DataManager.Stars){
			if(displayStars > DataManager.Stars){
				displayStars--;
			}
			else if(displayStars < DataManager.Stars){
				displayStars++;
			}
			else{
				dataStars = DataManager.Stars;	
			}
		}

		//Health
		if(dataHealth != DataManager.Health){
			if(displayHealth > DataManager.Health){
				displayHealth--;
			}
			else if(displayHealth < DataManager.Health){
				displayHealth++;
				if(IconSwitch){
					IconHealthSwitch = true;
					Grow();
					IconSwitch = false;				
				}
			}
			else{
				dataHealth = DataManager.Health;
				LeanTween.cancel(LeanTween.TweenEmpty,leantweencurrent);
				LeanTween.scale(roomgui.healthIconRect,new Vector2(60,60),0.1f);
				IconSwitch = true;
				IconHealthSwitch = false;
			}
		}

		//Mood
		if(dataMood != DataManager.Mood){
			if(displayMood > DataManager.Mood){
				displayMood--;
			}
			else if(displayMood < DataManager.Mood){
				displayMood++;
				if(IconSwitch){
					IconMoodSwitch = true;
					Grow();
					IconSwitch = false;				
				}
			}
			else{
				dataMood = DataManager.Mood;	
				LeanTween.cancel(LeanTween.TweenEmpty,leantweencurrent);
				LeanTween.scale(roomgui.moodIconRect,new Vector2(60,60),0.1f);
				IconMoodSwitch = false;
				IconSwitch = true;
			}
		}

		//Hunger
		if(dataHunger != DataManager.Hunger){
			if(displayHunger > DataManager.Hunger){
				displayHunger--;
			}
			else if(displayHunger < DataManager.Hunger){
				displayHunger++;
				if(IconSwitch){
					IconFoodSwitch = true;
					Grow();
					IconSwitch = false;				
				}
			}
			else{
				dataHunger = DataManager.Hunger;
				LeanTween.cancel(LeanTween.TweenEmpty,leantweencurrent);
				LeanTween.scale(roomgui.foodIconRect,new Vector2(60,60),0.1f);
				IconFoodSwitch = false;
				IconSwitch = true;
			}
		}
	}
}
