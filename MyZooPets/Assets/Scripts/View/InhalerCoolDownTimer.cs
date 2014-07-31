using UnityEngine;
using System;
using System.Collections;

public class InhalerCoolDownTimer : MonoBehaviour {
	public UILabel coolDownLabel;

	void Start(){
		PlayPeriodLogic.OnUpdateTimeLeftTillNextPlayPeriod += OnUpdateTimeLeft;
	}
	
	void OnDestroy(){
		PlayPeriodLogic.OnUpdateTimeLeftTillNextPlayPeriod -= OnUpdateTimeLeft;
	}

	private void OnUpdateTimeLeft(object sender, PlayPeriodEventArgs args){
		TimeSpan timeLeft = args.TimeLeft;
		string displayTime = "";
		
		if(timeLeft.Hours > 0) 
			displayTime = string.Format("{0}[f4a460]h[-] {1}[f4a460]m[-] {2}[f4a460]s[-]", 
			                            timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
		else if(timeLeft.Minutes > 0)
			displayTime = string.Format("{0}[f4a460]m[-] {1}[f4a460]s[-]", timeLeft.Minutes, timeLeft.Seconds);
		else
			displayTime = string.Format("{0}[f4a460]s[-]", timeLeft.Seconds);
		
		
		// set the label
		coolDownLabel.text = displayTime;
	}
}
