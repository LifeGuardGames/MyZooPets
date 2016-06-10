using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manager used for item and decoration dragging
/// </summary>
public class DragItemUIManager : Singleton<DragItemUIManager>, IBeginDragHandler, IDragHandler, IEndDragHandler{
	public GameObject statsHintPrefab;
	public RectTransform draggedTransform;

	private Canvas guiCanvas;
	private Camera guiCamera;
	private string dragItemID;
	private List<GameObject> statsHints = new List<GameObject>();

	void Start(){
		guiCanvas = GetComponentInParent<Canvas>();
		guiCamera = guiCanvas.worldCamera;
	}

	//TODO add some dragging pop animation

	public void OnBeginDrag(PointerEventData eventData){
//		if(SceneUtils.CurrentScene == SceneUtils.INHALERGAME){
//			return;
//		}
//		if(StoreUIManager.Instance.IsOpen()){
//			return;
//		}
//		if(!ClickManager.Instance.CanRespondToTap(goCaller: this.gameObject)){
//			return;
//		}
//
//		if(!mIsDragging){
//			isClickLock = true;
//		}
//
//		if(enabled){
//			if (isPressed){
//				if (!UICamera.current.stickyPress){
//					mSticky = true;
//					UICamera.current.stickyPress = true;
//				}
//
//				//send on press event	
//				if(OnItemPress != null){
//					InvDragDropArgs args = new InvDragDropArgs();
//					args.ParentTransform = mTrans.parent;
//
//					OnItemPress(this, args);
//				}
//			}
//			else if(mSticky){
//				mSticky = false;
//				UICamera.current.stickyPress = false;
//			}
//
//			HUDUIManager.Instance.ToggleLabels(false);
//			mIsDragging = false;
//			Collider col = GetComponent<Collider>();
//
//			if (col != null) col.enabled = !isPressed;
//			if (!isPressed) Drop();
//		}
	}

	public void OnDrag(PointerEventData eventData){
		#if UNITY_EDITOR
		Vector3 pointerPosition = Input.mousePosition;
		#else
		Vector3 pointerPosition = Input.GetTouch(0).position;
		#endif
		pointerPosition.z = guiCanvas.planeDistance;
		draggedTransform.position = guiCamera.ScreenToWorldPoint(pointerPosition);




//		if(SceneUtils.CurrentScene == SceneUtils.INHALERGAME){
//			return;
//		}
//		if(StoreUIManager.Instance.IsOpen()){
//			return;
//		}
//		if(!ClickManager.Instance.CanRespondToTap(goCaller: this.gameObject)){
//			if(RewardManager.Instance.IsRewardingActive){
//				Drop();
//			}
//			return;
//		}
//
//		if(enabled && UICamera.currentTouchID > -2){
//			// NOTE: There are 2 types of inventories, either regular or deco, prep each for later check
//			bool isAbleToStartDrag = false;
//			if(isDecorationItem){
//				// If the delta has positive Y and scrollable, pick up, If not scrollable, just pick up
//				isAbleToStartDrag = (DecoInventoryUIManager.Instance.IsDecoInventoryScrollable() && delta.y > 0) ||	
//					(!DecoInventoryUIManager.Instance.IsDecoInventoryScrollable());
//			}
//			else{
//				// If the delta has positive Y and scrollable, pick up, If not scrollable, just pick up
//				isAbleToStartDrag = (InventoryUIManager.Instance.IsInventoryScrollable() && delta.y > 0) ||	
//					(!InventoryUIManager.Instance.IsInventoryScrollable());
//			}
//
//			if(!mIsDragging && !isScrolling && isAbleToStartDrag){
//				isClickLock = false;
//				dragScrollScript.enabled = false;
//				savedLocalPosition = gameObject.transform.localPosition;	// Save original position detection failed
//
//				HUDUIManager.Instance.ToggleLabels(true);
//
//				mIsDragging = true;
//				mParent = mTrans.parent;
//				mTrans.parent = DragDropRoot.root;
//
//				//The following code changes the position of the dragged item so that it
//				//is always ontop of the finger. This way the items won't be blocked by
//				//the finger while dragging
//
//				//the finger position needs to be adjusted by the actual ratio of the game.
//				//screen size could be different on diff devices
//				Vector3 adjustedPosition = new Vector3(UICamera.currentTouch.pos.x * CameraManager.Instance.ratioX,
//					UICamera.currentTouch.pos.y * CameraManager.Instance.ratioY,
//					0f);
//
//				//after the position is adjusted we need to transform the position from BottomLeft anchor to center anchor
//				//NGUI's default anchor is BottomLeft so UICamera.currentTouch.pos is relative to BottomLeft.
//				//The dragged item is in Anchor-Center, so we need to transform the adjusted position for the 
//				//coordinates to work
//				Vector3 dragStartingPosition = CameraManager.Instance.TransformAnchorPosition(adjustedPosition, 
//					InterfaceAnchors.BottomLeft, 
//					InterfaceAnchors.Center);
//
//				//the starting position of the dragged item is always offseted by 30 unit
//				//from where the finger is pressing.
//				dragStartingPosition.y += 30;
//				mTrans.localPosition = dragStartingPosition;
//
//				NGUITools.MarkParentAsChanged(gameObject);
//			}
//			else if(mIsDragging){
//				// if item is being dragged and is not usable items play eat anticipation
//				string invItemID = this.gameObject.name;
//				InventoryItem invItem = InventoryManager.Instance.GetItemInInventory(invItemID);
//				if(invItem != null && invItem.ItemType != ItemType.Usables){
//					if(MiniPetHUDUIManager.Instance && !MiniPetHUDUIManager.Instance.IsOpen()){
//						PetAnimationManager.Instance.WaitingToBeFed();
//					}
//					else{
//						PetAnimationManager.Instance.WaitingToBeFed();
//					}
//				}
//
//
//				Vector3 newDelta = new Vector3(delta.x * CameraManager.Instance.ratioX, delta.y * CameraManager.Instance.ratioY, 0f);
//
//				mTrans.localPosition += newDelta;
//
//				if(OnItemDrag != null){
//					OnItemDrag(this, EventArgs.Empty);
//				}
//			}
//			else{
//				isScrolling = true;
//			}
//		}
	}

