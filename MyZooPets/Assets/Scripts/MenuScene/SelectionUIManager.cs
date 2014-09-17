using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectionUIManager : Singleton<SelectionUIManager> {
    public GameObject spotLight; //spotlight to shine on the egg when chosen	
//    public GameObject petSelectionOption; //reference to UI element 
    public GameObject selectionGrid;
    private string selectedPetID;


    void Awake(){
        Input.multiTouchEnabled = false;
    }
    
	// Use this for initialization
	void Start () {
        RefreshUI();	
	}

	void OnGUI(){
		if(GUI.Button(new Rect(0, 0, 100, 50), "New account")){
			DataManager.Instance.AddNewMenuSceneData();
			RefreshUI();
		}
	}

	/// <summary>
	/// Pet selected.
	/// </summary>
	/// <param name="selectedPetGO">Selected pet GO.</param>
    public void PetSelected(GameObject selectedPetGO){
		try{
			selectedPetID = selectedPetGO.name;
			Dictionary<string, MutableDataPetMenuInfo> petMenuInfoDict = SelectionManager.Instance.PetMenuInfo;
			MutableDataPetMenuInfo petMenuInfo = petMenuInfoDict[selectedPetID];
			bool isHatched = !string.IsNullOrEmpty(petMenuInfo.PetName);
			
			//probably shoudn't use spot light right away. should toggle spot light
			//after some logic check for the data
			ToggleEggAnimation(false);
			//        ToggleSpotLight(true, selectedPetGO);
			
			if(!isHatched){
				HideSelectionOption();
				
				//Open CustomizationUIManager to create/initiate new pet game data
				CustomizationUIManager.Instance.selectedEgg = selectedPetGO;
				CustomizationUIManager.Instance.OpenUI();
			}else{
				//open up pet start panel
				AudioManager.Instance.PlayClip("petStart");
				LoadGame();
			}
		}
		catch(Exception e){
			Debug.LogError("Exception caught with message: " + e.Message);
		}
    }

    private void ShowSelectionOption(){
//        petSelectionOption.GetComponent<TweenToggleDemux>().Show();
    }

    private void HideSelectionOption(){
//        TweenToggleDemux tweenToggleDemux = petSelectionOption.GetComponent<TweenToggleDemux>();
//
//        if(tweenToggleDemux.IsShowing)
//            tweenToggleDemux.Hide();
    }

    public void LoadGame(){

        //Lock it while loading
        ClickManager.Instance.Lock(UIModeTypes.IntroComic);

        //Load game data 
        DataManager.Instance.OnGameDataLoaded += EnterGameAfterGameDataDeserialized;
        SelectionManager.Instance.LoadPetGameData(selectedPetID);
    }

    public void DeleteGameData(){
        //need to do double confirmation first
//        PopupNotificationNGUI.HashEntry button1Function = delegate(){
//            HideSelectionOption();
//
//            //Delete game data 
//            SelectionManager.Instance.RemovePetData(selectedPetID);
//
//            //Update UI
//            ToggleSpotLight(false);
//            RefreshUI();
//        };
//
//        PopupNotificationNGUI.HashEntry button2Function = delegate(){
//        };
//
//        Dictionary<string, MutableDataPetMenuInfo> petMenuInfoDict = SelectionManager.Instance.PetMenuInfo;
//        string petName = "";
//        if(petMenuInfoDict.ContainsKey(selectedPetID))
//            petName = petMenuInfoDict[selectedPetID].PetName;
//
//        string deleteMessage = String.Format(Localization.Localize("DELETE_CONFIRM"),
//            petName, StringUtils.FormatStringPossession(petName));
//
//        Hashtable notificationEntry = new Hashtable();
//        notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TwoButtons);
//        notificationEntry.Add(NotificationPopupFields.Message, deleteMessage);
//        notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
//        notificationEntry.Add(NotificationPopupFields.Button2Callback, button2Function);
//
//        NotificationUIManager.Instance.AddToQueue(notificationEntry);
    }

    //---------------------------------------------------
    // ToggleSpotLight()
    // Turn spot light on the egg or pet animation off
    //---------------------------------------------------
//    public void ToggleSpotLight(bool isOn, GameObject selectedGO = null){
//        if(isOn){
//            spotLight.SetActive(true);
//            Vector3 selectedPos = selectedGO.transform.position;
//            //offset is hardcoded
//            spotLight.transform.position = new Vector3(selectedPos.x, selectedPos.y, spotLight.transform.position.z);
//        }
//        else{
//            spotLight.SetActive(false);
//        }
//    }

    //---------------------------------------------------
    // ToggleEggAnimation()
    // Turn egg wiggle animation on/off
    //---------------------------------------------------
    public void ToggleEggAnimation(bool isOn){
	
		try{
			Dictionary<string, MutableDataPetMenuInfo> petMenuInfoDict = SelectionManager.Instance.PetMenuInfo;
			MutableDataPetMenuInfo petMenuInfo = petMenuInfoDict[selectedPetID];
			
			foreach(Transform child in selectionGrid.transform){
				bool isHatched = !string.IsNullOrEmpty(petMenuInfo.PetName);
				
				if(!isHatched){
					Transform eggParent = child.Find("SpriteGrandparent/SpriteParent (Animation)");
					if(isOn)
						eggParent.GetComponent<RandomAnimation>().Enable();
					else
						eggParent.GetComponent<RandomAnimation>().Disable();
				}
			}
		}
		catch(Exception e){
			Debug.Log("Exception caught in function ToggleEggAnimation with message: " + e.Message);
		}
    }
    
    //First initialization of the PetSelectionArea
    private void RefreshUI(){

        //Remove old data
        foreach(Transform childTransform in selectionGrid.transform){
            childTransform.gameObject.SetActive(false);
            Destroy(childTransform.gameObject);
        }

		Dictionary<string, MutableDataPetMenuInfo> petMenuInfoDict = SelectionManager.Instance.PetMenuInfo;
		foreach(KeyValuePair<string, MutableDataPetMenuInfo> keyValuePair in petMenuInfoDict){
			string petID = keyValuePair.Key;
			MutableDataPetMenuInfo petMenuInfo = keyValuePair.Value;
			bool isHatched = !string.IsNullOrEmpty(petMenuInfo.PetName);

			//pet name empty = not hatched yet
			if(!isHatched){
				GameObject menuSceneEggPrefab = Resources.Load("MenuSceneEgg") as GameObject;
				GameObject menuSceneEggGO = NGUITools.AddChild(selectionGrid, menuSceneEggPrefab);

				menuSceneEggGO.name = petID;
				menuSceneEggGO.transform.localScale = menuSceneEggPrefab.transform.localScale;
				menuSceneEggGO.GetComponent<LgButtonMessage>().target = this.gameObject;
				menuSceneEggGO.GetComponent<LgButtonMessage>().functionName = "PetSelected";
			}
			else{
				GameObject menuScenePetPrefab = Resources.Load("MenuScenePet") as GameObject;
				GameObject menuScenePetGO = NGUITools.AddChild(selectionGrid, menuScenePetPrefab);

				menuScenePetGO.name = petID;
				UILabel petNameLabel = menuScenePetGO.transform.Find("Label_PetName").GetComponent<UILabel>();
				petNameLabel.text = petMenuInfo.PetName;

				menuScenePetGO.GetComponent<LgButtonMessage>().target = this.gameObject;
				menuScenePetGO.GetComponent<LgButtonMessage>().functionName = "PetSelected";
			}
		}

//        foreach(Transform petSelectionTransform  in selectionGrid.transform){
//            GameObject petSelectionGO = petSelectionTransform.gameObject;
//            string petID = petSelectionTransform.name;
//            bool isHatched = petMenuInfo != null;
//
//            //Turn show case animation on or off
//            if(!isHatched){
//                GameObject menuSceneEggPrefab = Resources.Load("MenuSceneEgg") as GameObject;
//                GameObject menuSceneEggGO = NGUITools.AddChild(petSelectionGO, menuSceneEggPrefab);
//
//                menuSceneEggGO.name = "MenuSceneEgg";
//                menuSceneEggGO.transform.localScale = menuSceneEggPrefab.transform.localScale;
//                menuSceneEggGO.GetComponent<LgButtonMessage>().target = this.gameObject;
//                menuSceneEggGO.GetComponent<LgButtonMessage>().functionName = "PetSelected";
//
////                if(petID == "Pet1"){
////                    menuSceneEggGO.transform.Find("SpriteGrandparent/SpriteParent (Animation)/Sprite").GetComponent<UISprite>().spriteName = "eggPurpleLime";
////                }
//
//            }else{
//                GameObject menuScenePetPrefab = Resources.Load("MenuScenePet") as GameObject;
//                GameObject menuScenePetGO = NGUITools.AddChild(petSelectionGO, menuScenePetPrefab);
//                // GameObject lwfObject = menuScenePetGO.transform.Find("UILWFObject").gameObject;
//
//                menuScenePetGO.name = "MenuScenePet";
//                UILabel petNameLabel = menuScenePetGO.transform.Find("Label_PetName").GetComponent<UILabel>();
//                petNameLabel.text = petMenuInfo.PetName;
//
//                // lwfObject.transform.localScale = menuScenePetPrefab.transform.localScale;
//                menuScenePetGO.GetComponent<LgButtonMessage>().target = this.gameObject;
//                menuScenePetGO.GetComponent<LgButtonMessage>().functionName = "PetSelected";
//            }
//        }
    }	

    //After existing game data has been loaded. Enter the game
    private void EnterGameAfterGameDataDeserialized(object sender, DataManager.SerializerEventArgs args){
        if(args.IsSuccessful){
            LoadScene();
        
            //Unregister itself from the event
            DataManager.Instance.OnGameDataLoaded -= EnterGameAfterGameDataDeserialized;
        }
    }

    private void LoadScene(){
        LoadLevelUIManager.Instance.StartLoadTransition(SceneUtils.BEDROOM, "");
    }
}
