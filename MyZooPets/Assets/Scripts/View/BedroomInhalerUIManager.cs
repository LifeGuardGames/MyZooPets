﻿using UnityEngine;
using System;
using System.Collections;

public class BedroomInhalerUIManager : Singleton<BedroomInhalerUIManager> {

	public Animation spinningAnimation;
	public GameObject starParticle;
	public GameObject rechargeParticle;

	public GameObject progressBar3D;
	public UILabel coolDownLabel;
	public UISlider coolDownSlider;

	// Start the correct animations based on its state
	void Start(){
		PlayPeriodLogic.OnUpdateTimeLeftTillNextPlayPeriod += OnUpdateTimeLeft;
		PlayPeriodLogic.OnNextPlayPeriod += OnNextPlayPeriod;

		if(PlayPeriodLogic.Instance.CanUseRealInhaler()){
			ReadyToUseMode();
			DataManager.Instance.GameData.Inhaler.HasReceivedFireOrb = false;
		}
		else{
			CoolDownMode();
			CheckToDropFireOrb();
		}
	}

	void OnDestroy(){
		PlayPeriodLogic.OnUpdateTimeLeftTillNextPlayPeriod -= OnUpdateTimeLeft;
		PlayPeriodLogic.OnNextPlayPeriod -= OnNextPlayPeriod;
	}

	public void CheckToDropFireOrb(){
		bool hasReceivedFireOrb = DataManager.Instance.GameData.Inhaler.HasReceivedFireOrb;
		if(!hasReceivedFireOrb){
			//Activate animation here

			DataManager.Instance.GameData.Inhaler.HasReceivedFireOrb = true;
		}
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
			displayTime = string.Format("{0}H {1}M {2}S", 
			                            timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
		else if(timeLeft.Minutes > 0)
			displayTime = string.Format("{0}M {1}S", timeLeft.Minutes, timeLeft.Seconds);
		else
			displayTime = string.Format("{0}S", timeLeft.Seconds);

		
		// set the label
		coolDownLabel.text = displayTime;

		TimeSpan totalRemainTime = PlayPeriodLogic.Instance.TotalTimeRemain;
		float completePercentage = ((float)totalRemainTime.TotalMinutes - (float)timeLeft.TotalMinutes) / (float)totalRemainTime.TotalMinutes;
		coolDownSlider.sliderValue = completePercentage;
	}
	
}
