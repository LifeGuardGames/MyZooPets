using UnityEngine;
using System.Collections;
using System;

public class SelectionUIManager : Singleton<SelectionUIManager> {
    public GameObject selectionGrid;
    public GameObject petSelectionPrefab; //Prefab that holds the basic structure for pet display layout
    public GameObject spotLight; //spotlight to shine on the egg when chosen	
	public SceneTransition scriptTransition; //transition
	
    private string selectedPetID;

	// Use this for initialization
	void Start () {
        InitializeSelection();	
	}

    //---------------------------------------------------
    // PetSelected()
    // Call when select button is clicked. Decides to start 
    // a new pet or load existing game data
    //---------------------------------------------------
    public void PetSelected(GameObject selectedPetGO){
        selectedPetID = selectedPetGO.transform.parent.name;
        string petStatus = DataManager.Instance.GetPetStatus(selectedPetID);

        ToggleEggAnimation(false);
        ToggleSpotLight(true, selectedPetGO);

        if(petStatus == "Egg"){
            //Open CustomizationUIManager to create/initiate new pet game data
            DataManager.Instance.CurrentPetID = selectedPetID;
            CustomizationUIManager.Instance.selectedEgg = selectedPetGO.transform.parent.Find("egg").gameObject;
            CustomizationUIManager.Instance.OpenUI();
        }else{
            //Load game data only if the selected pet is different from the current pet
            if(DataManager.Instance.CurrentPetID != selectedPetID){
                DataManager.Instance.CurrentPetID = selectedPetID;
                DataManager.Instance.OnGameDataLoaded += EnterGameAfterGameDataDeserialized;
                DataManager.Instance.LoadGameData();
            }else{
                LoadScene();
            }
        }
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
        foreach(Transform child in selectionGrid.transform){
            string petStatus = DataManager.Instance.GetPetStatus(child.name);

            if(petStatus == "Egg"){
                Transform egg = child.Find("egg");
                if(isOn)
                    egg.GetComponent<RandomAnimation>().Enable();
                else
                    egg.GetComponent<RandomAnimation>().Disable();
            }
        }
    }
    
    //First initialization of the PetSelectionArea
    private void InitializeSelection(){
        int numOfPets = DataManager.Instance.NumOfPets;
        for(int i = 0; i < numOfPets; i++){
            GameObject petSelectionGO = NGUITools.AddChild(selectionGrid, petSelectionPrefab);
            petSelectionGO.name = "Pet" + i;

            //Turn show case animation on or off
            string petStatus = DataManager.Instance.GetPetStatus(petSelectionGO.name);
            if(petStatus == "Egg"){
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
    private void EnterGameAfterGameDataDeserialized(object sender, EventArgs args){
        LoadScene();
        
        //Unregister itself from the event
        DataManager.Instance.OnGameDataLoaded -= EnterGameAfterGameDataDeserialized;
    }

    private void LoadScene(){
        scriptTransition.StartTransition( SceneUtils.BEDROOM );
    }
}
