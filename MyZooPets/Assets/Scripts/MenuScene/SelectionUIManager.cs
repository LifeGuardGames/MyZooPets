using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectionUIManager : Singleton<SelectionUIManager>{
	public GameObject selectionGrid;

	private Transform eggAnimTransform;	// Egg animation object to be cached
	private GameObject crack1;
	private GameObject crack2;

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
		RefreshUI();
	}

	/// <summary>
	/// Pet selected and will trigger a membership check
	/// </summary>
	/// <param name="selectedPetGO">Selected pet</param>
	public void PetSelected(GameObject selectedPetGO){
		SelectedPet = selectedPetGO;

		// See if no-sync debug is turned on
		if(Constants.GetConstant<bool>("IsMenusceneConnectionOn")){
			//lock the UI to prevent user from spam clicking while waiting for membership
			//check to finish
			ClickManager.Instance.Lock(UIModeTypes.MembershipCheck);
			StartMembershipCheck();
		}
		else{
			// Skip into create pet
			Debug.LogWarning("Connection debug turned off");
			OpenCustomizationManager();
		}
	}

	/// <summary>
	/// Starts the membership check.
	/// </summary>
	private void StartMembershipCheck(){
		SubscriptionAlertController.Instance.ShowSpinner();
		
		if(MembershipCheck.Instance){
			MembershipCheck.OnCheckDoneEvent += MembershipCheckDoneEventHandler;
			MembershipCheck.Instance.StartCheck();
		}
	}

	/// <summary>
	/// Handles Membership Check Done event. Check if pet is hatched and load game
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void MembershipCheckDoneEventHandler(object sender, EventArgs args){
		MembershipCheck.OnCheckDoneEvent -= MembershipCheckDoneEventHandler;
		ClickManager.Instance.ReleaseLock();

		bool hasMembershipError = SubscriptionAlertController.Instance.CheckMembershipError();
		ParentPortalUIManager.Instance.ParentPortalTextCheck();
		if(!hasMembershipError){
			OpenCustomizationManager();
		}
	}

	private void OpenCustomizationManager(){
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
	}

	public void LoadGame(){
		//Lock it while loading
		ClickManager.Instance.Lock(UIModeTypes.IntroComic);
		LoadScene();
	}

	//---------------------------------------------------
	// ToggleEggAnimation()
	// Turn egg wiggle animation on/off
	//---------------------------------------------------
	public void ToggleEggAnimation(bool isOn){
		foreach(Transform child in selectionGrid.transform){
			eggAnimTransform = child.Find("MenuSceneEgg/SpriteGrandparent/SpriteParent (Animation)");

			if(eggAnimTransform != null){
				if(isOn)
					eggAnimTransform.GetComponent<RandomAnimation>().Enable();
				else{
					eggAnimTransform.GetComponent<RandomAnimation>().Disable();
				}
			}
		}
	}

	public void EggCrack(int crackNumber){
		if(eggAnimTransform == null){
			foreach(Transform child in selectionGrid.transform){
				eggAnimTransform = child.Find("MenuSceneEgg/SpriteGrandparent/SpriteParent (Animation)");
			}
		}

		if(crack1 == null || crack2 == null){
			crack1 = eggAnimTransform.Find("Crack1").gameObject;
			crack2 = eggAnimTransform.Find("Crack2").gameObject;
		}

		switch(crackNumber){
		case 1:
			crack1.SetActive(true);
			crack2.SetActive(false);
			eggAnimTransform.GetComponent<Animation>().Play("eggShakeOnce");
			break;
		case 2:
			crack1.SetActive(false);
			crack2.SetActive(true);
			eggAnimTransform.GetComponent<Animation>().Play("eggShakeOnce");
			break;
		default:
			Debug.LogError("Invalid crack number " + crackNumber);
			break;
		}
	}
    
	//First initialization of the PetSelectionArea
	private void RefreshUI(){
		//Remove old data
		foreach(Transform petSelectionTransform in selectionGrid.transform){
			foreach(Transform childTransform in petSelectionTransform){
				childTransform.gameObject.SetActive(false);
				Destroy(childTransform.gameObject);
			}
		}

		MutableDataPetInfo petMenuInfo = SelectionManager.Instance.PetMenuInfo;

		foreach(Transform petSelectionTransform  in selectionGrid.transform){
			GameObject petSelectionGO = petSelectionTransform.gameObject;
			bool isHatched = petMenuInfo.IsHatched;

			//Turn show case animation on or off
			if(!isHatched){
				GameObject menuSceneEggPrefab = Resources.Load("MenuSceneEgg") as GameObject;
				GameObject menuSceneEggGO = NGUITools.AddChild(petSelectionGO, menuSceneEggPrefab);

				menuSceneEggGO.name = "MenuSceneEgg";
				menuSceneEggGO.transform.localScale = menuSceneEggPrefab.transform.localScale;
				menuSceneEggGO.GetComponent<LgButtonMessage>().target = this.gameObject;
				menuSceneEggGO.GetComponent<LgButtonMessage>().functionName = "PetSelected";
			}
			else{
				GameObject menuScenePetPrefab = Resources.Load("MenuScenePet") as GameObject;
				GameObject menuScenePetGO = NGUITools.AddChild(petSelectionGO, menuScenePetPrefab);

				menuScenePetGO.name = "MenuScenePet";
				TextMesh petNameLabel = menuScenePetGO.transform.Find("Label_PetName").GetComponent<TextMesh>();
				petNameLabel.text = petMenuInfo.PetName;

				menuScenePetGO.GetComponent<LgButtonMessage>().target = this.gameObject;
				menuScenePetGO.GetComponent<LgButtonMessage>().functionName = "PetSelected";
			}
		}
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

	private void LoadScene(){
		LoadLevelUIManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
	}
}
