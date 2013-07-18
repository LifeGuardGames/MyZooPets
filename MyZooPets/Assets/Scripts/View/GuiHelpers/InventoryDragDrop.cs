using UnityEngine;
using System.Collections;
using System;

public class InventoryDragDrop : MonoBehaviour {

	/// <summary>
	/// Modified to work with UIDragPanelContents
	/// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
	/// </summary>

	public class InvDragDropArgs : EventArgs{
		public bool IsValidTarget{get; set;}
		public Transform ItemTransform{get; set;}
		public Transform ParentTransform{get; set;}
		public Collider TargetCollider{get; set;}
	}

	//============Event==============
	public delegate void DragDropCallBack(object sender, InvDragDropArgs e);
	public event DragDropCallBack OnItemDrop; //Event will be fired when an item is dropped
	//==============================

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
		//Debug.Log("dropped");
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

	/// <summary>
	/// Cache the transform.
	/// </summary>

	void Awake () { 
		mTrans = transform; 
		dragScrollScript = GetComponent<UIDragPanelContents>();
	}

	/// <summary>
	/// Start the drag event and perform the dragging.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
		//Debug.Log("drag");
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
				mTrans.localPosition += (Vector3)delta;
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
		
		//Debug.Log("pressed");
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
