using UnityEngine;
using System.Collections;

public class RotateAroundCenter : MonoBehaviour {
	public bool isPlaying = true;
	public float speed = 100;
	public bool isIntervalOn = false;
	public float intervalIncrement = 45f;
	public float intervalSpeed = 0.2f;


	void Start(){
		if(isPlaying){
			isPlaying = false;	// Hacky toggle here for init case so it plays
			Play();
		}
	}

	private void IntervalTick(){
		transform.Rotate(Vector3.forward * intervalIncrement);
	}

	private void Update(){
		if(isPlaying && speed != 0){
			transform.Rotate(Vector3.forward * Time.deltaTime * speed);
		}
	}

	public void Play(){
		if(!isPlaying){
			isPlaying = true;

			if(isIntervalOn){
				InvokeRepeating("IntervalTick", 0f, intervalSpeed);
			}
		}
	}

	public void Stop(){
		isPlaying = false;
		CancelInvoke("IntervalTick");
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(300, 100, 100, 100), "Play")){
//			Play();
//		}
//		if(GUI.Button(new Rect(400, 100, 100, 100), "Stop")){
//			Stop();
//		}
//	}
}
