using UnityEngine;
using System;
using System.Collections;

public class BedroomInhalerUIManager : Singleton<BedroomInhalerUIManager> {
	public Animation inhalerAnimationController;
	public GameObject fireOrbParent;
	public GameObject starParticle;
	public GameObject rechargeParticle;

	public GameObject progressBar3D;
	public UILabel coolDownLabel;
	public UISprite coolDownSprite;

	// Start the correct animations based on its state
	void Start(){
		PlayPeriodLogic.OnNextPlayPeriod += OnNextPlayPeriod;

		if(PlayPeriodLogic.Instance.CanUseEverydayInhaler()){
			ReadyToUseMode();
		}
		else{
			CoolDownMode();
		}
	}

	void OnDestroy(){
		PlayPeriodLogic.OnNextPlayPeriod -= OnNextPlayPeriod;
	}

	/// <summary>
	/// Cools down mode.
	/// </summary>
	private void CoolDownMode(){
		inhalerAnimationController.Stop();
		starParticle.SetActive(false);
		rechargeParticle.SetActive(true);

		coolDownLabel.enabled = true;
		progressBar3D.animation.Stop();
		progressBar3D.transform.localScale = Vector3.one;
	}

	/// <summary>
	/// Readies to use mode.
	/// </summary>
	private void ReadyToUseMode(){
		inhalerAnimationController.Play("roomEntrance");
		starParticle.SetActive(true);
		rechargeParticle.SetActive(false);

		coolDownLabel.enabled = false;
		coolDownSprite.fillAmount = 1f;
		progressBar3D.animation.Play();
	}

	private void OnNextPlayPeriod(object sender, EventArgs args){
		ReadyToUseMode();
	}

	void Update(){
		// Update the cool down timer
		TimeSpan timeLeft = PlayPeriodLogic.Instance.CalculateTimeLeftTillNextPlayPeriod();
		string displayTime = "";
		
		if(timeLeft.Hours > 0){
			displayTime = string.Format("{0}[FFFF33]h[-] {1}[FFFF33]m[-] {2}[FFFF33]s[-]", 
			                            timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
		}
		else if(timeLeft.Minutes > 0){
			displayTime = string.Format("{0}[FFFF33]m[-] {1}[FFFF33]s[-]", timeLeft.Minutes, timeLeft.Seconds);
		}
		else{
			displayTime = string.Format("{0}[FFFF33]s[-]", timeLeft.Seconds);
		}
		
		// set the label
		coolDownLabel.text = displayTime;
		
		Debug.LogWarning("DANGING LOGIC HERE");
//		float completePercentage = ((float)totalRemainTime.TotalMinutes - (float)timeLeft.TotalMinutes) / (float)totalRemainTime.TotalMinutes;
//		coolDownSprite.fillAmount = completePercentage;
	}

	//	void OnGUI(){
	//		if(GUI.Button(new Rect(0, 0, 100, 100), "start")){
	//			CheckToDropFireOrb();
	//		}
	//	}
}
