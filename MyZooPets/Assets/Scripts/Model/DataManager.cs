using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//This class handles all game data. No game logic
//Saves and loads data into player preference
[DoNotSerializePublic]
public class DataManager : MonoBehaviour {
    //#region Developer option
    public bool isDebugging = false;

    //#endregion

    private static bool loaded = false;
    private static bool isCreated = false; //prevent DataManager from being loaded
                                            //again during scene change
    
    //#region SaveData    
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
    
    //Calendar Data
    // [SerializeThis]
    private static List<CalendarEntry> entries; //list of entries that represent a weak
    [SerializeThis]
    private static int calenderCombo; //how many times user has open the calendar consecutively
    [SerializeThis]
    private static DateTime dateOfSunday; // keep track of the last day of the week,
                                          // so we know if we have to clear the calendar
                                          // for a new week or not.
    [SerializeThis]
    private static DateTime lastPlayedDate; //the last time that the user used the calendar
    //#endregion 

    //#region Getters
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

    //calender 
    public static List<CalendarEntry> Entries{get;set;}
    public static int CalendarCombo{
        get {return calenderCombo;}
    }
    public static DateTime LastPlayedDate{get; set;}
    public static DateTime DateOfSunday{get; set;}
    //#endregion

    //#region StatsModifiers
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
    //#endregion

    void Awake(){
        if(isCreated){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
        isCreated = true;
    }

    // Use this for initialization
    // save and load data here
    void Start () {
        //first time playing the game. values need to be initialized
        if(isDebugging) PlayerPrefs.DeleteAll();
        if (PlayerPrefs.GetInt("FirstTime", 1) > 0){
            //Evolution data initialization
            lastUpdatedTime = DateTime.UtcNow;
            durationCum = new TimeSpan(0);
            lastEvoMeter = 0;
            evoAverageCum = 0;

            //Pet stats initialization
            health = 100;
            mood = 80;
            hunger = 30;

            //Game currency initialization
            points = 0;
            stars = 100;

            //Calendar data initialization
            entries = new List<CalendarEntry>();
            // entries.Add(newDayOfWeek.Saturday, DosageRecord.Hit, DosageRecord.Hit);
            // entries.Add(DayOfWeek.Saturday, DosageRecord.Hit, DosageRecord.Hit);
            // entries.Add(DayOfWeek.Saturday, DosageRecord.Hit, DosageRecord.Hit);
            calenderCombo = 0;
            dateOfSunday = CalendarLogic.GetDateOfSunday(DateTime.Now);
            lastPlayedDate = DateTime.Now;

            //turn first time initialization off
            PlayerPrefs.SetInt("FirstTime", 0);
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

    //called when level data are loaded
    void OnDeserialized(){
        Debug.Log("health " + health + "mood " + mood);
    }

    void OnApplicationPause(bool paused){
        // Debug.Log(JSONLevelSerializer.SerializeLevel());
        if(paused){
            PlayerPrefs.SetString("_SAVE_GAME_", LevelSerializer.SerializeLevel());
            Debug.Log(LevelSerializer.SerializeLevel());
        }
    }

    //Save data before the game is quit
    void OnApplicationQuit(){

    }
}
