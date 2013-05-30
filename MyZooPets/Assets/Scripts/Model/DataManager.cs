using UnityEngine;
using System.Collections;
using System;

//This class handles all game data. No game logic
//Saves and loads data into player preference
public class DataManager : MonoBehaviour {

    private static bool firstTime = true; // starting game for the first time

    //points that are used for activating evolution
    private static int points; //evolution points
    private static int stars; //currency of the game
    private static int health; //pet's health
    private static int mood; //pet's mood (weighted or unweighted)
    private static int hunger; //pet's hunger

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

    //Evolution Data
    public static DateTime lastUpdatedTime; //last time evolution meter was calculated
    public static TimeSpan durationCum; //the total time since hatching the pet
    public static double lastEvoMeter; //last calculated evolution meter
    public static double evoAverageCum; //cumulative average of evolution meter
                                        //use this to decide how to evolve pet

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
    void Start () {
        //first time playing the game. values need to be initialized
        if (firstTime){
            //Evolution Data initialization
            lastUpdatedTime = DateTime.UtcNow;
            durationCum = new TimeSpan(0);
            lastEvoMeter = 0;
            evoAverageCum = 0;

            //Pet stats initialization
            health = 50;
            mood = 80;
            hunger = 30;

            //Game currency initialization
            points = 0;
            stars = 100;

            firstTime = false;
        }
    }

    // Update is called once per frame
    void Update () {

    }


}
