using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using GameAnalyticsSDK;

public class Analytics : MonoBehaviour {

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

    private static bool isCreated = false;
    private static Analytics instance;
    // private DateTime playTime;
    // private bool isGameTimerOn = false;
    private bool isAnalyticsEnabled = false;


    //This instance creates itself if it's not in the scene.
    //Mainly for debugging purpose
    public static Analytics Instance{
        get{
            if(instance == null){

                instance = (Analytics) FindObjectOfType(typeof(Analytics));
                
                if(instance == null){
                    GameObject analyticsGO = (GameObject) Instantiate((GameObject) Resources.Load("Analytics"));
                    instance = analyticsGO.GetComponent<Analytics>();
                }
            }
            return instance;
        }
    }
    //--------------------------------------------------------------------------

    void Awake(){
        //make persistent
        if(isCreated){
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        isCreated = true;

        //Get constants and check if analytics event should be sent
        isAnalyticsEnabled = Constants.GetConstant<bool>("AnalyticsEnabled");
    }

	#region Ninja Trigger

	public void NinjaHighScore(int score){
	if(isAnalyticsEnabled){
			GA_Design.NewEvent(NINJA_CATEGORY + "HighScore", (float) score);
		}
	}

	public void NinjaBonusRounds(int bonus){
		if(isAnalyticsEnabled){
			GA_Design.NewEvent(NINJA_CATEGORY + "BonusRound", (float) bonus);
		}
	}

	public void NinjaTimesPlayedTick(){
	if(isAnalyticsEnabled){
			GA_Design.NewEvent(NINJA_CATEGORY + "TimesPlayed", 1f);
		}
	}
	#endregion

	#region Shooter Game
	public void ShooterHighScore(int score){
		if(isAnalyticsEnabled){
			GA_Design.NewEvent(SHOOTER_CATEGORY + "HighScore", (float) score);
		}
	}

	public void ShooterPercentageMissed(int percentage){
		if(isAnalyticsEnabled){
			GA_Design.NewEvent(SHOOTER_CATEGORY + "Missed inhaler percentage: ", (float) percentage);
		}
	}
	public void ShooterTimesPlayedTick(){
		if(isAnalyticsEnabled){
			GA_Design.NewEvent(SHOOTER_CATEGORY + "TimesPlayed", 1f);
		}
	}
	#endregion

	#region Memory Game
	public void MemoryHighScore(int score){
		if(isAnalyticsEnabled){
			GA_Design.NewEvent(MEMORY_CATEGORY + "HighScore", (float) score);
		}
	}

	public void MemoryTimesPlayedTick(){
		if(isAnalyticsEnabled){
			GA_Design.NewEvent(MEMORY_CATEGORY + "TimesPlayed", 1f);
		}
	}
	#endregion

	#region Runner Game
    //Where did the user die most often in the runner game?
//    public void RunnerPlayerDied(string levelComponentName){
//        if(!String.IsNullOrEmpty(levelComponentName) && isAnalyticsEnabled)
//            GA_Design.NewEvent(RUNNER_CATEGORY + "Dead:" + levelComponentName);
//    }

	public void RunnerHighScore (int score){
	if(isAnalyticsEnabled){
		GA_Design.NewEvent(RUNNER_CATEGORY + "HighScore", (float) score);
		}
	}

    public void RunnerPlayerDistanceRan(int distanceRan){
        if(isAnalyticsEnabled)
            GA_Design.NewEvent(RUNNER_CATEGORY + "DistanceRan", (float) distanceRan);
    }

	public void RunnerTimesPlayedTick(){
		if(isAnalyticsEnabled){
			GA_Design.NewEvent(RUNNER_CATEGORY + "TimesPlayed", 1f);
		}
	}

	#endregion

	#region Inhaler Game
    //Which steps in inhaler game does the kids need help the most
    public void InhalerHintRequired(int stepID){
        if(isAnalyticsEnabled)
            GA_Design.NewEvent(INHALER_CATEGORY + "HintRequired:" + stepID);
    }
	#endregion

	#region Doctor Match
    //==========================Diagnose track game=============================
    //Number of correct diagnose. 
    //Which symptom is the user having trouble identifying
    public void DiagnoseResult(string diagnoseResult, AsthmaStage petStatus, AsthmaStage zone){
//        if(!String.IsNullOrEmpty(diagnoseResult) && isAnalyticsEnabled)
//            GA_Design.NewEvent(DIAGNOSE_CATEGORY + "Diagnose:" + diagnoseResult + ":" + 
//                Enum.GetName(typeof(AsthmaStage), petStatus) + ":" + Enum.GetName(typeof(AsthmaStage), zone));
    }

	public void WrongDiagnose(){

	}

	public void DieAtWhatSpeed(){

	}

	public void DoctorHighScore (int score){
		if(isAnalyticsEnabled){
			GA_Design.NewEvent(DIAGNOSE_CATEGORY + "HighScore", (float) score);
		}
	}

	public void DoctorTimesPlayedTick(){
		if(isAnalyticsEnabled){
			GA_Design.NewEvent(DIAGNOSE_CATEGORY + "TimesPlayed", 1f);
		}
	}

	#endregion

	#region MiniPet
	public void MiniPetLevelUp(string miniPetID, int currentLevel){
		string levelString = currentLevel.ToString();
		if(!String.IsNullOrEmpty(miniPetID) && !String.IsNullOrEmpty(levelString) && isAnalyticsEnabled){
			GA_Design.NewEvent("MiniPet:LevelUnlocked:" + levelString + ":" + miniPetID);
		}
	}

	public void MiniPetVisited(string miniPetID, int timesVisted){
		string visitString = timesVisted.ToString();
		if(!String.IsNullOrEmpty(miniPetID) && !String.IsNullOrEmpty(visitString) && isAnalyticsEnabled){
			GA_Design.NewEvent("MiniPet:LevelUnlocked:" + visitString + ":" + miniPetID);
		}
	}

	#endregion

	#region Friend Network
	/*public void EnterFriendTree(){
		GA_Design.NewEvent("Social:EnterFriendTree");
	}

	public void AddFriend(){
		GA_Design.NewEvent("Social:AddFriend");
	}

	public void AcceptFriendRequest(){
		GA_Design.NewEvent("Social:AcceptFriendRequest");
	}*/
	#endregion

    //=======================General Analytics==================================
    //Will be use in different mini games

    public void PetColorChosen(string petColor){
        if(!String.IsNullOrEmpty(petColor) && isAnalyticsEnabled)
            GA_Design.NewEvent("PetColorChosen:" + petColor);
    }

	//start game from menu scene
	public void StartGame(){
		GA_Design.NewEvent("StartGame");
	}

    //when the user clean the triggers
 /*   public void TriggersCleaned(String triggerID){
        if(!String.IsNullOrEmpty(triggerID) && isAnalyticsEnabled)
            GA_Design.NewEvent("TriggersCleaned:" + triggerID);
    }*/

	public void RemainingTriggers(int triggers){
		if(isAnalyticsEnabled){
			GA_Design.NewEvent("Uncleaned Triggers", (float) triggers);
		}
	}

    //Badges unlock
    public void BadgeUnlocked(string badgeID){
        if(!String.IsNullOrEmpty(badgeID) && isAnalyticsEnabled)
            GA_Design.NewEvent("Badge:Unlocked:" + badgeID);
    }

    //Items used or purchased
    public void ItemEvent(string itemStatus, ItemType itemType, string itemID){
        if(!String.IsNullOrEmpty(itemStatus) && !String.IsNullOrEmpty(itemID) && isAnalyticsEnabled){
            GA_Design.NewEvent("Items:" + itemStatus + ":" + 
                Enum.GetName(typeof(ItemType), itemType) + ":" + itemID);
        }
    }

    //What is the pet's health or mood when an item is used
    public void ConsumableEventWithPetStats(string itemID, string statsType, int statsValue){
        if(!String.IsNullOrEmpty(itemID) && !String.IsNullOrEmpty(statsType) && isAnalyticsEnabled){
            GA_Design.NewEvent("Items:" + ITEM_STATUS_USED + ":" + itemID + ":" +
                statsType, (float) statsValue);
        }
    }

    //Wellapad
    public void WellapadTaskEvent(string taskStatus, string missionID, string taskID){
        if(!String.IsNullOrEmpty(taskStatus) && !String.IsNullOrEmpty(missionID) && 
            !String.IsNullOrEmpty(taskID) && isAnalyticsEnabled)
            GA_Design.NewEvent("Wellapad:Task:" + taskStatus + ":" + missionID + ":" + taskID);
    }

    //Wellapad xp reward claim
    public void ClaimWellapadReward(){
        if(isAnalyticsEnabled)
            GA_Design.NewEvent("Wellapad:Collect:Reward");
    }

    //Gating
    public void GateUnlocked(string gateID){
        if(!String.IsNullOrEmpty(gateID) && isAnalyticsEnabled)
            GA_Design.NewEvent("Gate:Unlocked:" + gateID);
    }

    //Tutorial completed
    public void TutorialCompleted(string tutorialID){
        if(!String.IsNullOrEmpty(tutorialID) && isAnalyticsEnabled)
            GA_Design.NewEvent("Tutorial:Completed:" + tutorialID);
    }
	
    //Pet level up
    public void LevelUnlocked(Level levelID){
        if(isAnalyticsEnabled)
            GA_Design.NewEvent("PetLevel:Unlocked:" + levelID.ToString());
    }

    public void ZeroHealth(){
        if(isAnalyticsEnabled)
            GA_Design.NewEvent("ZeroHealth");
    }

    /*public void TriggerHitPet(){
        if(isAnalyticsEnabled)
            GA_Design.NewEvent("TriggerHitPet");
    }*/

	public void UserAge(int age){
		if(isAnalyticsEnabled)
			GA_Design.NewEvent("UserInfo:Age:" + age.ToString());
	}

	public void UserAsthma(bool hasAsthma){
		if(isAnalyticsEnabled)
			GA_Design.NewEvent("UserInfo:Asthma:" + hasAsthma.ToString());
	}

	public void TimeBetweenPlaySession(int hours){
		if(isAnalyticsEnabled){
			if(hours > 0){
				GA_Design.NewEvent("Time between session:" + hours.ToString());
				GA_Design.NewEvent("Avg time between session", hours);
			}
			else{
				GA_Design.NewEvent("Time between session:" + "< 1 hr");
			}
		}
	}

	public void DidUseInhaler(bool choice){
		GA_Design.NewEvent("Did use Inhaler " + choice);
	}

	public void RunnerDied(string level){
		GA_Design.NewEvent("Runner died On: " + level);
	}

	public void QuitGame(string scene){
		GA_Design.NewEvent("Game Quit: " + scene);
	}

}
