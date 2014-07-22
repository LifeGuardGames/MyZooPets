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
	public UISlider coolDownSlider;

	private GameObject fireOrbObject;

	public GameObject FireOrbReference{
		get{
			return fireOrbObject;
		}
	}

	// Start the correct animations based on its state
	void Start(){
		PlayPeriodLogic.OnUpdateTimeLeftTillNextPlayPeriod += OnUpdateTimeLeft;
		PlayPeriodLogic.OnNextPlayPeriod += OnNextPlayPeriod;

		if(PlayPeriodLogic.Instance.CanUseEverydayInhaler()){
			ReadyToUseMode();
			DataManager.Instance.GameData.Inhaler.HasReceivedFireOrb = false;
		}
		else{
			CoolDownMode();

			if(!TutorialManager.Instance.IsTutorialActive())
				CheckToDropFireOrb();
		}
	}

	void OnDestroy(){
		PlayPeriodLogic.OnUpdateTimeLeftTillNextPlayPeriod -= OnUpdateTimeLeft;
		PlayPeriodLogic.OnNextPlayPeriod -= OnNextPlayPeriod;
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(0, 0, 100, 100), "start")){
//			CheckToDropFireOrb();
//		}
//	}

	/// <summary>
	/// Checks to drop fire orb.
	/// If user hasn't received fire orb after using inhaler, a fire orb will be 
	/// dropped from the inhaler
	/// </summary>
	public void CheckToDropFireOrb(){
		bool hasReceivedFireOrb = DataManager.Instance.GameData.Inhaler.HasReceivedFireOrb;
		if(!hasReceivedFireOrb){
			//spawn fire orb
			GameObject fireOrbPrefab = Resources.Load("DroppedItemFireOrb") as GameObject;
			fireOrbObject = NGUITools.AddChild(fireOrbParent, fireOrbPrefab);

			fireOrbObject.layer = LayerMask.NameToLayer("Default");
			fireOrbObject.transform.parent = fireOrbParent.transform;
			DroppedObjectItem droppedObjectItem = fireOrbObject.GetComponent<DroppedObjectItem>();

			Item fireOrbData = DataLoaderItems.GetItem("Usable1");

			droppedObjectItem.Init(fireOrbData);
			droppedObjectItem.ChangeAutoCollectTime(15f);

			fireOrbObject.SetActive(false);

			//Activate animation here
			Invoke("SpawnFireOrb", 1.5f);

			DataManager.Instance.GameData.Inhaler.HasReceivedFireOrb = true;
		}
	}

	private void SpawnFireOrb(){
		inhalerAnimationController.Play("SpawnFireOrb");
		fireOrbObject.SetActive(true);
	}

	/// <summary>
	/// Cools down mode.
	/// </summary>
	private void CoolDownMode(){
		inhalerAnimationController.Stop();
		starParticle.SetActive(false);
		rechargeParticle.SetActive(true);
		progressBar3D.SetActive(true);
	}

	/// <summary>
	/// Readies to use mode.
	/// </summary>
	private void ReadyToUseMode(){
		inhalerAnimationController.Play("roomEntrance");
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
