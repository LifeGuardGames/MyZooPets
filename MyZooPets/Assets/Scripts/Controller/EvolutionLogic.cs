using UnityEngine;
using System.Collections;
using System;

//Calculates evolution meter every 30 secs
//Decides when pet hits evolution stage
public class EvolutionLogic : MonoBehaviour {
	private float timer = 0;
	private float timeInterval = 10f;
    private int level1EvolutionPoints = 50000;
    private int level2EvolutionPoints = 150000;

    //#region API
    public bool CanEvolve(){
        // awaiting first evolution and has enough points
        if (DataManager.evoStage == EvoStage.Stage0 && DataManager.Points >= level1EvolutionPoints){
            return true;
        }
        // awaiting second evolution and has enough points
        else if (DataManager.evoStage == EvoStage.Stage1 && DataManager.Points >= level2EvolutionPoints){
            return true;
        }
        return false;
    }

    // Should only be called if CanEvolve() returns true.
    public void Evolve(){

        //TO DO: Check points and decide how the pet should evolve according to evoAverageCum

        if (DataManager.evoStage == EvoStage.Stage0){ // awaiting first evolution
            if (DataManager.evoAverageCum >= 50){
                // good care
            }
            else { // < 50
                // bad care
            }
            DataManager.evoStage = EvoStage.Stage1;
        }
        else if (DataManager.evoStage == EvoStage.Stage1){ // awaiting second evolution
            if (DataManager.evoAverageCum <= 30){
                // bad care
            }
            else if (DataManager.evoAverageCum <= 70){
                // OK care
            }
            else { // > 70
                // good care
            }
            DataManager.evoStage = EvoStage.Stage2;
        }
    }

    //#endregion

    public void Init () {
        timer = timeInterval;

    }

	// Update is called once per frame
	void Update () {
        if(!LoadDataLogic.IsDataLoaded) return;

		timer -= Time.deltaTime;
		if (timer <= 0){
			timer = timeInterval;
			UpdateEvoAverage();

		}
	}

    //calculate evolution meter
	private void UpdateEvoAverage(){
		int cumDurationSecs = (int)DataManager.durationCum.TotalSeconds;

		DateTime now = DateTime.Now;
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

    //get the weighted evolution meter
	private static double getEvoMeter(){
		return 0.5 * DataManager.Mood + 0.5 * DataManager.Health;
	}
}
