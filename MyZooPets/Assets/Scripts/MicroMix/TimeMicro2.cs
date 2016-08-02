using UnityEngine;
using System.Collections;

public class TimeMicro2 : Micro{
	public GameObject clock1;
	public GameObject clock2;
	public GameObject sunParent;
	public GameObject moon;
	public GameObject background;
	private Vector3 moonHigh = new Vector3(0, 1.5f);
	private Vector3 moonLow = new Vector3(0, -4f);
	private float time;
	private float transitionLength = .6f;
	//Must be set according to Glinda's anim
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
		if (MicroMixManager.Instance.IsPaused){
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
		float angle = -time/4f*360;
		sunParent.transform.rotation= Quaternion.Euler(new Vector3(0,0,angle));
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		time = 0;
		clock1.SetActive(true);
		clock2.SetActive(true);
		isDay = false;
		moon.transform.position = moonHigh;
		//background.SetActive(true); 
		background.GetComponent<Animator>().Play("MicroMixBackgroundSky", 0, .5f);//(Maybe we can just use a reset)
		background.GetComponent<Animator>().speed = 1;
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
		//LeanTween.move(moon, moonLow, transitionLength).setEase(LeanTweenType.easeInOutQuad);
		LeanTween.alpha(moon,0,transitionLength).setEase(LeanTweenType.easeOutQuint);
	}

	private void DayToNight(){
		//LeanTween.move(moon, moonHigh, transitionLength).setEase(LeanTweenType.easeInOutQuad);
		LeanTween.alpha(moon,1,transitionLength).setEase(LeanTweenType.easeInQuint);
	}
}
