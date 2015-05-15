using UnityEngine;
using System.Collections;

//---------------------------------------------------
// StatsData 
// Save data for stats 
// Mutable data.
//---------------------------------------------------
public class MutableDataStats{
    public int Points {get; set;} //evolution Points
    public int Stars {get; set;} //currency of the game
	public int totalStars {get; set;}
    public int Health {get; set;} //pet's Health
    public int Mood {get; set;} //pet's mood (refer to as hungry)
	public int Shards {get; set;}	// Shard count, get out of 100 for now
	
	// constants
	// if the pet's mood <= this number, it will be sad
	private static int SAD_THRESH = 50;
	
	// if the pet's Health <= this number, it will be sick
	private static int SICK_THRESH = 60;
	
	// if the pet's Health <= this number, it will be very sick
	private static int VERY_SICK_THRESH = 30;	

    //=======================Initialization==================
    public MutableDataStats(){
        Init();
    }

    private void Init(){
        Health = 80;
        Mood = 80;
        Points = 0;
		Stars = 100;
		Shards = 0;
		totalStars = 100;
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
		totalStars += val;
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
	
    public void AddMood(int val){
		// cache old mood
		//int nOldMood = Mood;
		
		// update mood
        Mood += val;
		
		// max mood out at 100%
        if (Mood > 100){
            Mood = 100;
        }
		
		// do a completion check for the pet's hunger (mood) level
		if ( Mood == 100 )
			WellapadMissionController.Instance.TaskCompleted( "FeedPet" );
    }
    public void SubtractMood(int val){
        Mood -= val;
        if (Mood < 0){
            Mood = 0;
        }
    }

	// The animation takes care of rewarding the crystal after 100 shard... take care of it here?
	public void AddShard(int val){
		Shards += val;
		if(Shards >= 100){
			ResetShard();
		}
	}

	public void ResetShard(){
		Shards = 0;
	}

	/// <summary>
	/// Gets the state of the mood. Based on the numerical value of the mood stat,
	/// returns an enum of the pet's mood
	/// </summary>
	/// <returns>The mood state.</returns>
	public PetMoods GetMoodState() {
		PetMoods eMood = PetMoods.Happy;
		
		if ( Mood <= SAD_THRESH )
			eMood = PetMoods.Sad;
		
		return eMood;
	}

	/// <summary>
	/// Gets the state of the health. Based on the numerical value of the Health stat,
	/// returns an enum of the pet's health.
	/// </summary>
	/// <returns>The health state.</returns>
	public PetHealthStates GetHealthState() {
		PetHealthStates eHealth = PetHealthStates.Healthy;
		
		if ( Health <= VERY_SICK_THRESH )
			eHealth = PetHealthStates.VerySick;
		else if ( Health <= SICK_THRESH )
			eHealth = PetHealthStates.Sick;
		
		return eHealth;
	}	
}
