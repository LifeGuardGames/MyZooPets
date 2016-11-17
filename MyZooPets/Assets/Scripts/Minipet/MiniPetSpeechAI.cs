using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpeechController))]
public class MiniPetSpeechAI : MonoBehaviour {
	public SpeechController speechController;
	public TweenToggle speechCanvasTween;

	public void PetSpeechZoomToggle(bool isZoomed) {
		if(isZoomed) {
			speechCanvasTween.Hide();
		}
		else {
			speechCanvasTween.Show();
		}
	}

	public void BeQuiet() {
		speechController.BeQuiet();
    }

	public void ShowTipMsg() {
		int rand = Random.Range(0, 4);
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, string.Format(Localization.Localize("MINIPET_RETENTION_TIP_" + rand.ToString()), DataManager.Instance.GameData.PetInfo.PetName));
		speechController.TalkMiniPet(msgOption);
	}

	public void ShowIdleMessage(MiniPetTypes minipetType) {
		int rand = Random.Range(0, 7);
		Hashtable msgOption = new Hashtable();

		switch(minipetType) {
			case MiniPetTypes.Retention:
				msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("MINIPET_RETENTION_IDLE_" + rand.ToString()));
				speechController.TalkMiniPet(msgOption);
				break;
			case MiniPetTypes.GameMaster:
				msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_IDLE_" + rand.ToString()));
				speechController.TalkMiniPet(msgOption);
				break;
			case MiniPetTypes.Merchant:
				msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("MINIPET_MERCHANT_IDLE_" + rand.ToString()));
				speechController.TalkMiniPet(msgOption);
				break;
			default:
				Debug.LogWarning("Bad minipet type " + minipetType.ToString());
				break;
		}
	}

	public void ShowChallengeMsg(MinigameTypes type) {
		Hashtable msgOption = new Hashtable();
		switch(type) {
			case MinigameTypes.TriggerNinja:
				msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_NINJA"));
				break;
			case MinigameTypes.Clinic:
				msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_DOC"));
				break;
			case MinigameTypes.Memory:
				msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_MEMORY"));
				break;
			case MinigameTypes.Runner:
				msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_RUNNER"));
				break;
			case MinigameTypes.Shooter:
				msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("MINIPET_GAMEMASTER_SHOOTER"));
				break;
		}
		speechController.TalkMiniPet(msgOption);
	}

	public void ShowMerchantShopMessage() {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("MINIPET_MERCHANT_" + Random.Range(0, 5).ToString()));
		speechController.TalkMiniPet(msgOption);
	}

	public void ShowFoodPreferenceMsg(string itemTextureName) {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.ImageTextureName, itemTextureName);
		msgOption.Add(SpeechController.SpeechKeys.MessageText, "Feed me this!!");

		msgOption.Add(SpeechController.SpeechKeys.ImageButtonModeType, UIModeTypes.MiniPet);
		msgOption.Add(SpeechController.SpeechKeys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(SpeechController.SpeechKeys.ImageClickFunctionName, "OpenToSubCategoryFoodWithLockAndCallBack");
		speechController.TalkMiniPet(msgOption);
	}
}
