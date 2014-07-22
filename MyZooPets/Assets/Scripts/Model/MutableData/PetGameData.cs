using UnityEngine;

public class PetGameData{
	public MutableDataWellapad Wellapad {get; set;}
    public MutableDataPetInfo PetInfo {get; set;}
    public MutableDataCutscene Cutscenes {get; set;} //no longer in use
    public MutableDataDecorationSystem Decorations {get; set;}
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
	
    public PetGameData(){
        Init();
    }

    private void Init(){
		Wellapad = new MutableDataWellapad();
        PetInfo = new MutableDataPetInfo();
        Cutscenes = new MutableDataCutscene();
        Decorations = new MutableDataDecorationSystem();
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
		MiniPets = new MutableDataMiniPets();
		FirstTimeEntrance = new MutableDataFirstTimeEntrance();
    }

	/// <summary>
	/// This function is called after the game data has
	/// been loaded.  It will call individual game data
	/// version checks so that save data can properly
	/// be updated when the app is updated.
	/// </summary>
	public void VersionCheck() {
		GatingProgress.VersionCheck();
		Calendar.VersionCheck();
	}
}