using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// PetLevelMutableData
// Mutable data.
//---------------------------------------------------
public class PetLevelMutableData{
    public Level CurrentLevel {get; set;} //pets current level 

    //================Initialization============
    public PetLevelMutableData(){
        Init();
    }

    private void Init(){
//        CurrentLevel = Level.Level1;
         CurrentLevel = Level.Level2;
    } 
}
