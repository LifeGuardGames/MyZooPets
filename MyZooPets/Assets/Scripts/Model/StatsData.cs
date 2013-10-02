using UnityEngine;
using System;
using System.Collections;

[DoNotSerializePublic]
public class StatsData{
    //stats
    [SerializeThis]
    private int points; //evolution points
    [SerializeThis]
    private int stars; //currency of the game
    [SerializeThis]
    private int health; //pet's health
    [SerializeThis]
    private int mood; //pet's mood (weighted or unweighted)
	
	// constants
	// if the pet's mood <= this number, it will be sad
	private static int SAD_THRESH = 50;
	
	// if the pet's health <= this number, it will be sick
	private static int SICK_THRESH = 60;
	
	// if the pet's health <= this number, it will be very sick
	private static int VERY_SICK_THRESH = 30;	

    //=====================Getters & Setters=============
    public int Points{
        get{return points;}
    }
    public int Stars{
        get{return stars;}
    }
    public int Health{
        get{return health;}
    }
    public int Mood{
        get{return mood;}
    }
    public bool UseDummyData{get; set;} //initialize with test data

    //=======================Initialization==================
    public StatsData(){}

    //Populate with dummy data
    public void Init(){
        health = 80;
        mood = 80;
        points = 250;
        stars = 100;
    }

    //==============StatsModifiers================
    //Points
    public void AddPoints(int val){
        points += val;
    }
    public void SubtractPoints(int val){
        points -= val;
        if (points < 0)
            points = 0;
    }
    public void ResetPoints(){
        points = 0;
    }

    //Stars
    public void AddStars(int val){
        stars += val;
    }
    public void SubtractStars(int val){
        stars -= val;
        if (stars < 0)
            stars = 0;
    }

    //Health
    public void AddHealth(int val){
        health += val;
        if (health > 100){
            health = 100;
        }
    }
    public void SubtractHealth(int val){
        health -= val;
        if (health < 0){
            health = 0;
        }
    }

    //Mood
    public double GetWeightedMood(){
        return (0.5*mood + 0.5*health);
    }
    public void AddMood(int val){
        mood += val;
        if (mood > 100){
            mood = 100;
        }
    }
    public void SubtractMood(int val){
        mood -= val;
        if (mood < 0){
            mood = 0;
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
	// Based on the numerical value of the health stat,
	// returns an enum of the pet's health.
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
