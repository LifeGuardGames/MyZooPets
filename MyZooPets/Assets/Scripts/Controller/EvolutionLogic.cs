using UnityEngine;
using System.Collections;
using System;

//Decides and calculates when evolution should happen
//Evolution meter = 0.5*mood + 0.5*health
public class EvolutionLogic : MonoBehaviour {
	private float timer = 0;
	private float timeInterval = 30f;

	// Use this for initialization
	void Start () {

		if (DataManager.FirstTime){
			DataManager.lastUpdatedTime = DateTime.UtcNow;
			DataManager.durationCum = new TimeSpan(0);

			double evoVal = getEvoVal();
			DataManager.lastEvoVal = evoVal;
			DataManager.evoAverageCum = evoVal;

			DataManager.FirstTime = false;

			timer = timeInterval;
		}
	}

	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0){
			timer = timeInterval;
			UpdateEvoAverage();
		}
	}

	void UpdateEvoAverage(){
		int cumDurationSecs = (int)DataManager.durationCum.TotalSeconds;
		DateTime now = DateTime.UtcNow;
		TimeSpan tempd = now.Subtract(DataManager.lastUpdatedTime);
		int timeElapsedInSecs = (int)tempd.TotalSeconds;

		double evoVal = getEvoVal();
		double evoAverageNow = (evoVal + DataManager.lastEvoVal) / 2;

		// calculate the average evolution value, over the period of gameplay starting from hatching the pet up to now
		DataManager.evoAverageCum = (DataManager.evoAverageCum * cumDurationSecs + evoAverageNow * timeElapsedInSecs) / (cumDurationSecs + timeElapsedInSecs);
		DataManager.lastUpdatedTime = now;
		DataManager.durationCum += tempd;
		DataManager.lastEvoVal = evoVal;
	}

	double getEvoVal(){
		return 0.5 * DataManager.Mood + 0.5 * DataManager.Health;
	}
}
