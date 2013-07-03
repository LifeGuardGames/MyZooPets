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
	
	//Below are for Icon pulsing.
	//Each 1 2 3, coorespond to Health, Mood, Food
	private bool IconSwitch1 = true;
	private bool IconSwitch2 = true;
	private bool IconSwitch3 = true;
	private int leantween1;
	private int leantween2;
	private int leantween3;
	Hashtable optionalGrow1 = new Hashtable();
	Hashtable optionalGrow2 = new Hashtable();
	Hashtable optionalGrow3 = new Hashtable();
    Hashtable optionalShrink1 = new Hashtable();
    Hashtable optionalShrink2 = new Hashtable();
    Hashtable optionalShrink3 = new Hashtable();
	
	
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
		
		//Had to make 3 hashtable for each icon pulsing
		optionalGrow1.Add("onCompleteTarget", gameObject);	
		optionalGrow2.Add("onCompleteTarget", gameObject);	
		optionalGrow3.Add("onCompleteTarget", gameObject);	
		optionalShrink1.Add("onCompleteTarget", gameObject);
		optionalShrink2.Add("onCompleteTarget", gameObject);
		optionalShrink3.Add("onCompleteTarget", gameObject);
    	optionalGrow1.Add("onComplete", "ShrinkHealthIcon");
    	optionalGrow2.Add("onComplete", "ShrinkMoodIcon");
    	optionalGrow3.Add("onComplete", "ShrinkFoodIcon");
        optionalShrink1.Add("onComplete", "GrowHealthIcon");
        optionalShrink2.Add("onComplete", "GrowMoodIcon");
        optionalShrink3.Add("onComplete", "GrowFoodIcon");
	}
	
	//Below functions for Icon pulsing.
    public void GrowHealthIcon(){
    	leantween1 = LeanTween.scale(roomgui.healthIconRect,new Vector2(80,80),0.2f, optionalGrow1);
    }
    public void GrowMoodIcon(){
    	leantween2 = LeanTween.scale(roomgui.moodIconRect,new Vector2(80,80),0.2f, optionalGrow2);
    }
	public void GrowFoodIcon(){
    	leantween3 = LeanTween.scale(roomgui.foodIconRect,new Vector2(80,80),0.2f, optionalGrow3);
    }

    public void ShrinkHealthIcon(){
    	leantween1 = LeanTween.scale(roomgui.healthIconRect,new Vector2(60,60),0.2f, optionalShrink1);
    }
    public void ShrinkMoodIcon(){
    	leantween2 = LeanTween.scale(roomgui.moodIconRect,new Vector2(60,60),0.2f, optionalShrink2);
    }
	public void ShrinkFoodIcon(){
    	leantween3 = LeanTween.scale(roomgui.foodIconRect,new Vector2(60,60),0.2f, optionalShrink3);
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
				if(IconSwitch1){
					GrowHealthIcon();
					IconSwitch1 = false;				
				}
			}
			else{
				dataHealth = DataManager.Health;
				LeanTween.cancel(LeanTween.TweenEmpty,leantween1);
				LeanTween.scale(roomgui.healthIconRect,new Vector2(60,60),0.1f);
				IconSwitch1 = true;
			}
		}

		//Mood
		if(dataMood != DataManager.Mood){
			if(displayMood > DataManager.Mood){
				displayMood--;
			}
			else if(displayMood < DataManager.Mood){
				displayMood++;
				if(IconSwitch2){
					GrowMoodIcon();
					IconSwitch2 = false;				
				}
			}
			else{
				dataMood = DataManager.Mood;	
				LeanTween.cancel(LeanTween.TweenEmpty,leantween2);
				LeanTween.scale(roomgui.moodIconRect,new Vector2(60,60),0.1f);
				IconSwitch2 = true;
			}
		}

		//Hunger
		if(dataHunger != DataManager.Hunger){
			if(displayHunger > DataManager.Hunger){
				displayHunger--;
			}
			else if(displayHunger < DataManager.Hunger){
				displayHunger++;
				if(IconSwitch3){
					GrowFoodIcon();
					IconSwitch3 = false;				
				}
			}
			else{
				dataHunger = DataManager.Hunger;
				LeanTween.cancel(LeanTween.TweenEmpty,leantween3);
				LeanTween.scale(roomgui.foodIconRect,new Vector2(60,60),0.1f);
				IconSwitch3 = true;
			}
		}
	}
}
