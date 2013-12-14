using UnityEngine;
using System;
using System.Collections;

public class PetLevelMutableData{
    public Level CurrentLevel {get; set;} //pets current level 

    //================Initialization============
    public PetLevelMutableData(){}

    public void Init(){
        CurrentLevel = Level.Level0;
    } 
}
