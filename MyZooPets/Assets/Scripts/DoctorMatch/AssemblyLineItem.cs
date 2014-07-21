using UnityEngine;
using System.Collections;

public class AssemblyLineItem : MonoBehaviour {

	public string itemKey;	// Key to match with the zone key
	private string currentHoverKey = null;	// Key of the zone it is hovering over, null otherwise
	private int dragFingerIndex;
	private bool isOnAssemblyLine = false;
	private GameObject destination;
	private float speed;
	
	public void SetupItem(AssemblyLineController assemblyLineController){
		speed = assemblyLineController.Speed;
		destination = assemblyLineController.endLocation;
		isOnAssemblyLine = true;	// Start moving the object
	}

	public void SetSpeed(float newSpeed){
		speed = newSpeed;
	}

	void Update(){
		// While the item is not being dragged
		if(isOnAssemblyLine){
			transform.position = Vector3.MoveTowards(transform.position, destination.transform.position, Time.deltaTime * speed);
			if(transform.position == destination.transform.position){
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

				/// DO REWARD CHECKING HERE ///
				if(itemKey == currentHoverKey){
					DoctorMatchManager.Instance.CharacterScoredRight();
				}
				else{
					DoctorMatchManager.Instance.CharacterScoredWrong();
				}
				///////////////////////////////

				Destroy(gameObject);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		DoctorMatchZone zoneScript = collider.gameObject.GetComponent<DoctorMatchZone>();
		if(zoneScript){
			currentHoverKey = collider.gameObject.GetComponent<DoctorMatchZone>().zoneKey;
		}
	}

	void OnTriggerExit2D(Collider2D collider){
		currentHoverKey = null;
	}
}
