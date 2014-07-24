using UnityEngine;
using System;
using System.Collections;

public class NinjaTutorial : MinigameTutorial {
    public static string TUT_KEY = "NINJA_TUT";

	private Animation swipeTutAnimation;
	private GameObject swipeTutObject;
	private GameObject trigger1Object;
	  
    protected override void SetMaxSteps(){
        maxSteps = 1;
    }
	     
    protected override void SetKey(){
        tutorialKey = TUT_KEY;
    }

	protected override void _End(bool isFinished){
		if(swipeTutObject != null)
			GameObject.Destroy(swipeTutObject);
	}

    protected override void ProcessStep(int step){
        Hashtable option = new Hashtable();

        switch(step){
            case 0:
				trigger1Object = NinjaManager.Instance.SpawnSingleTriggerTutorial();
				GameObject swipeTut = (GameObject) Resources.Load("NinjaSwipeTut");
				swipeTutObject = LgNGUITools.AddChildWithPosition(GameObject.Find("Anchor-Center"), swipeTut);

				//listen to when trigger gets cut
				trigger1Object.GetComponent<NinjaTrigger>().NinjaTriggerCut += NinjaTriggerFirstCutEventHandler;

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
		trigger1Object.GetComponent<NinjaTrigger>().NinjaTriggerCut -= NinjaTriggerFirstCutEventHandler;
		Advance();
	}
}
