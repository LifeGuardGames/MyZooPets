using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//This class handles all game data. No game logic
//Saves and loads data into player preference
[DoNotSerializePublic]
public class DataManager : Singleton<DataManager>{
    public bool removeDataOnApplicationQuit; //delete all from PlayerPrefs
    public bool isDebug = false; //turn isDebug to true if working on independent scene

    private static bool isCreated = false; //prevent DataManager from being loaded
                                            //again during scene change (needs to be static)
    private bool firstTime; //Is the user playing for the first time
    private int numOfPets; //Number of pets saved on this device
    private string currentPetID; //The id that will be used for pet serialization
    private bool loaded = false; //Has the data been deserialized

    [SerializeThis]
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

    void Awake(){
        MakePersistant();


        if(removeDataOnApplicationQuit){
            firstTime = true;
            numOfPets = 0;
            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.SetInt("NumOfPets", 0);
        }

        //debug for independent scene. only initialize data no serialization or scene loading
        if(!isDebug){
            //Check if the user is opening the game for the first time
            firstTime = PlayerPrefs.GetInt("FirstTime", 1) > 0;
            numOfPets = PlayerPrefs.GetInt("NumOfPets", 0);
        }
        else{
            currentPetID = "Pet0";
            InitializeDataForDebug();
        }
    }

    //LevelSerailizer.LoadSavedLevel needs to be called in Start()
    void Start(){
        if(!isDebug){
            if(firstTime && numOfPets == 0){ 
                //First time opening the game, so need to initialize the first 3 pets
                PlayerPrefs.SetString("Pet0", "Egg");
                PlayerPrefs.SetString("Pet1", "Egg");
                PlayerPrefs.SetString("Pet2", "Egg");

                //Turn off first time
                PlayerPrefs.SetInt("FirstTime", 0);

                //Reset num of pets
                numOfPets = 3;
                PlayerPrefs.SetInt("NumOfPets", 3);
            }
        }
    }

    //serialize the data whenever the game is paused
    void OnApplicationPause(bool paused){
        if(paused){
            // special case: when we are about to serialize the game, we have to cache the moment it happens so we know when the user stopped
            DataManager.Instance.GameData.Degradation.LastTimeUserPlayedGame = DateTime.Now;
            
            SerializePetData();
        }
    }

    //called when pet data has be deserialized and ready to be used
    void OnDeserialized(){
        Application.LoadLevel("NewBedRoom");
    }

    //"Egg" not born yet (needs to be initialize), "Hatch" borned
    public string GetPetStatus(string petID){
        string retVal;

        if(!isDebug)
            retVal = PlayerPrefs.GetString(petID);
        else
            retVal = "Egg";

        return retVal; 
    }

    public void InitializeDataForNewPet(){
        if(!String.IsNullOrEmpty(currentPetID)){
            PlayerPrefs.SetString(currentPetID, "Hatch");
            gameData = new PetGameData();
            gameData.Init();
        }else{
            Debug.LogError("PetID is null or empty. Can't initialize pet with ID. Check  currentPetID");
        }
    }

    //Load the data of the pet that has been chosen
    public void DeserializePetData(){
        if(!String.IsNullOrEmpty(currentPetID)){
            //Deserialize data
            if(!loaded){
                loaded = true;
                string data = PlayerPrefs.GetString(currentPetID + "GameData", "");
                if(!String.IsNullOrEmpty(data))
                    LevelSerializer.LoadSavedLevel(data);
            }
        }else{
            Debug.LogError("PetID is null or empty. Can't load pet without ID. Check  currentPetID");
        }
    }

    //serialize data into byte array and store locally in PlayerPrefs
    public void SerializePetData(){
        if(!String.IsNullOrEmpty(currentPetID)){
            PlayerPrefs.SetString(currentPetID + "GameData", LevelSerializer.SerializeLevel());

#if UNITY_EDITOR
        print(JSONLevelSerializer.SerializeLevel());
#endif
        }else{
            Debug.LogError("PetID is null or empty, so data cannot be serialized");
        }
    }

    private void MakePersistant(){
        if(isCreated) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        isCreated = true;
    }

#if UNITY_EDITOR
    private void InitializeDataForDebug(){
        InitializeDataForNewPet(); 
    }
#endif

    

}
