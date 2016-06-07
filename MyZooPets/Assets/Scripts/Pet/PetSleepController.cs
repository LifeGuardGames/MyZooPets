using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PetSleepController : MonoBehaviour {

	public GameObject blackScreen;
	public bool isNight;
	Color c;
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
		c = blackScreen.GetComponent<Image>().color;
		c.a += 0.1f;
		blackScreen.GetComponent<Image>().color = c;
        if(blackScreen.GetComponent<Image>().color.a <= 1){
			StartCoroutine(NightTimer());
		}
		else{
			isNight = true;
		}
	}

	IEnumerator WakeUP(){
		yield return new WaitForSeconds(0.1f);
		c = blackScreen.GetComponent<Image>().color;
		c.a -= 0.1f;
		blackScreen.GetComponent<Image>().color = c;
		if(blackScreen.GetComponent<Image>().color.a > 0){
			StartCoroutine(WakeUP());
		}
		else if (blackScreen.GetComponent<Image>().color.a <= 1){
			blackScreen.SetActive(false);
			isNight = false;
		}
	}
}
