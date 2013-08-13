using UnityEngine;
using System.Collections;

/// <summary>
/// Speech bubble controller NGUI
/// NOTE: Make sure the anchor is semantically set!
/// NOTE: Please keep high level queue implementations symmetrical with SpeechBubbleControllerTK2D.cs
/// </summary>

public class SpeechBubbleControllerNGUI : Singleton<SpeechBubbleControllerNGUI> {

	// Constant maximum dimension of the speech bubbles width and height
	const float MAX_WIDTH = 450f;
	const float MAX_HEIGHT = 270f;

	public GameObject textObject;
	public GameObject spriteBubbleObject;

	public Vector3 selfOffset = new Vector3(0f, 0f, 0f);

	public bool isFollowTarget;
	public GameObject followTarget;

	public float speechDuration = 2f;

	public bool isDebug = false;

	private bool isActive = false; // If this is active, check things every frame
	private Queue q = new Queue();
	private bool qLock = false;

	void Start(){
		SetInactive();
	}

	public void Talk(string text){
		SetActive();
		q.Enqueue(text);
	}

	void Update(){
		if(isActive){
			if(isFollowTarget){
				if(followTarget != null){
					gameObject.transform.position = new Vector3(followTarget.transform.position.x + selfOffset.x, followTarget.transform.position.y + selfOffset.y, followTarget.transform.position.z + selfOffset.z);
				}
				else{
					Debug.LogError("No follow target");
				}
			}
			if(q.Count >= 1 && !qLock){
				PopQueueAndDisplay();
			}
			if(q.Count == 0 && !qLock){
				SetInactive();
			}
		}
	}

	// Queue handlers ////////////////////////////////
	private void PopQueueAndDisplay(){
		SetText((string)q.Dequeue());
		qLock = true;
		Invoke("UnlockQueue", speechDuration);
	}

	private void UnlockQueue(){
		qLock = false;
	}
	////////////////////////////////////////////////////////////////

	// Show the components and start checking queue
	public void SetActive(){
		isActive = true;
		textObject.SetActive(true);
		spriteBubbleObject.SetActive(true);
	}

	// Hide the components and stop checking queue
	public void SetInactive(){
		isActive = false;
		textObject.SetActive(false);
		spriteBubbleObject.SetActive(false);
	}


	public void SetText(string text){
		UILabel label = textObject.GetComponent<UILabel>();
		label.text = text;
	}

	public void SetBubbleSize(float width, float height){
		D.Assert(width > 0 && height > 0);

		if(width > MAX_WIDTH){
			Debug.LogWarning("speech bubble max width exceeded, setting to max");
			width = MAX_WIDTH;
		}
		if(height > MAX_HEIGHT){
			Debug.LogWarning("speech bubble max height exceeded, setting to max");
			height = MAX_HEIGHT;
		}
		spriteBubbleObject.transform.localScale = new Vector3(width, height, 1);
	}

	public void SetOffset(Vector3 offset){
		selfOffset = offset;
	}

	public void SetLocalPosition(Vector3 localPosition, Vector3 offset){
		isFollowTarget = false;
		SetOffset(offset);
		gameObject.transform.localPosition = new Vector3(localPosition.x + selfOffset.x, localPosition.y + selfOffset.y, localPosition.z + selfOffset.z);
	}

	public void SetGlobalPosition(Vector3 globalPosition, Vector3 offset){
		isFollowTarget = false;
		SetOffset(offset);
		gameObject.transform.position = new Vector3(globalPosition.x + selfOffset.x, globalPosition.y + selfOffset.y, globalPosition.z + selfOffset.z);
	}

	// Follows position, with offset
	public void FollowGameObject(GameObject target, Vector3 offset){
		isFollowTarget = true;
		followTarget = target;
		SetOffset(offset);
	}

	void OnGUI(){
		if(isDebug){
			if(GUI.Button(new Rect(20, 20, 20, 20), "1")){
				Talk("asdddddddddddddddddddddddddddddddddddd");
			}
			if(GUI.Button(new Rect(50, 20, 20, 20), "2")){
				Talk("aaaaaa");
			}
			if(GUI.Button(new Rect(80, 20, 20, 20), "3")){
				Talk("a222");
			}
			if(GUI.Button(new Rect(110, 20, 20, 20), "4")){

			}
		}
	}
}
