using UnityEngine;
using System.Collections;

public class RetentionPet : MiniPet {

	private int timesVisted = 0;

	void Start(){
		//temp
		timesVisted = PlayerPrefs.GetInt("TimesVisted");
		name = "retention";
	}

	public override void FinishEating(){
		this.gameObject.GetComponent<MiniPet>().isFinishEating = true; 
		miniPetSpeechAI.ShowTipMsg();
		timesVisted++;
		giveOutMission();
	}

	private void giveOutMission(){
	}


}
