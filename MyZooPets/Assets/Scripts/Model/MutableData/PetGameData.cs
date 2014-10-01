using UnityEngine;
using System;
using System.Collections.Generic;

public class PetGameData{
	public MutableDataWellapad Wellapad {get; set;}
    public MutableDataPetInfo PetInfo {get; set;}
    public MutableDataCutscene Cutscenes {get; set;} //no longer in use
    public MutableDataDecorationSystem Decorations {get; set;}
	public MutableDataAccessories Accessories {get; set;}
    public MutableDataStats Stats {get; set;}
    public MutableDataPetLevel Level {get; set;}
    public MutableDataCalendar Calendar {get; set;}
    public MutableDataDegradation Degradation {get; set;}
    public MutableDataInhaler Inhaler {get; set;}
    public MutableDataTutorial Tutorial {get; set;}
    public MutableDataInventory Inventory {get; set;}
    public MutableDataSkill Flame {get; set;}
    public MutableDataBadge Badge {get; set;}
    public MutableDataGatingProgress GatingProgress {get; set;}
    public MutableDataRunnerGame RunnerGame {get; set;}
    public MutableDataHighScore HighScore {get; set;}
	public MutableDataMiniPets MiniPets {get; set;}
	public MutableDataFirstTimeEntrance FirstTimeEntrance {get; set;}
	public MutableDataSickNotification SickNotification {get; set;}

	private List<MutableData> allMutableData;
	
    public PetGameData(){
		allMutableData = new List<MutableData>();
        Init();
    }

    private void Init(){
		Wellapad = new MutableDataWellapad();
        PetInfo = new MutableDataPetInfo();
		allMutableData.Add(PetInfo);

        Cutscenes = new MutableDataCutscene();
        Decorations = new MutableDataDecorationSystem();

		Accessories = new MutableDataAccessories();
		allMutableData.Add(Accessories);

        Stats = new MutableDataStats();
        Level = new MutableDataPetLevel();
        Calendar = new MutableDataCalendar();
        Degradation = new MutableDataDegradation();
        Inhaler = new MutableDataInhaler();
        Tutorial = new MutableDataTutorial();
        Inventory = new MutableDataInventory();
        Flame = new MutableDataSkill(); 
        Badge = new MutableDataBadge();
        GatingProgress = new MutableDataGatingProgress();
        RunnerGame = new MutableDataRunnerGame();
        HighScore = new MutableDataHighScore();
		MiniPets = new  MutableDataMiniPets();
		FirstTimeEntrance = new MutableDataFirstTimeEntrance();
		SickNotification = new MutableDataSickNotification();
    }

	/// <summary>
	/// This function is called after the game data has
	/// been loaded.  It will call individual game data
	/// version checks so that save data can properly
	/// be updated when the app is updated.
	/// </summary>
	public void VersionCheck(Version currentDataVersion) {
		GatingProgress.VersionCheck(currentDataVersion);
		Calendar.VersionCheck(currentDataVersion);
		PetInfo.VersionCheck(currentDataVersion);
		Tutorial.VersionCheck(currentDataVersion);
	}

	public void SaveAsyncToParse(){

		ExtraParseLogic.Instance.UserAndKidAccountCheck().ContinueWith(t => {
			KidAccount kidAccount = t.Result;

			foreach(MutableData data in allMutableData)
				if(data.IsDirty)
					data.SaveAsyncToParseServer(kidAccount.ObjectId);
		});

	}
}