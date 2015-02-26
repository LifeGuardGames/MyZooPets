using UnityEngine;
using System.Collections;

public class RetentionPet : MiniPet {

	private int timesVisted = 0;

	void Awake(){
		//temp
		timesVisted = PlayerPrefs.GetInt("TimesVisted");
		name = "retention";
	}

	public override void FinishEating(){
		MiniPetManager.Instance.canLevel = true;
		this.gameObject.GetComponent<MiniPet>().isFinishEating = true; 
		miniPetSpeechAI.ShowTipMsg();
		timesVisted++;
		giveOutMission();
	}

	private void turnInMission(){
		MiniPetManager.Instance.IncreaseXP(id);
	}

	private void giveOutMission(){
		WellapadMissionController.Instance.UnlockTask("Ninja");
		MiniPetManager.Instance.needMission = true;
		WellapadMissionController.Instance.AddMission("Ninja");
	}


}
