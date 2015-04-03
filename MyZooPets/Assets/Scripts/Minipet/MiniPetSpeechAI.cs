using UnityEngine;
using System.Collections;

public class MiniPetSpeechAI : MonoBehaviour{
	
	public void ShowDirtyMsg(string pet){
		Hashtable msgOption = new Hashtable();
		if(pet == "general"){
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		}
		else if (pet == "rentention"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		}
		else if (pet == "gameMaster"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		}
		else if (pet == "merch"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		}
		else if (pet == "warrior"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		}
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowSadMsg(string pet){
		Hashtable msgOption = new Hashtable();
		if(pet == "general"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		}
		else if (pet == "rentention"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		}
		else if (pet == "gameMaster"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		}
		else if (pet == "merch"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		}
		else if (pet == "warrior"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
		}
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowMaxLevelMsg(string pet){
		Hashtable msgOption = new Hashtable();
		if(pet == "general"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_MAX_LEVEL_MSG"));
		}
		else if (pet == "rentention"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_MAX_LEVEL_MSG"));
		}
		else if (pet == "gameMaster"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_MAX_LEVEL_MSG"));
		}
		else if (pet == "merch"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_MAX_LEVEL_MSG"));
		}
		else if (pet == "warrior"){
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_MAX_LEVEL_MSG"));
		}
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.Talk(msgOption);
	}

	public void ShowTipMsg(){
		int rand = Random.Range (0, 4);
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_RETENTION_TIP_"+ rand.ToString()));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.TalkM(msgOption);
	}

	public void ShowGMIdleMsg(){
		int rand = Random.Range (0, 7);
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_IDLE_"+ rand.ToString()));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.TalkM(msgOption);
	}
	public void ShowRetentionIdelMsg(){
		int rand = Random.Range (0, 7);
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_RETENTION_IDLE_"+ rand.ToString()));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.TalkM(msgOption);
	}

	public void ShowMerchantIdleMsg(){
		int rand = Random.Range (0, 7);
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_MERCHANT_IDLE_"+ rand.ToString()));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.TalkM(msgOption);
	}

	public void showChallengeMsg(MinigameTypes type){
		Hashtable msgOption = new Hashtable();
		switch(type){
		case MinigameTypes.TriggerNinja:
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_NINJA"));
			break;
		case MinigameTypes.Clinic:
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_DOC"));
			break;
		case MinigameTypes.Memory:
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_MEMORY"));
			break;
		case MinigameTypes.Runner:
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_RUNNER"));
			break;
		case MinigameTypes.Shooter:
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_SHOOTER"));
			break;
		}
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.TalkM(msgOption);
	}

	public void showBlackShopMessage(){
		int rand = Random.Range (0, 5);
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_MERCHANT_" + rand.ToString()));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.TalkM(msgOption);
	}

	public void ShowFoodPreferenceMsg(string itemTextureName){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.AtlasName, "ItemAtlas");
		msgOption.Add(PetSpeechManager.Keys.ImageTextureName, itemTextureName);
		msgOption.Add(PetSpeechManager.Keys.MessageText, "Feed me this!!");
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);

		msgOption.Add(PetSpeechManager.Keys.ImageButtonModeType, UIModeTypes.MiniPet);
		msgOption.Add(PetSpeechManager.Keys.ImageClickTarget, MiniPetHUDUIManager.Instance.gameObject);
		msgOption.Add(PetSpeechManager.Keys.ImageClickFunctionName, "OpenShop");
		PetSpeechManager.Instance.Talk(msgOption);
	}
}
