using UnityEngine;
using System.Collections;

public class SpeechController: Singleton<SpeechController> {

	// Constant maximum dimension of the speech bubbles width and height
	const float MAX_WIDTH = 450f;
	const float MAX_HEIGHT = 270f;

	public float speechDuration = .7f; //how long the speech bubble appears before destroyed
	public bool isDebug = false;

	private bool isActive = false; // If this is active, check things every frame
	private Queue speechQueue = new Queue(); //queue that contains waiting messages
	private bool qLock = false;
	private GameObject currentMessage; //current message that is displayed to the user
	private GameObject speechWithTextPrefab;
	private GameObject speechWithImagePrefab;
	private GameObject speechWithImageAndTextPrefab; //TO DO-j: not implemented yet

	//Message options keys
	// text:
	// image: (use the sprite name in the atlas)

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
	public void Talk(string text){
		SetActive();
		Hashtable message = new Hashtable();
		message.Add("text", text);

		speechQueue.Enqueue(message);
	}

	//---------------------------------------------------
	// TalkWithImage()
	// Display image on top of the pet
	//---------------------------------------------------
	public void TalkWithImage(string spriteName){
		SetActive();
		Hashtable message = new Hashtable();
		message.Add("image", spriteName);

		speechQueue.Enqueue(message);
	}

	//---------------------------------------------------
	// TalkWithImageAndText()
	// Display image and text on top of the pet
	//---------------------------------------------------
	public void TalkWithImageAndText(string text, string spriteName){
		SetActive();
		Hashtable message = new Hashtable();
		message.Add("image", spriteName);
		message.Add("text", text);

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

	// Queue handlers ////////////////////////////////
	private void PopQueueAndDisplay(){
		Hashtable message = (Hashtable)speechQueue.Dequeue();
		SpawnMessage(message);

		//Unlock message queue	
		qLock = true;
		Invoke("UnlockQueue", speechDuration);
	}

	private void UnlockQueue(){
		qLock = false;

		//Destroy message 
		if(currentMessage != null)
			Destroy(currentMessage);
	}
	////////////////////////////////////////////////////////////////

	private void SpawnMessage(Hashtable message){
		currentMessage = null;

		//Use SpeechWithImageAndText prefab
		if(message.ContainsKey("text") && message.ContainsKey("image")){
			if(speechWithImageAndTextPrefab == null)
				speechWithImageAndTextPrefab = Resources.Load("SpeechWithImageAndText") as GameObject;

			currentMessage = LgNGUITools.AddChildWithPosition(this.gameObject, speechWithImageAndTextPrefab);
			currentMessage.transform.Find("Label_Message").GetComponent<UILabel>().text = (string) message["text"];
			currentMessage.transform.Find("Sprite_Message").GetComponent<UISprite>().spriteName = (string) message["image"];
		}
		//Use SpeechWithText prefab
		else if(message.ContainsKey("text")){
			if(speechWithTextPrefab == null)
				speechWithTextPrefab = Resources.Load("SpeechWithText") as GameObject;

			currentMessage = LgNGUITools.AddChildWithPosition(this.gameObject, speechWithTextPrefab);
			currentMessage.transform.Find("Label_Message").GetComponent<UILabel>().text = (string) message["text"];
		}
		//Use SpeechWithImage prefab
		else if(message.ContainsKey("image")){
			if(speechWithImagePrefab == null)
				speechWithImagePrefab = Resources.Load("SpeechWithImage") as GameObject;

			currentMessage = LgNGUITools.AddChildWithPosition(this.gameObject, speechWithImagePrefab);
			currentMessage.transform.Find("Sprite_Message").GetComponent<UISprite>().spriteName = (string) message["image"];
		}
		else{
		}
	}

	//---------------------------------------------------
	// SetText()
	// Set input text to UILabel
	//---------------------------------------------------
	// private void SetText(string text){
	// 	UILabel label = textObject.GetComponent<UILabel>();
	// 	label.text = text;
	// }


	// public void SetBubbleSize(float width, float height){
	// 	D.Assert(width > 0 && height > 0);

	// 	if(width > MAX_WIDTH){
	// 		Debug.LogWarning("speech bubble max width exceeded, setting to max");
	// 		width = MAX_WIDTH;
	// 	}
	// 	if(height > MAX_HEIGHT){
	// 		Debug.LogWarning("speech bubble max height exceeded, setting to max");
	// 		height = MAX_HEIGHT;
	// 	}
	// 	spriteBubbleObject.transform.localScale = new Vector3(width, height, 1);
	// }

	// public void SetOffset(Vector3 offset){
	// 	selfOffset = offset;
	// }

	// public void SetLocalPosition(Vector3 localPosition, Vector3 offset){
	// 	isFollowTarget = false;
	// 	SetOffset(offset);
	// 	gameObject.transform.localPosition = new Vector3(localPosition.x + selfOffset.x, localPosition.y + selfOffset.y, localPosition.z + selfOffset.z);
	// }

	// public void SetGlobalPosition(Vector3 globalPosition, Vector3 offset){
	// 	isFollowTarget = false;
	// 	SetOffset(offset);
	// 	gameObject.transform.position = new Vector3(globalPosition.x + selfOffset.x, globalPosition.y + selfOffset.y, globalPosition.z + selfOffset.z);
	// }

	// // Follows position, with offset
	// public void FollowGameObject(GameObject target, Vector3 offset){
	// 	isFollowTarget = true;
	// 	followTarget = target;
	// 	SetOffset(offset);
	// }

	void OnGUI(){
		if(isDebug){
			if(GUI.Button(new Rect(20, 20, 20, 20), "1")){
				TalkWithImageAndText("Give me food!", "iconStore");
			}
			if(GUI.Button(new Rect(50, 20, 20, 20), "2")){
				TalkWithImage("speechImageHeart");
			}
			if(GUI.Button(new Rect(80, 20, 20, 20), "3")){
				Talk("fit as many words in this text box as possible. let's go");
			}
			if(GUI.Button(new Rect(110, 20, 20, 20), "4")){

			}
		}
	}
}
