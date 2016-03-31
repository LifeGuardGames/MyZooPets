using UnityEngine;
using System.Collections;

public class PetSleepController : MonoBehaviour {

	public GameObject blackScreen;
	public bool isNight;
	public void OnTapped(){
		if(!isNight){
				PetSpeechAI.Instance.ShowSleepingMessageMsg();
				blackScreen.SetActive(true);
				StartCoroutine(NightTimer());
		}
		else{
			StartCoroutine(WakeUP());
		}
	}

	IEnumerator NightTimer(){
		yield return new WaitForSeconds(0.1f);
		blackScreen.GetComponent<UISprite>().alpha += 0.1f;
		if(blackScreen.GetComponent<UISprite>().alpha <= 1){
			StartCoroutine(NightTimer());
		}
		else{
			isNight = true;
		}
	}

	IEnumerator WakeUP(){
		yield return new WaitForSeconds(0.1f);
		blackScreen.GetComponent<UISprite>().alpha -= 0.1f;
		if(blackScreen.GetComponent<UISprite>().alpha > 0){
			StartCoroutine(WakeUP());
		}
		else if (blackScreen.GetComponent<UISprite>().alpha <= 1){
			blackScreen.SetActive(false);
			isNight = false;
		}
	}
}
