using UnityEngine;
using System.Collections;

public class MiniPetSpeechAI : MonoBehaviour{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowDirtyMsg(){
		Hashtable msgOption = new Hashtable();
		msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("MINIPET_DIRTY_MSG"));
//		msgOption.Add(PetSpeechController.Keys.ImageTextureName, "itemInhalerMain");
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


}
