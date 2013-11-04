using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
    
*/

public class CustomizationUIManager : SingletonUI<CustomizationUIManager> {
    public GameObject customizationPanel;
    public GameObject popupTitle;
    public UILabel nameField;
    public GameObject selectedEgg;
	
	public Camera NGUICamera;
    private string petColor;
    private string petName;

    private Color currentRenderColor;
    private bool finishClicked = false;
	
	// transition
	public SceneTransition scriptTransition;

    void Start(){
    	Invoke ("ShowTitle", 1f);	// TODO-s DIRTY HACK GET THIS WORKING, MAYBE NEXT FRAME CALL?
	}
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------	
	protected override void _OpenUI(){
        HideTitle();
        ShowChooseGUI();	
	}
	
	// Used when pressing back button in the panel, NOT finished
	protected override void _CloseUI(){
		ShowTitle();
		HideChooseGUI(false);
	}
    
    public void ChangeEggColor( string strSprite, string strColor ) {
        if (!finishClicked){
			ParticlePlane.Instance.PlayParticle(NGUICamera.camera.WorldToScreenPoint(selectedEgg.transform.position));
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

            HideChooseGUI(true);
        }
    }

    public void ShowTitle(){
        // Splash finished, Drop down the title and the egg sprite, only called once
        popupTitle.GetComponent<PositionTweenToggle>().Show();
    }
    
	public void HideTitle(){
        popupTitle.GetComponent<PositionTweenToggle>().Hide();
    }
	
    private void ShowChooseGUI(){
        customizationPanel.GetComponent<TweenToggleDemux>().Show();
    }

    private void HideChooseGUI(bool showMovie){
        customizationPanel.GetComponent<TweenToggleDemux>().Hide();
		if(showMovie){
        	Invoke("ShowIntroMovie", 1);
		}
    }
	
	private void ShowIntroMovie() {
		if ( DataManager.Instance.GameData.Cutscenes.ListViewed.Contains("Cutscene_Intro") )
			LoadScene();
		
		GameObject resourceMovie = Resources.Load("LWF_Cutscene_Intro") as GameObject;
		LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceMovie );
		LgLwfCutscene.OnCutsceneDone += IntroMovieDone;
	}
	
    private void IntroMovieDone(object sender, EventArgs args){
		DataManager.Instance.GameData.Cutscenes.ListViewed.Add("Cutscene_Intro");
		LgLwfCutscene.OnCutsceneDone -= IntroMovieDone;
		LoadScene();
    }
	
	private void LoadScene() {
        scriptTransition.StartTransition( SceneUtils.BEDROOM );
	}
}
