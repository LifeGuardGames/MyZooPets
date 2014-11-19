using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectionUIManager : Singleton<SelectionUIManager> {
    public GameObject spotLight; //spotlight to shine on the egg when chosen	
    public GameObject selectionGrid;

    void Awake(){
        Input.multiTouchEnabled = false;
    }
    
	// Use this for initialization
	void Start () {
        RefreshUI();	
	}

    //---------------------------------------------------
    // PetSelected()
    // Call when select button is clicked. Decides to start 
    // a new pet or load existing game data
    //---------------------------------------------------
    public void PetSelected(GameObject selectedPetGO){
        MutableDataPetInfo petMenuInfo = SelectionManager.Instance.PetMenuInfo;
        bool isHatched = petMenuInfo.IsHatched;

        //probably shoudn't use spot light right away. should toggle spot light
        //after some logic check for the data
        ToggleEggAnimation(false);

        if(!isHatched){
            //Open CustomizationUIManager to create/initiate new pet game data
            CustomizationUIManager.Instance.selectedEgg = selectedPetGO;
            CustomizationUIManager.Instance.OpenUI();
        }else{
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
            Transform eggParent = child.Find("MenuSceneEgg/SpriteGrandparent/SpriteParent (Animation)");
			if(eggParent != null){
				if(isOn)
					eggParent.GetComponent<RandomAnimation>().Enable();
				else{
					eggParent.GetComponent<RandomAnimation>().Disable();
				}
			}
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
            }else{
                GameObject menuScenePetPrefab = Resources.Load("MenuScenePet") as GameObject;
                GameObject menuScenePetGO = NGUITools.AddChild(petSelectionGO, menuScenePetPrefab);

                menuScenePetGO.name = "MenuScenePet";
                UILabel petNameLabel = menuScenePetGO.transform.Find("Label_PetName").GetComponent<UILabel>();
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
