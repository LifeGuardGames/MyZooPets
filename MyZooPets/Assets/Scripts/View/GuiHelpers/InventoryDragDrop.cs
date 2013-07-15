using UnityEngine;
using System.Collections;

public class InventoryDragDrop : MonoBehaviour {

	/// <summary>
	/// Modified to work with UIDragPanelContents
	/// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
	/// </summary>

	public GameObject prefab;

	Transform mTrans;
	bool mIsDragging = false;
	bool mSticky = false;
	bool isScrolling = false;
	bool isClickLock = false;
	Transform mParent;						// Store parent when dragging
	Vector3 savedLocalPosition;
	UIDragPanelContents dragScrollScript;	// The scroll script to turn disable when item picked up
	
	GameObject inventoryGUIObject;
	InventoryGUI inventoryGUI;
	
	void Start(){
		dragScrollScript = GetComponent<UIDragPanelContents>();
		inventoryGUIObject = GameObject.Find("Panel");
		inventoryGUI = inventoryGUIObject.GetComponent<InventoryGUI>();
	}
	
	/// <summary>
	/// Update the table, if there is one.
	/// </summary>

	void UpdateTable ()
	{
		UITable table = NGUITools.FindInParents<UITable>(gameObject);
		if (table != null) table.repositionNow = true;
	}

	/// <summary>
	/// Drop the dragged object.
	/// </summary>

	void Drop ()
	{
		Debug.Log("dropped");
		if(!isScrolling && !isClickLock){	// Picked up drop
//			// Is there a droppable container?
//			Collider col = UICamera.lastHit.collider;
//			Debug.Log ("hit! " + gameObject.name);
//			DragDropContainer container = (col != null) ? col.gameObject.GetComponent<DragDropContainer>() : null;
//			if (container != null)
//			{
//				// Container found -- parent this object to the container
//				mTrans.parent = container.transform;
//	
//				Vector3 pos = mTrans.localPosition;
//				pos.z = 0f;
//				mTrans.localPosition = pos;
//			}
			if(inventoryGUI.NotifyDroppedItem(int.Parse(gameObject.name))){
				Debug.Log("hit! PLEASE DELETE OBJECT NOW");
			}
			else
			{
				Debug.Log ("no hit");
				// No valid container under the mouse -- revert the item's parent
				mTrans.parent = mParent;
				
				gameObject.transform.localPosition = savedLocalPosition;		// Revert to original position
				isClickLock = false;
			}
	
			// Notify the table of this change
			UpdateTable();
	
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

	void Awake () { mTrans = transform; }

	/// <summary>
	/// Start the drag event and perform the dragging.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
		Debug.Log("drag");
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
		
		Debug.Log("pressed");
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
