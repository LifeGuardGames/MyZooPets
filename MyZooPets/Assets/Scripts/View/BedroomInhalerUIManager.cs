using UnityEngine;
using System;
using System.Collections;

public class BedroomInhalerUIManager : MonoBehaviour {

	public Animation spinningAnimation;
	public GameObject starParticle;
	public GameObject rechargeParticle;

	public GameObject progressBar3D;
	public UILabel coolDownLabel;
	public UISlider coolDownSlider;

	// Start the correct animations based on its state
	void Start(){

		if(PlayPeriodLogic.Instance.CanUseRealInhaler()){
			ReadyToUseMode();
		}
		else{
			CoolDownMode();
		}

		PlayPeriodLogic.OnUpdateTimeLeftTillNextPlayPeriod += OnUpdateTimeLeft;
		PlayPeriodLogic.OnNextPlayPeriod += OnNextPlayPeriod;
	}

	void OnDestroy(){
		PlayPeriodLogic.OnUpdateTimeLeftTillNextPlayPeriod -= OnUpdateTimeLeft;
		PlayPeriodLogic.OnNextPlayPeriod -= OnNextPlayPeriod;
	}

	/// <summary>
	/// Cools down mode.
	/// </summary>
	private void CoolDownMode(){
		spinningAnimation.Stop();
		starParticle.SetActive(false);
		rechargeParticle.SetActive(true);
		progressBar3D.SetActive(true);
	}

	/// <summary>
	/// Readies to use mode.
	/// </summary>
	private void ReadyToUseMode(){
		spinningAnimation.Play();
		starParticle.SetActive(true);
		rechargeParticle.SetActive(false);
		progressBar3D.SetActive(false);
	}

	private void OnNextPlayPeriod(object sender, EventArgs args){
		ReadyToUseMode();
	}

	/// <summary>
	/// Raises the update time left event. Keep updating the cool down timer
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnUpdateTimeLeft(object sender, PlayPeriodEventArgs args){
		TimeSpan timeLeft = args.TimeLeft;
		string displayTime = "";

		if(timeLeft.Hours > 0)
			displayTime = string.Format("{0}[FFFF33]h[-] {1}[FFFF33]m[-] {2}[FFFF33]s[-]", 
			                            timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
		else if(timeLeft.Minutes > 0)
			displayTime = string.Format("{0}[FFFF33]m[-] {1}[FFFF33]s[-]", timeLeft.Minutes, timeLeft.Seconds);
		else
			displayTime = string.Format("{0}[FFFF33]s[-]", timeLeft.Seconds);

		
		// set the label
		coolDownLabel.text = displayTime;

		TimeSpan totalRemainTime = PlayPeriodLogic.Instance.TotalTimeRemain;
		float completePercentage = ((float)totalRemainTime.TotalMinutes - (float)timeLeft.TotalMinutes) / (float)totalRemainTime.TotalMinutes;
		coolDownSlider.sliderValue = completePercentage;
	}
	
}
