using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MicroMixLives : MonoBehaviour{
	private Image[] lives;
	private float tweenTime = 1f;
	private int currentLives;
	private int fadeOffset;

	void Start(){
		lives = GetComponentsInChildren<Image>();
		currentLives = 3;
	}

	public void ResetLives(){
		currentLives = 3;
		for(int i = 0; i < lives.Length; i++){
			LeanTween.alpha(lives[i].rectTransform, 1, tweenTime / 2f);
		}
	}

	public bool LoseLife(){
		currentLives--;
		StartCoroutine(LoseLifeHelper());
		fadeOffset=1; //Used to show the life next time, so that it can be hidden
		return currentLives == 0;
	}

	public void Hide(){
		for(int i = 0; i < currentLives; i++){
			LeanTween.alpha(lives[i].rectTransform, 0, tweenTime / 2f);
		}
	}

	public void Show(){ //Lose life is actually called before Show, so we change immediately, and then let LeanTween take over
		for(int i = 0; i < currentLives+fadeOffset; i++){
			LeanTween.alpha(lives[i].rectTransform, 1, tweenTime / 2f);
		}
		fadeOffset=0;
	}

	public IEnumerator LoseLifeHelper(){
		yield return new WaitForSeconds(tweenTime/2f);
		LeanTween.alpha(lives[currentLives].rectTransform, 0, tweenTime);
	}
}
