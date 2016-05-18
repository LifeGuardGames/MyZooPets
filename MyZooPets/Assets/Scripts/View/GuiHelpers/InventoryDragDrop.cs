using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Modified to work with UIDragPanelContents
/// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
/// </summary>
public class InventoryDragDrop : MonoBehaviour{
	public class InvDragDropArgs : EventArgs{
		public bool IsValidTarget{get; set;}
		public Transform ItemTransform{get; set;}
		public Transform ParentTransform{get; set;}
		public Collider TargetCollider{get; set;}
	}

	public event EventHandler<InvDragDropArgs> OnItemDrop; //Event will be fired when an item is dropped
	public event EventHandler<InvDragDropArgs> OnItemPress; //Event will be fired when item is pressed
	public event EventHandler<EventArgs> OnItemDrag;

	public bool isDecorationItem = false;

	private Transform mTrans;
	private bool mIsDragging = false;
	private bool mSticky = false;
	private bool isScrolling = false;
	private bool isClickLock = false;
	private Transform mParent;						// Store parent when dragging
	private Vector3 savedLocalPosition;
	private UIDragPanelContents dragScrollScript;	// The scroll script to turn disable when item picked up
	
	void Awake(){ 
		mTrans = transform; 	
	}
	
	void Start(){
		dragScrollScript = GetComponent<UIDragPanelContents>();
		RewardManager.OnAllRewardsDone += reAddClick;

	}

	/// <summary>
	/// Update the table, if there is one.
	/// </summary>
	private void UpdateGrid(){
		UIGrid grid = NGUITools.FindInParents<UIGrid>(mTrans.parent.gameObject);
		if(grid != null){
			grid.repositionNow = true;
		}
	}

	//Update the position of the Grid when the item has been destroyed
	void OnDestroy(){
		UpdateGrid();
		RewardManager.OnAllRewardsDone -= reAddClick;
	}

	public void reAddClick(object sender, EventArgs args){
		if(this.GetComponent<Collider>() != null){
			this.GetComponent<Collider>().enabled = true;
		}
	}

	/// <summary>
	/// Drop the dragged object.
	/// </summary>

	private void Drop(){
		if(Application.loadedLevelName == SceneUtils.INHALERGAME){
			return;
		}
		if(!isScrolling && !isClickLock){	// Picked up drop
			InvDragDropArgs args = new InvDragDropArgs();
			args.IsValidTarget = false;
			args.ItemTransform = gameObject.transform; 
			args.ParentTransform = mParent;
			args.TargetCollider = UICamera.lastHit.collider;

			if(!ClickManager.Instance.CanRespondToTap(goCaller: this.gameObject)){
				args.IsValidTarget = false;
			}
			else{
				if(OnItemDrop != null){
					OnItemDrop(this, args); //fire event!!
				}
			}
			if(!args.IsValidTarget){
				// No valid container under the mouse -- revert the item's parent
				mTrans.parent = mParent;
				
				gameObject.transform.localPosition = savedLocalPosition;		// Revert to original position
				isClickLock = false;

				if(MiniPetHUDUIManager.Instance && !MiniPetHUDUIManager.Instance.IsOpen()){
					PetAnimationManager.Instance.AbortFeeding();
				}
				else{
					PetAnimationManager.Instance.AbortFeeding();
				}
			}
			else{
				mTrans.parent = mParent;	
				gameObject.transform.localPosition = savedLocalPosition;		// Revert to original position
				isClickLock = false;
			}
			
			UpdateGrid();

			// Make all widgets update their parents
			NGUITools.MarkParentAsChanged(gameObject);
			
			dragScrollScript.enabled = true;	// Re-enable the drag script
		}
		else{
			isScrolling = false;	// Done scrolling
		}
	}

