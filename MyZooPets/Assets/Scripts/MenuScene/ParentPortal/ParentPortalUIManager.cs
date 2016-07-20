/*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Parse;

public class ParentPortalUIManager : SingletonUI<ParentPortalUIManager> {
	public UILabel code;

	public TweenToggle backgroundTween;

	public TweenToggle mathTween;
	public UILabel questionLabel;
	public UILabel answerLabel;
	private int answerToMath;
	private int sequenceCount = 1;
	private int answerSoFar;

	public UILocalize parentPortalText;
	public ParticleSystemController leafParticle;
	public TweenToggle parentPortalTween;

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.ParentPortal;
	}

	protected override void OnDestroy(){
		base.OnDestroy();
		ParentPortalManager.OnDataRefreshed -= DataRefreshedHandler;
	}

	protected override void Start(){
		ParentPortalManager.OnDataRefreshed += DataRefreshedHandler;

		ParentPortalTextCheck();
	}

	/// <summary>
	/// Check the trial status and membership status and display the appropriate text
	/// </summary>
	public void ParentPortalTextCheck(){
		if(!Constants.GetConstant<bool>("IsMenusceneConnectionOn")){
			Debug.LogWarning("Connection debug turned off");
			return;
		}

		Debug.Log("Parent portal check");
		MembershipCheck.Status membershipStatus = MembershipCheck.Instance.MembershipStatus;
		MembershipCheck.Status trialStatus = MembershipCheck.Instance.TrialStatus;
		
		//Display trial message in parent portal
		if(membershipStatus == MembershipCheck.Status.None){
			if(trialStatus == MembershipCheck.Status.Active){
				SetParentPortalText("PARENT_PORTAL_TEXT_TRIAL");
			}
			else{
				SetParentPortalText("PARENT_PORTAL_TEXT_TRIAL_EXPIRED");
			}
		}
		//Display membership message in parent portal
		else{
			if(membershipStatus == MembershipCheck.Status.Active){
				SetParentPortalText("PARENT_PORTAL_TEXT_MEMBERSHIP_ACTIVE");
			}
			else{
				SetParentPortalText("PARENT_PORTAL_TEXT_MEMBERSHIP_EXPIRED");
			}
		}
	}

	private void DataRefreshedHandler(object sender, ServerEventArgs args){
		ParseObjectKidAccount account = ParentPortalManager.Instance.KidAccount;
		code.text = account.AccountCode;
	}

	protected override void _OpenUI(){
		leafParticle.Stop();
		SubscriptionAlertController.Instance.HideAll();

		OpenBackground();
		OpenMathQuestion();

		ParentPortalManager.Instance.RefreshData();
		MenuSceneManager.Instance.gameObject.SetActive(false);
	}

	protected override void _CloseUI(){
		leafParticle.Play();

		CloseBackground();
		CloseMathQuestion();
		CloseParentPortal();

		MenuSceneManager.Instance.gameObject.SetActive(true);
	}

	private void OpenBackground(){
		backgroundTween.Show();
	}

	private void CloseBackground(){
		backgroundTween.Hide();
	}

	private void OpenMathQuestion(){
		GenerateMathQuestion();
		mathTween.Show();
	}

	private void CloseMathQuestion(){
		mathTween.Hide();
	}

	private void OpenParentPortal(){
		parentPortalTween.Show();
	}

	private void CloseParentPortal(){
		parentPortalTween.Hide();
	}


	private void SetParentPortalText(string key){
		parentPortalText.key = key;
		parentPortalText.Localize();
	}

	/// <summary>
	/// Generates the math question.
	/// NOTE: Answer will always be a 3 digit number
	/// </summary>
	private void GenerateMathQuestion(){
		// Reset values
		answerSoFar = 0;
		sequenceCount = 1;
		answerLabel.text = "___";

		int firstNumber = UnityEngine.Random.Range(501, 999);
		int secondNumber = UnityEngine.Random.Range(101, 499);
		questionLabel.text = firstNumber.ToString() + " - " + secondNumber.ToString() + " = ";
		answerToMath = firstNumber - secondNumber;
	}

	private void EnteredSequence(int digit){
		switch(sequenceCount){
		case 1:
			answerSoFar = digit * 100;
			answerLabel.text = digit.ToString() + "__";
			break;
		case 2:
			answerSoFar += (digit * 10);
			answerLabel.text = (answerSoFar / 10).ToString() + "_";
			break;
		case 3:
			answerSoFar += digit;
			answerLabel.text = answerSoFar.ToString();
			break;
		}
		sequenceCount++;
		if(sequenceCount >=4){
			CheckAnswer();
		}
	}

	private void CheckAnswer(){
		if(answerSoFar == answerToMath){
			// Success, go into parent portal
			parentPortalTween.Show();
			mathTween.Hide();
		}
		else{
			// Fail, start over
			GenerateMathQuestion();
		}
	}

	public void Button1(){ EnteredSequence(1); }
	public void Button2(){ EnteredSequence(2); }
	public void Button3(){ EnteredSequence(3); }
	public void Button4(){ EnteredSequence(4); }
	public void Button5(){ EnteredSequence(5); }
	public void Button6(){ EnteredSequence(6); }
	public void Button7(){ EnteredSequence(7); }
	public void Button8(){ EnteredSequence(8); }
	public void Button9(){ EnteredSequence(9); }
	public void Button0(){ EnteredSequence(0); }

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Open")){
//			OpenUI();
//		}
//		if(GUI.Button(new Rect(200, 100, 100, 100), "Close")){
//			CloseUI();
//		}
//	}
}
*/
