using UnityEngine;
using System.Collections;
using System;

//This class handles all game data. No game logic
//Saves and loads data into player preference
public class DataManager : MonoBehaviour {

    //points that are used for activating evolution
    private int points;
    private int stars;
    private int health;
    private int mood;
    private int hunger;

    //Data for evolution calculation
    public DateTime lastUpdatedTime;
    public TimeSpan durationCum;
    public double lastEvoVal;
    public double evoAverageCum;

    //other mini game data

    //Points
    public int GetPoints(){
        return points;
    }
    public void AddPoints(int val){
        points += val;
    }
    public void SubtractPoints(int val){
        points -= val;
        if (points < 0)
            points = 0;
    }

    //Stars
    public int GetStars(){
        return stars;
    }
    public void AddStars(int val){
        stars += val;
    }
    public void SubtractStars(int val){
        stars -= val;
        if (stars < 0)
            stars = 0;
    }

    //Health
    public int GetHealth(){
        return health;
    }
    public void AddHealth(int val){
        health += val;
        if (health > 100){
            health = 100;
        }
    }
    public void SubtractHealth(int val){
        health -= val;
        if (health < 0){
            health = 0;
        }
    }

    //Mood 
    public int GetMood(){
        return mood;
    }
    public double GetWeightedMood(){
        return (0.3*mood + 0.35*hunger + 0.35*health);
    }
    public void AddMood(int val){
        mood += val;
        if (mood > 100){
            mood = 100;
        }
    }
    public void SubtractMood(int val){
        mood -= val;
        if (mood < 0){
            mood = 0;
        }
    }

    //Hunger
    public int GetHunger(){
        return hunger;
    }
    public void AddHunger(int val){
        hunger += val;
        if (hunger > 100){
            hunger = 100;
        }
    }
    public void SubtractHunger(int val){
        hunger -= val;
        if (hunger < 0){
            hunger = 0;
        }
    }

    // Use this for initialization
    // save and load data here
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }


}
