using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShooterUIManager :Singleton<ShooterUIManager> {
	public GameObject Sun;
	public GameObject Moon;
	public GameObject PosSky;
	public GameObject PosBottom;

	// Use this for initialization
	void Start () {
	
	}

	public void AChangeOfTimeActOne(){
		if(Sun.GetComponent<MovingSky>().InSky==true){
			LeanTween.move(Sun,PosBottom.transform.position,2.0f).setOnComplete(AChangeOfTimeActTwo);
		}
		else{
			LeanTween.move(Moon,PosBottom.transform.position,2.0f).setOnComplete(AChangeOfTimeActTwo);
		}

	}
	public void AChangeOfTimeActTwo(){
		if(Sun.GetComponent<MovingSky>().InSky==true){
			LeanTween.move(Moon,PosSky.transform.position,2.0f).setOnComplete(ShooterGameManager.Instance.ChangeWaves);
			Moon.GetComponent<MovingSky>().InSky=true;
			Sun.GetComponent<MovingSky>().InSky=false;
		}
		else{
			LeanTween.move(Sun,PosSky.transform.position,2.0f).setOnComplete(ShooterGameManager.Instance.ChangeWaves);
			Sun.GetComponent<MovingSky>().InSky=true;
			Moon.GetComponent<MovingSky>().InSky=false;
		}
	}


}
