using UnityEngine;
using System.Collections;

public class TimeMicro2 : Micro{
	public GameObject clock1;
	public GameObject clock2;
	public GameObject sunParent;
	public GameObject moon;
	public GameObject background;
	private Vector3 moonHigh;
	private Vector3 moonLow;
	private float time;
	private float transitionLength=.3f; //Must be set according to Glinda's anim
	private bool isDay;
	public override string Title{
		get{
			return "Time Inhaler";
		}
	}

	public override int Background{
		get{
			return -1;
		}
	}
	void Update(){
		time+=Time.deltaTime;
		if (time>1 && time < 3 && !isDay){ 
			isDay=true;
			NightToDay();
		}
		if (time>3 && isDay){
			isDay=false;
			DayToNight();
		}
	}
	protected override void _StartMicro(int difficulty, bool randomize){
		time = 0;
		clock1.SetActive(true);
		clock2.SetActive(true);
		isDay=false;
		moon.transform.position=moonHigh;
		sunParent.gameObject.SetActive(false);
		//background.SetActive(true); 
		//background.GetComponent<Animator>().Play("MicroMixBackgroundSky", 0, 1); //(Maybe we can just use a reset)
		//background.GetComponent<Animator>().speed=-1;
		background.GetComponent<Animator>().speed=-1;
		background.GetComponent<Animator>().SetTime(4f);
	}

	protected override void _EndMicro(){ //Not necessary to set clocks or background inactive, because we are their parent and will set them inactive regardless
		//background.SetActive(false);
	}

	protected override void _Pause(){
		LeanTween.pause(moon);
		//Pause all tweens
		//Pause all animations
	}

	protected override void _Resume(){
		LeanTween.pause(moon);
		//Resume all tweens
		//Resume all animations
	}
	protected override IEnumerator _Tutorial(){
		yield return 0;
	}

	private void NightToDay(){
		LeanTween.move(moon,moonLow,transitionLength).setEase(LeanTweenType.easeInOutQuad);
		sunParent.gameObject.SetActive(true);
	}

	private void DayToNight(){
		LeanTween.move(moon,moonHigh,transitionLength).setEase(LeanTweenType.easeInOutQuad);
		sunParent.gameObject.SetActive(false);
	}
}
