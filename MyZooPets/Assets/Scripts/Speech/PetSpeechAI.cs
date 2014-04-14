using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// PetSpeechAI
// Controls when the pet will speak
//---------------------------------------------------
public class PetSpeechAI : MonoBehaviour{
    private bool enableAutoSpeech = false;
    private float timer = 0; 
    private float timeBeforeSpeech = 15; //30 seconds interval

    void Awake(){
        //load pet speech from xml
    }

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
        msgOption.Add(PetSpeechController.Keys.ImageTextureName, "shopButtonItems");
        msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, 
            "OpenToSubCategoryItemsWithLockAndCallBack");
        GetComponent<PetSpeechController>().Talk(msgOption);
    }

    private void ShowHealthyToVerySickMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("HEALTHY_TO_VERYSICK_0"));
        msgOption.Add(PetSpeechController.Keys.ImageTextureName, "shopButtonItems");
        msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, 
            "OpenToSubCategoryItemsWithLockAndCallBack");
        GetComponent<PetSpeechController>().Talk(msgOption);

        enableAutoSpeech = true;
    }

}
