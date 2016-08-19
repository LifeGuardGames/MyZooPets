using UnityEngine;
using System.Collections;

public class TimeMicroOld : Micro{
	public GameObject inhalerButton;
	public GameObject petPrefab;
	public GameObject solarSystem;
	public GameObject sun;
	public GameObject moon;
	public GameObject dayBackground;
	public GameObject nightBackground;
	private GameObject petInstance;
	private float currentDegree;
	private bool isPaused = false;
	//How far off 180 and 360 we can be
	private float range = 30;
	private bool isDay = false;

	public override string Title{
		get{
			return "Time Inhaler";
		}
	}

	public override int Background{
		get{
			return 0;
		}
	}

	void Update(){
		if(MicroMixManager.Instance.IsPaused || isPaused){
			return;
		}
		currentDegree -= 45 * Time.deltaTime;
		if(currentDegree <= 0){
			currentDegree += 360;
			if(isDay){
				isDay = false;
				LeanTween.alpha(dayBackground, 0, .5f);
				LeanTween.alpha(nightBackground, 1, .5f);
				LeanTween.color(inhalerButton, Color.white, .75f).setEase(LeanTweenType.easeOutQuad).setOnComplete(ColorTweenBack);
				if(MicroMixManager.Instance.IsTutorial){
					isPaused = true;
					StartCoroutine(ShowFinger());
				}
				//If we are in the tutorial, we want to pause;
			}
		}
		if(currentDegree <= 180 && !isDay){
			isDay = true;
			LeanTween.alpha(dayBackground, 1, .5f);
			LeanTween.alpha(nightBackground, 0, .5f);
			LeanTween.color(inhalerButton, Color.white, .75f).setEase(LeanTweenType.easeOutQuad).setOnComplete(ColorTweenBack);
			if(MicroMixManager.Instance.IsTutorial){
				isPaused = true;
				StartCoroutine(ShowFinger());
			}
			//If we are in the tutorial, we want to pause;
		}
		solarSystem.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, currentDegree));
	}

	public bool IsValid(){
		return (currentDegree <= range || currentDegree >= 360 - range//From 330 to 30 (or is used because x cannot be less than 0 or greater than 360, but these situations are separate
		|| currentDegree <= 180 + range && currentDegree >= 180 - range); //From 150 to 210
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		petInstance = (GameObject)Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
		petInstance.transform.SetParent(transform);	
		inhalerButton.GetComponent<TimeItemOld>().petInstance = petInstance;
		if(!randomize){
			inhalerButton.SetActive(true);
		}
		inhalerButton.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f);
		Setup();
	}

	protected override void _EndMicro(){
		Close();
	}

	protected override void _Pause(){
		if(petInstance){
			petInstance.GetComponentInChildren<Animator>().enabled = false;
		}
		LeanTween.pause(inhalerButton);
		LeanTween.pause(dayBackground);
		LeanTween.pause(nightBackground);
	}

	protected override void _Resume(){
		if(petInstance){
			petInstance.GetComponentInChildren<Animator>().enabled = true;
		}
		LeanTween.resume(inhalerButton);
		LeanTween.resume(dayBackground);
		LeanTween.resume(nightBackground);
	}

	protected override IEnumerator _Tutorial(){
		Setup();
		inhalerButton.SetActive(false);
		inhalerButton.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f);
		yield return WaitSecondsPause(3.5f);
	}

	private void Setup(){
		dayBackground.SetActive(true);
		nightBackground.SetActive(true);
		solarSystem.transform.rotation = Quaternion.Euler(Vector3.zero);
		sun.transform.localPosition = new Vector3(5, 0);
		moon.transform.localPosition = new Vector3(-5, 0);
		if(Random.value > .5f){
			currentDegree = 90f;
			isDay = true;
			dayBackground.GetComponent<SpriteRenderer>().color = new Color(3f / 4f, 3f / 4f, 3f / 4f);
			nightBackground.GetComponent<SpriteRenderer>().color = new Color(3f / 4f, 3f / 4f, 3f / 4f, 0);
		}
		else{
			currentDegree = 270f;
			isDay = false;
			dayBackground.GetComponent<SpriteRenderer>().color = new Color(3f / 4f, 3f / 4f, 3f / 4f, 0);
			nightBackground.GetComponent<SpriteRenderer>().color = new Color(3f / 4f, 3f / 4f, 3f / 4f);
		}
		solarSystem.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, currentDegree));
	}

	private void Close(){
		Destroy(petInstance);

		dayBackground.GetComponent<SpriteRenderer>().color = Color.white;
		nightBackground.GetComponent<SpriteRenderer>().color = Color.white;
		dayBackground.SetActive(false);
		nightBackground.SetActive(false);
		LeanTween.cancel(dayBackground);
		LeanTween.cancel(nightBackground);
	}

	private IEnumerator ShowFinger(){
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		Vector3 offset = new Vector3(0, .75f);
		inhalerButton.SetActive(true);
		finger.EnableBlur(false);
		yield return finger.ShakeToBack(inhalerButton.transform.position, inhalerButton.transform.position + offset, delay: .5f, time: .5f);
		inhalerButton.SetActive(false);
		finger.EnableBlur(true);
		isPaused = false;
		finger.gameObject.SetActive(false);
		Destroy(petInstance);
	}

	private void ColorTweenBack(){
		LeanTween.color(inhalerButton, new Color(.7f, .7f, .7f), .75f).setEase(LeanTweenType.easeOutQuad);
	}
}
