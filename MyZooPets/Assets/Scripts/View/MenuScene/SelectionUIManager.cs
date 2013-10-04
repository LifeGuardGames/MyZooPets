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
        print(numOfPets);
        for(int i = 0; i < numOfPets; i++){
            GameObject petSelectionGO = NGUITools.AddChild(selectionGrid, petSelectionPrefab);
            petSelectionGO.name = "Pet" + i;

            //Turn show case animation on or off
            string petStatus = DataManager.Instance.GetPetStatus(petSelectionGO.name);
            print(petStatus);
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
        print(selectedPetID);

        if(petStatus == "Egg"){
            //Open CustomizationUIManager to create/initiate new pet game data
            print("working");
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

    private void EnterGameAfterGameDataDeserialized(object sender, EventArgs args){
        print("what is going on");
        LoadScene();
        //Unregister itself from the event
        DataManager.Instance.OnGameDataLoaded -= EnterGameAfterGameDataDeserialized;
    }

    private void LoadScene(){
        GetComponent<SceneTransition>().StartTransition();
    }

    // //After the current pet game data have been serialized. Load the game data for selected pet
    // private void LoadGameDataForSelectedPet(object sender, EventArgs args){
    //     print("what's going on");

    //     string currentPetID = DataManager.Instance.CurrentPetID;

    //     //Deserialize data from selected pet
    //     DataManager.Instance.CurrentPetID = selectedPetID;
    //     DataManager.Instance.OnGameDataLoaded += EnterGameAfterGameDataDeserialized;
    //     DataManager.Instance.LoadGameData();

    //     //Unregister itself from OnGameDataSaved event
    //     DataManager.Instance.OnGameDataSaved -= LoadGameDataForSelectedPet;
    // }
}
