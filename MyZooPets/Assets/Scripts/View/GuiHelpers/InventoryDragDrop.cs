using UnityEngine;
using System.Collections;

public class InventoryDragDrop : MonoBehaviour {

	/// <summary>
	/// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
	/// </summary>

	public GameObject prefab;

	Transform mTrans;
	bool mIsDragging = false;
	bool mSticky = false;
	Transform mParent;	// Store parent when dragging
	Vector3 savedLocalPosition;
	
	
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
		// Is there a droppable container?
		Collider col = UICamera.lastHit.collider;
		Debug.Log ("hit! " + gameObject.name);
		DragDropContainer container = (col != null) ? col.gameObject.GetComponent<DragDropContainer>() : null;
		//Debug.Log("3 " + mParent.localPosition + "     " + mTrans.position);
		if (container != null)
		{
			// Container found -- parent this object to the container
			mTrans.parent = container.transform;

			Vector3 pos = mTrans.localPosition;
			pos.z = 0f;
			mTrans.localPosition = pos;
		}
		else
		{
			Debug.Log ("no hit");
			// No valid container under the mouse -- revert the item's parent
			mTrans.parent = mParent;
			
			gameObject.transform.localPosition = savedLocalPosition;		// Revert to original position
		}

		// Notify the table of this change
		UpdateTable();

		// Make all widgets update their parents
		NGUITools.MarkParentAsChanged(gameObject);
		
		Debug.Log("3 " + mParent.localPosition + "     " + mTrans.position);

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
			if (!mIsDragging)
			{
				savedLocalPosition = gameObject.transform.localPosition;	// Save original position detection failed
				
				mIsDragging = true;
				mParent = mTrans.parent;
				Debug.Log("1 " + mParent.localPosition + "     " + mTrans.position);
				mTrans.parent = DragDropRoot.root;
				
				Vector3 pos = mTrans.localPosition;
				pos.z = 0f;
				mTrans.localPosition = pos;

				NGUITools.MarkParentAsChanged(gameObject);
			}
			else
			{
				Debug.Log("2 " + mParent.localPosition);
				mTrans.localPosition += (Vector3)delta;
			}
		}
	}

	/// <summary>
	/// Start or stop the drag operation.
	/// </summary>

	void OnPress (bool isPressed)
	{
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
