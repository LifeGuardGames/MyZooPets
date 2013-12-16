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
        nMaxSteps = 3;
    }   
    
    //---------------------------------------------------
    // SetKey()
    //---------------------------------------------------   
    protected override void SetKey() {
        strKey = TUT_KEY;
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
        string strResourceKey = Tutorial.POPUP_STD;
        vPos = POS_TOP; 
        switch ( nStep ) {
            case 0:
                PlayerController.OnJump += TutorialJump;
                SpawnTutorial("tutorialRunnerTap");
                break;
            case 1:
                PlayerController.OnDrop += TutorialDrop;
                SpawnTutorial("tutorialRunnerSwipeDown");
                break;
            case 2:
                strResourceKey = "TutorialMessageEnd";
                ShowMessage( strResourceKey, vPos);            
                break;
            default:
                Debug.Log("Runner tutorial has an unhandled step: " + nStep );
                break;      
        }       
            
        // show the proper tutorial message
    }

    private void SpawnTutorial(string spriteName){
        GameObject tutPrefab = (GameObject) Resources.Load("TutorialPopup_Runner");
        GameObject tutGO = (GameObject) GameObject.Instantiate(tutPrefab);

        tutGO.transform.Find("Label_Message").GetComponent<UILabel>().text = 
            Localization.Localize(GetKey() + "_" + GetStep());
        tutGO.transform.Find("Sprite_Hint").GetComponent<UISprite>().spriteName = spriteName; 

        ShowPopup( tutGO, POS_TOP, false );            
    }

    public void TutorialJump(object sender, EventArgs args){
        PlayerController.OnJump -= TutorialJump;
        RemovePopup();
        RunnerGameManager.Instance.StartCoroutine(WaitBeforeAdvance());
    }

    public void TutorialDrop(object sender, EventArgs args){
        PlayerController.OnDrop -= TutorialDrop;
        RemovePopup();
        RunnerGameManager.Instance.StartCoroutine(WaitBeforeAdvance());
    }

    private IEnumerator WaitBeforeAdvance(){
        yield return new WaitForSeconds(2);
        Advance();
    }
}