	public void OnEndDrag(PointerEventData eventData){
//		if(SceneUtils.CurrentScene == SceneUtils.INHALERGAME){
//			return;
//		}
//		if(!isScrolling && !isClickLock){	// Picked up drop
//			InvDragDropArgs args = new InvDragDropArgs();
//			args.IsValidTarget = false;
//			args.ItemTransform = gameObject.transform; 
//			args.ParentTransform = mParent;
//			args.TargetCollider = UICamera.lastHit.collider;
//
//			if(!ClickManager.Instance.CanRespondToTap(goCaller: this.gameObject)){
//				args.IsValidTarget = false;
//			}
//			else{
//				if(OnItemDrop != null){
//					OnItemDrop(this, args); //fire event!!
//				}
//			}
//			if(!args.IsValidTarget){
//				// No valid container under the mouse -- revert the item's parent
//				mTrans.parent = mParent;
//
//				gameObject.transform.localPosition = savedLocalPosition;		// Revert to original position
//				isClickLock = false;
//
//				if(MiniPetHUDUIManager.Instance && !MiniPetHUDUIManager.Instance.IsOpen()){
//					PetAnimationManager.Instance.AbortFeeding();
//				}
//				else{
//					PetAnimationManager.Instance.AbortFeeding();
//				}
//			}
//			else{
//				mTrans.parent = mParent;	
//				gameObject.transform.localPosition = savedLocalPosition;		// Revert to original position
//				isClickLock = false;
//			}
//
//			UpdateGrid();
//
//			// Make all widgets update their parents
//			NGUITools.MarkParentAsChanged(gameObject);
//
//			dragScrollScript.enabled = true;	// Re-enable the drag script
//		}
//		else{
//			isScrolling = false;	// Done scrolling
//		}
	}


	public void DragConsumableShowStats(string itemID){
//		Dictionary<StatType, int> statsDict = statsItem.Stats;
//		int hintCounter = 1;
//
//		foreach(KeyValuePair<StatType, int> stat in statsDict){
//			string spriteName = "";
//			int statEffect = stat.Value;
//
//			switch(stat.Key){
//			case StatType.Hunger:
//				spriteName = "iconHunger";
//				break;
//			case StatType.Health:
//				spriteName = "iconHeart";
//				break;
//			case StatType.Fire:
//				spriteName = "iconFire";
//				break;
//			}
//
//			//instantiate the prefab
//			GameObject statHint = GameObjectUtils.AddChildWithPositionAndScale(this.gameObject, inventoryStatsHintPrefab);
//			statHint.transform.localPosition = new Vector3(statHint.transform.localPosition.x, 
//				yPositionOfFirstHint * hintCounter,
//				statHint.transform.localPosition.z);
//
//			//set value to UI element
//			string modifier = statEffect > 0 ? "+" : "";
//			statHint.transform.Find("Label").GetComponent<UILabel>().text = modifier + statEffect;
//			statHint.transform.Find("Sprite").GetComponent<UISprite>().spriteName = spriteName;
//
//			statHint.SetActive(false);
//			statsHints.Add(statHint);
//
//			hintCounter++;
//		}
	}
}
