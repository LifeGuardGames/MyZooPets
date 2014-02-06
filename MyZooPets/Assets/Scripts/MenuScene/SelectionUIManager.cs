using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectionUIManager : Singleton<SelectionUIManager> {
    public GameObject selectionGrid;
    public GameObject petSelectionPrefab; //Prefab that holds the basic structure for pet display layout
    public GameObject spotLight; //spotlight to shine on the egg when chosen	
    public GameObject petSelectionOption; //reference to UI element 
	public SceneTransition scriptTransition; //transition
	
    private string selectedPetID;

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
        Dictionary<string, MutableData_PetMenuInfo> petMenuInfoDict = DataManager.Instance.MenuSceneData;
        selectedPetID = selectedPetGO.transform.parent.name;
        bool isHatched = petMenuInfoDict.ContainsKey(selectedPetID);

        //probably shoudn't use spot light right away. should toggle spot light
        //after some logic check for the data
        ToggleEggAnimation(false);
        ToggleSpotLight(true, selectedPetGO);

        if(!isHatched){
            HideSelectionOption();

            //Open CustomizationUIManager to create/initiate new pet game data
            DataManager.Instance.CurrentPetID = selectedPetID;
            CustomizationUIManager.Instance.selectedEgg = selectedPetGO.transform.parent.Find("egg").gameObject;
            CustomizationUIManager.Instance.OpenUI();
        }else{
            //open up pet start panel
            ShowSelectionOption();

        }
    }

    private void ShowSelectionOption(){
        petSelectionOption.GetComponent<TweenToggleDemux>().Show();
    }

    private void HideSelectionOption(){
        TweenToggleDemux tweenToggleDemux = petSelectionOption.GetComponent<TweenToggleDemux>();

        if(tweenToggleDemux.IsShowing)
            tweenToggleDemux.Hide();
    }

    public void LoadGame(){
        //Load game data only if the selected pet is different from the current pet
        if(DataManager.Instance.CurrentPetID != selectedPetID){
            DataManager.Instance.CurrentPetID = selectedPetID;
            DataManager.Instance.OnGameDataLoaded += EnterGameAfterGameDataDeserialized;
            DataManager.Instance.LoadGameData();
        }else{
            if(DataManager.Instance.IsGameDataLoaded())
                LoadScene();
        }
    }

    public void DeleteGameData(){
        //need to do double confirmation first
        PopupNotificationNGUI.HashEntry button1Function = delegate(){
            HideSelectionOption();

            //Delete from DataManager
            DataManager.Instance.RemovePetData(selectedPetID);

            //Update UI
            ToggleSpotLight(false);
            RefreshUI();
        };

        PopupNotificationNGUI.HashEntry button2Function = delegate(){
        };

        Dictionary<string, MutableData_PetMenuInfo> petMenuInfoDict = DataManager.Instance.MenuSceneData;
        string petName = "";
        if(petMenuInfoDict.ContainsKey(selectedPetID))
            petName = petMenuInfoDict[selectedPetID].PetName;

        string deleteMessage = String.Format(Localization.Localize("DELETE_CONFIRM"),
            petName, StringUtils.FormatStringPossession(petName));

        Hashtable notificationEntry = new Hashtable();
        notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.TwoButtons);
        notificationEntry.Add(NotificationPopupFields.Message, deleteMessage);
        notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
        notificationEntry.Add(NotificationPopupFields.Button2Callback, button2Function);

        NotificationUIManager.Instance.AddToQueue(notificationEntry);
    }

    //---------------------------------------------------
    // ToggleSpotLight()
    // Turn spot light on the egg or pet animation off
    //---------------------------------------------------
    public void ToggleSpotLight(bool isOn, GameObject selectedGO = null){
        if(isOn){
            spotLight.SetActive(true);
            Vector3 selectedPos = selectedGO.transform.position;
            spotLight.transform.position = new Vector3(selectedPos.x, selectedPos.y, spotLight.transform.position.z);
        }
        else{
            spotLight.SetActive(false);
        }
    }

    //---------------------------------------------------
    // ToggleEggAnimation()
    // Turn egg wiggle animation on/off
    //---------------------------------------------------
    public void ToggleEggAnimation(bool isOn){
        Dictionary<string, MutableData_PetMenuInfo> petMenuInfoDict = DataManager.Instance.MenuSceneData;

        foreach(Transform child in selectionGrid.transform){
            string petID = child.name;
            bool isHatched = petMenuInfoDict.ContainsKey(petID);

            if(!isHatched){
                Transform eggParent = child.Find("egg/SpriteGrandparent/SpriteParent (Animation)");
                if(isOn)
                    eggParent.GetComponent<RandomAnimation>().Enable();
                else
                    eggParent.GetComponent<RandomAnimation>().Disable();
            }
        }
    }
    
    //First initialization of the PetSelectionArea
    private void RefreshUI(){
        //Remove old data
        foreach(Transform childTransform in selectionGrid.transform){
            Destroy(childTransform.gameObject);
        }

        Dictionary<string, MutableData_PetMenuInfo> petMenuInfoDict = DataManager.Instance.MenuSceneData;
        int numOfPets = DataManager.Instance.NumOfPets;
        for(int i = 0; i < numOfPets; i++){
            GameObject petSelectionGO = NGUITools.AddChild(selectionGrid, petSelectionPrefab);
            string petID = "Pet" + i;

            //If pet info data can't be found then it's not hatched yet
            bool isHatched = petMenuInfoDict.ContainsKey(petID);

            //assign id to NGUI GameObject
            petSelectionGO.name = petID; 

            //Turn show case animation on or off
            if(!isHatched){
                GameObject eggPrefab = Resources.Load("Sprite_Egg") as GameObject;
                GameObject eggGO = NGUITools.AddChild(petSelectionGO, eggPrefab);
                eggGO.name = "egg";
                eggGO.transform.localScale = eggPrefab.transform.localScale;

                eggGO.GetComponent<LgButtonMessage>().target = this.gameObject;
                eggGO.GetComponent<LgButtonMessage>().functionName = "PetSelected";

            }else{
                GameObject animatorPrefab = Resources.Load("Animator") as GameObject;
                GameObject animator = NGUITools.AddChild(petSelectionGO, animatorPrefab);
                animator.name = "animator";
                animator.transform.localScale = animatorPrefab.transform.localScale;

                animator.GetComponent<LgButtonMessage>().target = this.gameObject;
                animator.GetComponent<LgButtonMessage>().functionName = "PetSelected";
            }
        }

        selectionGrid.GetComponent<UIGrid>().Reposition();
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
        scriptTransition.StartTransition( SceneUtils.BEDROOM );
    }
}
