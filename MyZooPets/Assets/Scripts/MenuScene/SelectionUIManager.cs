using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectionUIManager : Singleton<SelectionUIManager>{
	public GameObject PetSelectionParent;

	/// <summary>
	/// Gets or sets the selected pet.
	/// </summary>
	/// <value>The selected pet.</value>
	public GameObject SelectedPet { get; set; }

	void Awake(){
		Input.multiTouchEnabled = false;
	}
    
	// Use this for initialization
	void Start(){
		Initialize();
	}

	//First initialization of the PetSelectionArea
	private void Initialize(){
		//Remove old data
		foreach(Transform petSelectionTransform in PetSelectionParent.transform){
			Destroy(petSelectionTransform.gameObject);
		}
		
		MutableDataPetInfo petMenuInfo = SelectionManager.Instance.PetMenuInfo;
		bool isHatched = petMenuInfo.IsHatched;
		
		//Turn show case animation on or off
		if(!isHatched){
			GameObject menuSceneEggPrefab = Resources.Load("MenuSceneEgg") as GameObject;
			GameObject menuSceneEggGO = GameObjectUtils.AddChildWithPositionAndScale(PetSelectionParent, menuSceneEggPrefab);
			
			menuSceneEggGO.name = "MenuSceneEgg";
			menuSceneEggGO.transform.localScale = menuSceneEggPrefab.transform.localScale;
			menuSceneEggGO.GetComponent<EggController>().Init();
			
			
		}
		else{
			GameObject menuScenePetPrefab = Resources.Load("MenuScenePet") as GameObject;
			GameObject menuScenePetGO = GameObjectUtils.AddChildWithPositionAndScale(PetSelectionParent, menuScenePetPrefab);
			
			menuScenePetGO.name = "MenuScenePet";
			TextMesh petNameLabel = menuScenePetGO.transform.Find("Label_PetName").GetComponent<TextMesh>();
			petNameLabel.text = petMenuInfo.PetName;
			
			menuScenePetGO.GetComponent<LgButtonMessage>().target = this.gameObject;
			menuScenePetGO.GetComponent<LgButtonMessage>().functionName = "PetSelected";
		}
	}

	/// <summary>
	/// Pet selected and will trigger a membership check
	/// </summary>
	/// <param name="selectedPetGO">Selected pet</param>
	public void PetSelected(GameObject selectedPetGO){
		SelectedPet = selectedPetGO;

		MutableDataPetInfo petMenuInfo = SelectionManager.Instance.PetMenuInfo;
		bool isHatched = petMenuInfo.IsHatched;
		
		if(!isHatched){
			//Open CustomizationUIManager to create/initiate new pet game data
			CustomizationUIManager.Instance.OpenUI();
		}
		else{
			//open up pet start panel
			AudioManager.Instance.PlayClip("petStart");
			LoadGame();
		}

		// See if no-sync debug is turned on
//		if(Constants.GetConstant<bool>("IsMenusceneConnectionOn")){
//			//lock the UI to prevent user from spam clicking while waiting for membership
//			//check to finish
//			ClickManager.Instance.Lock(UIModeTypes.MembershipCheck);
//			StartMembershipCheck();
//		}
//		else{
//			// Skip into create pet
//			Debug.LogWarning("Connection debug turned off");
//			OpenCustomizationManager();
//		}
	}

	public void PlayMovie(string petColor){
		if(Constants.GetConstant<bool>("IsComicIntroOn")){
			StartCoroutine(ShowIntroMovie(petColor));
		}
		else{
			LoadGame();
		}
	}

	private IEnumerator ShowIntroMovie(string petColor){
		ClickManager.Instance.Lock(UIModeTypes.IntroComic);
		yield return new WaitForSeconds(.3f);

		if(DataManager.Instance.GameData.Cutscenes.ListViewed.Contains("Comic_Intro")){
			LoadGame();
		}
		
		//		SelectionUIManager.Instance.ToggleEggAnimation(false);
		//		AudioManager.Instance.LowerBackgroundVolume(0.1f);
		
		GameObject comicPlayerPrefab = Resources.Load("IntroComicPlayer") as GameObject;
		GameObject goComicPlayer = GameObjectUtils.AddChildWithPositionAndScale(null, comicPlayerPrefab) as GameObject;
		goComicPlayer.GetComponent<ComicPlayer>().Init(petColor);
		ComicPlayer.OnComicPlayerDone += IntroComicDone;

		// Play the comic music
		AudioManager.Instance.FadeOutPlayNewBackground("bgComic", isLoop:false);
	}
	
	private void IntroComicDone(object sender, EventArgs args){
		DataManager.Instance.GameData.Cutscenes.ListViewed.Add("Comic_Intro");
		ComicPlayer.OnComicPlayerDone -= IntroComicDone;
		Analytics.Instance.StartGame();
		LoadGame();
	}

	public void LoadGame(){
		//Lock it while loading
		ClickManager.Instance.Lock(UIModeTypes.None);
		LoadLevelManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
	}

	//After existing game data has been loaded. Enter the game
	//    private void EnterGameAfterGameDataDeserialized(object sender, DataManager.SerializerEventArgs args){
	//        if(args.IsSuccessful){
	//            LoadScene();
	//        
	//            //Unregister itself from the event
	//            DataManager.Instance.OnGameDataLoaded -= EnterGameAfterGameDataDeserialized;
	//        }
	//    }

	/// <summary>
	/// Starts the membership check.
	/// </summary>
	//	private void StartMembershipCheck(){
	//		SubscriptionAlertController.Instance.ShowSpinner();
	//		
	//		if(MembershipCheck.Instance){
	//			MembershipCheck.OnCheckDoneEvent += MembershipCheckDoneEventHandler;
	//			MembershipCheck.Instance.StartCheck();
	//		}
	//	}
	
	/// <summary>
	/// Handles Membership Check Done event. Check if pet is hatched and load game
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	//	private void MembershipCheckDoneEventHandler(object sender, EventArgs args){
	//		MembershipCheck.OnCheckDoneEvent -= MembershipCheckDoneEventHandler;
	//		ClickManager.Instance.ReleaseLock();
	//
	//		bool hasMembershipError = SubscriptionAlertController.Instance.CheckMembershipError();
	//		ParentPortalUIManager.Instance.ParentPortalTextCheck();
	//		if(!hasMembershipError){
	//			OpenCustomizationManager();
	//		}
	//	}
}
