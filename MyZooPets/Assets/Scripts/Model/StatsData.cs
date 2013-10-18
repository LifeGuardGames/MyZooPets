using UnityEngine;
using System;
using System.Collections;

public class StatsData{
    public int Points {get; set;} //evolution Points
    public int Stars {get; set;} //currency of the game
    public int Health {get; set;} //pet's Health
    public int Mood {get; set;} //pet's mood (weighted or unweighted)
	
	// constants
	// if the pet's mood <= this number, it will be sad
	private static int SAD_THRESH = 50;
	
	// if the pet's Health <= this number, it will be sick
	private static int SICK_THRESH = 60;
	
	// if the pet's Health <= this number, it will be very sick
	private static int VERY_SICK_THRESH = 30;	

    //=======================Initialization==================
    public StatsData(){}

    //Populate with dummy data
    public void Init(){
        Health = 80;
        Mood = 80;
        Points = 250;
        Stars = 100;
    }
	
	public int GetStat( HUDElementType eStat ) {
		int nStat = 0;
		
		switch ( eStat ) {
		case HUDElementType.Points:
			nStat = Points;
			break;
		case HUDElementType.Health:
			nStat = Health;
			break;
		case HUDElementType.Mood:
			nStat = Mood;
			break;
		case HUDElementType.Stars:
			nStat = Stars;
			break;
		default:
			Debug.Log("No such display target for " + eStat);
			break;
		}	
		
		return nStat;
	}

    //==============StatsModifiers================
    //Points
    public void AddPoints(int val){
        Points += val;
    }
    public void SubtractPoints(int val){
        Points -= val;
        if (Points < 0)
            Points = 0;
    }
    public void ResetPoints(){
        Points = 0;
    }

    //Stars
    public void AddStars(int val){
        Stars += val;
    }
    public void SubtractStars(int val){
        Stars -= val;
        if (Stars < 0)
            Stars = 0;
    }

    //Health
    public void AddHealth(int val){
        Health += val;
        if (Health > 100){
            Health = 100;
        }
    }
    public void SubtractHealth(int val){
        Health -= val;
        if (Health < 0){
            Health = 0;
        }
    }

    //Mood
    public double GetWeightedMood(){
        return (0.5*Mood + 0.5*Health);
    }
    public void AddMood(int val){
        Mood += val;
        if (Mood > 100){
            Mood = 100;
        }
    }
    public void SubtractMood(int val){
        Mood -= val;
        if (Mood < 0){
            Mood = 0;
        }
    }
	
	//---------------------------------------------------
	// GetMoodState()
	// Based on the numerical value of the mood stat,
	// returns an enum of the pet's mood.
	//---------------------------------------------------		
	public PetMoods GetMoodState() {
		PetMoods eMood = PetMoods.Happy;
		
		if ( Mood <= SAD_THRESH )
			eMood = PetMoods.Sad;
		
		return eMood;
	}
	
	//---------------------------------------------------
	// GetHealthState()
	// Based on the numerical value of the Health stat,
	// returns an enum of the pet's Health.
	//---------------------------------------------------		
	public PetHealthStates GetHealthState() {
		PetHealthStates eHealth = PetHealthStates.Healthy;
		
		if ( Health <= VERY_SICK_THRESH )
			eHealth = PetHealthStates.VerySick;
		else if ( Health <= SICK_THRESH )
			eHealth = PetHealthStates.Sick;
		
		return eHealth;
	}	
}
