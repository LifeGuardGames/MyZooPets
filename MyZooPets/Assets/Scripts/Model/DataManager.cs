using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//This class handles all game data. No game logic
//Saves and loads data into player preference
[DoNotSerializePublic]
public class DataManager : MonoBehaviour {
    //========Developer option=============
    public bool removePreviouslySavedData; //delete all from PlayerPrefs
    public bool isDebug = false; //turn isDebug to true if working on independent scene
    private bool loaded = false;
    //=====================================

    private static bool isCreated = false; //prevent DataManager from being loaded
                                            //again during scene change
    private static bool firstTime; //is the user playing for the first time

    //==========SaveData============
    //pet info
    [SerializeThis]
    private static string petName; //name of the pet
    [SerializeThis]
    private static string petColor; //color of the pet

    //stats
    [SerializeThis]
    private static int points; //evolution points
    [SerializeThis]
    private static int stars; //currency of the game
    [SerializeThis]
    private static int health; //pet's health
    [SerializeThis]
    private static int mood; //pet's mood (weighted or unweighted)
    [SerializeThis]
    private static int hunger; //pet's hunger

    //Evolution Data
    [SerializeThis]
    public static DateTime lastUpdatedTime; //last time evolution meter was calculated
    [SerializeThis]
    public static TimeSpan durationCum; //the total time since hatching the pet
    [SerializeThis]
    public static double lastEvoMeter; //last calculated evolution meter
    [SerializeThis]
    public static double evoAverageCum; //cumulative average of evolution meter
                                        //use this to decide how to evolve pet
    [SerializeThis]
    public static EvoStage evoStage; //current evolution stage

    //Calendar Data
    [SerializeThis]
    private static List<CalendarEntry> entries; //list of entries that represent a weak
    [SerializeThis]
    private static int calendarCombo; //how many times user has open the calendar consecutively
    [SerializeThis]
    private static DateTime dateOfSunday; // keep track of the last day of the week,
                                          // so we know if we have to clear the calendar
                                          // for a new week or not.
    [SerializeThis]
    private static DateTime lastCalendarOpenedTime; //the last time that the user used the calendar

    //Inhaler Data
    [SerializeThis]
    private static int numberOfTimesLeft; //max 1. once for every optimal time
    [SerializeThis]
    private static DateTime optimalMorningStartTime; //the optimal start time to take inhaler in the morning
    [SerializeThis]
    private static DateTime optimalAfternoonStartTime; //optimal start time to take inhaler in the afternoon
    [SerializeThis]
    private static DateTime lastInhalerGamePlayed; //the time user last played the game
    [SerializeThis]
    private static bool canPlayGameMorning; //can user play in the morning (optimal game)
    [SerializeThis]
    private static bool canPlayGameAfternoon; //can user play in the afternoon (optimal game)
    [SerializeThis]
    private static bool firstTimeAdvair;
    // first time the player has seen the advair inhaler (this tells us whether to show tutorial arrows in the Inhaler Game)
    [SerializeThis]
    private static bool firstTimeRescue;
    // first time the player has seen the rescue inhaler (this tells us whether to show tutorial arrows in the Inhaler Game)
    //========================

    //=============Getters & Setters===============
    public static string PetName{
        get{return petName;}
        set{petName = value;}
    }
    public static string PetColor{
        get{return petColor;}
        set{petColor = value;}
    }

    //First Time. use this to know if user is player game for the first time
    public static bool FirstTime{
        get {return firstTime;}
        set {firstTime = value;}
    }

    //Stats
    public static int Points{
        get {return points;}
    }
    public static int Stars{
        get {return stars;}
    }
    public static int Health{
        get {return health;}
    }
    public static int Mood{
        get {return mood;}
    }
    public static int Hunger{
        get {return hunger;}
    }

    //calendar
    public static List<CalendarEntry> Entries{
        get{return entries;}
        set{entries = value;}
    }
    public static int CalendarCombo{
        get {return calendarCombo;}
    }
    public static DateTime LastCalendarOpenedTime{
        get { return lastCalendarOpenedTime;}
        set { lastCalendarOpenedTime = value;}
    }

    public static DateTime DateOfSunday{
        get { return dateOfSunday;}
        set { dateOfSunday = value;}
    }

