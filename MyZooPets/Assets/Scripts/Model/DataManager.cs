using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using fastJSON;

//This class handles all game data. No game logic
//Saves and loads data into player preference
public class DataManager : Singleton<DataManager>{
    public event EventHandler<EventArgs> OnGameDataLoaded;
    public event EventHandler<EventArgs> OnGameDataSaved;

    public bool removeDataOnApplicationQuit; //delete all from PlayerPrefs
    public bool isDebug = false; //turn isDebug to true if working on independent scene
    public string currentPetID; //The id that will be used for pet serialization

    private static bool isCreated = false; //prevent DataManager from being loaded
                                            //again during scene change (needs to be static)
    private bool firstTime; //Is the user playing for the first time
    private int numOfPets; //Number of pets saved on this device
    private PetGameData gameData; //Super class that stores all the game data related to a specific petID

    public PetGameData GameData{
        get{return gameData;}
    }

    public string CurrentPetID{
        get{return currentPetID;}
        set{currentPetID = value;}
    }

    public int NumOfPets{
        get{return numOfPets;}
    }
    
    //Save temporary data when transitioning to new scene
    public LoadSceneData SceneData{get; set;} 

    //TO Do: hash the pin before saving
    public string ParentPortalPin{
        get{
            return PlayerPrefs.GetString("ParentPortalPin", "");
        }

        set{
            PlayerPrefs.SetString("ParentPortalPin", value);
        }
    }

    public string ParentEmail{
        get{
            return PlayerPrefs.GetString("ParentEmail", "");
        }

        set{
            PlayerPrefs.SetString("ParentEmail", value);
        }
    }

    void Awake(){
        //Make Object persistent
        if(isCreated){
            //If There is a duplicate in the scene. delete the object and jump Awake
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        isCreated = true;

        //Use for Demo or Debug to remove and reset ALL GAME DATA
        if(removeDataOnApplicationQuit){
            firstTime = true;
            numOfPets = 0;
            PlayerPrefs.DeleteKey("FirstTime");
            PlayerPrefs.DeleteKey("NumOfPets");
        }

        //Retrieve data from PlayerPrefs
        firstTime = PlayerPrefs.GetInt("FirstTime", 1) > 0;
        numOfPets = PlayerPrefs.GetInt("NumOfPets", 0);


        //First time opening the game, so need to initialize the first 3 pets
        if(firstTime && numOfPets == 0){ 
            InitDataForFirstTimeInstall();
        }

        //Use this when developing on an independent scene. Will initialize all the data
        //before other classes call DataManager
        if(isDebug){
            currentPetID = "Pet0";
            InitializeGameDataForNewPet("BasicOrangeYellow");
        }
    }

    //----------------------------------------------------
    // InitDataForFirstTimeInstall()
    // 
    //----------------------------------------------------
    private void InitDataForFirstTimeInstall(){
        PlayerPrefs.SetString("Pet0_PetStatus", "Egg");
        PlayerPrefs.SetString("Pet1_PetStatus", "Egg");
        PlayerPrefs.SetString("Pet2_PetStatus", "Egg");

        //Turn off first time
        firstTime = false;
        PlayerPrefs.SetInt("FirstTime", 0);

        //Reset num of pets
        numOfPets = 3;
        PlayerPrefs.SetInt("NumOfPets", 3);
    }

    //This function gets called before the script variables are ready so don't try
    //to use class variables here
    void OnLevelWasLoaded(){

        /*
            Game data should be serialized whenever user return to MenuScene because
            from the MenuScene user can switch to a different pet, so the previous game
            data need to be saved
        */
        if(Application.loadedLevelName == "MenuScene"){
            PlayerPrefs.SetString("CanSaveData", "Yes");
        }
    }

    void Update(){
        string loadedLevelName = Application.loadedLevelName;

        if(loadedLevelName == "MenuScene"){
            string canSaveData = PlayerPrefs.GetString("CanSaveData", "");

            //Serialize current pet data when loading into MenuScene
            if(!firstTime && canSaveData == "Yes"){
                SaveGameData();
                PlayerPrefs.SetString("CanSaveData", "No");
            }
        }
    }

    //Serialize the data whenever the game is paused
    void OnApplicationPause(bool paused){
        if(paused){
			// check immediately if a tutorial is playing...if one is, we don't want to save the game on pause
			if ( TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive() ) {
				Debug.Log("Auto save canceled because we are in a tutorial");
				return;
			}
			
            string loadedLevelName = Application.loadedLevelName;
            if(loadedLevelName != "MenuScene" && loadedLevelName != "LoadScene"){
                // special case: when we are about to serialize the game, we have to cache the moment it happens so we know when the user stopped
                DataManager.Instance.GameData.Degradation.LastTimeUserPlayedGame = DateTime.Now;
                
                SaveGameData();
            }
        }
    }

     //"Egg" not born yet (needs to be initialize), "Hatch" borned
    public string GetPetStatus(string petID){
        return PlayerPrefs.GetString(petID + "_PetStatus");
    }

    //Return the species and the color of the pet so the selection
    //pet animation can be displayed correctly
    public string GetPetSpeciesColor(string petID){
        return PlayerPrefs.GetString(petID + "_SpeciesColor");
    }

    //Initalize New PetGameData
    public void InitializeGameDataForNewPet(string speciesColor){
        if(!String.IsNullOrEmpty(currentPetID)){

            //Set basic pet data that will be used in MenuScene
            PlayerPrefs.SetString(currentPetID + "_PetStatus", "Hatch");
            PlayerPrefs.SetString(currentPetID + "_SpeciesColor", speciesColor);

            gameData = new PetGameData();
            gameData.Init();
        }else{
            Debug.LogError("PetID is null or empty. Can't initialize pet with ID. Check  currentPetID");
        }
    }

    //Load the data of the pet that has been chosen
    public void LoadGameData(){
        if(!String.IsNullOrEmpty(currentPetID)){
            PetGameData newGameData = null;

            string jsonString = PlayerPrefs.GetString(currentPetID + "GameData", "");
            if(!String.IsNullOrEmpty(jsonString)){
                newGameData = JSON.Instance.ToObject<PetGameData>(jsonString);
                gameData = newGameData;
                Deserialized();
            }
        }else{
            Debug.LogError("PetID is null or empty. Can't load pet without ID. Check  currentPetID");
        }
    }

    //serialize data into byte array and store locally in PlayerPrefs
    public void SaveGameData(){
        // hopefully we are safe here, but do an absolute check if a tutorial is playing
        if ( TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive() ) {
            Debug.Log("Something trying to save the game while a tutorial is playing.  Investigate.");
            return;
        }
        
        Debug.Log("Game is saving");
        
        //JSON serializer setting
        JSON.Instance.Parameters.UseExtensions = false;

        if(!String.IsNullOrEmpty(currentPetID)){
            string jsonString = JSON.Instance.ToJSON(gameData);

#if UNITY_EDITOR
    Debug.Log(jsonString);
#endif

            PlayerPrefs.SetString(currentPetID + "GameData", jsonString); 
            Serialized();

        }else{
            Debug.LogError("PetID is null or empty, so data cannot be serialized");
        }
    }

    //called when game data has be deserialized and ready to be used
    private void Deserialized(){
        Debug.Log("Deserialized");
        if(OnGameDataLoaded != null)
            OnGameDataLoaded(this, EventArgs.Empty);
    }

    //called when game data has been serialized
    private void Serialized(){
        Debug.Log("Serialized");
        if(OnGameDataSaved != null)
            OnGameDataSaved(this, EventArgs.Empty);
    }
}
