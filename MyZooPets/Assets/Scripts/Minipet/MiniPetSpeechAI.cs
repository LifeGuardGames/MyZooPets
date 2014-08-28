using UnityEngine;
using System.Collections;

public class MiniPetSpeechAI : MonoBehaviour{
	
	public void ShowDirtyMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowSadMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_SAD_MSG"));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowMaxLevelMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_MAX_LEVEL_MSG"));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowFoodPreferenceMsg(string itemTextureName){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.AtlasName, "ItemAtlas");
		msgOption.Add(PetSpeechManager.Keys.ImageTextureName, itemTextureName);
		msgOption.Add(PetSpeechManager.Keys.MessageText, "Feed me this!!");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);

		msgOption.Add(PetSpeechManager.Keys.ImageButtonModeType, UIModeTypes.MiniPet);
		msgOption.Add(PetSpeechManager.Keys.ImageClickTarget, MiniPetHUDUIManager.Instance.gameObject);
		msgOption.Add(PetSpeechManager.Keys.ImageClickFunctionName, 
		              "OpenShop");
		PetSpeechManager.Instance.Talk(msgOption);
	}
}
