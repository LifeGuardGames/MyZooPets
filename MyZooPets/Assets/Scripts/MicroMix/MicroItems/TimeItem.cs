using UnityEngine;
using System.Collections;

public class TimeItem : MicroItem{
	public GameObject solarSystem;
	public GameObject petInstance;
	public GameObject dayBackground;
	public GameObject nightBackground;
	private bool complete = false;
	private float currentDegree;
	//How far off 180 and 360 we can be
	private float range = 30;
	private bool isDay = false;

	void Update(){
		currentDegree -= 45 * Time.deltaTime;

		if(currentDegree <= 0){
			currentDegree += 360;
			if(isDay){
				Debug.Log("WAW");

				isDay = false;
				LeanTween.alpha(dayBackground, 0, .5f);
				LeanTween.alpha(nightBackground, 1, .5f);
			}
		}
		if(currentDegree <= 180 && !isDay){
			Debug.Log("CALLED");
			isDay = true;
			LeanTween.alpha(dayBackground, 1, .5f);
			LeanTween.alpha(nightBackground, 0, .5f);
		}
		solarSystem.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, currentDegree));
	}

	public override void StartItem(){
		complete = false;
		dayBackground.SetActive(true);
		nightBackground.SetActive(true);
		if(Random.value > .5f){
			currentDegree = 90f;
			isDay = true;
			dayBackground.GetComponent<SpriteRenderer>().color = Color.white;
			nightBackground.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
		}
		else{
			currentDegree = 270f;
			isDay = false;
			dayBackground.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
			nightBackground.GetComponent<SpriteRenderer>().color = Color.white;
		}
		Debug.Log(currentDegree + ":" + isDay);
		solarSystem.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, currentDegree));
	}

	public override void OnComplete(){
		dayBackground.SetActive(false);
		nightBackground.SetActive(false);
		LeanTween.cancel(dayBackground);
		LeanTween.cancel(nightBackground);
	}

	void OnTap(TapGesture gesture){
		if(gesture.StartSelection == null || complete){
			return;
		}
		else if(gesture.StartSelection.Equals(gameObject)){
			complete = true;
			if(currentDegree <= range || currentDegree >= 360 - range//From 330 to 30 (or is used because x cannot be less than 0 or greater than 360, but these situations are separate
			   || currentDegree <= 180 + range && currentDegree >= 180 - range){ //From 150 to 210
				parent.SetWon(true);
				petInstance.GetComponentInChildren<Animator>().SetTrigger("InhalerHappy1");
			} 
		}
	}

	void OnGUI(){
		GUI.Box(new Rect(Screen.width - 100, 0, 100, 100), currentDegree.ToString() + ":" + isDay.ToString());
	}
}
