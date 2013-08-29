using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Modified to work with UIDragPanelContents
/// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
/// </summary>
public class InventoryDragDrop : MonoBehaviour {
	public event EventHandler<InvDragDropArgs> OnItemDrop; //Event will be fired when an item is dropped
	public class InvDragDropArgs : EventArgs{
		public bool IsValidTarget{get; set;}
		public Transform ItemTransform{get; set;}
		public Transform ParentTransform{get; set;}
		public Collider TargetCollider{get; set;}
	}

	private Transform mTrans;
	private bool mIsDragging = false;
	private bool mSticky = false;
	private bool isScrolling = false;
	private bool isClickLock = false;
	private Transform mParent;						// Store parent when dragging
	private Vector3 savedLocalPosition;
	private UIDragPanelContents dragScrollScript;	// The scroll script to turn disable when item picked up

	/// <summary>
	/// Update the table, if there is one.
	/// </summary>
	private void UpdateGrid ()
	{
		UIGrid grid = NGUITools.FindInParents<UIGrid>(mTrans.parent.gameObject);
		if(grid != null) grid.repositionNow = true;
	}

	//Update the position of the Grid when the item has been destroyed
	void OnDestroy(){
		UpdateGrid();
	}

	/// <summary>
	/// Drop the dragged object.
	/// </summary>

	private void Drop ()
	{
		if(!isScrolling && !isClickLock){	// Picked up drop

			InvDragDropArgs args = new InvDragDropArgs();
			args.IsValidTarget = false;
			args.ItemTransform = gameObject.transform; 
			args.ParentTransform = mParent;
			args.TargetCollider = UICamera.lastHit.collider;
			if(OnItemDrop != null) OnItemDrop(this, args); //fire event!!
			
			if(!args.IsValidTarget){
				Debug.Log ("no hit");
				// No valid container under the mouse -- revert the item's parent
				mTrans.parent = mParent;
				
				gameObject.transform.localPosition = savedLocalPosition;		// Revert to original position
				isClickLock = false;
			}else{
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

	void Start(){
		dragScrollScript = GetComponent<UIDragPanelContents>();
	}
	/// <summary>
	/// Cache the transform.
	/// </summary>

	void Awake () { mTrans = transform; }

	/// <summary>
	/// Start the drag event and perform the dragging.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
		if (enabled && UICamera.currentTouchID > -2)
		{
			if (!mIsDragging && delta.y > 0 && !isScrolling)	// If the delta has positive Y, pick up
			{
				isClickLock = false;
				dragScrollScript.enabled = false;
				savedLocalPosition = gameObject.transform.localPosition;	// Save original position detection failed
				
				mIsDragging = true;
				mParent = mTrans.parent;
				mTrans.parent = DragDropRoot.root;
				
				Vector3 pos = mTrans.localPosition;
				pos.z = 0f;
				mTrans.localPosition = pos;

				NGUITools.MarkParentAsChanged(gameObject);
			}
			else if(mIsDragging)
			{
				// Sean: Not syncing with UIRoot manual height for ratio change account for them here
				// TODO-s Find pernament solution to this
				float ratioX = 1280f/(Screen.width * 1.0f);
				float ratioY = 800f/(Screen.height * 1.0f);
				//Debug.Log(ratioX + " " + ratioY + " " + Screen.height + " " + Screen.width);
				Vector3 newDelta = new Vector3(delta.x * ratioX, delta.y * ratioY, 0f);
				mTrans.localPosition += newDelta;
			}
			else{
				isScrolling = true;
			}
		}
	}

	/// <summary>
	/// Start or stop the drag operation.
	/// </summary>

	void OnPress (bool isPressed)
	{
		if(!mIsDragging)
			isClickLock = true;
		
		if (enabled)
		{
			if (isPressed)
			{
				if (!UICamera.current.stickyPress)
				{
					mSticky = true;
					UICamera.current.stickyPress = true;
				}
			}
			else if (mSticky)
			{
				mSticky = false;
				UICamera.current.stickyPress = false;
			}

			mIsDragging = false;
			Collider col = collider;
			if (col != null) col.enabled = !isPressed;
			if (!isPressed) Drop();
		}
	}
}
