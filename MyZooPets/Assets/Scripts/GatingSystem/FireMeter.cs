using UnityEngine;
using System.Collections;

public class FireMeter : MonoBehaviour {
	public UISlider slider;
	
	public float fFillRate;
	
	void Start() {
		slider.sliderValue = 0;	
	}
	
	// Update is called once per frame
	void Update () {
		//if ( FingerGestures.GetFinger(0).IsDown )
		slider.sliderValue += fFillRate;
	}
	
	public bool IsFull() {
		bool bFull = slider.sliderValue >= 1;
		
		return bFull;
	}
}
