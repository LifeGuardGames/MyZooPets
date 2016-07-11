using UnityEngine;
using System;
using System.Collections;

public class OldRunnerTutorial : MinigameTutorial{
	public static string TUT_KEY = "RUNNER_TUT";

	//---------------------------------------------------
	// SetMaxSteps()
	//---------------------------------------------------
	protected override void SetMaxSteps(){
		maxSteps = 3;
	}
    
	//---------------------------------------------------
	// SetKey()
	//---------------------------------------------------
	protected override void SetKey(){
		tutorialKey = TUT_KEY;
	}
    
	//---------------------------------------------------
	// _End()
	//---------------------------------------------------
	protected override void _End(bool isFinished){
		//tutorial aborted. need to do some clean up
		if(!isFinished){
			PlayerController.OnJump -= TutorialJump;
			PlayerController.OnDrop -= TutorialDrop;
			RemovePopup();
		}
		RunnerGameManager.Instance.NewGame();
	}
    
	//---------------------------------------------------
	// ProcessStep()
	//---------------------------------------------------
	protected override void ProcessStep(int nStep){
		switch(nStep){
		case 0:
			GameObject goResource1 = Resources.Load("RunnerTutorialPanelTop") as GameObject;
			goPopup = GameObjectUtils.AddChild(GameObject.Find("Anchor-Center"), goResource1);
			PlayerController.OnJump += TutorialJump;
			break;
		case 1:
			GameObject goResource2 = Resources.Load("RunnerTutorialPanelBottom") as GameObject;
			goPopup = GameObjectUtils.AddChild(GameObject.Find("Anchor-Center"), goResource2);
			PlayerController.OnDrop += TutorialDrop;
			break;
		case 2:
			TutorialPopup.Callback button1Fuction = delegate(){
				Advance();
			};

			Hashtable option = new Hashtable();
			option.Add(TutorialPopupFields.Button1Callback, button1Fuction);
			string strResourceKey = Tutorial.POPUP_LONG_WITH_BUTTON;      
			ShowPopup(strResourceKey, POS_TOP, option);
			break;
		default:
			Debug.Log("Runner tutorial has an unhandled step: " + nStep);
			break;      
		}
	}

	private void TutorialJump(object sender, EventArgs args){
		Debug.Log("Called");
		PlayerController.OnJump -= TutorialJump;
		RemovePopup();
		RunnerGameManager.Instance.StartCoroutine(WaitBeforeAdvance());
	}

	private void TutorialDrop(object sender, EventArgs args){
		PlayerController.OnDrop -= TutorialDrop;
		RemovePopup();
		RunnerGameManager.Instance.StartCoroutine(WaitBeforeAdvance());
	}

	private IEnumerator WaitBeforeAdvance(){
		yield return new WaitForSeconds(2);
		Advance();
	}
}
