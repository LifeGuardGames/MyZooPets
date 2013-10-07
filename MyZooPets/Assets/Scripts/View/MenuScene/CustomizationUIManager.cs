using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
    
*/

public class CustomizationUIManager : SingletonUI<CustomizationUIManager> {

    // public GameObject eggObject;
    // public GameObject nestObject;
    public GameObject customizationPanel;
    public GameObject popupTitle;
    public UILabel nameField;
    public GameObject selectedEgg; 
    
    // Camera moving
    private float smooth = 1.0f;
    //private bool isZoomed = false;
    //private Vector3 initPosition = new Vector3(0.1938391f, 11.47f, 2.83f);
    //private Vector3 initFaceDirection = new Vector3(11.3f, 0, 0);
    //private Vector3 finalPosition = new Vector3(4.7f, 7.08f, 12.23f);
    //private Vector3 finalFaceDirection = new Vector3(11.3f, 0, 0);

    private string petColor;
    private string petName;

    private Color currentRenderColor;
    private bool finishClicked = false;

    void Start(){
        // // eggSpriteScript = eggObject.GetComponent<tk2dSprite>();
        // currentRenderColor = RenderSettings.ambientLight;
        // RenderSettings.ambientLight = Color.black;

      //  ShowDropInAnimation();
    	Invoke ("ShowDropInAnimation", 1f);	// TODO-s DIRTY HACK GET THIS WORKING, MAYBE NEXT FRAME CALL?
	}
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------	
	protected override void _OpenUI(){
       	// eggClicked = true;
        // CameraTransform(finalPosition,finalFaceDirection);
        // isZoomed = true;
        HideTitle();
        ShowChooseGUI();	
	}	

    // Callback for closing edit panel
    public void HelperFinishEditPet(){
        // DataManager.Instance.GameData.PetName = petName;
        // DataManager.Instance.GameData.PetColor = petColor;
        // DataManager.Instance.GameData.TurnFirstTimeOff();
    }
    
    public void ChangeEggColor( string strSprite, string strColor ) {
        if (!finishClicked){
            selectedEgg.GetComponent<UISprite>().spriteName = strSprite;
            petColor = strColor;
        }       
    }

    public void ButtonClicked_Finish(){
        if (!finishClicked){
            // play sound
            AudioManager.Instance.PlayClip( "introDoneNaming" );
            
            finishClicked = true;
            petName = nameField.text;

            //Initialize data for new pet
            DataManager.Instance.InitializeGameDataForNewPet();

            //Set the PetInfo
            DataManager.Instance.GameData.PetInfo.PetID = selectedEgg.transform.parent.name;
            DataManager.Instance.GameData.PetInfo.PetName = petName;
            DataManager.Instance.GameData.PetInfo.PetColor = petColor;
            DataManager.Instance.GameData.PetInfo.IsHatched = true;
            // if(isZoomed){
            //     ZoomOutMove();
            //     isZoomed = false;
            //     HideChooseGUI();
            // }
            HideChooseGUI();
        }
    }

    private void ShowDropInAnimation(){
        // Splash finished, Drop down the title and the egg sprite, only called once
        popupTitle.GetComponent<PositionTweenToggle>().Show();

        // Hashtable optional = new Hashtable();
        // optional.Add("ease", LeanTweenType.easeOutBounce);
        // LeanTween.move(eggObject, eggSpritePosition, 1.5f, optional);
    }
    
    private void ShowChooseGUI(){
        customizationPanel.GetComponent<PositionTweenToggle>().Show(smooth);
    }

    private void HideChooseGUI(){
        customizationPanel.GetComponent<PositionTweenToggle>().Hide(smooth);
        Invoke("ShowIntroMovie", 1);
    }

    private void HideTitle(){
        popupTitle.GetComponent<PositionTweenToggle>().Hide();
//        Destroy(popupTitle, 3.0f);
    }

    // private void CameraTransform (Vector3 newPosition, Vector3 newDirection){
    //     Hashtable optional = new Hashtable();
    //     optional.Add("ease", LeanTweenType.easeInOutQuad);
    //     LeanTween.move(mCamera, newPosition, smooth, optional);
    //     LeanTween.rotate(mCamera, newDirection, smooth, optional);
    // }
	
	private void ShowIntroMovie() {
		if ( DataManager.Instance.GameData.Cutscenes.ListViewed.Contains("Cutscene_Intro") )
			LoadScene();
		
		GameObject resourceMovie = Resources.Load("Cutscene_Intro") as GameObject;
		LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceMovie );
		CutsceneFrames.OnCutsceneDone += IntroMovieDone;
	}
	
    private void IntroMovieDone(object sender, EventArgs args){
		DataManager.Instance.GameData.Cutscenes.ListViewed.Add("Cutscene_Intro");
		CutsceneFrames.OnCutsceneDone -= IntroMovieDone;
		LoadScene();
    }
	
	private void LoadScene() {
        GetComponent<SceneTransition>().StartTransition();
	}
}
