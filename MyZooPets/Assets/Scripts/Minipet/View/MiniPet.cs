using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Script to control Minipet and contains the basic properties of a minipet.
/// Should attach this script to the highest parent in the minipet prefab
/// </summary>
public class MiniPet : MonoBehaviour {
	public MiniPetAnimationManager animationManager;
//	public ParticleSystem bubbleParticle;
	//public ParticleSystem dirtyParticle;
	public MiniPetSpeechAI miniPetSpeechAI;
	public Transform spawnItemTransform;
	public GameObject flippable;
	public GameObject eggParent;
	public Animation eggAnimation;
	public MinipetEggClickController eggClickController;

	public Vector3 zoomPositionOffset = new Vector3(-3, 4, -11);
	public Vector3 zoomRotation = new Vector3(12, 0, 0);
	public string minipetName = "general";
	private bool isVisible;
	private bool isHatchedAux;
	public Camera nguiCamera;
	protected string id; //pet id
	public string ID{
		get{return id;}
	}
	private new string name;
	
	private float currentDistanceInCentimeters = 0;
	private float targetDistanceInCentimetersForCleanGesture = 300; //clean gestures will be recognized after the finger moved 300cm (both x and y position)

	private float tickleTimer = 0;
	private float timeBeforeTickleAnimationStops = 3f; //tickle animation will be stopped in 3 seconds

	private bool isMiniPetColliderLocked = false; //use this to disable click on minipet when zooming in
	public bool isFinishEating = false; //F: Need to finish the eating logic after camera zooms in
	private string invItemID; //local reference to the item that is dropped on the minipet


	void Start(){
		nguiCamera = GameObject.Find("Camera").camera;
		MiniPetHUDUIManager.Instance.OnManagerOpen += ShouldPauseIdleAnimations;
		MiniPetHUDUIManager.OnLevelUpAnimationCompleted += LevelUpEventHandler;
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;
		MiniPetManager.MiniPetStatusUpdate += UpdateAnimation;
		//MiniPetManager.Instance.CheckToRefreshMiniPetStatus(id);
		Debug.Log(DataManager.Instance.GameData.MiniPetLocations.GetHunger(id));
		isFinishEating = DataManager.Instance.GameData.MiniPetLocations.GetHunger(id);
		
		RefreshUnlockState();
	}

	// Check to see if you want to display pet or egg
	public void RefreshUnlockState(){
		ToggleHatched(DataManager.Instance.GameData.MiniPets.IsMiniPetUnlocked(ID));
		RefreshMiniPetUIState(isForceHideFoodMsg:true);
	}

	public void ToggleHatched(bool isHatched){
		isHatchedAux = isHatched;
		if(isHatched){
			eggParent.SetActive(false);
			eggAnimation.animation.Stop();
			flippable.SetActive(true);
			gameObject.collider.enabled = true;
		//	bubbleParticle.gameObject.SetActive(true);
		//	dirtyParticle.gameObject.SetActive(true);
			if(eggClickController != null){	// Remove unused components on the egg parent
				Destroy(eggClickController);
				Destroy(eggClickController.collider);
			}
			isVisible = true;
		}
		else{	// Not hatched yet
			eggParent.SetActive(true);
			eggAnimation.animation.Play();
			flippable.SetActive(false);
			gameObject.collider.enabled = false;
//			bubbleParticle.gameObject.SetActive(false);
		//	dirtyParticle.gameObject.SetActive(false);
			isVisible = false;
		}
	}
	
	void OnDestroy(){
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
		if(MiniPetHUDUIManager.Instance)
			MiniPetHUDUIManager.Instance.OnManagerOpen -= ShouldPauseIdleAnimations;
		MiniPetHUDUIManager.OnLevelUpAnimationCompleted -= LevelUpEventHandler;
		MiniPetManager.MiniPetStatusUpdate -= UpdateAnimation;
	}

