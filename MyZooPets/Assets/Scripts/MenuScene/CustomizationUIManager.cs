using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CustomizationUIManager : SingletonUI<CustomizationUIManager> {
    public GameObject customizationPanel;
    public GameObject popupTitle;
    public UILabel nameField;
    public GameObject selectedEgg;
	public Camera NGUICamera;
    public SceneTransition scriptTransition;
    public ButtonSetHighlight buttonHighLight;
	public ParticleSystemController leafParticle;

    private string petColor = "OrangeYellow"; //Default pet color
    private string petName; //Default pet name
    private Color currentRenderColor;
    private bool finishClicked = false;
     private bool isComicOn;
	
    void Awake(){
        eModeType = UIModeTypes.CustomizePet;
        isComicOn = Constants.GetConstant<bool>("IsComicIntroOn");
    }

	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------	
	protected override void _OpenUI(){
        //HideTitle();
        ShowChooseGUI();	
	}
	
	// Used when pressing back button in the panel
	protected override void _CloseUI(){
	}
    
    public void ChangeEggColor( string strSprite, string strColor ) {
        if (!finishClicked){
			ParticlePlane.Instance.PlayParticle(NGUICamera.camera.WorldToScreenPoint(selectedEgg.transform.position));
            selectedEgg.transform.FindChild("SpriteGrandparent/SpriteParent (Animation)/Sprite").GetComponent<UISprite>().spriteName = strSprite;
            petColor = strColor;
        }       
    }

    public void ButtonClicked_Back(){
        base.CloseUI();
        HideChooseGUI(false);
    }

    public void ButtonClicked_Finish(){
        if (!finishClicked){
            // play sound
             AudioManager.Instance.PlayClip("introDoneNaming");
            
            finishClicked = true;
            petName = nameField.text;

            Analytics.Instance.PetColorChosen(petColor);
			Analytics.Instance.StartGame();

            //Initialize data for new pet
            DataManager.Instance.InitializeGameDataForNewPet(selectedEgg.name, 
                petName, "Basic", petColor);

        }
        base.CloseUI();
        HideChooseGUI(true);
    }
	
    private void ShowChooseGUI(){
		SelectionUIManager.Instance.ToggleEggAnimation(true);
		leafParticle.Stop();
        customizationPanel.GetComponent<TweenToggleDemux>().Show();

        //find out what color is the egg and change the color selection button
        string defaultEggColor = selectedEgg.transform.FindChild("SpriteGrandparent/SpriteParent (Animation)/Sprite").GetComponent<UISprite>().spriteName;
        LgButton colorButton = null;

        switch(defaultEggColor){
            case "eggOrangeYellow":
                colorButton = buttonHighLight.buttonList[0];
                petColor = "OrangeYellow";
            break;
            case "eggPurpleLime":
                colorButton = buttonHighLight.buttonList[1];
                petColor = "PurpleLime";
            break;
        }

        buttonHighLight.firstButton = colorButton;
        buttonHighLight.SetFirstButton();
    }

    private void HideChooseGUI(bool showMovie){
        customizationPanel.GetComponent<TweenToggleDemux>().Hide();
		if(showMovie){
			ClickManager.Instance.Lock(UIModeTypes.IntroComic);
			if(isComicOn)
				Invoke("ShowIntroMovie", 1);
			else
				LoadScene();
		}

        //since we turn on spotlight and turn off animation for customization UI
        //need to reverse them 
        else{
			leafParticle.Play();
            SelectionUIManager.Instance.ToggleEggAnimation(true);
//            SelectionUIManager.Instance.ToggleSpotLight(false);
        }
    }

	private void ShowIntroMovie() {
		if(DataManager.Instance.GameData.Cutscenes.ListViewed.Contains("Comic_Intro"))
			LoadScene();

		SelectionUIManager.Instance.ToggleEggAnimation(false);
		AudioManager.Instance.LowerBackgroundVolume(0.1f);
		
		GameObject resourceMovie = Resources.Load("IntroComicPlayer") as GameObject;
		LgNGUITools.AddChildWithPositionAndScale( GameObject.Find("Anchor-Center"), resourceMovie );
		ComicPlayer.OnComicPlayerDone += IntroComicDone;
	}

	private void IntroComicDone(object sender, EventArgs args){
		DataManager.Instance.GameData.Cutscenes.ListViewed.Add("Comic_Intro");
		ComicPlayer.OnComicPlayerDone -= IntroComicDone;
		LoadScene();
	}
	
	private void LoadScene(){
		LoadLevelUIManager.Instance.StartLoadTransition(SceneUtils.BEDROOM, "");
	}
}
