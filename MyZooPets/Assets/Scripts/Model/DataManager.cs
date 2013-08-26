using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//This class handles all game data. No game logic
//Saves and loads data into player preference
[DoNotSerializePublic]
public class DataManager : Singleton<DataManager>{
    //========Developer option=============
    public bool removeDataOnDestroy; //delete all from PlayerPrefs
    public bool isDebug = false; //turn isDebug to true if working on independent scene
    public bool calendarDummyData = false;
    private bool loaded = false;
    //=====================================

    private static bool isCreated = false; //prevent DataManager from being loaded
                                            //again during scene change (needs to be static)
    private bool firstTime; //is the user playing for the first time

    //==========SaveData============

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
    public DojoRawData Dojo;
    [SerializeThis]
    public BadgeRawData Badge; 

    //pet info
    [SerializeThis]
    private string petName; //name of the pet
    [SerializeThis]
    private string petColor; //color of the pet

    //=============Getters & Setters===============
    public string PetName{
        get{return petName;}
        set{petName = value;}
    }
    public string PetColor{
        get{return petColor;}
        set{petColor = value;}
    }

    //First Time. use this to know if user is player game for the first time
    public bool FirstTime{
        get {return firstTime;}
        set {firstTime = value;}
    }

    // //Badge
    // public BadgeData[] BadgeStatus{
    //     get{return badgeStatus;}
    //     set{badgeStatus = value;}
    // }
    //===================================
    public void TurnFirstTimeOff(){
        firstTime = false;
        PlayerPrefs.SetInt("FirstTime", 0);
    }

    void Awake(){
        if(isCreated){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
        isCreated = true;

        firstTime = PlayerPrefs.GetInt("FirstTime", 1) > 0;

        if(isDebug){ //debug for independent scene. only initialize data no
                    //serialization or scene loading
            InitializeAllDataFirstTime();
        }
    }

    //LevelSerailizer.LoadSavedLevel needs to be called in Start()
    void Start(){
        if(!isDebug){
            if(firstTime){ //first time data initialization logic
                InitializeAllDataFirstTime();
                SerializeGame();
                DataLoaded();
            }else{ //load saved data
                if(!loaded){
                    loaded = true;
                    string data = PlayerPrefs.GetString("_SAVE_GAME_","");
                    if(!string.IsNullOrEmpty(data)){
                        LevelSerializer.LoadSavedLevel(data);
                    }
                }
            }
        }
    }

    void OnDestroy(){
        if(isDebug){
            if(removeDataOnDestroy) PlayerPrefs.DeleteAll();
        }
    }

    //initialize all data for the first time
    private void InitializeAllDataFirstTime(){
        Stats = new StatsData();
        Stats.Init();
        Level = new LevelUpData();
        Level.Init();
        Calendar = new CalendarData();
        Calendar.Init(calendarDummyData);
        Degradation = new DegradationData();
        Degradation.Init();
        Inhaler = new InhalerData();
        Inhaler.Init();
        Tutorial = new TutorialData();
        Tutorial.Init();
        Inventory = new InventoryData();
        Inventory.Init();
        Dojo = new DojoRawData(); 
        Dojo.Init();
        Badge = new BadgeRawData();
        Badge.Init();

        //Pet info
        petName = "LazyWinkle";
        //Badge
        // badgeStatus = new BadgeData[BadgeLogic.MAX_BADGE_COUNT];
        // int counter = 0;
        // for(int i=0; i<BadgeLogic.MAX_BADGE_COUNT; i++){
        //     BadgeData badgeData = new BadgeData();
        //     // if(counter < 7){
        //     //     badgeData.IsAwarded = true;
        //     //     badgeData.Tier = BadgeTier.Gold;
        //     // }
        //     // counter++;
        //     badgeStatus[i] = badgeData;
        // }
        // badgeStatus[0].IsAwarded = true;
        // badgeStatus[0].Tier = BadgeTier.Gold;
    }

    //call the delegate when data initialization or deserialziation is done
    private void DataLoaded(){
        if(firstTime){
            Application.LoadLevel("FirstTime");
        }else{
            Application.LoadLevel("NewBedRoom");
        }
    }

    //serialize data into byte array and store locally in PlayerPrefs
    private void SerializeGame(){
        PlayerPrefs.SetString("_SAVE_GAME_", LevelSerializer.SerializeLevel());
        // print(JSONLevelSerializer.SerializeLevel());
    }

    //called when level data are loaded
    void OnDeserialized(){
        DataLoaded();
    }

    //serialize the data whenever the game is paused
    void OnApplicationPause(bool paused){
        if(paused){
            SerializeGame();
        }
    }
}
