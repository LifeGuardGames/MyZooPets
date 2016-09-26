using System;

/// <summary>
/// Controller that deals with modifying pet's level
/// </summary>
public class LevelLogic : Singleton<LevelLogic> {
	private int maxLevel;       //what is the max level of the game
	public int MaxLevel {
		get { return maxLevel; }
	}
	
	public Level CurrentLevel { //Return current level
		get { return DataManager.Instance.GameData.Level.CurrentLevel; }
	}

	public int NextLevel {      //Return next level
		get { return (int)DataManager.Instance.GameData.Level.CurrentLevel + 1; }
	}

	void Awake() {
		maxLevel = Enum.GetNames(typeof(Level)).Length; // no need to do -1 because the index begins at 1, not 0
	}

	public bool IsAtMaxLevel() {
		bool isMaxLevel = (int)CurrentLevel == maxLevel;
		return isMaxLevel;
	}

	// Return the required points for next level up
	public int NextLevelPoints() {
		int adjustedNextLevel = NextLevel;

		// check for max level, because there may not be data that exists after it
		if(IsAtMaxLevel()) {
			adjustedNextLevel -= 1;
		}
		ImmutableDataPetLevel petLevel = DataLoaderPetLevels.GetLevel((Level)adjustedNextLevel);
		return petLevel.LevelUpCondition;
	}

	// Return the message to be displayed in notification when level up
	public string GetLevelUpMessage() {
		Level currentLevel = DataManager.Instance.GameData.Level.CurrentLevel;
		ImmutableDataPetLevel petLevel = DataLoaderPetLevels.GetLevel(currentLevel);
		return petLevel.LevelUpMessage;
	}

	public void IncrementLevel() {
		if(NextLevel <= maxLevel) {
			DataManager.Instance.GameData.Level.CurrentLevel = (Level)NextLevel;
		}
	}
}
