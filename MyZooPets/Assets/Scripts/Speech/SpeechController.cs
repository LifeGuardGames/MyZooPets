using UnityEngine;
using System.Collections;

//---------------------------------------------------
// Inherit from this class to get the speech queue 
// functionality. 
//---------------------------------------------------
/// <summary>
/// Speech controller. Inherit from this class to get the speech queue functionality
/// </summary>
public abstract class SpeechController<T> : Singleton<T> where T : MonoBehaviour {
	public float speechDuration = .7f; //how long the speech bubble appears before destroyed
	public bool isDebug = false;

	private bool isActive = false; // If this is active, check things every frame
	private Queue speechQueue = new Queue(); //queue that contains waiting messages
	private bool isQueueLocked = false;

	protected GameObject currentMessage; //current message that is displayed to the user

	//-------------abstract methods----------------------
	/// <summary>
	/// Spawns the message. Look at the message keys and decide what is the
	/// appropriate layout to use for this message
	/// </summary>
	/// <param name="message">Message.</param>
	protected abstract void SpawnMessage(Hashtable message);

	void Update(){
		if(isActive){
			//if there are messages in the queue pop and display
			if(speechQueue.Count >= 1 && !isQueueLocked){
				PopQueueAndDisplay();
			}

			//If queue is empty then turn the queue to inactive
			if(speechQueue.Count == 0 && !isQueueLocked){
				SetInactive();
			}
		}
	}
	// for minipet for longer last messages
	public void TalkM(Hashtable message){
		SetActive();
		speechQueue.Enqueue(message);
		speechDuration = 3000.25f;
	}

	public void BeQuiet(){
		UnlockQueue();
		speechDuration = 0.7f;
	}

	/// <summary>
	/// Talk the specified message.
	/// </summary>
	/// <param name="message">Message.</param>
	public void Talk(Hashtable message){
		SetActive();
		speechQueue.Enqueue(message);
	}
	
	/// <summary>
	/// Show the components and start checking queue
	/// </summary>
	private void SetActive(){
		isActive = true;
	}
	
	/// <summary>
	/// Hide the components and stop checking queue.
	/// </summary>
	private void SetInactive(){
		isActive = false;
	}

	/// <summary>
	/// Pops the queue and display.
	/// </summary>
	private void PopQueueAndDisplay(){
		Hashtable message = (Hashtable)speechQueue.Dequeue();
		SpawnMessage(message);

		//Unlock message queue	
		isQueueLocked = true;
		Invoke("UnlockQueue", speechDuration);
	}
	
	/// <summary>
	/// After speech duration destroyed the spawned message then Unlocks the queue.
	/// </summary>
	private void UnlockQueue(){
		isQueueLocked = false;

		//Destroy message 
		if(currentMessage != null)
			Destroy(currentMessage);
	}
}
