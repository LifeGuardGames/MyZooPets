using UnityEngine;

/*
    Class that contains all the game data of a pet
*/
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

    public PetGameData(){
        Init();
    }

    private void Init(){
		Wellapad = new WellapadData();
		// Wellapad.Init();
        PetInfo = new PetInfoData();
        // PetInfo.Init();
        Cutscenes = new CutsceneData();
        // Cutscenes.Init();
        Decorations = new DecorationSystemData();
        // Decorations.Init();
        Stats = new StatsData();
        // Stats.Init();
        Level = new PetLevelMutableData();
        // Level.Init();
        Calendar = new CalendarData();
        // Calendar.Init();
        Degradation = new DegradationData();
        // Degradation.Init();
        Inhaler = new InhalerData();
        // Inhaler.Init();
        Tutorial = new TutorialData();
        // Tutorial.Init();
        Inventory = new InventoryData();
        // Inventory.Init();
        Flame = new SkillMutableData(); 
        // Flame.Init();
        Badge = new BadgeMutableData();
        // Badge.Init();
        GatingProgress = new GatingProgressData();
        // GatingProgress.Init();
        RunnerGame = new RunnerGameData();
        // RunnerGame.Init();
    }
}