using UnityEngine;
using System.Collections;

/// <summary>
/// Script to control Minipet and contains the basic properties of a minipet.
/// Should attach this script to the highest parent in the minipet prefab
/// </summary>
public class MiniPet : MonoBehaviour {
	public Animator animator;
	public ParticleSystem bubbleParticle;
	public ParticleSystem dirtyParticle;
	public MiniPetSpeechAI miniPetSpeechAI;

	private string id;
	private string name;
	private float currentDistanceInCentimeters = 0;
	private float targetDistanceInCentimetersForCleanGesture = 300;

	void Start(){
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;

		RefreshMiniPetState();
	}
	
	void OnDestroy(){
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused)
			RefreshMiniPetState();
	}

	void OnTap(TapGesture gesture){
		bool isUIOpened = MiniPetHUDUIManager.Instance.IsOpen();
		if(!isUIOpened){
			Vector3 positionOffset = new Vector3(3, 4, -11);
			Vector3 position = this.transform.position + positionOffset;
			Vector3 rotation = new Vector3(12, 0, 0);

			ClickManager.Instance.Lock(mode: UIModeTypes.MiniPet);
			CameraManager.Instance.ZoomToTarget(position, rotation, 1f, this.gameObject);
		}
		else{
			string colliderName = gesture.Selection.collider.name;
			
			if(colliderName == this.gameObject.name){
				animator.SetTrigger("GestureWiggle");
				MiniPetManager.Instance.SetTickle(id, true);
				animator.SetBool("Sad", false);
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

		switch(gesture.Phase){
		case ContinuousGesturePhase.Started:
			bubbleParticle.Play();

			MoveBubbleParticleWithUserTouch(gesture);
			break;
		case ContinuousGesturePhase.Updated:
			MoveBubbleParticleWithUserTouch(gesture);

			float totalMoveXInCentimeters = Mathf.Abs(gesture.TotalMove.Centimeters().x);
			float totalMoveYInCentimeters = Mathf.Abs(gesture.TotalMove.Centimeters().y);

			currentDistanceInCentimeters += (totalMoveXInCentimeters + totalMoveYInCentimeters);

			//if clean gesture is recognized. stop dirty particle and play happy animation
			if(currentDistanceInCentimeters >= targetDistanceInCentimetersForCleanGesture){
				MiniPetManager.Instance.SetCleaned(id, true);
				MiniPetManager.Instance.SetFirstTimeCleaning(id);
				dirtyParticle.Stop();
				animator.SetTrigger("Happy");
				currentDistanceInCentimeters = 0;
			}

			break;
		case ContinuousGesturePhase.Ended:
			bubbleParticle.Stop();
			break;
		}
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

	private void MoveBubbleParticleWithUserTouch(DragGesture gesture){
		bool isDraggingOnMP = gesture.Raycast.Hit3D.collider &&
			gesture.Raycast.Hit3D.collider.name == this.gameObject.name;


		if(isDraggingOnMP){
			//z position is constant
			Vector3 touchPositionInWorld = 
				Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x, gesture.Position.y, 10));
			
			bubbleParticle.transform.position = touchPositionInWorld;
		}
		else{
			bubbleParticle.Stop();
		}
	}

	private void RefreshMiniPetState(){
		//check if pet is sad and dirty
		bool isTickled = MiniPetManager.Instance.IsTickled(id);
		bool isCleaned = MiniPetManager.Instance.IsCleaned(id);
		
		if(!isTickled)
			animator.SetBool("Sad", true);
		else
			animator.SetBool("Sad", false);
		
		if(!isCleaned){
			dirtyParticle.Play();
			miniPetSpeechAI.ShowDirtyMsg();
		}
		else{
			dirtyParticle.Stop();
		}
	}
	
	private void CameraMoveDone() {
		ClickManager.Instance.ReleaseLock();
		MiniPetHUDUIManager.Instance.SelectedMiniPetID = id;
		MiniPetHUDUIManager.Instance.SelectedMiniPetName = name;
		MiniPetHUDUIManager.Instance.SelectedMiniPetGameObject = this.gameObject;
		MiniPetHUDUIManager.Instance.OpenUI();
	}

	private void ItemDroppedOnTargetEventHandler(object sender, InventoryDragDrop.InvDragDropArgs args){
		bool isLevelUpAnimationLockOn = MiniPetHUDUIManager.Instance.IsLevelUpAnimationLockOn;

		if(args.TargetCollider.name == this.gameObject.name && 
		   !isLevelUpAnimationLockOn){
			string invItemID = args.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);

			//check if minipet needs food
			if(MiniPetManager.Instance.CanModifyFoodXP(id)){
				//use item if so
				args.IsValidTarget = true;
				
				//notify inventory logic that this item is being used
				InventoryLogic.Instance.UseMiniPetItem(invItemID);
				MiniPetManager.Instance.IncreaseFoodXP(id);

				animator.SetTrigger("Happy");
			}
			else{
				bool isTickled = MiniPetManager.Instance.IsTickled(id);
				bool isCleaned = MiniPetManager.Instance.IsCleaned(id);
				bool isMaxLevel = MiniPetManager.Instance.IsMaxLevel(id);

				if(!isTickled){
					miniPetSpeechAI.ShowSadMsg();
				}
				else if(!isCleaned){
					miniPetSpeechAI.ShowDirtyMsg();
				}
				else if(isMaxLevel){
					miniPetSpeechAI.ShowMaxLevelMsg();
				}
				else{}
			}
		}
	}
}
