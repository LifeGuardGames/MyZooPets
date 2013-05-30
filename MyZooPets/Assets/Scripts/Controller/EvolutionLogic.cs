using UnityEngine;
using System.Collections;
using System;

//Decides and calculates when evolution should happen
//Evolution meter = 0.5*mood + 0.5*health
public class EvolutionLogic : MonoBehaviour {
    private DataManager dataManager; //get persistent data
    private DateTime lastUpdatedTime;
    private TimeSpan durationCum;
    private double lastEvoVal;
    private double evoAverageCum;

	// Use this for initialization
	void Start () {
	   dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
       lastUpdatedTime = dataManager.lastUpdatedTime;
       durationCum = dataManager.durationCum;
       lastEvoVal = dataManager.lastEvoVal;
       evoAverageCum = dataManager.evoAverageCum;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void UpdateEvoAverage(){

    }
}
