using UnityEngine;
using UnityEngine.UI;

public class MicroMixBossTimer : MonoBehaviour{
	public GameObject timerBoss;
	public Transform position1;
	public Transform position2;
	public Image[] images;
	public bool isMoving;
	private int nextIndex;
	// Use this for initialization
	void Start(){
		GetComponentInChildren<Animator>().speed = 0;
		timerBoss.transform.position = position1.position;
	}

	void Update(){
		if(!isMoving){
			return;
		}
		if(Vector2.Distance(timerBoss.transform.position, images[nextIndex].transform.position) < .5f){
			if(nextIndex > 0){ //Do not set 0 invisible (that is the Pet)
				images[nextIndex].gameObject.SetActive(false);
			} else {
				isMoving = false;
			}
			nextIndex--;
		}
	}

	public void StartTimer(){
		timerBoss.transform.position = position1.position;
		LeanTween.move(timerBoss, position2.position, 4f).setOnComplete(OnComplete);
		nextIndex = images.Length - 1;
		isMoving = true;
		/*foreach(Image toShow in images){
			toShow.gameObject.SetActive(true);
		}*/
	}

	public void Pause(){
		LeanTween.pause(timerBoss);
	}

	public void Resume(){
		LeanTween.resume(timerBoss);
	}
	private void OnComplete(){
		MicroMixManager.Instance.EndMicro();
	}
}
