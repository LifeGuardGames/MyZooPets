using UnityEngine;
using System.Collections;

/// <summary>
/// Room GUI animator.
/// This is the middleman proxy class that listens to the statLogic to display animation for roomGUI.
/// </summary>

public class RoomGUIAnimator : MonoBehaviour {
	
//	public int dataPoints{
//        get {return dataPoints;}
//		set {dataPoints = value;}
//	}
//	
//	public int dataStars{
//        get {return dataStars;}
//		set {dataStars = value;}
//	}
//	
//	public int dataHealth{
//        get {return dataHealth;}
//		set {dataHealth = value;}
//	}
//	
//	public int dataMood{
//        get {return dataMood;}
//		set {dataMood = value;}
//	}
//	
//	public int dataHunger{
//        get {return dataHunger;}
//		set {dataHunger = value;}
//	}
	
	public int dataPoints, dataStars, dataHealth, dataMood, dataHunger;
	public int displayPoints, displayStars, displayHealth, displayMood, displayHunger;
	
	void Start(){
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
	}
	
	void FixedUpdate(){
		Debug.Log(DataManager.Health);
		//Listen for changes
		if(dataPoints != DataManager.Points){
			if(displayPoints > DataManager.Points){
				displayPoints--;
			}
			else if(displayPoints < DataManager.Points){
				displayPoints++;
			}
			else{
				dataPoints = DataManager.Points;	
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
