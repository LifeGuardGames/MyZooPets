using UnityEngine;
using System.Collections;
using System;

public class SelectionUIManager : Singleton<SelectionUIManager> {
    public GameObject selectionGrid;
    public GameObject petSelectionPrefab;

    private string selectedPetID;

	// Use this for initialization
	void Start () {
        InitializeSelection();	
	}
    
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

    public void PetSelected(GameObject selectedPetGO){
        selectedPetID = selectedPetGO.transform.parent.name;
        string petStatus = DataManager.Instance.GetPetStatus(selectedPetID);
        if(petStatus == "Egg"){
            //call open customization
            print("working");
            CustomizationUIManager.Instance.OpenUI();
        }else{
            //Load game

            //check if serialization is required. serialize if petID != currentPetID
            string currentPetID = DataManager.Instance.CurrentPetID;
            if(String.IsNullOrEmpty(currentPetID)){
                //Nothing has been selected before, so load data right away
                currentPetID = selectedPetID;

                DataManager.Instance.OnDeserialized += EnterGameAfterGameDataDeserialized;
                DataManager.Instance.DeserializeGameData();
            }else{
                if(currentPetID != selectedPetID){
                    //Switching from one pet to another. Need to serialize current pet data 
                    //before loading the data for the selected pet
                    DataManager.Instance.OnSerialized += LoadGameDataForSelectedPet;
                    DataManager.Instance.SerializeGameData();
                }
            }
        }
    }

    private void EnterGameAfterGameDataDeserialized(object sender, EventArgs args){
        DataManager.Instance.OnDeserialized -= EnterGameAfterGameDataDeserialized;

        Application.LoadLevel("NewBedRoom");
    }

    //After the current pet game data have been serialized. Load the game data for selected pet
    private void LoadGameDataForSelectedPet(object sender, EventArgs args){
        DataManager.Instance.OnSerialized -= LoadGameDataForSelectedPet;
        string currentPetID = DataManager.Instance.CurrentPetID;

        currentPetID = selectedPetID;
        DataManager.Instance.OnDeserialized += EnterGameAfterGameDataDeserialized;
        DataManager.Instance.DeserializeGameData();
    }
}
