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
	public MutableDataPlayPeriod PlayPeriod {get; set;}
    public MutableDataDegradation Degradation {get; set;}
    public MutableDataInhaler Inhaler {get; set;}
    public MutableDataTutorial Tutorial {get; set;}
    public MutableDataInventory Inventory {get; set;}
    public MutableDataSkill Flame {get; set;}
    public MutableDataBadge Badge {get; set;}
    public MutableDataGatingProgress GatingProgress {get; set;}
    public MutableDataRunnerGame RunnerGame {get; set;}
    public MutableDataHighScore HighScore {get; set;}
	public MutableDataFirstTimeEntrance FirstTimeEntrance {get; set;}
	public MutableDataSickNotification SickNotification {get; set;}
	public MutableDataMiniPets MiniPets {get; set;}
	public MutableDataMiniPetLocations MiniPetLocations {get; set;}
	public MutableDataMicroMix MicroMix {get; set;}

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
        Stats = new MutableDataStats();
        Level = new MutableDataPetLevel();
		PlayPeriod = new MutableDataPlayPeriod();
		Degradation = new MutableDataDegradation();
        Inhaler = new MutableDataInhaler();
        Tutorial = new MutableDataTutorial();
        Inventory = new MutableDataInventory();
        Flame = new MutableDataSkill(); 
        Badge = new MutableDataBadge();
        GatingProgress = new MutableDataGatingProgress();
        RunnerGame = new MutableDataRunnerGame();
        HighScore = new MutableDataHighScore();
		FirstTimeEntrance = new MutableDataFirstTimeEntrance();
		SickNotification = new MutableDataSickNotification();
		MiniPets = new  MutableDataMiniPets();
		MiniPetLocations = new MutableDataMiniPetLocations();
		MicroMix = new MutableDataMicroMix();
    }

	/// <summary>
	/// This function is called after the game data has been loaded. It will call individual game data
	/// version checks so that save data can properly be updated when the app is updated.
	/// </summary>
	public void VersionCheck(Version currentDataVersion) {
		Tutorial.VersionCheck(currentDataVersion);
//		GatingProgress.VersionCheck(currentDataVersion);
//		Calendar.VersionCheck(currentDataVersion);
//		PetInfo.VersionCheck(currentDataVersion);
	}
}