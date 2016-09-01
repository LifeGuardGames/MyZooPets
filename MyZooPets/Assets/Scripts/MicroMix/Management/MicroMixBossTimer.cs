using UnityEngine;
using UnityEngine.UI;

public class MicroMixBossTimer : MonoBehaviour{
	public GameObject timerBoss;
	public Transform position1;
	public Transform position2;
	public Image[] images;
	public bool isMoving;
	private int nextIndex;
	private float totalTime = 4f;
	private float currentTime;
	// Use this for initialization
	void Start(){
		GetComponentInChildren<Animator>().speed = 0;
		timerBoss.transform.position = position1.position;
	}

	void Update(){
		if(MicroMixManager.Instance.IsPaused){
			return;
		}
		if(currentTime > 0){
			currentTime -= Time.deltaTime;
		}

		if(currentTime <= 0){
			MicroMixManager.Instance.EndMicro();
		}

		if(Vector2.Distance(timerBoss.transform.position, images[nextIndex].transform.position) < .5f && nextIndex > 0){
			images[nextIndex].gameObject.SetActive(false);
			nextIndex--;
		}
	}

	public void StartTimer(){
		timerBoss.transform.position = position1.position;
		LeanTween.move(timerBoss, position2.position, 4f);
		nextIndex = images.Length - 1;
		currentTime = totalTime;
		isMoving = true;
		foreach(Image toShow in images){
			toShow.gameObject.SetActive(true);
		}
	}

	public void Pause(){
		LeanTween.pause(timerBoss);
	}

	public void Resume(){
		LeanTween.resume(timerBoss);
	}
}
