using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------
// PetSpeechAI
// Controls when the pet will speak
//---------------------------------------------------
public class PetSpeechAI : MonoBehaviour{


    void Awake(){
        //load pet speech from xml
    }

    void Start(){
        StatsController.OnHappyToSad += ShowHappyToSadMsg;
        StatsController.OnSadToHappy += ShowSadToHappyMsg;
        StatsController.OnSickToVerySick += ShowSickToVerySickMsg;
        StatsController.OnHealthyToVerySick += ShowHealthyToVerySickMsg;
    }

    void OnDestory(){
        StatsController.OnHappyToSad -= ShowHappyToSadMsg;
        StatsController.OnSadToHappy -= ShowSadToHappyMsg;
        StatsController.OnSickToVerySick -= ShowSickToVerySickMsg;
        StatsController.OnHealthyToVerySick -= ShowHealthyToVerySickMsg;
    }

    public void ShowHappyToSadMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("HAPPY_TO_SAD_0"));
        msgOption.Add(PetSpeechController.Keys.ImageTextureName, "shopButtonFood");
        msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, "OpenToSubCategoryFood");
        GetComponent<PetSpeechController>().Talk(msgOption);
    }

    public void ShowSadToHappyMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechController.Keys.ImageTextureName, "speechImageHeart");
        GetComponent<PetSpeechController>().Talk(msgOption);
    }

    public void ShowSickToVerySickMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("SICK_TO_VERYSICK_0"));
        msgOption.Add(PetSpeechController.Keys.ImageTextureName, "shopButtonItems");
        msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, "OpenToSubCategoryItems");
        GetComponent<PetSpeechController>().Talk(msgOption);
    }

    public void ShowHealthyToVerySickMsg(object sender, EventArgs args){
        Hashtable msgOption = new Hashtable();
        msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("HEALTHY_TO_VERYSICK_0"));
        msgOption.Add(PetSpeechController.Keys.ImageTextureName, "shopButtonItems");
        msgOption.Add(PetSpeechController.Keys.ImageClickTarget, StoreUIManager.Instance.gameObject);
        msgOption.Add(PetSpeechController.Keys.ImageClickFunctionName, "OpenToSubCategoryItems");
        GetComponent<PetSpeechController>().Talk(msgOption);
    }

}
