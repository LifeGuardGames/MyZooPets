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
		msgOption.Add(PetSpeechController.Keys.MessageText, Localization.Localize("NO_FIRE_INHALER_0"));
		msgOption.Add(PetSpeechController.Keys.ImageTextureName, "itemInhalerMain");
		GetComponent<PetSpeechController>().Talk(msgOption);
	}
}
