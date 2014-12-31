using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CustomizationUIManager : SingletonUI<CustomizationUIManager> {
    public GameObject customizationPanel;
    public GameObject popupTitle;
    public UILabel nameField;
	public Camera NGUICamera;
    public SceneTransition scriptTransition;
    public ButtonSetHighlight buttonHighLight;
	public ParticleSystemController leafParticle;
	public Animation requireNameAnimation;

    private string petColor;
    private string petName; //Default pet name
    private Color currentRenderColor;
    private bool finishClicked = false;
    private bool isComicOn;
	
    protected override void Awake(){
		base.Awake();
		petColor = PetColor.OrangeYellow.ToString(); //Default pet color
        eModeType = UIModeTypes.CustomizePet;
        isComicOn = Constants.GetConstant<bool>("IsComicIntroOn");
    }
	
	protected override void _OpenUI(){
        //HideTitle();
        ShowChooseGUI();	
	}
	
	// Used when pressing back button in the panel
	protected override void _CloseUI(){
	}
    
    public void ChangeEggColor(string spriteName, string petColor){
        if (!finishClicked){
			GameObject selectedEgg = SelectionUIManager.Instance.SelectedPet;
			ParticlePlane.Instance.PlayParticle(NGUICamera.camera.WorldToScreenPoint(selectedEgg.transform.position));
			Sprite sprite = Resources.Load<Sprite>(spriteName);
            selectedEgg.transform.FindChild("SpriteGrandparent/SpriteParent (Animation)/Sprite").GetComponent<SpriteRenderer>().sprite = sprite;
            this.petColor = petColor;
        }       
    }

    public void ButtonClicked_Back(){
        base.CloseUI();
        HideChooseGUI(false);
    }

    public void ButtonClicked_Finish(){
		if(!String.IsNullOrEmpty(nameField.text)){
			if (!finishClicked){
				// play sound
				AudioManager.Instance.PlayClip("introDoneNaming");
				
				finishClicked = true;
				petName = nameField.text;
				
				Analytics.Instance.PetColorChosen(this.petColor);
				Analytics.Instance.StartGame();
				
				//Initialize data for new pet
				DataManager.Instance.ModifyBasicPetInfo(petName:petName, petSpecies:"Basic", petColor:this.petColor);
			}
			base.CloseUI();
			HideChooseGUI(true);
		}
		// Play the animation to prompt user to enter name
		else{
			requireNameAnimation.Play();
		}
    }
	
    private void ShowChooseGUI(){
		SelectionUIManager.Instance.ToggleEggAnimation(true);
		leafParticle.Stop();
        customizationPanel.GetComponent<TweenToggleDemux>().Show();

        //find out what color is the egg and change the color selection button
		GameObject selectedEgg = SelectionUIManager.Instance.SelectedPet;
        string defaultEggColor = selectedEgg.transform.FindChild("SpriteGrandparent/SpriteParent (Animation)/Sprite").GetComponent<SpriteRenderer>().sprite.name;
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
		LoadLevelUIManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
	}
}
