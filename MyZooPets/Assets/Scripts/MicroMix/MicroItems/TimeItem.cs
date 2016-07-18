using UnityEngine;
using System.Collections;

public class TimeItem : MicroItem {
	private bool complete = false;
	public GameObject solarSystem;
	public GameObject petInstance;
	private float currentDegree;
	private float range = 30; //How far off 180 and 360 we can be
	void Update(){
		currentDegree-=45*Time.deltaTime;
		if (currentDegree<=0){
			currentDegree+=360;
		}
		solarSystem.transform.rotation= Quaternion.Euler(new Vector3(0f,0f,currentDegree));
	}
	public override void StartItem(){
		complete = false;
		if (Random.value>.5f){
			currentDegree=Random.Range(range,180-range);
		} else {
			currentDegree=Random.Range(180+range,360-range);

		}
		solarSystem.transform.rotation= Quaternion.Euler(new Vector3(0f,0f,currentDegree));
	}
	void OnTap(TapGesture gesture){
		if(gesture.StartSelection == null||complete){
			return;
		}
		else if(gesture.StartSelection.Equals(gameObject)){
			complete = true;
			if (currentDegree<=range||currentDegree>=360-range //From 330 to 30 (or is used because x cannot be less than 0 or greater than 360, but these situations are separate
				||currentDegree<=180+range&&currentDegree>=180-range){ //From 150 to 210
				parent.SetWon(true);
				petInstance.GetComponentInChildren<Animator>().SetTrigger("InhalerHappy1");
			} 
		}
	}
	/*void OnGUI(){
		bool correct = false;
		if (currentDegree<=range||currentDegree>=360-range //From 330 to 30 (or is used because x cannot be less than 0 or greater than 360, but these situations are separate
			||currentDegree<=180+range&&currentDegree>=180-range){ //From 150 to 210
			correct=true;
		} 
		GUI.Box(new Rect(Screen.width/2-90,Screen.width/2-40,180,80),currentDegree.ToString() + ":" + correct.ToString());
	}*/
}
