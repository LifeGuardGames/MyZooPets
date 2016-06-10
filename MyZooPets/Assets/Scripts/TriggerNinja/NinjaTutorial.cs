using UnityEngine;
using System;
using System.Collections;

public class NinjaTutorial : MinigameTutorial {
    public static string TUT_KEY = "NINJA_TUT";

	private Animation swipeTutAnimation;
	private GameObject swipeTutObject;
	// one game object for each type of trigger
	private GameObject trigger1Object;
	private GameObject trigger2Object;
	private GameObject trigger3Object;
	private GameObject trigger4Object;
	private GameObject trigger5Object;
	private GameObject trigger6Object;
	// number of triggers cut, this is a subsitute from having more steps
	private int numOfTriggersCut;
	  
    protected override void SetMaxSteps(){
        maxSteps = 1;
    }
	     
    protected override void SetKey(){
        tutorialKey = TUT_KEY;
    }

	protected override void _End(bool isFinished){
		if(!isFinished){
			if(trigger1Object){
				trigger1Object.GetComponent<NinjaTrigger>().NinjaTriggerCut -= NinjaTriggerFirstCutEventHandler;
				GameObject.Destroy(trigger1Object);
			}
		}
		NinjaManager.Instance.isTutorialRunning = false;

		if(swipeTutObject != null)
			GameObject.Destroy(swipeTutObject);
	}

    protected override void ProcessStep(int step){
        switch(step){
            case 0:
				// spawn each trigger
				trigger1Object = NinjaManager.Instance.SpawnTriggersTutorial(1);
				trigger2Object = NinjaManager.Instance.SpawnTriggersTutorial(2);
				trigger3Object = NinjaManager.Instance.SpawnTriggersTutorial(3);
				trigger4Object = NinjaManager.Instance.SpawnTriggersTutorial(4);
				trigger5Object = NinjaManager.Instance.SpawnTriggersTutorial(5);
				trigger6Object = NinjaManager.Instance.SpawnTriggersTutorial(6);
				GameObject swipeTut = (GameObject) Resources.Load("NinjaSwipeTut");
				swipeTutObject = GameObjectUtils.AddChildWithPositionAndScale(GameObject.Find("Anchor-Center"), swipeTut);

				//listen to when trigger gets cut
				trigger1Object.GetComponent<NinjaTrigger>().NinjaTriggerCut += NinjaTriggerFirstCutEventHandler;
				trigger2Object.GetComponent<NinjaTrigger>().NinjaTriggerCut += NinjaTriggerFirstCutEventHandler;
				trigger3Object.GetComponent<NinjaTrigger>().NinjaTriggerCut += NinjaTriggerFirstCutEventHandler;
				trigger4Object.GetComponent<NinjaTrigger>().NinjaTriggerCut += NinjaTriggerFirstCutEventHandler;
				trigger5Object.GetComponent<NinjaTrigger>().NinjaTriggerCut += NinjaTriggerFirstCutEventHandler;
				trigger6Object.GetComponent<NinjaTrigger>().NinjaTriggerCut += NinjaTriggerFirstCutEventHandler;
				//play swipe tutorial
				try{
					swipeTutAnimation = swipeTutObject.FindInChildren("AnimationParent").GetComponent<Animation>();
					swipeTutAnimation.Play("NinjaSwipeTut1");
				}
				catch(NullReferenceException e){
					Debug.LogException(e);
				}

                break;
            default:
                Debug.LogError("Ninja tutorial has an unhandled step: " + step);
                break;
        }
    }

	private void NinjaTriggerFirstCutEventHandler(object sender, EventArgs args){
		// convoluted way of removing the listener 
		if(sender.ToString() == "NinjaTrigger1(Clone) (NinjaTriggerTarget)"){
		trigger1Object.GetComponent<NinjaTrigger>().NinjaTriggerCut -= NinjaTriggerFirstCutEventHandler;
		}
		else if(sender.ToString() == "NinjaTrigger2(Clone) (NinjaTriggerTarget)"){
			trigger2Object.GetComponent<NinjaTrigger>().NinjaTriggerCut -= NinjaTriggerFirstCutEventHandler;
		}
		else if(sender.ToString() == "NinjaTrigger3(Clone) (NinjaTriggerTarget)"){
			trigger3Object.GetComponent<NinjaTrigger>().NinjaTriggerCut -= NinjaTriggerFirstCutEventHandler;
		}
		else if(sender.ToString() == "NinjaTrigger4(Clone) (NinjaTriggerTarget)"){
			trigger4Object.GetComponent<NinjaTrigger>().NinjaTriggerCut -= NinjaTriggerFirstCutEventHandler;
		}
		else if(sender.ToString() == "NinjaTrigger5(Clone) (NinjaTriggerTarget)"){
			trigger5Object.GetComponent<NinjaTrigger>().NinjaTriggerCut -= NinjaTriggerFirstCutEventHandler;
		}
		else if(sender.ToString() == "NinjaTrigger6(Clone) (NinjaTriggerTarget)"){
			trigger6Object.GetComponent<NinjaTrigger>().NinjaTriggerCut -= NinjaTriggerFirstCutEventHandler;
		}
		numOfTriggersCut++;
		//end the tutorial when all triggers are cut
		if(numOfTriggersCut >= 6) {
			Advance();
		}
	}
}
