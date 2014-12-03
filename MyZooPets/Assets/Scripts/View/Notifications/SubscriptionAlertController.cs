﻿using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Alert controller.
/// This is for an alert popup that isnt an notification and doesnt lock anything
/// </summary>
public class SubscriptionAlertController : Singleton<SubscriptionAlertController>{

	public TweenToggle tweenToggle;
	public UILocalize labelLocalize;
	public GameObject spinner;
	public AnimationControl parentPortalButtonController;

	public string trialUpKey;
	public string membershipExpiredKey;
	public string connectInternetKey;

	private string auxStringKeyToShow;	// The next message to show after hide tween is done
	private bool isFirstTween = true;

	void Start(){
		if(tweenToggle.HideTarget == null){
			tweenToggle.HideTarget = gameObject;
		}
		if(string.IsNullOrEmpty(tweenToggle.HideFunctionName)){
			tweenToggle.HideFunctionName = "HideFinishedCallback";
		}
		spinner.gameObject.SetActive(false);

		//Do an instance check here so we can debug MenuScene without MembershipCheck Instance
		if(MembershipCheck.Instance){
			CheckMembershipError();
		}
	}

	/// <summary>
	/// Checks Membership Check Errors. Show the appropriate error message
	/// </summary>
	public bool CheckMembershipError(){
		bool retVal = false;

		switch(MembershipCheck.Instance.MembershipCheckError){
		case MembershipCheck.Errors.OverConnectionErrorLimit:
			Debug.Log("connectionerror");
			retVal = true;
			ShowConnectToInternetAlert();
			break;
		case MembershipCheck.Errors.TrialExpired:
			retVal = true;
			ShowTrialExpiredAlert();
			break;
		case MembershipCheck.Errors.MembershipExpired:
			retVal = true;
			ShowMembershipExpiredAlert();
			break;
		default:
			retVal = false;
			HideAll();
			break;
		}
		
		MembershipCheck.Instance.MembershipCheckError = MembershipCheck.Errors.None;
		return retVal;
	}

	public void ShowTrialExpiredAlert(){
		StopSpinner();
		ShowLabelKey(trialUpKey);
		parentPortalButtonController.Play("scalePulseHeartbeat");
	}

	public void ShowMembershipExpiredAlert(){
		StopSpinner();
		ShowLabelKey(membershipExpiredKey);
		parentPortalButtonController.Play("scalePulseHeartbeat");
	}

	public void ShowConnectToInternetAlert(){
		StopSpinner();
		ShowLabelKey(connectInternetKey);
	}

	public void ShowSpinner(){
		Hide();
		spinner.gameObject.SetActive(true);
		parentPortalButtonController.StopAndResetFrame("zero");
	}

	public void HideAll(){
		parentPortalButtonController.StopAndResetFrame("zero");
		StopSpinner();
		Hide();
	}

	private void StopSpinner(){
		spinner.gameObject.SetActive(false);
	}

	// Regular instance of hide when the parent portal is opened
	private void Hide(){
		tweenToggle.Hide();
	}

	private void ShowLabelKey(string newKey){
		auxStringKeyToShow = newKey;
		LocalizeKeyWithAux();
		tweenToggle.Show();
	}

	private void LocalizeKeyWithAux(){
		labelLocalize.key = auxStringKeyToShow;
		labelLocalize.Localize();
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Alert")){
//			ShowSpinner();
//			Invoke("ShowTrialUpAlert", 0.1f);
//		}
//		if(GUI.Button(new Rect(200, 100, 100, 100), "Expired")){
//			ShowSpinner();
//			Invoke("ShowMembershipExpiredAlert", 0.2f);
//		}
//		if(GUI.Button(new Rect(300, 100, 100, 100), "Internet")){
//			ShowSpinner();
//			Invoke("ShowConnectToInternetAlert", 0.05f);
//		}
//	}
}