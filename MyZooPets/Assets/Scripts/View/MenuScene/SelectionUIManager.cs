using UnityEngine;
using System.Collections;
using System;

public class SelectionUIManager : Singleton<SelectionUIManager> {
    public GameObject selectionGrid;
    public GameObject petSelectionPrefab;
	
	// transition
	public SceneTransition scriptTransition;
	
    private string selectedPetID;

	// Use this for initialization
	void Start () {
        InitializeSelection();	
	}

    //Call when select button is clicked. Decides to start a new pet or load existing game data
    public void PetSelected(GameObject selectedPetGO){
        selectedPetID = selectedPetGO.transform.parent.name;
        string petStatus = DataManager.Instance.GetPetStatus(selectedPetID);

        if(petStatus == "Egg"){
            //Open CustomizationUIManager to create/initiate new pet game data
            DataManager.Instance.CurrentPetID = selectedPetID;
            CustomizationUIManager.Instance.selectedEgg = selectedPetGO.transform.parent.Find("Sprite_Egg").gameObject;
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
    
    //First initialization of the PetSelectionArea
    private void InitializeSelection(){
        int numOfPets = DataManager.Instance.NumOfPets;
        for(int i = 0; i < numOfPets; i++){
            GameObject petSelectionGO = NGUITools.AddChild(selectionGrid, petSelectionPrefab);
            petSelectionGO.name = "Pet" + i;

            //Turn show case animation on or off
            string petStatus = DataManager.Instance.GetPetStatus(petSelectionGO.name);
            if(petStatus == "Egg"){
                petSelectionGO.transform.Find("Animator").gameObject.SetActive(false);
                petSelectionGO.transform.Find("Sprite_Egg").gameObject.SetActive(true);
            }else{
                petSelectionGO.transform.Find("Animator").gameObject.SetActive(true);
                petSelectionGO.transform.Find("Sprite_Egg").gameObject.SetActive(false);
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
