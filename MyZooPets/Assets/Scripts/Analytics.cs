using System;
using System.Collections.Generic;

public class Analytics : Singleton<Analytics> {
	public const string ITEM_STATUS_BOUGHT = "Bought";
	public const string ITEM_STATUS_USED = "Used";
	public const string ITEM_STATUS_RECEIVED = "Received";

	public const string ITEM_STATS_HEALTH = "Health";
	public const string ITEM_STATS_MOOD = "Mood";

	public const string TASK_STATUS_COMPLETE = "Complete";
	public const string TASK_STATUS_FAIL = "Fail";

	public const string INHALER_CATEGORY = "MiniGame:Inhaler:";
	public const string RUNNER_CATEGORY = "MiniGame:Runner:";
	public const string DIAGNOSE_CATEGORY = "MiniGame:Clinic:";
	public const string NINJA_CATEGORY = "MiniGame:Ninja:";
	public const string MEMORY_CATEGORY = "MiniGame:Memory:";
	public const string SHOOTER_CATEGORY = "MiniGame:Shooter:";

	private bool isAnalyticsEnabled = false;

	void Awake() {
		//Get constants and check if analytics event should be sent
		isAnalyticsEnabled = Constants.GetConstant<bool>("AnalyticsEnabled");
	}

	#region Ninja Trigger
	public void NinjaGameData(int score, int bonus) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Ninja HighScore", new Dictionary<string, object>{
				{ "High Score: ", score},
				{ "Bonus: ", bonus},
			});
		}
	}
	#endregion

	#region Shooter Game
	public void ShooterGameData(int highScore, int percentage, string waveName, int combo) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Shooter HighScore", new Dictionary<string, object>{
				{ "High Score: ", highScore},
				{ "Inhaler misses: ", percentage},
				{ "Wave Number died at: ", waveName },
				{ "Inhaler combo: ", combo }
			});
		}
	}
	#endregion

	#region Memory Game
	public void MemoryGameData(int score) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Memory HighScore", new Dictionary<string, object>{
				{ "Memory HighScore: ", score}
			});
		}
	}
	#endregion

	#region RUNNER Game
	public void RUNNERGameData(int score, string level, int distanceRan) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("RUNNER HighScore", new Dictionary<string, object>{
				{ "RUNNER HighScore: ", score},
				{ "Died at: ", level},
				{ "Distance Ran: ", distanceRan}
			});
		}
	}
	#endregion

	#region Doctor Match
	public void DoctorMatchGameData(int score, int combo) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Doctor HighScore", new Dictionary<string, object>{
				{ "Doctor High Score: ", score},
				{"Combo", combo }
			});
		}
	}
	#endregion

	#region Inhaler Game
	//Which steps in inhaler game does the kids need help the most
	public void InhalerHintRequired(int stepID) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Inhaler Hint", new Dictionary<string, object>{
				{ "Hint: ", stepID}
			});
		}
	}
	#endregion

	#region MiniPet
	public void MiniPetLevelUp(string miniPetID, int currentLevel) {
		string levelString = currentLevel.ToString();
		if(!string.IsNullOrEmpty(miniPetID) && !string.IsNullOrEmpty(levelString) && isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Minipet Level Up", new Dictionary<string, object>{
				{ miniPetID + "Level up: ", currentLevel}
			});
		}
	}

	public void MiniPetVisited(string miniPetID) {
		if(!string.IsNullOrEmpty(miniPetID) && isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Times Visited Minipet", new Dictionary<string, object>{
				{ "Times Visited: ", miniPetID}
			});
		}
	}

	#endregion

	//=======================General Analytics==================================
	//Will be use in different mini games
	public void PetColorChosen(string petColor) {
		if(!string.IsNullOrEmpty(petColor) && isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Pet Color", new Dictionary<string, object>{
				{ "Color chosen: ", petColor}
			});
		}
	}

	//start game from menu scene
	public void StartGame() {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Start Game");
		}
	}

	public void RemainingTriggers(int triggers) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Remaining Triggers", new Dictionary<string, object>{
				{ "Remainder: ", triggers}
			});
		}
	}

	//Badges unlock
	public void BadgeUnlocked(string badgeID) {
		if(!string.IsNullOrEmpty(badgeID) && isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Badge Unlocked", new Dictionary<string, object>{
				{ "Badge: ", badgeID}
			});
		}
	}

	//Items used or purchased
	public void ItemEvent(string itemStatus, ItemType itemType, string itemID) {
		if(!string.IsNullOrEmpty(itemStatus) && !String.IsNullOrEmpty(itemID) && isAnalyticsEnabled) {
			if(itemStatus == "Bought") {
				Amplitude.Instance.logEvent("Item Bought", new Dictionary<string, object>{
					{ itemType.ToString()+": ", itemID}
				});
			}
			else {
				Amplitude.Instance.logEvent("Item Used", new Dictionary<string, object>{
					{ itemType.ToString()+": ", itemID}
				});
			}
		}
	}

	//What is the pet's health or mood when an item is used
	public void ConsumableEventWithPetStats(string itemID, string statsType, int statsValue) {
		if(!string.IsNullOrEmpty(itemID) && !string.IsNullOrEmpty(statsType) && isAnalyticsEnabled) {
			Amplitude.Instance.logEvent(statsType + "Increased", new Dictionary<string, object>{
				{ itemID+": ", statsValue}
			});
		}
	}

	//Wellapad
	public void WellapadTaskEvent(string taskStatus, string missionID, string taskID) {
		if(!string.IsNullOrEmpty(taskStatus) && !string.IsNullOrEmpty(missionID) &&
			!string.IsNullOrEmpty(taskID) && isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Task Status", new Dictionary<string, object>{
				{ taskID +": ", taskStatus}
			});
		}
	}

	//Wellapad xp reward claim
	public void ClaimWellapadReward() {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Reward Claimed");
		}
	}

	//Gating
	public void GateUnlocked(string gateID) {
		if(!string.IsNullOrEmpty(gateID) && isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Gate Unlocked", new Dictionary<string, object>{
				{ "Gate Unlocked: ", gateID}
			});
		}
	}

	//Tutorial completed
	public void TutorialCompleted(string tutorialID) {
		if(!string.IsNullOrEmpty(tutorialID) && isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Tutorial Completed", new Dictionary<string, object>{
				{ "Tutorial Completed: ", tutorialID}
			});
		}
	}

	//Pet level up
	public void LevelUnlocked(Level levelID) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Level Unlocked", new Dictionary<string, object>{
				{ "Level Unlocked: ", levelID}
			});
		}
	}

	public void ZeroHealth() {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("ZeroHealth");
		}
	}

	public void UserAge(int age) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("User Age", new Dictionary<string, object>{
				{ "User Age: ", age}
			});
		}
	}

	public void UserAsthma(bool hasAsthma) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("User Asthma", new Dictionary<string, object>{
				{ "User Asthma: ", hasAsthma}
			});
		}
	}

	public void TimeBetweenPlaySession(int hours) {
		if(isAnalyticsEnabled) {
			if(hours < 2000) {
				if(hours > 0) {
					Amplitude.Instance.logEvent("Time data", new Dictionary<string, object>{
						{ "Time between sessions: ",hours}
					});
				}
				else {
					Amplitude.Instance.logEvent("Time between session:" + "< 1 hr");
				}
			}
		}
	}

	public void DidUseInhaler(bool choice) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Did Use Inhaler", new Dictionary<string, object>{
				{ "Did use Inhaler: ", choice}
			});
		}
	}


	public void QuitGame(string scene) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Quit Game", new Dictionary<string, object>{
				{ "Quit At: ", scene}
			});
		}
	}

	public void BlowFire(string timesFireBlow) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Blow Fire", new Dictionary<string, object>{
				{ "Times blown fire: ", timesFireBlow}
			});
		}
	}
	public void ShowAd(string AdName) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Showing Ad", new Dictionary<string, object>{
				{ "Ad Name: ", AdName}
			});
		}
	}
	public void ClickedAd(string AdName) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Clicked Ad", new Dictionary<string, object>{
				{ "Ad Name: ", AdName}
			});
		}
	}
	public void ExitAd(string AdName) {
		if(isAnalyticsEnabled) {
			Amplitude.Instance.logEvent("Exit Ad", new Dictionary<string, object>{
				{ "Ad Name: ", AdName}
			});
		}
	}
}