	/// <summary>
	/// Start the drag event and perform the dragging.
	/// </summary>
	void OnDrag(Vector2 delta){
		if(Application.loadedLevelName == SceneUtils.INHALERGAME){
			return;
		}
		if(StoreUIManager.Instance.IsOpen()){
			return;
		}
		if(!ClickManager.Instance.CanRespondToTap(goCaller: this.gameObject)){
			if(RewardManager.Instance.IsRewardingActive){
				Drop();
			}
			return;
		}

		if(enabled && UICamera.currentTouchID > -2){
			// NOTE: There are 2 types of inventories, either regular or deco, prep each for later check
			bool isAbleToStartDrag = false;
			if(isDecorationItem){
				// If the delta has positive Y and scrollable, pick up, If not scrollable, just pick up
				isAbleToStartDrag = (DecoInventoryUIManager.Instance.IsDecoInventoryScrollable() && delta.y > 0) ||	
									(!DecoInventoryUIManager.Instance.IsDecoInventoryScrollable());
			}
			else{
				// If the delta has positive Y and scrollable, pick up, If not scrollable, just pick up
				isAbleToStartDrag = (InventoryUIManager.Instance.IsInventoryScrollable() && delta.y > 0) ||	
									(!InventoryUIManager.Instance.IsInventoryScrollable());
			}

			if(!mIsDragging && !isScrolling && isAbleToStartDrag){
				isClickLock = false;
				dragScrollScript.enabled = false;
				savedLocalPosition = gameObject.transform.localPosition;	// Save original position detection failed

				HUDUIManager.Instance.ToggleLabels(true);

				mIsDragging = true;
				mParent = mTrans.parent;
				mTrans.parent = DragDropRoot.root;

				//The following code changes the position of the dragged item so that it
				//is always ontop of the finger. This way the items won't be blocked by
				//the finger while dragging

				//the finger position needs to be adjusted by the actual ratio of the game.
				//screen size could be different on diff devices
				Vector3 adjustedPosition = new Vector3(UICamera.currentTouch.pos.x * CameraManager.Instance.ratioX,
				                                  UICamera.currentTouch.pos.y * CameraManager.Instance.ratioY,
				                                  0f);

				//after the position is adjusted we need to transform the position from BottomLeft anchor to center anchor
				//NGUI's default anchor is BottomLeft so UICamera.currentTouch.pos is relative to BottomLeft.
				//The dragged item is in Anchor-Center, so we need to transform the adjusted position for the 
				//coordinates to work
				Vector3 dragStartingPosition = CameraManager.Instance.TransformAnchorPosition(adjustedPosition, 
				                                               InterfaceAnchors.BottomLeft, 
				                                               InterfaceAnchors.Center);

				//the starting position of the dragged item is always offseted by 30 unit
				//from where the finger is pressing.
				dragStartingPosition.y += 30;
				mTrans.localPosition = dragStartingPosition;

				NGUITools.MarkParentAsChanged(gameObject);
			}
			else if(mIsDragging){
				// if item is being dragged and is not usable items play eat anticipation
				string invItemID = this.gameObject.name;
				InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);
				if(invItem != null && invItem.ItemType != ItemType.Usables){
					if(MiniPetHUDUIManager.Instance && !MiniPetHUDUIManager.Instance.IsOpen()){
						PetAnimationManager.Instance.WaitingToBeFed();
					}
					else{
						PetAnimationManager.Instance.WaitingToBeFed();
					}
				}
					

				Vector3 newDelta = new Vector3(delta.x * CameraManager.Instance.ratioX, delta.y * CameraManager.Instance.ratioY, 0f);
				
				mTrans.localPosition += newDelta;

				if(OnItemDrag != null){
					OnItemDrag(this, EventArgs.Empty);
				}
			}
			else{
				isScrolling = true;
			}
		}
	}

	/// <summary>
	/// Start or stop the drag operation.
	/// </summary>

	void OnPress(bool isPressed){
		if(Application.loadedLevelName == SceneUtils.INHALERGAME){
			return;
		}
		if(StoreUIManager.Instance.IsOpen()){
			return;
		}
		if(!ClickManager.Instance.CanRespondToTap(goCaller: this.gameObject)){
			return;
		}

		if(!mIsDragging){
			isClickLock = true;
		}
		
		if(enabled){
			if (isPressed){
				if (!UICamera.current.stickyPress){
					mSticky = true;
					UICamera.current.stickyPress = true;
				}

				//send on press event	
				if(OnItemPress != null){
					InvDragDropArgs args = new InvDragDropArgs();
					args.ParentTransform = mTrans.parent;

					OnItemPress(this, args);
				}
			}
			else if(mSticky){
				mSticky = false;
				UICamera.current.stickyPress = false;
			}
			
			HUDUIManager.Instance.ToggleLabels(false);
			mIsDragging = false;
			Collider col = GetComponent<Collider>();

			if (col != null) col.enabled = !isPressed;
			if (!isPressed) Drop();
		}
	}
}
