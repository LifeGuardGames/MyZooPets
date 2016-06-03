using UnityEngine;
using UnityEngine.UI;
using System;

public class BedroomInhalerUIManager : Singleton<BedroomInhalerUIManager> {
	public Animation inhalerAnimationController;
	public GameObject starParticle;
	public GameObject rechargeParticle;

	public GameObject canvas;
	public Text coolDownLabel;
	public Image coolDownSprite;

	private bool isCoolDownMode = false;
	private bool isInitialCalculatedOffsetCached = false;
	private TimeSpan initialCalculatedOffset;

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
		isCoolDownMode = true;
		inhalerAnimationController.Stop();
		starParticle.SetActive(false);
		rechargeParticle.SetActive(true);

		coolDownLabel.enabled = true;
		canvas.GetComponent<Animation>().Stop();
		canvas.transform.localScale = Vector3.one;
		
		isInitialCalculatedOffsetCached = false;	// Force recalculate the cache
	}

	/// <summary>
	/// Readies to use mode.
	/// </summary>
	private void ReadyToUseMode(){
		isCoolDownMode = false;
		inhalerAnimationController.Play("roomEntrance");
		starParticle.SetActive(true);
		rechargeParticle.SetActive(false);

		coolDownLabel.enabled = false;
		coolDownSprite.fillAmount = 1f;
		canvas.GetComponent<Animation>().Play();
	}

	private void OnNextPlayPeriod(object sender, EventArgs args){
		ReadyToUseMode();
	}

	void Update(){
		if(isCoolDownMode){
			// Update the cool down timer
			TimeSpan timeLeft = PlayPeriodLogic.Instance.CalculateTimeLeftTillNextPlayPeriod();
			coolDownLabel.text = StringUtils.FormatTimeLeft(timeLeft);

			DateTime initialSavedTime = PlayPeriodLogic.Instance.GetLastInhalerTime();
			TimeSpan timeOffset = LgDateTime.GetTimeNow() - initialSavedTime;

			// Calculate the denomicator once and cache it
			if(!isInitialCalculatedOffsetCached){
				initialCalculatedOffset = PlayPeriodLogic.Instance.NextPlayPeriod - initialSavedTime;
				isInitialCalculatedOffsetCached = true;
			}

			float completePercentage = (float)timeOffset.TotalMinutes / (float)initialCalculatedOffset.TotalMinutes;
			coolDownSprite.fillAmount = completePercentage;
		}
	}
}
