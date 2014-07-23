using UnityEngine;
using System.Collections;

public class AssemblyLineItem : MonoBehaviour {

	public string itemKey;	// Key to match with the zone key
	private string currentHoverKey = null;	// Key of the zone it is hovering over, null otherwise
	private int dragFingerIndex;
	private bool isOnAssemblyLine = false;
	private bool isPaused = false;
	private GameObject destination;
	private float speed;
	public float Speed{
		get{return speed;}
		set{speed = value;}
	}

	private AssemblyLineController assemblyLineParent;


	void Start(){
		// listen for various messages from the game manager
		DoctorMatchManager.OnSpeedChange += OnSpeedChange;			// speed change events
		DoctorMatchManager.OnStateChanged += OnGameStateChanged; 	// game state changes so the character can react appropriately
		DoctorMatchManager.OnNewGame += OnNewGame;					// new game
	}

	void OnDestroy(){
		// stop listening to messages
		DoctorMatchManager.OnSpeedChange -= OnSpeedChange;			// speed change events	
		DoctorMatchManager.OnStateChanged -= OnGameStateChanged;	// game state changes
		DoctorMatchManager.OnNewGame -= OnNewGame;					// new game
	}

	public void SetupItem(AssemblyLineController assemblyLineController){

		assemblyLineParent = assemblyLineController;

		// Pass self object to the game manager for random sprite setup
		DoctorMatchManager.Instance.SetUpRandomAssemblyItemSprite(this.gameObject);

		speed = assemblyLineController.Speed;
		destination = assemblyLineController.endLocation;
		isOnAssemblyLine = true;	// Start moving the object
	}

	void Update(){
		// While the item is not being dragged
		if(isOnAssemblyLine && !isPaused){
			transform.position = Vector3.MoveTowards(transform.position, destination.transform.position, Time.deltaTime * speed);
			if(transform.position == destination.transform.position){	// Reached end platform
				DoctorMatchManager.Instance.CharacterScoredWrong();
				Destroy(gameObject);
			}
		}
	}

	void OnDrag(DragGesture gesture){
		// First finger
		FingerGestures.Finger finger = gesture.Fingers[0];

		if(gesture.Phase == ContinuousGesturePhase.Started){
			isOnAssemblyLine = false;	// Disable automatic moving
			dragFingerIndex = finger.Index;
		}
		// Gesture in progress, make sure that this event comes from the finger that is dragging our dragObject
		else if(finger.Index == dragFingerIndex){
			if(gesture.Phase == ContinuousGesturePhase.Updated){	// Drag continued
				Vector3 worldPoint = CameraManager.Instance.cameraMain.ScreenToWorldPoint(new Vector3(gesture.Position.x, gesture.Position.y, 0f));
				gameObject.transform.position = new Vector3(worldPoint.x, worldPoint.y, 0f);
			}
			else{	// End drag
				dragFingerIndex = -1;

				if(itemKey == null || currentHoverKey == null){	// Dragged and released in empty area
					DoctorMatchManager.Instance.CharacterScoredWrong();
				}
				else if(itemKey == currentHoverKey){
					DoctorMatchManager.Instance.CharacterScoredRight();
				}
				else{
					DoctorMatchManager.Instance.CharacterScoredWrong();
				}
				Destroy(gameObject);
			}
		}
	}

	public void SetHoverZoneKey(string zoneKey){
		currentHoverKey = zoneKey;
	}

	void OnSpeedChange(object sender, System.EventArgs args){
		speed = assemblyLineParent.Speed;	// Get the speed from the parent assembly line
	}

	void OnGameStateChanged(object sender, GameStateArgs args){
		MinigameStates eState = args.GetGameState();
		
		switch(eState){
		case MinigameStates.GameOver:
			// stop them in their tracks
			isPaused = true;
			break;
		case MinigameStates.Paused:
			// stop them in their tracks
			isPaused = true;
			break;
		case MinigameStates.Playing:
			// resume their movement
			isPaused = false;
			break;
		}
	}

	void OnNewGame(object sender, System.EventArgs args){
		Destroy(gameObject);
	}
}
