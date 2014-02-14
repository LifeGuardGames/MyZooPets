using UnityEngine;
using System.Collections;

public class NinjaTutorial : MinigameTutorial {
    public static string TUT_KEY = "NINJA_TUT";

    public NinjaTutorial(){

    }

    //---------------------------------------------------
    // SetMaxSteps()
    //---------------------------------------------------       
    protected override void SetMaxSteps(){
        nMaxSteps = 2;
    }

    //---------------------------------------------------
    // SetKey()
    //---------------------------------------------------       
    protected override void SetKey(){
        strKey = TUT_KEY;
    }

    //---------------------------------------------------       
    // _End()
    //---------------------------------------------------       
    protected override void _End(bool bFinished){
    }

    //---------------------------------------------------       
    // ProcessStep()
    //---------------------------------------------------       
    protected override void ProcessStep(int nStep){
        Vector3 vPos = new Vector3();
        string strResourceKey = Tutorial.POPUP_LONG_WITH_BUTTON_AND_IMAGE; 
        vPos = POS_TOP;
        Hashtable option = new Hashtable();

        TutorialPopup.Callback button1Fuction = delegate(){
            Advance();
        };
        option.Add(TutorialPopupFields.SpriteAtlas, "TriggerNinjaAtlas");
        option.Add(TutorialPopupFields.Button1Callback, button1Fuction);

        switch(nStep){
            case 0:
                option.Add(TutorialPopupFields.SpriteName, "tutorialNinjaSwipe");
                break;
            case 1:
                option.Add(TutorialPopupFields.SpriteName, "tutorialNinjaAvoid");
                break;
            default:
                Debug.LogError("Ninja tutorial has an unhandled step: " + nStep);
                break;
        }

        ShowPopup(strResourceKey, vPos, false, option);
    }
}
