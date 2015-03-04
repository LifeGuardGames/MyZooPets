using UnityEngine;
using System.Collections;

public class Merchant : MiniPet {

	private int timesVisited = 0;
	private GameObject blackStoreButton;

	void Awake(){
		//temp
		timesVisited = PlayerPrefs.GetInt("TimesVisited");
		name = "Merchant";
		blackStoreButton = GameObject.Find("BlackStoreButton");
	}

	public override void FinishEating(){
		base.FinishEating();
		MiniPetManager.Instance.canLevel = true;
		isFinishEating = true; 
		miniPetSpeechAI.ShowTipMsg();
		timesVisited++;
		PlayerPrefs.SetInt("TimesVisited", timesVisited);
		ShowStoreButton();
	}

	public void ShowStoreButton(){
		blackStoreButton.SetActive(true);
	}

	public void OpenStore(){

	}
	

}
