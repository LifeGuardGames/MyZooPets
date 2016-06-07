using UnityEngine;
using System.Collections;

public class MiniPetSpeechAI : MonoBehaviour{
	public void ShowTipMsg(){
		int rand = Random.Range (0, 4);
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_RETENTION_TIP_"+ rand.ToString() + "P1") + DataManager.Instance.GameData.PetInfo.PetName + Localization.Localize("MINIPET_RETENTION_TIP_" + rand.ToString() + "P2"));
		msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
		PetSpeechManager.Instance.TalkM(msgOption);
	}

	public void ShowIdleMessage(MiniPetTypes minipetType){
		int rand = Random.Range (0, 7);
		Hashtable msgOption = new Hashtable();

		switch(minipetType){
		case MiniPetTypes.Retention:
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_RETENTION_IDLE_"+ rand.ToString()));
			msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
			PetSpeechManager.Instance.TalkM(msgOption);
			break;
		case MiniPetTypes.GameMaster:
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_IDLE_"+ rand.ToString()));
			msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
			PetSpeechManager.Instance.TalkM(msgOption);
			break;
		case MiniPetTypes.Merchant:
			msgOption.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("MINIPET_MERCHANT_IDLE_"+ rand.ToString()));
			msgOption.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
			PetSpeechManager.Instance.TalkM(msgOption);
			break;
		default:
			Debug.LogWarning("Bad minipet type " + minipetType.ToString());
			break;
		}
	}

	public void ShowChallengeMsg(MinigameTypes type){
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

	public void ShowMerchantShopMessage(){
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
		PetSpeechManager.Instance.TalkM(msgOption);
	}
}
