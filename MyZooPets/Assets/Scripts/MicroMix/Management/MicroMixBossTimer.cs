using UnityEngine;
using UnityEngine.UI;

public class MicroMixBossTimer : MonoBehaviour{
	public GameObject timerBoss;
	public Transform position1;
	public Transform position2;
	public Image[] images;
	private Micro toEnd;
	private bool isMoving = false;
	private int nextIndex;
	private float totalTime = 4f;
	private float currentTime;

	void Start(){
		timerBoss.transform.position = position1.position;
	}

	void Update(){
		if(MicroMixManager.Instance.IsPaused || !isMoving){
			return;
		}

		if(currentTime > 0){
			currentTime -= Time.deltaTime;
		}

		if(currentTime <= 0){
			toEnd.EndMicro();
			isMoving = false;
		}

		if(nextIndex >= 0 && Vector2.Distance(timerBoss.transform.position, images[nextIndex].transform.position) < .5f){
			images[nextIndex].gameObject.SetActive(false);
			nextIndex--;
		}
	}

	public void StartTimer(Micro toEnd){
		timerBoss.transform.position = position1.position;
		LeanTween.move(timerBoss, position2.position, 4f);
		nextIndex = images.Length - 1;
		currentTime = totalTime;
		isMoving = true;
		this.toEnd = toEnd;
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

	public void Reset() {
		LeanTween.cancel(this.gameObject);
		foreach(Image toShow in images) {
			toShow.gameObject.SetActive(false);
		}
	}
}