    //Inhaler
    public static DateTime LastInhalerGamePlayed{
        get{return lastInhalerGamePlayed;}
        set{lastInhalerGamePlayed = value;}
    }
    public static int SlotMachineCounter{
        get{return slotMachineCounter;}
        set{slotMachineCounter = value;}
    }
    public static int NumberOfTimesLeft{
        get{return numberOfTimesLeft;}
        set{numberOfTimesLeft = value;}
    }
    public static bool CanPlayGameMorning{
        get{return canPlayGameMorning;}
        set{canPlayGameMorning = value;}
    }
    public static bool CanPlayGameAfternoon{
        get{return canPlayGameAfternoon;}
        set{canPlayGameAfternoon = value;}
    }
    public static DateTime OptimalMorningStartTime{
        get{return optimalMorningStartTime;}
        set{optimalMorningStartTime = value;}
    }
    public static DateTime OptimalAfternoonStartTime{
        get{return optimalAfternoonStartTime;}
        set{optimalAfternoonStartTime = value;}
    }
    public static bool FirstTimeAdvair{
        get{return firstTimeAdvair;}
        set{firstTimeAdvair = value;}
    }
    public static bool FirstTimeRescue{
        get{return firstTimeRescue;}
        set{firstTimeRescue = value;}
    }
    //===============================

    //==============StatsModifiers================
    //Points
    public static void AddPoints(int val){
        points += val;
    }
    public static void SubtractPoints(int val){
        points -= val;
        if (points < 0)
            points = 0;
    }

    //Stars
    public static void AddStars(int val){
        stars += val;
    }
    public static void SubtractStars(int val){
        stars -= val;
        if (stars < 0)
            stars = 0;
    }

    //Health
    public static void AddHealth(int val){
        health += val;
        if (health > 100){
            health = 100;
        }
    }
    public static void SubtractHealth(int val){
        health -= val;
        if (health < 0){
            health = 0;
        }
    }

    //Mood
    public static double GetWeightedMood(){
        return (0.3*mood + 0.35*hunger + 0.35*health);
    }
    public static void AddMood(int val){
        mood += val;
        if (mood > 100){
            mood = 100;
        }
    }
    public static void SubtractMood(int val){
        mood -= val;
        if (mood < 0){
            mood = 0;
        }
    }

    //Hunger
    public static void AddHunger(int val){
        hunger += val;
        if (hunger > 100){
            hunger = 100;
        }
    }
    public static void SubtractHunger(int val){
        hunger -= val;
        if (hunger < 0){
            hunger = 0;
        }
    }

    // Calendar Combo
    public static void IncrementCalendarCombo(){
        calendarCombo ++;
    }
    public static void ResetCalendarCombo(){
        calendarCombo = 0;
    }
    //===================================

    void Awake(){
        if(isCreated){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
        isCreated = true;

        // Use this for initialization
        // save and load data here
        //first time playing the game. values need to be initialized
        if(removePreviouslySavedData) PlayerPrefs.DeleteAll();

        firstTime = PlayerPrefs.GetInt("FirstTime", 1) > 0;

    }

    void Start(){
        if(isDebug){ //debug for independent scene. only initialize data no
                    //serialization or scene loading
            InitializeAllDataFirstTime();
        }else{
            if (firstTime){ //first time data initialization logic
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

    //initialize all data for the first time
    private void InitializeAllDataFirstTime(){
        //Evolution data initialization
            lastUpdatedTime = DateTime.Now;
            durationCum = new TimeSpan(0,0,10);
            lastEvoMeter = 90;
            evoAverageCum = 90;

            //Pet stats initialization
            health = 100;
            mood = 80;
            hunger = 30;

            //Game currency initialization
            points = 0;
            stars = 100;

            //Calendar data initialization
            entries = new List<CalendarEntry>();
            calendarCombo = 0;
            dateOfSunday = CalendarLogic.GetDateOfSunday(DateTime.Now);

            // populate previous entries with DosageRecord.Null up to today's entry
            // to make it more consistent - DataManager.Entries will always start with Monday's entry
            DateTime lastSunday = dateOfSunday.AddDays(-7);
            TimeSpan sinceLastSunday = DateTime.Today.Subtract(lastSunday);
            for (int i = 1; i < sinceLastSunday.Days; i++){ // exclude today's entry, as that will be generated later
                DateTime day = lastSunday.AddDays(i);
                entries.Add(new CalendarEntry(day.DayOfWeek, DosageRecord.Null, DosageRecord.Null) );
            }

            // set to one day before today so that the entry will be generated for the first day
            lastCalendarOpenedTime = DateTime.Today.AddDays(-1);

            //Inhaler game data initialization
            // optimalMorningStartTime = new DateTime();
            // optimalAfternoonStartTime = new DateTime();
            lastInhalerGamePlayed = DateTime.Now;
            canPlayGameMorning = true;
            canPlayGameAfternoon = true;
            firstTimeAdvair = true;
            firstTimeRescue = true;

            //turn first time initialization off
            PlayerPrefs.SetInt("FirstTime", 0);
    }

    //call the delegate when data initialization or deserialziation is done
    private void DataLoaded(){
        Application.LoadLevel("BedRoom");
    }

    //serialize data into byte array and store locally in PlayerPrefs
    private static void SerializeGame(){
        PlayerPrefs.SetString("_SAVE_GAME_", LevelSerializer.SerializeLevel());
        print(JSONLevelSerializer.SerializeLevel());
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
