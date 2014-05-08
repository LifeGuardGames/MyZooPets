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
		string strTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
		
		// set the label
		coolDownLabel.text = strTime;

		float completePercentage = (PlayPeriodLogic.PLAYPERIOD_LENGTH - timeLeft.Hours) / PlayPeriodLogic.PLAYPERIOD_LENGTH;
		coolDownSlider.sliderValue = completePercentage;
	}
}
