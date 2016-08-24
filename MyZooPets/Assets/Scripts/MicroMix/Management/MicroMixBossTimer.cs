using UnityEngine;
using UnityEngine.UI;

public class MicroMixBossTimer : MonoBehaviour{
	public GameObject timerBoss;
	public Text timerText;
	public Transform position1;
	public Transform position2;
	// Use this for initialization
	void Start(){
		GetComponentInChildren<Animator>().Play("PlayerWin", 0, 0);
		GetComponentInChildren<Animator>().speed = 0;
		timerBoss.transform.position = position1.position;
	}

	public void UpdateTimer(int toDisplay){
		timerText.text = toDisplay.ToString();
	}

	public void StartTimer(){
		timerBoss.transform.position = position1.position;
		LeanTween.move(timerBoss, position2.position, 4f);
	}

	public void Pause(){
		LeanTween.pause(timerBoss);
	}

	public void Resume(){
		LeanTween.resume(timerBoss);
	}
}
