using UnityEngine;
using System.Collections;

//---------------------------------------------------
// Inherit from this class to get the speech queue 
// functionality. 
//---------------------------------------------------
public abstract class SpeechController<T> : Singleton<T> where T : MonoBehaviour {
	public float speechDuration = .7f; //how long the speech bubble appears before destroyed
	public bool isDebug = false;

	private bool isActive = false; // If this is active, check things every frame
	private Queue speechQueue = new Queue(); //queue that contains waiting messages
	private bool qLock = false;

	protected GameObject currentMessage; //current message that is displayed to the user

	//-------------abstract methods----------------------
    //---------------------------------------------------
    // SpawnMessage()
    // Look at the message keys and decide what is the 
    // appropriate layout to use for this message
    //---------------------------------------------------
	protected abstract void SpawnMessage(Hashtable message);

	void Update(){
		if(isActive){
			//if there are messages in the queue pop and display
			if(speechQueue.Count >= 1 && !qLock){
				PopQueueAndDisplay();
			}

			//If queue is empty then turn the queue to inactive
			if(speechQueue.Count == 0 && !qLock){
				SetInactive();
			}
		}
	}

	//---------------------------------------------------
	// Talk()
	// Display text ontop of the pet
	//---------------------------------------------------
	public void Talk(Hashtable message){
		SetActive();
		speechQueue.Enqueue(message);
	}

	
	//---------------------------------------------------
	// SetActive()
	// Show the components and start checking queue
	//---------------------------------------------------
	private void SetActive(){
		isActive = true;
	}

	//---------------------------------------------------
	// SetInactive()
	// Hide the components and stop checking queue
	//---------------------------------------------------
	private void SetInactive(){
		isActive = false;
	}

	//---------------------------------------------------
	// PopQueueAndDisplay()
	// get message hash from queue and display
	//---------------------------------------------------
	private void PopQueueAndDisplay(){
		Hashtable message = (Hashtable)speechQueue.Dequeue();
		SpawnMessage(message);

		//Unlock message queue	
		qLock = true;
		Invoke("UnlockQueue", speechDuration);
	}

	//---------------------------------------------------
	// UnlockQueue()
	// After speechDuration destroyed the spawned message
	// and unlock message queue
	//---------------------------------------------------
	private void UnlockQueue(){
		qLock = false;

		//Destroy message 
		if(currentMessage != null)
			Destroy(currentMessage);
	}
}
