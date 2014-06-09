using UnityEngine;
using System;
using System.Collections;

public class RunnerTutorial : MinigameTutorial {
    public static string TUT_KEY = "RUNNER_TUT";

    public RunnerTutorial(){

    }

    //---------------------------------------------------
    // SetMaxSteps()
    //---------------------------------------------------       
    protected override void SetMaxSteps() {
        maxSteps = 3;
    }   
    
    //---------------------------------------------------
    // SetKey()
    //---------------------------------------------------   
    protected override void SetKey() {
        tutorialKey = TUT_KEY;
    }
    
    //---------------------------------------------------
    // _End()
    //---------------------------------------------------   
    protected override void _End( bool bFinished ) {
    }
    
    //---------------------------------------------------
    // ProcessStep()
    //---------------------------------------------------       
    protected override void ProcessStep( int nStep ) {
        // location and type of the tutorial message
        Vector3 vPos = new Vector3();
        string strResourceKey = Tutorial.POPUP_STD_WITH_IMAGE;
        vPos = POS_TOP; 
        Hashtable option = new Hashtable();

        switch ( nStep ) {
            case 0:
                PlayerController.OnJump += TutorialJump;
                option.Add(TutorialPopupFields.SpriteAtlas, "RunnerAtlas");
                option.Add(TutorialPopupFields.SpriteName, "tutorialRunnerTap");
                break;
            case 1:
                PlayerController.OnDrop += TutorialDrop;
                option.Add(TutorialPopupFields.SpriteAtlas, "RunnerAtlas");
                option.Add(TutorialPopupFields.SpriteName, "tutorialRunnerSwipeDown");
                break;
            case 2:
                TutorialPopup.Callback button1Fuction = delegate(){
                    Advance();
                };

                option.Add(TutorialPopupFields.Button1Callback, button1Fuction);
                strResourceKey = Tutorial.POPUP_LONG_WITH_BUTTON;      
                break;
            default:
                Debug.Log("Runner tutorial has an unhandled step: " + nStep );
                break;      
        }       

        ShowPopup(strResourceKey, vPos, false, option);
    }

    private void TutorialJump(object sender, EventArgs args){
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
