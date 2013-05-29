using UnityEngine;
using System.Collections;
using System;

//This class handles all game data. No game logic
//Saves and loads data into player preference
public class DataManager : MonoBehaviour {

    //points that are used for activating evolution
    private int points;
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

    //currency in the game
    private int stars;
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

    //health of the pet
    private int health;
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

    //mood of the pet. weighted or unweighted
    private int mood;
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

    //hungriness of the pet
    private int hunger;
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

    //information that need to be persistent
    private float evolutionAverageCum;
    private float lastEvolutionAverage;
    private float duration; 
    private DateTime lastUpdated; 

    // Use this for initialization
    // save and load data here
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }


}
