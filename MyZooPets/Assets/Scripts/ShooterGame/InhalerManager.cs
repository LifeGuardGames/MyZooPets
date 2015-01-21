using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class InhalerManager : MonoBehaviour {

	//Speed at which the arrow scrolls
	//public float scrollSpeed;
	//start Time
	private int startTime;
	//how often we increment time
	public float timeIncrementer;
	// a vector 2 holding our optimal use window
	public Vector2 justRight;
	// the current time
	public int currTime;
	// the number of times the user has missed the optimal use window
	//public int numMissed=0;
	// window offset
	public int offset;
	// variable to prevent multiple used in the window
	private bool justUsed = true;
	public float elaspedTime;
	//Player Health
	public float playerHealth;
	//Our Shooter Game Manager
	public GameObject Player; 
	//UI
	//public GameObject arrowLabel;
	//public GameObject arrow;
	//public GameObject arrowstartPos;
	//public GameObject arrowLabelStartPos;

	// we use this to get our start time and set up variables for our window
	void Start () {
		startTime=int.Parse(DateTime.Now.ToString("hh"));
		elaspedTime = Time.time;
		justRight.x = startTime;
		justRight.y = justRight.x + offset;
		if(justRight.y>12)
		{
			justRight.y-=12;
		}
		currTime= startTime;
		//arrowLabel.gameObject.GetComponent<UILabel>().text= currTime.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		//Scrolling the arrow and label
		//arrow.transform.Translate(0,Time.deltaTime*scrollSpeed,0);
		//arrowLabel.transform.Translate(-Time.deltaTime*scrollSpeed,0,0);
		/*once we have vacated the window assumeing the window will always be a 12 hour difference 
		  this allows us to prevent button spamming while the window is open*/
		if(justRight.y == currTime)
		{
			justUsed=false;
		}
		//make the button shine if its time
		if(currTime>= justRight.x || currTime<=justRight.y){

		}
		if(Time.time-elaspedTime>= timeIncrementer){
			if(currTime!= 12){
				currTime+=1;
			}
			else{
				currTime=1;
			}
			//arrowLabel.gameObject.GetComponent<UILabel>().text= currTime.ToString();
			elaspedTime=Time.time;
		}
		
	}

	//on button Tap
	public void ClickIt(){

		calculateStrength();
		justRight.x = currTime;
		justRight.y = justRight.x + offset;
		if(justRight.y>12)
		{
			justRight.y-=12;
		}
		justUsed=true;
		elaspedTime=Time.time;
		//arrow.transform.position= arrowstartPos.transform.position;
		//arrowLabel.transform.position= arrowLabelStartPos.transform.position;
	}
	// calculates health of the player based off a number of factors
	private void calculateStrength (){
		if (currTime>= justRight.x || currTime<=justRight.y){
			if( justUsed==false)
			{
				ShooterGameManager.Instance.AddScore(10);
			playerHealth+=3;
			justUsed=true;
			//if (numMissed>0){
			//	numMissed--;
			//}
		
			//playerHealth-=(1*numMissed);
		}
			/*else{
				playerHealth++;
			}*/
		}
		/*else{
			playerHealth+=(3/(numMissed+1));
			playerHealth-=(1*numMissed);

		}*/
		Player.GetComponent<PlayerShooterController>().removeHealth(playerHealth);
	}


}
