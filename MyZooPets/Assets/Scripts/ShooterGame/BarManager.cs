using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BarManager : MonoBehaviour {

	//Speed at which the bar scrolls
	public float scrollSpeed;
	//start Time
	private int startTime;
	//how often we increment time
	public float timeIncrementer;
	// a vector 2 holding our optimal use window
	public Vector2 justRight;
	// the current time
	public int currTime;
	// the number of times the user has missed the optimal use window
	public int numMissed=0;
	// number of hours over
	public int overBy=0;
	// number of hours early by
	public int earlyBy=0;
	// window offset
	public int offset;
	// variable to prevent multiple used in the window
	private bool justUsed = false;
	public float elaspedTime;
	//Player Health
	public float playerHealth;
	//Our Shooter Game Manager
	public GameObject ShooterGameManager;
	// we use this to get our start time and set up variables for our window
	void Start () {
		startTime=int.Parse(DateTime.Now.ToString("hh"));
		elaspedTime = Time.time;
		justRight.x = startTime;
		justRight.y = justRight.x + offset;
		currTime= startTime;
	}
	
	// Update is called once per frame
	void Update () {
		/*once we have vacated the window assumeing the window will always be a 12 hour difference 
		  this allows us to prevent button spamming while the window is open*/
		if(justRight.y == currTime)
		{
			justUsed=false;
		}
		//make the button shine if its time
		if(currTime>= justRight.x || currTime<=justRight.y)
		{
			if(justUsed=false)
			{
				//todo Make button shine
				
			}
			ShooterGameManager.GetComponent<ShooterGameManager>().removeHealth(-.02f);
		}
		if(Time.time-elaspedTime>= timeIncrementer)
		{
			if(currTime!= 12)
			{
				currTime+=1;
			}
			else
			{
				currTime=1;
			}
			elaspedTime=Time.time;
			playerHealth-=0.01f;
			ShooterGameManager.GetComponent<ShooterGameManager>().removeHealth(playerHealth);
		}
		
	}

	//on button Tap
	void OnTap(TapGesture gesture)
	{
		Debug.Log("hi");
		calculateStrength();
		justRight.x = currTime;
		justRight.y = justRight.x + offset;
		justUsed=true;
		elaspedTime=Time.time;
	}
	void calculateStrength ()
	{
		if (currTime>= justRight.x || currTime<=justRight.y)
		{
			playerHealth+=3;
			if (numMissed>0)
			{
				numMissed--;
			}
			if (earlyBy>0)
			{
				earlyBy--;
			}
			if (overBy>0)
			{
				overBy--;
			}
			playerHealth-=(1*numMissed);
			playerHealth-=(1*overBy);
			playerHealth-=(1*earlyBy);
		}
		else
		{
			playerHealth+=(3/numMissed);
			playerHealth-=(1*numMissed);
			playerHealth-=(1*overBy);
			playerHealth-=(1*earlyBy);
		}
		ShooterGameManager.GetComponent<ShooterGameManager>().removeHealth(playerHealth);
	}


}
