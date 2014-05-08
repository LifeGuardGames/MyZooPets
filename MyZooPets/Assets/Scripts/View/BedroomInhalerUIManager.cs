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
//		coolDownLabel = progressBar3D.transform.Find("Label").GetComponent<UILabel>();
//		coolDownSlider = progressBar3D.transform.Find("Progress Bar").GetComponent<UISlider>();

		if(PlayPeriodLogic.Instance.CanUseRealInhaler()){
			spinningAnimation.Play();
			starParticle.SetActive(true);
			rechargeParticle.SetActive(false);
		}
		else{
			spinningAnimation.Stop();
			starParticle.SetActive(false);
			rechargeParticle.SetActive(true);
		}

		PlayPeriodLogic.OnUpdateTimeLeftTillNextPlayPeriod += OnUpdateTimeLeft;
		PlayPeriodLogic.OnNextPlayPeriod += OnNextPlayPeriod;
	}

	void OnDestroy(){
		PlayPeriodLogic.OnUpdateTimeLeftTillNextPlayPeriod -= OnUpdateTimeLeft;
		PlayPeriodLogic.OnNextPlayPeriod -= OnNextPlayPeriod;
	}

	private void OnNextPlayPeriod(object sender, EventArgs args){

	}
	
	private void OnUpdateTimeLeft(object sender, PlayPeriodEventArgs args){
		TimeSpan timeLeft = args.TimeLeft;
		string strTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
		
		// set the label
//		string strLabel = Localization.Localize("WELLAPAD_NO_MISSIONS_2");
		coolDownLabel.text = strTime;

		float completePercentage = (PlayPeriodLogic.PLAYPERIOD_LENGTH - timeLeft.Hours) / PlayPeriodLogic.PLAYPERIOD_LENGTH;
		coolDownSlider.sliderValue = completePercentage;
	}
}
