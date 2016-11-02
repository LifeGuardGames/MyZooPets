using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(SpeechController))]
public class PetSpeechAI : Singleton<PetSpeechAI> {
	public SpeechController speechController;

	private bool enableAutoSpeech = false;
	private float timer = 0;
	private float timeBeforeSpeech = 15; //30 seconds interval

	void Start() {
		StatsManager.OnHappyToSad += ShowHappyToSadMsg;
		StatsManager.OnSadToHappy += ShowSadToHappyMsg;
		StatsManager.OnSickToVerySick += ShowSickToVerySickMsg;

		StatsManager.OnHealthyToVerySick += ShowHealthyToVerySickMsg;
		StatsManager.OnHealthyToSick += ShowHealthyToVerySickMsg;

		StatsManager.OnSickToHealthy += StopAutoSpeech;
		StatsManager.OnVerySickToHealthy += StopAutoSpeech;
	}

	void OnDestroy() {
		StatsManager.OnHappyToSad -= ShowHappyToSadMsg;
		StatsManager.OnSadToHappy -= ShowSadToHappyMsg;
		StatsManager.OnSickToVerySick -= ShowSickToVerySickMsg;

		StatsManager.OnHealthyToVerySick -= ShowHealthyToVerySickMsg;
		StatsManager.OnHealthyToSick -= ShowHealthyToVerySickMsg;

		StatsManager.OnSickToHealthy -= StopAutoSpeech;
		StatsManager.OnVerySickToHealthy -= StopAutoSpeech;
	}

	void Update() {
		if(enableAutoSpeech) {
			timer += Time.deltaTime;
			if(timer > timeBeforeSpeech) {

				//say sth here
				ShowHealthyToVerySickMsg(this, EventArgs.Empty);

				timer = 0;
			}
		}
	}

	public void ShowItemNotHungryMsg() {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("ITEM_NOT_HUNGRY"));
		speechController.Talk(msgOption);
	}

	public void ShowSleepingMessageMsg() {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("PET_SLEEPING"));
		speechController.Talk(msgOption);
	}

	public void ShowSadMessage() {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("TOO_SAD_TO_PLAY"));
		speechController.Talk(msgOption);
	}

	public void ShowItemNoThanksMsg() {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("ITEM_NO_THANKS"));
		speechController.Talk(msgOption);
	}

	public void ShowFireOrbMsg() {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("NO_FIRE_FIRE_ORB"));
		msgOption.Add(SpeechController.SpeechKeys.ImageTextureName, "itemFireOrb");
		msgOption.Add(SpeechController.SpeechKeys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(SpeechController.SpeechKeys.ImageClickFunctionName, "OpenToSubCategoryItemsWithLockAndCallBack");
		speechController.Talk(msgOption);
	}

	public void ShowInhalerMsg() {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("NO_FIRE_INHALER_0"));
		msgOption.Add(SpeechController.SpeechKeys.ImageTextureName, "itemInhalerMain");
		speechController.Talk(msgOption);
	}

	public void ShowOutOfFireMsg() {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("NO_FIRE_INHALER_1"));
		msgOption.Add(SpeechController.SpeechKeys.ImageTextureName, "progressFireCrystal");
		speechController.Talk(msgOption);
	}

	public void ShowNoFireSickMsg() {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("NO_FIRE_SICK"));
		msgOption.Add(SpeechController.SpeechKeys.ImageTextureName, "itemInhalerEmergency");
		msgOption.Add(SpeechController.SpeechKeys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(SpeechController.SpeechKeys.ImageClickFunctionName, "OpenToSubCategoryItemsWithLockAndCallBack");
		speechController.Talk(msgOption);
	}

	public void ShowNoFireHungryMsg() {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("NO_FIRE_HUNGRY"));
		msgOption.Add(SpeechController.SpeechKeys.ImageTextureName, "shopButtonFood");
		msgOption.Add(SpeechController.SpeechKeys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(SpeechController.SpeechKeys.ImageClickFunctionName, "OpenToSubCategoryFoodWithLockAndCallBack");
		speechController.Talk(msgOption);
	}

	private void StopAutoSpeech(object sender, EventArgs args) {
		enableAutoSpeech = false;
		timer = 0;
	}

	private void ShowHappyToSadMsg(object sender, EventArgs args) {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("HAPPY_TO_SAD_0"));
		msgOption.Add(SpeechController.SpeechKeys.ImageTextureName, "shopButtonFood");
		msgOption.Add(SpeechController.SpeechKeys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(SpeechController.SpeechKeys.ImageClickFunctionName, "OpenToSubCategoryFoodWithLockAndCallBack");
		speechController.Talk(msgOption);
	}

	private void ShowSadToHappyMsg(object sender, EventArgs args) {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.ImageTextureName, "speechImageHeart");
		speechController.Talk(msgOption);
	}

	private void ShowSickToVerySickMsg(object sender, EventArgs args) {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("SICK_TO_VERYSICK_0"));
		msgOption.Add(SpeechController.SpeechKeys.ImageTextureName, "itemInhalerEmergency");
		msgOption.Add(SpeechController.SpeechKeys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(SpeechController.SpeechKeys.ImageClickFunctionName, "OpenToSubCategoryItemsWithLockAndCallBack");
		speechController.Talk(msgOption);
	}

	private void ShowHealthyToVerySickMsg(object sender, EventArgs args) {
		Hashtable msgOption = new Hashtable();
		msgOption.Add(SpeechController.SpeechKeys.MessageText, Localization.Localize("HEALTHY_TO_VERYSICK_0"));
		msgOption.Add(SpeechController.SpeechKeys.ImageTextureName, "itemInhalerEmergency");
		msgOption.Add(SpeechController.SpeechKeys.ImageClickTarget, StoreUIManager.Instance.gameObject);
		msgOption.Add(SpeechController.SpeechKeys.ImageClickFunctionName, "OpenToSubCategoryItemsWithLockAndCallBack");
		speechController.Talk(msgOption);

		enableAutoSpeech = true;
	}
}
