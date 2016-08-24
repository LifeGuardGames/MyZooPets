using UnityEngine;
using System.Collections;

public class TimeMicro : Micro{
	public GameObject sunParent;
	public GameObject sun;
	public GameObject moon;
	public GameObject background;
	private Vector3 moonHigh = new Vector3(0, 1.5f);
	private Vector3 moonLow = new Vector3(0, -4f);
	private float time;
	private float transitionLength = .6f;
	//Must be set according to Glinda's anim
	private bool isDay;
	private bool paused = false;
	private float angle;
	//Used for tutorial

	public override string Title{
		get{
			return "Time Inhaler";
		}
	}

	public override int Background{
		get{
			return 6;
		}
	}

	void Update(){
		if(MicroMixManager.Instance.IsPaused || paused){
			return;
		}
		time += Time.deltaTime;
		if(time > 1 && time < 3 && !isDay){ 
			isDay = true;
			NightToDay();
		}
		if(time > 3 && isDay){
			isDay = false;
			DayToNight();
		}
		angle -= 270 / 4f * Time.deltaTime;
		sunParent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		time = 0;
		sunParent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		sun.transform.localPosition = Vector3.down * 5;
		isDay = false;
		Setup();
	}

	protected override void _EndMicro(){ //Not necessary to set clocks or background inactive, because we are their parent and will set them inactive regardless
		LeanTween.cancel(moon);
	}

	protected override void _Pause(){
		LeanTween.pause(moon);
		background.GetComponent<Animator>().enabled = false;
	}

	protected override void _Resume(){
		LeanTween.pause(moon);
		background.GetComponent<Animator>().enabled = true;

		//Resume all tweens
		//Resume all animations
	}

	protected override IEnumerator _Tutorial(){
		Setup();
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		TimeItem time = GetComponentInChildren<TimeItem>();
		Vector3 offset = Vector3.up * .5f;
		finger.transform.position = time.transform.position + offset; //Set up the finger and the sky...

		while(angle > 255){ //Position ourselves in the first clock
			yield return 0;
		}

		paused = true;
		finger.gameObject.SetActive(true);
		yield return MicroMixManager.Instance.WaitSecondsPause(.3f); //Position finger on button and wait
		time.ShrinkDown();
		yield return finger.MoveTo(time.transform.position + offset, time.transform.position, delay: 0f, time: .15f); //Now press it
		yield return finger.MoveTo(time.transform.position, time.transform.position + offset, delay: 0f, time: .15f);
		paused = false;
		finger.gameObject.SetActive(false);
		time.clock1.SetActive(false); //Remove the clock and us

		while(angle > 106){ //Position ourselves in the first clock
			yield return 0;
		}

		paused = true;
		finger.gameObject.SetActive(true);
		yield return MicroMixManager.Instance.WaitSecondsPause(.3f); //Give a visual cue we will press
		time.ShrinkDown();
		yield return finger.MoveTo(time.transform.position + offset, time.transform.position, delay: 0f, time: .15f); //Now press it again
		yield return finger.MoveTo(time.transform.position, time.transform.position + offset, delay: 0f, time: .15f);
		paused = false;
		finger.gameObject.SetActive(false); // Remove the clocks etc.
		time.clock2.SetActive(false);

		yield return MicroMixManager.Instance.WaitSecondsPause(.4f); //Let the sun go beyond horizon
	}

	private void NightToDay(){
		//LeanTween.move(moon, moonLow, transitionLength).setEase(LeanTweenType.easeInOutQuad);
		LeanTween.alpha(moon, 0, transitionLength).setEase(LeanTweenType.easeOutQuint);
	}

	private void DayToNight(){
		//LeanTween.move(moon, moonHigh, transitionLength).setEase(LeanTweenType.easeInOutQuad);
		LeanTween.alpha(moon, 1, transitionLength).setEase(LeanTweenType.easeInQuint);
	}

	private void Setup(){
		moon.transform.position = moonHigh;
		moon.GetComponent<SpriteRenderer>().color = Color.white;
		background.GetComponent<Animator>().Play("MicroMixBackgroundSky", 0, .54f);
		background.GetComponent<Animator>().speed = 3 / 4f;
		angle = 345;
	}
}
