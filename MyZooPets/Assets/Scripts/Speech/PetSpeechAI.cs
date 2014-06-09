using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// PetSpeechAI
// Controls when the pet will speak
//---------------------------------------------------
public class PetSpeechAI : Singleton<PetSpeechAI>{
    private bool enableAutoSpeech = false;
    private float timer = 0; 
    private float timeBeforeSpeech = 15; //30 seconds interval

    void Start(){
        StatsController.OnHappyToSad += ShowHappyToSadMsg;
        StatsController.OnSadToHappy += ShowSadToHappyMsg;
        StatsController.OnSickToVerySick += ShowSickToVerySickMsg;

        StatsController.OnHealthyToVerySick += ShowHealthyToVerySickMsg;
        StatsController.OnHealthyToSick += ShowHealthyToVerySickMsg;

        StatsController.OnSickToHealthy += StopAutoSpeech;
        StatsController.OnVerySickToHealthy += StopAutoSpeech;
    }

    void OnDestroy(){
        StatsController.OnHappyToSad -= ShowHappyToSadMsg;
        StatsController.OnSadToHappy -= ShowSadToHappyMsg;
        StatsController.OnSickToVerySick -= ShowSickToVerySickMsg;

        StatsController.OnHealthyToVerySick -= ShowHealthyToVerySickMsg;
        StatsController.OnHealthyToSick -= ShowHealthyToVerySickMsg;

        StatsController.OnSickToHealthy -= StopAutoSpeech;
        StatsController.OnVerySickToHealthy -= StopAutoSpeech;
    }

    void Update(){
        if(enableAutoSpeech){
            timer += Time.deltaTime;
            if(timer > timeBeforeSpeech){

                //say sth here
                ShowHealthyToVerySickMsg(this, EventArgs.Empty);

                timer = 0;
            }
        }
    }

	public void ShowFireOrbMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("NO_FIRE_FIRE_ORB"));
		msgOption.Add(PetSpeechController.Keys.ImageTextureName, "itemFireOrb");
		msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, 
		              "OpenToSubCategoryItemsWithLockAndCallBack");
		GetComponent<PetSpeechController>().Talk(msgOption);
	}

	public void ShowInhalerMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("NO_FIRE_INHALER_0"));
		msgOption.Add(PetSpeechController.Keys.ImageTextureName, "itemInhalerMain");
		GetComponent<PetSpeechController>().Talk(msgOption);
	}

	public void ShowOutOfFireMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("NO_FIRE_INHALER_1"));
		msgOption.Add(PetSpeechController.Keys.ImageTextureName, "itemInhalerMain");
		GetComponent<PetSpeechController>().Talk(msgOption);
	}

	public void ShowNoFireSickMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("NO_FIRE_SICK"));
		msgOption.Add(PetSpeechController.Keys.ImageTextureName, "itemInhalerEmergency");
		msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, 
		              "OpenToSubCategoryItemsWithLockAndCallBack");
		GetComponent<PetSpeechController>().Talk(msgOption);
	}

	public void ShowNoFireHungryMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("NO_FIRE_HUNGRY"));
		msgOption.Add(PetSpeechController.Keys.ImageTextureName, "shopButtonFood");
		msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, 
		              "OpenToSubCategoryFoodWithLockAndCallBack");
		GetComponent<PetSpeechController>().Talk(msgOption);
	}
	
    private void StopAutoSpeech(object sender, EventArgs args){
        enableAutoSpeech = false;
        timer = 0;
    }

    private void ShowHappyToSadMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("HAPPY_TO_SAD_0"));
        msgOption.Add(PetSpeechController.Keys.ImageTextureName, "shopButtonFood");
        msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, 
            "OpenToSubCategoryFoodWithLockAndCallBack");
        GetComponent<PetSpeechController>().Talk(msgOption);
    }

    private void ShowSadToHappyMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechController.Keys.ImageTextureName, "speechImageHeart");
        GetComponent<PetSpeechController>().Talk(msgOption);
    }

    private void ShowSickToVerySickMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("SICK_TO_VERYSICK_0"));
        msgOption.Add(PetSpeechController.Keys.ImageTextureName, "itemInhalerEmergency");
        msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, 
            "OpenToSubCategoryItemsWithLockAndCallBack");
        GetComponent<PetSpeechController>().Talk(msgOption);
    }

    private void ShowHealthyToVerySickMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("HEALTHY_TO_VERYSICK_0"));
        msgOption.Add(PetSpeechController.Keys.ImageTextureName, "itemInhalerEmergency");
        msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, 
            "OpenToSubCategoryItemsWithLockAndCallBack");
        GetComponent<PetSpeechController>().Talk(msgOption);

        enableAutoSpeech = true;
    }

}
