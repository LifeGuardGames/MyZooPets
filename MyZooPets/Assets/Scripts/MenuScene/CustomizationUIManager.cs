using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CustomizationUIManager:SingletonUI<CustomizationUIManager>{
	public TweenToggle nameTweenParent;		// Part 1 of the selection process
	public TweenToggle colorTweenParent;	// Part 2 of the selection process
    public UILabel nameField;
	public Camera NGUICamera;
	public ParticleSystemController leafParticle;
	public Animation requireNameAnimation;
	public ParticleSystem poofParticle;
	public TweenToggle logoTitleTween;

    private string petColor;
    private string petName; //Default pet name
    private Color currentRenderColor;
	
    protected override void Awake(){
		base.Awake();
		petColor = PetColor.OrangeYellow.ToString(); //Default pet color
        eModeType = UIModeTypes.CustomizePet;
    }
	
	protected override void _OpenUI(){
		logoTitleTween.Hide();
        ShowFirstChooseUI();
	}
	
	// Used when pressing back button in the panel
	protected override void _CloseUI(){
	}
    
    public void ChangeEggColor(string spriteName, string petColor){
		GameObject selectedEgg = SelectionUIManager.Instance.SelectedPet;
		poofParticle.Play();
		Sprite sprite = Resources.Load<Sprite>(spriteName);
        selectedEgg.transform.FindChild("SpriteGrandparent/SpriteParent (Animation)/Sprite").GetComponent<SpriteRenderer>().sprite = sprite;
        this.petColor = petColor;     
    }

	private void ShowFirstChooseUI(){
		SelectionUIManager.Instance.ToggleEggAnimation(false);
		SelectionUIManager.Instance.EggCrack(1);
		leafParticle.Stop();
		nameTweenParent.Show();
	}

	public void FirstFinishClicked(){
		if(!String.IsNullOrEmpty(nameField.text)){
			petName = nameField.text;
			nameTweenParent.Hide();
			SelectionUIManager.Instance.EggCrack(2);
		}
		else{
			// Play the animation to prompt user to enter name
			requireNameAnimation.Play();
		}
	}

	/// <summary>
	/// Shows the second choose UI, called from first finish callback
	/// </summary>
	public void ShowSecondChooseUI(){
		colorTweenParent.Show();
	}

	public void SecondFinishClicked(){
		colorTweenParent.Hide();

		Analytics.Instance.PetColorChosen(this.petColor);
		Analytics.Instance.StartGame();

		// Initialize data for new pet
		DataManager.Instance.ModifyBasicPetInfo(petName:petName, petSpecies:"Basic", petColor:this.petColor);

		// Play the movie
		if(Constants.GetConstant<bool>("IsComicIntroOn")){
			Invoke("ShowIntroMovie", 1);
		}
		else{
			LoadScene();
		}
	}

	private void ShowIntroMovie() {
//		ClickManager.Instance.Lock(UIModeTypes.IntroComic);

		if(DataManager.Instance.GameData.Cutscenes.ListViewed.Contains("Comic_Intro")){
			LoadScene();
		}

//		SelectionUIManager.Instance.ToggleEggAnimation(false);
//		AudioManager.Instance.LowerBackgroundVolume(0.1f);
		
		GameObject comicPlayerPrefab = Resources.Load("IntroComicPlayer") as GameObject;
		GameObject goComicPlayer = GameObjectUtils.AddChildWithPositionAndScale(null, comicPlayerPrefab) as GameObject;
		goComicPlayer.GetComponent<ComicPlayer>().Init(petColor);
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
