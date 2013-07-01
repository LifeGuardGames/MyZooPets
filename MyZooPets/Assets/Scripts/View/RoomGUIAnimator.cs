using UnityEngine;
using System.Collections;

/// <summary>
/// Room GUI animator.
/// This is the middleman proxy class that listens to the statLogic to display animation for roomGUI.
/// </summary>

public class RoomGUIAnimator : MonoBehaviour {
	
	public int dataPoints, dataStars, dataHealth, dataMood, dataHunger;
	public int displayPoints, displayStars, displayHealth, displayMood, displayHunger;
	public int nextLevelPoints; //the minimum requirement for next level up
	private Level lastLevel; 

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
	}

	void FixedUpdate(){
		if(!LoadDataLogic.IsDataLoaded) return;
		
		//Listen for changes
		//TODO untested!
		if(dataPoints != DataManager.Points){
			if(displayPoints > DataManager.Points){
				// if(displayPoints - 5 >= DataManager.Points){
				// 	displayPoints -= 5;
				// }
				// else{
				// 	displayPoints -= displayPoints - DataManager.Points;
				// }
			}
			else if(displayPoints < DataManager.Points){
				if(displayPoints + 3 <= DataManager.Points){
					displayPoints += 3;
				}
				else{
					displayPoints += DataManager.Points - displayPoints;
				}
			}
			else{
				
				dataPoints = DataManager.Points;	
			}
		}else{ //animation is done and dataPoints is now == to DataManager.Points
			//check if points have went beyond level up requirements
			if(!lastLevel.Equals(DataManager.CurrentLevel)){ 
				//update the nxt level points if pet has leveled up
				nextLevelPoints = LevelUpLogic.NextLevelPoints();
				DataManager.ResetPoints(); //reset points back to 0
				displayPoints = DataManager.Points; //display 0 in RoomGUI
			}
		}
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
		if(dataHealth != DataManager.Health){
			if(displayHealth > DataManager.Health){
				displayHealth--;
			}
			else if(displayHealth < DataManager.Health){
				displayHealth++;
			}
			else{
				dataHealth = DataManager.Health;	
			}
		}
		if(dataMood != DataManager.Mood){
			if(displayMood > DataManager.Mood){
				displayMood--;
			}
			else if(displayMood < DataManager.Mood){
				displayMood++;
			}
			else{
				dataMood = DataManager.Mood;	
			}
		}
		if(dataHunger != DataManager.Hunger){
			if(displayHunger > DataManager.Hunger){
				displayHunger--;
			}
			else if(displayHunger < DataManager.Hunger){
				displayHunger++;
			}
			else{
				dataHunger = DataManager.Hunger;	
			}
		}
	}
}