	void Update(){
		if(isVisible){
			//count down starts if tickling animation is playing.
			if(animationManager.IsTickling()){
				tickleTimer += Time.deltaTime;

				//turn tickling animation off after certain time
				if(tickleTimer > timeBeforeTickleAnimationStops){
					tickleTimer = 0;
					animationManager.StopTickling();
				}
			}
		}
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
		//	MiniPetManager.Instance.CheckToRefreshMiniPetStatus(id);

			RefreshMiniPetUIState(isForceHideFoodMsg:true);
		}
	}
	
	protected virtual void OnTap(TapGesture gesture){
		if(!IsTouchingNGUI(gesture.Position)){
		bool isUIOpened = MiniPetHUDUIManager.Instance.IsOpen();
		bool isModeLockEmpty = ClickManager.Instance.IsModeLockEmpty;

		if(!isMiniPetColliderLocked){
			if(!isUIOpened && isModeLockEmpty){
				ZoomInToMiniPet();
			}
			else{

				UIModeTypes currentLockMode = ClickManager.Instance.CurrentMode;

				if(currentLockMode == UIModeTypes.MiniPet){
					string colliderName = gesture.Selection.collider.name;
					//bool isFirstTimeCleaning = DataManager.Instance.GameData.MiniPets.IsFirstTimeCleaning;
					
					//only allow tap gesture if cleaning tutorial is finished
				//	if(colliderName == this.gameObject.name && !isFirstTimeCleaning){
					if(colliderName == this.gameObject.name){
						//if tickling animation is still playing reset timer
						if(animationManager.IsTickling()){
							tickleTimer = 0;
						}
						else{
							animationManager.StartTickling();
							
						//	bool isTickled = MiniPetManager.Instance.IsTickled(id);
						/*	if(!isTickled)
								MiniPetManager.Instance.SetTickle(id, true);*/
							
							//MiniPetManager.Instance.IsFirstTimeTickling = false;
							}
						}
					}
				}
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
		UIModeTypes currentLockMode = ClickManager.Instance.CurrentMode;
		if(!isUIOpened || currentLockMode != UIModeTypes.MiniPet) return;

		string colliderName = "";
		if(gesture.Selection)
			 colliderName = gesture.Selection.collider.name;

		if(colliderName == this.gameObject.name){
			switch(gesture.Phase){
			case ContinuousGesturePhase.Started:
				//bubbleParticle.Play();
				//MoveBubbleParticleWithUserTouch(gesture);
				break;
			case ContinuousGesturePhase.Updated:
			//	if(!bubbleParticle.isPlaying)
				//	bubbleParticle.Play();

				//MoveBubbleParticleWithUserTouch(gesture);
				
				float totalMoveXInCentimeters = Mathf.Abs(gesture.TotalMove.Centimeters().x);
				float totalMoveYInCentimeters = Mathf.Abs(gesture.TotalMove.Centimeters().y);
				
				currentDistanceInCentimeters += (totalMoveXInCentimeters + totalMoveYInCentimeters);
				
				//if clean gesture is recognized. stop dirty particle and play happy animation
				if(currentDistanceInCentimeters >= targetDistanceInCentimetersForCleanGesture){
				//	MiniPetManager.Instance.SetCleaned(id, true);
					//MiniPetManager.Instance.IsFirstTimeCleaning = false;
					//dirtyParticle.Stop();
					animationManager.Cheer();
					currentDistanceInCentimeters = 0;
				}
				break;
			case ContinuousGesturePhase.Ended:
				//Particle.Stop();
				break;
			}
		}
		else{
			//bubbleParticle.Stop();
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

	private void ZoomInToMiniPet(){
		Vector3 position = this.transform.position + zoomPositionOffset;

		isMiniPetColliderLocked = true;

		CameraManager.Callback cameraDoneFunction = delegate(){
			CameraMoveDone();
		};
		CameraManager.Instance.ZoomToTarget(position, zoomRotation, 1f, cameraDoneFunction);
	}

	/// <summary>
	/// Logic to run after the camera has zoomed into the minipet
	/// </summary>
	private void CameraMoveDone() {
		isMiniPetColliderLocked = false;
		MiniPetHUDUIManager.Instance.SelectedMiniPetID = id;
		MiniPetHUDUIManager.Instance.SelectedMiniPetName = name;
		MiniPetHUDUIManager.Instance.SelectedMiniPetGameObject = this.gameObject;
		MiniPetHUDUIManager.Instance.OpenUI();
		
		//if pet not finish eating yet. finish eating logic
		if(!isFinishEating){
			//InventoryLogic.Instance.UseMiniPetItem(invItemID);
			//MiniPetManager.Instance.IncreaseXP(id);
			//animationManager.Eat();
		}
		//else check if tickle and cleaning is done. if both done 
		else{
			/*bool isTickled = MiniPetManager.Instance.IsTickled(id);
			bool isCleaned = MiniPetManager.Instance.IsCleaned(id);
			if(isTickled && isCleaned && MiniPetManager.Instance.CanModifyXP(id)){*/
				Invoke("ShowFoodPreferenceMessage", 1f);
			}
		//}
	}

	private void ShouldPauseIdleAnimations(object sender, UIManagerEventArgs args){
		// Check if the minipet is hatched first
		if(isHatchedAux){
			if(args.Opening)
				animationManager.IsRunningIdleAnimations = false;
			else
				animationManager.IsRunningIdleAnimations = true;
		}
	}
	public virtual void FinishEating(){
		isFinishEating = true;
		DataManager.Instance.GameData.MiniPetLocations.SaveHunger(id,isFinishEating);
		Debug.Log(DataManager.Instance.GameData.MiniPetLocations.GetHunger(id));
	}
	/// <summary>
	/// Updates the animation when minipet status is also updated
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void UpdateAnimation(object sender, MiniPetManager.StatusUpdateEventArgs args){
		MiniPetManager.UpdateStatuses status = args.UpdateStatus;
	
		if(status == MiniPetManager.UpdateStatuses.Tickle){
			RefreshMiniPetUIState();
		}
	}

	/*private void MoveBubbleParticleWithUserTouch(DragGesture gesture){
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
*/
	private void RefreshMiniPetUIState(bool isForceHideFoodMsg = false){
		if(isHatchedAux){
			//check if pet is sad and dirty
			//bool isTickled = MiniPetManager.Instance.IsTickled(id);
			//bool isCleaned = MiniPetManager.Instance.IsCleaned(id);
			
			//if(!isTickled)
				animationManager.Sad();
			//else
				animationManager.NotSad();
			
		//	if(!isCleaned){
				//dirtyParticle.Play();
			//}
			//else{
			//	dirtyParticle.Stop();
		//	}

			//if(isTickled && isCleaned){
				// Sometimes we want to control when the food message is hidden/shown
				if(!isForceHideFoodMsg){
					if(MiniPetManager.Instance.CanModifyXP(id)){
						Invoke("ShowFoodPreferenceMessage", 1f);
					//}
				}
			}
		}
	}

	private void ShowFoodPreferenceMessage(){
		string preferredFoodID = MiniPetManager.Instance.GetFoodPreference(id);
		Item item = ItemLogic.Instance.GetItem(preferredFoodID);
		miniPetSpeechAI.ShowFoodPreferenceMsg(item.TextureName);
	}

	public void TryShowDirtyOrSadMessage(){
		//bool isTickled = MiniPetManager.Instance.IsTickled(id);
		//bool isCleaned = MiniPetManager.Instance.IsCleaned(id);

	//	if(!isTickled){
			miniPetSpeechAI.ShowSadMsg(name);
		//}
	//	else if(!isCleaned){
			miniPetSpeechAI.ShowDirtyMsg(name);
		//}
	}

	/// <summary>
	/// When item is dropped on MP do the necessary check and carry out the action.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void ItemDroppedOnTargetEventHandler(object sender, InventoryDragDrop.InvDragDropArgs args){
		bool isLevelUpAnimationLockOn = MiniPetHUDUIManager.Instance.IsLevelUpAnimationLockOn;
		bool isUIOpened = MiniPetHUDUIManager.Instance.IsOpen();

		if(args.TargetCollider.name == this.gameObject.name && !isLevelUpAnimationLockOn){

			invItemID = args.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);
			string preferredFoodID = "";

			//check if minipet needs food
			if(MiniPetManager.Instance.CanModifyXP(id)){
				preferredFoodID = MiniPetManager.Instance.GetFoodPreference(id);

				//check if minipet wants this food
				if(preferredFoodID == invItem.ItemID){
					//use item if so
					args.IsValidTarget = true;

					if(!isUIOpened){
						ZoomInToMiniPet();
						isFinishEating = false;
					}
					else{
						//notify inventory logic that this item is being used
						InventoryLogic.Instance.UseMiniPetItem(invItemID);
						MiniPetManager.Instance.IncreaseXP(id);
						FinishEating();
						animationManager.Eat();
					}
				}
				// show notification that the mp wants a specific food
				else{
					Item item = ItemLogic.Instance.GetItem(preferredFoodID);
					miniPetSpeechAI.ShowFoodPreferenceMsg(item.TextureName);
				}
			}
			else{
				//bool isTickled = MiniPetManager.Instance.IsTickled(id);
				//bool isCleaned = MiniPetManager.Instance.IsCleaned(id);
				bool isMaxLevel = MiniPetManager.Instance.IsMaxLevel(id);

			/*	if(!isTickled){
					miniPetSpeechAI.ShowSadMsg(name);
				}
				else if(!isCleaned){
					miniPetSpeechAI.ShowDirtyMsg(name);
				}*/
				 if(isMaxLevel){
					miniPetSpeechAI.ShowMaxLevelMsg(name);
				}
				else{}
			}
		}
	}

	private void LevelUpEventHandler(object sender, EventArgs args){
		if(MiniPetHUDUIManager.Instance.SelectedMiniPetID == id){
			animationManager.Cheer();
			Debug.Log("RECEIVING GIFT HERE");
//			StatsController.Instance.ChangeStats(deltaGems: 1, gemsLoc: transform.position, is3DObject: true, animDelay: 1.5f);
		}
	}

	public void ToggleVisibility(bool isVisible){

		this.isVisible = isVisible;

		if(this.isVisible){
			//bubbleParticle.gameObject.SetActive(true);
			//dirtyParticle.gameObject.SetActive(true);

			ToggleHatched(isHatchedAux);	// Keep track of it internally already DWID
		}
		else{
		//	bubbleParticle.gameObject.SetActive(false);
		//	dirtyParticle.gameObject.SetActive(false);
			flippable.SetActive(false);
			eggParent.SetActive(false);
			gameObject.collider.enabled = false;
		}
	}
	//True: if finger touches NGUI 
	/// <summary>
	/// Determines whether if the touch is touching NGUI element
	/// </summary>
	/// <returns><c>true</c> if this instance is touching NGUI; otherwise, <c>false</c>.</returns>
	/// <param name="screenPos">Screen position.</param>
	private bool IsTouchingNGUI(Vector2 screenPos){
		Ray ray = nguiCamera.ScreenPointToRay(screenPos);
		RaycastHit hit;
		int layerMask = 1 << 10; 
		bool isOnNGUILayer = false;
		
		// Raycast
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
			isOnNGUILayer = true;
		}
		return isOnNGUILayer;
	}
}
