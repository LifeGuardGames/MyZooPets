using UnityEngine;
using System.Collections;
using System;

//---------------------------------------------------
// LevelLogic
// Controller that deals with modifying pet's level
//---------------------------------------------------
public class LevelLogic : Singleton<LevelLogic> {
    private int maxLevel;

    public Level CurrentLevel{
        get{return DataManager.Instance.GameData.Level.CurrentLevel;}
    }
	
	public bool IsAtMaxLevel() {
		bool bMax = (int)CurrentLevel == maxLevel;
		return bMax;
	}

    void Awake(){
        maxLevel = Enum.GetNames(typeof(Level)).Length;	// no need to do -1 because the index begins at 1, not 0
    }

    //------------------------------------------------
    // NextLevelPoints()
    // Return the required points for next level up
    //------------------------------------------------
    public int NextLevelPoints(){
        int nextLevel = (int) DataManager.Instance.GameData.Level.CurrentLevel + 1;
		
		// check for max level, because there may not be data that exists after it
		if ( IsAtMaxLevel() )
			nextLevel -= 1;
		
        PetLevel petLevel = DataPetLevels.GetLevel((Level) nextLevel);

        return petLevel.LevelUpCondition;
    }

    //------------------------------------------------
    // GetLevelUpMessage()
    // Return the message to be displayed in notification when
    // level up
    //------------------------------------------------
    public string GetLevelUpMessage(){
        Level currentLevel = DataManager.Instance.GameData.Level.CurrentLevel;
        PetLevel petLevel = DataPetLevels.GetLevel(currentLevel);
        return petLevel.LevelUpMessage;
    }

    //------------------------------------------------
    // IncrementLevel()
    // Levelup
    //------------------------------------------------
    public void IncrementLevel(){
        int nextLevel = (int) DataManager.Instance.GameData.Level.CurrentLevel + 1;

        if(nextLevel <= maxLevel)
            DataManager.Instance.GameData.Level.CurrentLevel = (Level)nextLevel;
    }

    // void OnGUI(){
    //     if(GUI.Button(new Rect(0, 0, 100, 100), "level up")){
    //         StatsController.Instance.ChangeStats(1000, Vector3.zero, 0, Vector3.zero,
    //             0, Vector3.zero, 0, Vector3.zero);
    //     }
    // }
}
