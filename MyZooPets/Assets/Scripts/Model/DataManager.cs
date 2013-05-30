using UnityEngine;
using System.Collections;
using System;

//This class handles all game data. No game logic
//Saves and loads data into player preference
public class DataManager : MonoBehaviour {

    private static bool firstTime = true; // starting game for the first time
    public static bool FirstTime{
        get {return firstTime;}
        set {
            if (!value) {
                firstTime = value;
            }
        }
    }

    //points that are used for activating evolution
    private static int points;
    private static int stars;
    private static int health;
    private static int mood;
    private static int hunger;

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

    //Data for evolution calculation
    public static DateTime lastUpdatedTime;
    public static TimeSpan durationCum;
    public static double lastEvoVal;
    public static double evoAverageCum;

    //other mini game data

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

    // Use this for initialization
    // save and load data here
    // handles first time login?
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }


}
