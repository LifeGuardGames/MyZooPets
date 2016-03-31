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

	public void ShowItemNotHungryMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("ITEM_NOT_HUNGRY"));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);	
	}

	public void ShowSleepingMessageMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("PET_SLEEPING"));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);	
	}

	public void ShowSadMessage(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("TOO_SAD_TO_PLAY"));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowItemNoThanksMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("ITEM_NO_THANKS"));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);	
	}

	public void ShowFireOrbMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("NO_FIRE_FIRE_ORB"));
		msgOption.Add(PetSpeechManager.Keys.AtlasName, "BedroomAtlas");
		msgOption.Add(PetSpeechManager.Keys.ImageTextureName, "itemFireOrb");
		msgOption.Add(PetSpeechManager.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(PetSpeechManager.Keys.ImageClickFunctionName, 
		              "OpenToSubCategoryItemsWithLockAndCallBack");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowInhalerMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("NO_FIRE_INHALER_0"));
		msgOption.Add(PetSpeechManager.Keys.AtlasName, "BedroomAtlas");
		msgOption.Add(PetSpeechManager.Keys.ImageTextureName, "itemInhalerMain");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowOutOfFireMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("NO_FIRE_INHALER_1"));
		msgOption.Add(PetSpeechManager.Keys.AtlasName, "BedroomAtlas");
		msgOption.Add(PetSpeechManager.Keys.ImageTextureName, "progressFireCrystal");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowNoFireSickMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("NO_FIRE_SICK"));
		msgOption.Add(PetSpeechManager.Keys.AtlasName, "BedroomAtlas");
		msgOption.Add(PetSpeechManager.Keys.ImageTextureName, "itemInhalerEmergency");
		msgOption.Add(PetSpeechManager.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(PetSpeechManager.Keys.ImageClickFunctionName, 
		              "OpenToSubCategoryItemsWithLockAndCallBack");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowNoFireHungryMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("NO_FIRE_HUNGRY"));
		msgOption.Add(PetSpeechManager.Keys.ImageTextureName, "shopButtonFood");
		msgOption.Add(PetSpeechManager.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(PetSpeechManager.Keys.ImageClickFunctionName, 
		              "OpenToSubCategoryFoodWithLockAndCallBack");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}
	
    private void StopAutoSpeech(object sender, EventArgs args){
        enableAutoSpeech = false;
        timer = 0;
    }

    private void ShowHappyToSadMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("HAPPY_TO_SAD_0"));
        msgOption.Add(PetSpeechManager.Keys.ImageTextureName, "shopButtonFood");
        msgOption.Add(PetSpeechManager.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechManager.Keys.ImageClickFunctionName, 
            "OpenToSubCategoryFoodWithLockAndCallBack");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
    }

    private void ShowSadToHappyMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.AtlasName, "BedroomAtlas");
        msgOption.Add(PetSpeechManager.Keys.ImageTextureName, "speechImageHeart");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
    }

    private void ShowSickToVerySickMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("SICK_TO_VERYSICK_0"));
		msgOption.Add(PetSpeechManager.Keys.AtlasName, "BedroomAtlas");
        msgOption.Add(PetSpeechManager.Keys.ImageTextureName, "itemInhalerEmergency");
        msgOption.Add(PetSpeechManager.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechManager.Keys.ImageClickFunctionName, 
            "OpenToSubCategoryItemsWithLockAndCallBack");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
    }

    private void ShowHealthyToVerySickMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("HEALTHY_TO_VERYSICK_0"));
		msgOption.Add(PetSpeechManager.Keys.AtlasName, "BedroomAtlas");
        msgOption.Add(PetSpeechManager.Keys.ImageTextureName, "itemInhalerEmergency");
        msgOption.Add(PetSpeechManager.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechManager.Keys.ImageClickFunctionName, 
            "OpenToSubCategoryItemsWithLockAndCallBack");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);

        enableAutoSpeech = true;
    }
}
