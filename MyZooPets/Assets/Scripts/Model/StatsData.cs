using UnityEngine;
using System;
using System.Collections;

[DoNotSerializePublic]
public class StatsData{
    //stats
    [SerializeThis]
    private int points; //evolution points
    [SerializeThis]
    private int stars; //currency of the game
    [SerializeThis]
    private int health; //pet's health
    [SerializeThis]
    private int mood; //pet's mood (weighted or unweighted)

    //=====================Getters & Setters=============
    public int Points{
        get{return points;}
    }
    public int Stars{
        get{return stars;}
    }
    public int Health{
        get{return health;}
    }
    public int Mood{
        get{return mood;}
    }
    public bool UseDummyData{get; set;} //initialize with test data

    //=======================Initialization==================
    public StatsData(){}

    //Populate with dummy data
    public void Init(){
        health = 80;
        mood = 80;
        points = 250;
        stars = 100;
    }

    //==============StatsModifiers================
    //Points
    public void AddPoints(int val){
        points += val;
    }
    public void SubtractPoints(int val){
        points -= val;
        if (points < 0)
            points = 0;
    }
    public void ResetPoints(){
        points = 0;
    }

    //Stars
    public void AddStars(int val){
        stars += val;
    }
    public void SubtractStars(int val){
        stars -= val;
        if (stars < 0)
            stars = 0;
    }

    //Health
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
    public double GetWeightedMood(){
        return (0.5*mood + 0.5*health);
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
}
