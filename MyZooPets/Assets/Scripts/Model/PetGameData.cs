using UnityEngine;

/*
    Class that contains all the game data of a pet
*/
[DoNotSerializePublic]
public class PetGameData{

    [SerializeThis]
    public PetInfoData PetInfo;
    [SerializeThis] 
    public CutsceneData Cutscenes;  
    [SerializeThis] 
    public DecorationSystemData Decorations;    
    [SerializeThis] 
    public StatsData Stats;
    [SerializeThis]
    public LevelUpData Level;
    [SerializeThis]
    public CalendarData Calendar;
    [SerializeThis]
    public DegradationData Degradation;
    [SerializeThis]
    public InhalerData Inhaler;
    [SerializeThis]
    public TutorialData Tutorial;
    [SerializeThis]
    public InventoryData Inventory;
    [SerializeThis]
    public SkillMutableData Dojo;
    [SerializeThis]
    public BadgeMutableData Badge; 
    [SerializeThis]
    public GatingProgressData GatingProgress;

    public PetGameData(){}

    public void Init(){
        PetInfo = new PetInfoData();
        PetInfo.Init();
        Cutscenes = new CutsceneData();
        Cutscenes.Init();
        Decorations = new DecorationSystemData();
        Decorations.Init();
        Stats = new StatsData();
        Stats.Init();
        Level = new LevelUpData();
        Level.Init();
        Calendar = new CalendarData();
        Calendar.Init();
        Degradation = new DegradationData();
        Degradation.Init();
        Inhaler = new InhalerData();
        Inhaler.Init();
        Tutorial = new TutorialData();
        Tutorial.Init();
        Inventory = new InventoryData();
        Inventory.Init();
        Dojo = new SkillMutableData(); 
        Dojo.Init();
        Badge = new BadgeMutableData();
        Badge.Init();
        GatingProgress = new GatingProgressData();
        GatingProgress.Init();
    }
}