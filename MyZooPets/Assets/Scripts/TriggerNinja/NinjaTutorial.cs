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
        vPos = POS_TOP;
        string strResourceKey = "TutorialMessageWithButton";
        switch(nStep){
            case 0:
                ShowMessage(strResourceKey, vPos);
                break;
            case 1:
                ShowMessage(strResourceKey, vPos);
                break;
            default:
                Debug.LogError("Ninja tutorial has an unhandled step: " + nStep);
                break;
        }
    }

    private void SpawnTutorial(string spriteName){
        GameObject tutPrefab = (GameObject) Resources.Load("TutorialMessage_Ninja");
        GameObject tutGO = (GameObject) GameObject.Instantiate(tutPrefab);

        tutGO.transform.Find("Label_Message").GetComponent<UILabel>().text = 
            Localization.Localize(GetKey() + "_" + GetStep());
        tutGO.transform.Find("Sprite_Hint").GetComponent<UISprite>().spriteName = spriteName; 

        ShowMessage(tutGO, POS_TOP);
    }
}
