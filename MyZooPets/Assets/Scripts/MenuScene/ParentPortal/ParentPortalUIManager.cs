using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Parse;

public class ParentPortalUIManager : SingletonUI<ParentPortalUIManager> {

	public UILabel playerName;
	public UILabel code;

	public TweenToggle backgroundTween;

	public TweenToggle mathTween;
	public UILabel questionLabel;
	public UILabel answerLabel;
	private int answerToMath;
	private int sequenceCount = 1;
	private int answerSoFar;

	public TweenToggle parentPortalTween;

	void Awake(){
		eModeType = UIModeTypes.ParentPortal;
	}

	void Start(){
		// TODO JASON POPULATE HERE
//		playerName.text = ;
//		code.text = ;
	}

	void OnDestroy(){
	}


	protected override void _OpenUI(){
		OpenBackground();
		OpenMathQuestion();

		SelectionUIManager.Instance.gameObject.SetActive(false);
	}

	protected override void _CloseUI(){
		CloseBackground();
		CloseMathQuestion();
		CloseParentPortal();

		SelectionUIManager.Instance.gameObject.SetActive(true);
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
			Debug.Log("Success!");
			parentPortalTween.Show();
			mathTween.Hide();
		}
		else{
			// Fail, start over
			Debug.Log("You're stupid!");
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

	void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 100, 100), "Open")){
			OpenUI();
		}
		if(GUI.Button(new Rect(200, 100, 100, 100), "Close")){
			CloseUI();
		}
	}
}
