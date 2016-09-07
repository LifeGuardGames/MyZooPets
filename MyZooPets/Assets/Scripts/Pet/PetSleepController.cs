using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PetSleepController : MonoBehaviour {

	public GameObject blackScreen;
	public bool isNight;
	private Color c;
    public void OnTapped(){
		if(!isNight){
				PetSpeechAI.Instance.ShowSleepingMessageMsg();
				blackScreen.SetActive(true);
			LeanTween.alpha(blackScreen.GetComponent<RectTransform>(),1.0f,2.0f).setOnComplete(NightTime);
		}
		else{
			LeanTween.alpha(blackScreen.GetComponent<RectTransform>(), 0.0f, 2.0f).setOnComplete(NightTime);
		}
	}

	private void NightTime(){
		isNight = true;	
	}

	private void WakeUP(){
		isNight = false;
	}
}
