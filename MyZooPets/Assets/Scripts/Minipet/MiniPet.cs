using UnityEngine;
using System.Collections;

/// <summary>
/// Script to control Minipet and contains the basic properties of a minipet.
/// Should attach this script to the highest parent in the minipet prefab
/// </summary>
public class MiniPet : MonoBehaviour {
	public Animator animator;
	private string id;
	private string name;
	private FingerGestures.SwipeDirection lastDirection;

	private int maxNumOfCleaningGesture = 5;
	private int currentNumOfCleaningGesture = 0;

	void Start(){
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;
	}
	
	void OnDestroy(){
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
	}

	private void CameraMoveDone() {
		ClickManager.Instance.ReleaseLock();
		MiniPetHUDUIManager.Instance.SelectedMiniPetID = id;
		MiniPetHUDUIManager.Instance.SelectedMiniPetName = name;
		MiniPetHUDUIManager.Instance.OpenUI();
	}

	void OnTap(TapGesture gesture){
		bool isUIOpened = MiniPetHUDUIManager.Instance.IsOpen();
		if(!isUIOpened){
			Vector3 positionOffset = new Vector3(3, 4, -11);
			Vector3 position = this.transform.position + positionOffset;
			Vector3 rotation = new Vector3(12, 0, 0);
//			MiniPetHUDUIManager.Instance.OpenUI();
			ClickManager.Instance.Lock(mode: UIModeTypes.MiniPet);
			CameraManager.Instance.ZoomToTarget(position, rotation, 1f, this.gameObject);
		}
		else{
			string colliderName = gesture.Selection.collider.name;
			
			if(colliderName == this.gameObject.name){
				//need to check clickmanager if can respond to tap
//				if(ClickManager.Instance.CanRespondToTap()){
					//do some
					Debug.Log("Minipet does some funny animation here");
					animator.SetTrigger("gestureWiggle");
					MiniPetManager.Instance.SetTickle(id, true);
//				}
			}
		}
	}

	/// <summary>
	/// Raises the drag event.
	/// Note: This needs to be here so it catches the OnDrag event sent out by UICamera
	/// which belongs to NGUI. Finger Gesture also send out the same event so they
	/// need to be specified otherwise error will be thrown
	/// </summary>
	/// <param name="delta">Delta.</param>
	void OnDrag(Vector2 delta){}

	void OnDrag(DragGesture gesture){
		bool isUIOpened = MiniPetHUDUIManager.Instance.IsOpen();
		if(!isUIOpened) return;

		Vector2 dir = gesture.TotalMove.normalized; // you could also use Gesture.DeltaMove to get the movement change since last frame
		FingerGestures.SwipeDirection swipeDir = FingerGestures.GetSwipeDirection(dir);

		if(swipeDir != lastDirection &&
		   swipeDir != FingerGestures.SwipeDirection.UpperLeftDiagonal &&
		   swipeDir != FingerGestures.SwipeDirection.UpperRightDiagonal &&
		   swipeDir != FingerGestures.SwipeDirection.LowerLeftDiagonal &&
		   swipeDir != FingerGestures.SwipeDirection.LowerRightDiagonal){

			currentNumOfCleaningGesture++;
			if(currentNumOfCleaningGesture == maxNumOfCleaningGesture){
				Debug.Log("pet cleaned");
				MiniPetManager.Instance.SetCleaned(id, true);
				currentNumOfCleaningGesture = 0;
			}
		}
		
		lastDirection = swipeDir;
	}

	/// <summary>
	/// Pass in the immutable data so this specific MiniPet instantiate can be instantiated
	/// with the proper information.
	/// </summary>
	/// <param name="data">ImmutableDataMiniPet.</param>
	public void Init(ImmutableDataMiniPet data){
		this.id = data.ID;
		this.name = data.Name;
	}

	private void ItemDroppedOnTargetEventHandler(object sender, InventoryDragDrop.InvDragDropArgs args){
		if(args.TargetCollider.name == this.gameObject.name){
			string invItemID = args.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);


			//check if minipet needs food
			if(MiniPetManager.Instance.CanModifyFoodXP(id)){
				//use item if so
				args.IsValidTarget = true;
				Debug.Log("item dropped on mini pet");
				
				//notify inventory logic that this item is being used
				InventoryLogic.Instance.UseMiniPetItem(invItemID);
				MiniPetManager.Instance.IncreaseFoodXP(id);

				animator.SetTrigger("happy");
			}
			else{
				//say sth if minipet doesn't want food anymore

				//if not tickled show not tickled message

				//if not cleaned show not cleaned message

				//if max level show max level message
			}

		}
	}
}
