using UnityEngine;
using System;
using System.Collections;

public class NinjaTutorial : MinigameTutorial {
    public static string TUT_KEY = "NINJA_TUT";

	private Animation swipeTutAnimation;
	private GameObject trigger1Object;
	  
    protected override void SetMaxSteps(){
        maxSteps = 2;
    }
	     
    protected override void SetKey(){
        tutorialKey = TUT_KEY;
    }

	protected override void _End(bool bFinished){
		throw new NotImplementedException();
	}

    protected override void ProcessStep(int nStep){
        Vector3 vPos = new Vector3();
        string strResourceKey = Tutorial.POPUP_LONG_WITH_BUTTON_AND_IMAGE; 
        vPos = POS_TOP;
        Hashtable option = new Hashtable();

//        TutorialPopup.Callback button1Fuction = delegate(){
//            Advance();
//        };
//        option.Add(TutorialPopupFields.SpriteAtlas, "TriggerNinjaAtlas");
//        option.Add(TutorialPopupFields.Button1Callback, button1Fuction);

        switch(nStep){
            case 0:
//                option.Add(TutorialPopupFields.SpriteName, "tutorialNinjaSwipe");
				trigger1Object = NinjaManager.Instance.SpawnSingleTriggerTutorial();
				GameObject swipeTut = (GameObject) Resources.Load("NinjaSwipeTut");
				GameObject swipeTutObject = LgNGUITools.AddChildWithPosition(GameObject.Find("Anchor-Center"), swipeTut);

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
            case 1:
//                option.Add(TutorialPopupFields.SpriteName, "tutorialNinjaAvoid");
				swipeTutAnimation.Play("NinjaSwipeTut2");
                break;
            default:
                Debug.LogError("Ninja tutorial has an unhandled step: " + nStep);
                break;
        }

//        ShowPopup(strResourceKey, vPos, false, option);
    }

	private void NinjaTriggerFirstCutEventHandler(object sender, EventArgs args){
		trigger1Object.GetComponent<NinjaTrigger>().NinjaTriggerCut -= NinjaTriggerFirstCutEventHandler;
		Advance();
	}
}
