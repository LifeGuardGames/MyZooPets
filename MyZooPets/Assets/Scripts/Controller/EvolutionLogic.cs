using UnityEngine;
using System.Collections;
using System;

//Calculates evolution meter every 30 secs
//Decides when pet hits evolution stage
public class EvolutionLogic : MonoBehaviour {
	private float timer = 0;
	private float timeInterval = 30f;
    private int level1EvolutionPoints = 50000;
    private int level2EvolutionPoints = 150000;

	// Use this for initialization
	void Start () {
        timer = timeInterval;

	}

	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0){
			timer = timeInterval;
			UpdateEvoAverage();
		}
	}

    //calculate evolution meter
	private void UpdateEvoAverage(){
		int cumDurationSecs = (int)DataManager.durationCum.TotalSeconds;
		DateTime now = DateTime.UtcNow;
		TimeSpan tempd = now.Subtract(DataManager.lastUpdatedTime);
		int timeElapsedInSecs = (int)tempd.TotalSeconds;

		double evoMeter = getEvoMeter();
		double evoAverageNow = (evoMeter + DataManager.lastEvoMeter) / 2;

		//calculate the average evolution value, over the period of gameplay starting 
        //from hatching the pet up to now
		DataManager.evoAverageCum = (DataManager.evoAverageCum * cumDurationSecs + 
            evoAverageNow * timeElapsedInSecs) / (cumDurationSecs + timeElapsedInSecs);
		DataManager.lastUpdatedTime = now;
		DataManager.durationCum += tempd;
		DataManager.lastEvoMeter = evoMeter;
	}

    //TO DO: Check points and decide how the pet should evolve according to evoAverageCum
    private void CheckForEvolution(){
        if(DataManager.Points >= level1EvolutionPoints){

        }
    }

    //get the weighted evolution meter
	private double getEvoMeter(){
		return 0.5 * DataManager.Mood + 0.5 * DataManager.Health;
	}
}
