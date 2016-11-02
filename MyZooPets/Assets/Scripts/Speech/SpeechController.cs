using UnityEngine;
using System.Collections;

/// <summary>
/// Speech controller for each pet/minipet that needs to talk
/// </summary>
public class SpeechController : MonoBehaviour {
	public enum SpeechKeys {
		MessageText,
		ImageTextureName,
		ImageClickTarget,
		ImageClickFunctionName,
		ImageButtonModeType
	}

	public Canvas parentCanvas;
	public GameObject speechTextPrefab;
	public GameObject speechImagePrefab;
	public GameObject speechImageTextPrefab;
	private float speechDuration = 4f;          //how long the speech bubble appears before destroyed

	private bool isActive = false;              // If this is active, check things every frame
	private Queue speechQueue = new Queue();    //queue that contains waiting messages
	private bool isQueueLocked = false;
	private GameObject currentSpeech;           //current message that is displayed to the user

	void Awake() {
		parentCanvas.worldCamera = Camera.main;
    }

	#region Queue implementation
	void Update() {
		if(isActive) {
			//if there are messages in the queue pop and display
			if(speechQueue.Count >= 1 && !isQueueLocked) {
				PopQueueAndDisplay();
			}

			//If queue is empty then turn the queue to inactive
			if(speechQueue.Count == 0 && !isQueueLocked) {
				isActive = false;
			}
		}
	}
	// for minipet for longer last messages
	public void TalkMiniPet(Hashtable message) {
		isActive = true;    // Show the components and start checking queue
		speechQueue.Enqueue(message);
		speechDuration = 3000f;
	}

	public void BeQuiet() {
		UnlockQueue();
		speechDuration = 4f;
	}

	public void Talk(Hashtable message) {
		isActive = true;    // Show the components and start checking queue
		speechQueue.Enqueue(message);
	}

	private void PopQueueAndDisplay() {
		Hashtable message = (Hashtable)speechQueue.Dequeue();
		SpawnMessage(message);

		//Unlock message queue	
		isQueueLocked = true;
		Invoke("UnlockQueue", speechDuration);
	}

	/// <summary>
	/// After speech duration destroyed the spawned message then Unlocks the queue.
	/// </summary>
	private void UnlockQueue() {
		isQueueLocked = false;

		//Destroy message 
		if(currentSpeech != null) {
			currentSpeech.GetComponent<SpeechBubble>().Finish();
		}
	}
	#endregion
	
	/// <summary>
	/// Look at the message keys and decide what is the appropriate layout to use for this message
	/// </summary>
	private void SpawnMessage(Hashtable message) {
		currentSpeech = null;

		//Use SpeechImageText prefab
		if(message.ContainsKey(SpeechKeys.MessageText) && message.ContainsKey(SpeechKeys.ImageTextureName)) {
			ShowSpeechWithImageAndText(message);
		}
		//Use SpeechText prefab
		else if(message.ContainsKey(SpeechKeys.MessageText)) {
			ShowSpeechWithText(message);
		}
		//Use SpeechImage prefab
		else if(message.ContainsKey(SpeechKeys.ImageTextureName)) {
			ShowSpeechWithImage(message);
		}
	}

	private void ShowSpeechWithImageAndText(Hashtable message) {
		currentSpeech = GameObjectUtils.AddChildGUI(parentCanvas.gameObject, speechImageTextPrefab);

		string textStringAux = message.ContainsKey(SpeechKeys.MessageText) ? (string)message[SpeechKeys.MessageText] : null;
		string imageSpriteAux = message.ContainsKey(SpeechKeys.ImageTextureName) ? (string)message[SpeechKeys.ImageTextureName] : null;
		GameObject objectToCall = message.ContainsKey(SpeechKeys.ImageClickTarget) ? (GameObject)message[SpeechKeys.ImageClickTarget] : null;
		string functionNameToCall = message.ContainsKey(SpeechKeys.ImageClickFunctionName) ? (string)message[SpeechKeys.ImageClickFunctionName] : null;
        UIModeTypes modeTypeAux = UIModeTypes.None;

		currentSpeech.GetComponent<SpeechBubble>().Init(textString: textStringAux, imageSprite: imageSpriteAux, _objectToCall: objectToCall, _functionNameToCall: functionNameToCall, _modeType: modeTypeAux);
	}

	private void ShowSpeechWithText(Hashtable message) {
		currentSpeech = GameObjectUtils.AddChildGUI(parentCanvas.gameObject, speechTextPrefab);

		string textStringAux = message.ContainsKey(SpeechKeys.MessageText) ? (string)message[SpeechKeys.MessageText] : null;

		currentSpeech.GetComponent<SpeechBubble>().Init(textString: textStringAux);
	}

	private void ShowSpeechWithImage(Hashtable message) {
		currentSpeech = GameObjectUtils.AddChildGUI(parentCanvas.gameObject, speechImagePrefab);

		string imageSpriteAux = message.ContainsKey(SpeechKeys.ImageTextureName) ? (string)message[SpeechKeys.ImageTextureName] : null;
		GameObject objectToCall = message.ContainsKey(SpeechKeys.ImageClickTarget) ? (GameObject)message[SpeechKeys.ImageClickTarget] : null;
		string functionNameToCall = message.ContainsKey(SpeechKeys.ImageClickFunctionName) ? (string)message[SpeechKeys.ImageClickFunctionName] : null;
		UIModeTypes modeTypeAux = UIModeTypes.None;

		currentSpeech.GetComponent<SpeechBubble>().Init(imageSprite: imageSpriteAux, _objectToCall: objectToCall, _functionNameToCall: functionNameToCall, _modeType: modeTypeAux);
	}

	// void OnGUI(){
	//         if(GUI.Button(new Rect(20, 20, 20, 20), "1")){
	//             Hashtable msgOption = new Hashtable();
	//             msgOption.Add(Keys.MessageText, "Give me food!");
	//             msgOption.Add(Keys.ImageTextureName, "iconStore");
	//             Talk(msgOption);
	//         }
	//         if(GUI.Button(new Rect(50, 20, 20, 20), "2")){
	//             Hashtable msgOption = new Hashtable();
	//             msgOption.Add(Keys.ImageTextureName, "speechImageHeart");
	//             Talk(msgOption);
	//         }
	//         if(GUI.Button(new Rect(80, 20, 20, 20), "3")){
	//             Hashtable msgOption = new Hashtable();
	//             msgOption.Add(Keys.MessageText, "fit as many words in this text box as possible. let's go");
	//             Talk(msgOption);
	//         }
	//         if(GUI.Button(new Rect(110, 20, 20, 20), "4")){
	//             Hashtable msgOption = new Hashtable();
	//             msgOption.Add(Keys.MessageText, "Give me food!");
	//             msgOption.Add(Keys.ImageTextureName, "iconStore");
	//             msgOption.Add(Keys.ImageClickTarget, this.gameObject);
	//             msgOption.Add(Keys.ImageClickFunctionName, "");
	//             Talk(msgOption);
	//         }
	// }
}
