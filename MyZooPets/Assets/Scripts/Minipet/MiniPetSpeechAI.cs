using UnityEngine;
using System.Collections;

public class MiniPetSpeechAI : MonoBehaviour{
	
	public void ShowDirtyMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		GetComponent<PetSpeechController>().Talk(msgOption);
	}

	public void ShowSadMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("MINIPET_SAD_MSG"));
		GetComponent<PetSpeechController>().Talk(msgOption);
	}

	public void ShowMaxLevelMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("MINIPET_MAX_LEVEL_MSG"));
		GetComponent<PetSpeechController>().Talk(msgOption);
	}

	public void ShowFoodPreferenceMsg(string itemTextureName){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechController.Keys.AtlasName, "ItemAtlas");
		msgOption.Add(PetSpeechController.Keys.BubbleSpriteName, "box20");
		msgOption.Add(PetSpeechController.Keys.ImageTextureName, itemTextureName);
		msgOption.Add(PetSpeechController.Keys.MessageText, "Feed me this!!");
		GetComponent<PetSpeechController>().Talk(msgOption);
	}
}
