using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectionUIManager : Singleton<SelectionUIManager> {
    public GameObject spotLight; //spotlight to shine on the egg when chosen	
    public GameObject petSelectionOption; //reference to UI element 
    public GameObject selectionGrid;
	public SceneTransition scriptTransition; //transition
	
    private string selectedPetID;

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
        Dictionary<string, MutableData_PetMenuInfo> petMenuInfoDict = SelectionManager.Instance.PetMenuInfo;
        selectedPetID = selectedPetGO.transform.parent.name;
        bool isHatched = petMenuInfoDict.ContainsKey(selectedPetID);

        //probably shoudn't use spot light right away. should toggle spot light
        //after some logic check for the data
        ToggleEggAnimation(false);
        ToggleSpotLight(true, selectedPetGO);

        if(!isHatched){
            HideSelectionOption();

            //Open CustomizationUIManager to create/initiate new pet game data
            SelectionManager.Instance.CurrentPetID = selectedPetID;
            CustomizationUIManager.Instance.selectedEgg = selectedPetGO;
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

        //Lock it while loading
        ClickManager.Instance.Lock(UIModeTypes.IntroComic);

        //Load game data only if the selected pet is different from the current pet
        if(SelectionManager.Instance.CurrentPetID != selectedPetID){
            SelectionManager.Instance.CurrentPetID = selectedPetID;
            DataManager.Instance.OnGameDataLoaded += EnterGameAfterGameDataDeserialized;
            SelectionManager.Instance.LoadPetGameData();
        }else{
            if(SelectionManager.Instance.IsGameDataLoaded)
                LoadScene();
        }
    }

    public void DeleteGameData(){
        //need to do double confirmation first
        PopupNotificationNGUI.HashEntry button1Function = delegate(){
            HideSelectionOption();

            //Delete game data 
            SelectionManager.Instance.RemovePetData(selectedPetID);

            //Update UI
            ToggleSpotLight(false);
            RefreshUI();
        };

        PopupNotificationNGUI.HashEntry button2Function = delegate(){
        };

        Dictionary<string, MutableData_PetMenuInfo> petMenuInfoDict = SelectionManager.Instance.PetMenuInfo;
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
            //offset is hardcoded
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
        Dictionary<string, MutableData_PetMenuInfo> petMenuInfoDict = SelectionManager.Instance.PetMenuInfo;

        foreach(Transform child in selectionGrid.transform){
            string petID = child.name;
            bool isHatched = petMenuInfoDict.ContainsKey(petID);

            if(!isHatched){
                Transform eggParent = child.Find("MenuSceneEgg/SpriteGrandparent/SpriteParent (Animation)");
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
        foreach(Transform petSelectionTransform in selectionGrid.transform){
            foreach(Transform childTransform in petSelectionTransform){
                childTransform.gameObject.SetActive(false);
                Destroy(childTransform.gameObject);
            }
        }

        Dictionary<string, MutableData_PetMenuInfo> petMenuInfoDict = SelectionManager.Instance.PetMenuInfo;

        foreach(Transform petSelectionTransform  in selectionGrid.transform){
            GameObject petSelectionGO = petSelectionTransform.gameObject;
            string petID = petSelectionTransform.name;
            bool isHatched = petMenuInfoDict.ContainsKey(petID);

            //Turn show case animation on or off
            if(!isHatched){
                GameObject menuSceneEggPrefab = Resources.Load("MenuSceneEgg") as GameObject;
                GameObject menuSceneEggGO = NGUITools.AddChild(petSelectionGO, menuSceneEggPrefab);

                menuSceneEggGO.name = "MenuSceneEgg";
                menuSceneEggGO.transform.localScale = menuSceneEggPrefab.transform.localScale;
                menuSceneEggGO.GetComponent<LgButtonMessage>().target = this.gameObject;
                menuSceneEggGO.GetComponent<LgButtonMessage>().functionName = "PetSelected";

                if(petID == "Pet1"){
                    menuSceneEggGO.transform.Find("SpriteGrandparent/SpriteParent (Animation)/Sprite").GetComponent<UISprite>().spriteName = "eggPurpleLime";
                }

            }else{
                GameObject menuScenePetPrefab = Resources.Load("MenuScenePet") as GameObject;
                GameObject menuScenePetGO = NGUITools.AddChild(petSelectionGO, menuScenePetPrefab);
                // GameObject lwfObject = menuScenePetGO.transform.Find("UILWFObject").gameObject;

                menuScenePetGO.name = "MenuScenePet";
                UILabel petNameLabel = menuScenePetGO.transform.Find("Label_PetName").GetComponent<UILabel>();
                petNameLabel.text = petMenuInfoDict[petID].PetName;

                // lwfObject.transform.localScale = menuScenePetPrefab.transform.localScale;
                menuScenePetGO.GetComponent<LgButtonMessage>().target = this.gameObject;
                menuScenePetGO.GetComponent<LgButtonMessage>().functionName = "PetSelected";
            }
        }
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
