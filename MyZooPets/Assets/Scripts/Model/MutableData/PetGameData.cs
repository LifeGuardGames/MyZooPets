using UnityEngine;

//---------------------------------------------------
// All mutable data
//---------------------------------------------------
public class PetGameData{
	public WellapadData Wellapad {get; set;}
    public PetInfoData PetInfo {get; set;}
    public CutsceneData Cutscenes {get; set;} 
    public DecorationSystemData Decorations {get; set;}
    public StatsData Stats {get; set;}
    public PetLevelMutableData Level {get; set;}
    public CalendarData Calendar {get; set;}
    public DegradationData Degradation {get; set;}
    public InhalerData Inhaler {get; set;}
    public TutorialData Tutorial {get; set;}
    public InventoryData Inventory {get; set;}
    public SkillMutableData Flame {get; set;}
    public BadgeMutableData Badge {get; set;}
    public GatingProgressData GatingProgress {get; set;}
    public RunnerGameData RunnerGame {get; set;}
    public HighScoreMutableData HighScore {get; set;}

    public PetGameData(){
        Init();
    }

    private void Init(){
		Wellapad = new WellapadData();
        PetInfo = new PetInfoData();
        Cutscenes = new CutsceneData();
        Decorations = new DecorationSystemData();
        Stats = new StatsData();
        Level = new PetLevelMutableData();
        Calendar = new CalendarData();
        Degradation = new DegradationData();
        Inhaler = new InhalerData();
        Tutorial = new TutorialData();
        Inventory = new InventoryData();
        Flame = new SkillMutableData(); 
        Badge = new BadgeMutableData();
        GatingProgress = new GatingProgressData();
        RunnerGame = new RunnerGameData();
        HighScore = new HighScoreMutableData();
    }
	
	//---------------------------------------------------
	// VersionCheck()
	// This function is called after the game data has
	// been loaded.  It will call individual game data
	// version checks so that save data can properly
	// be updated when the app is updated.
	//---------------------------------------------------	
	public void VersionCheck() {
		GatingProgress.VersionCheck();
		Calendar.VersionCheck();
	}
}