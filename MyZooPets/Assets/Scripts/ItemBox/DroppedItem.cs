using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DroppedItem
// This is an item that is on the ground, in the
// 3D world (although it may be 2D) that the player
// can pick up to obtain.
//---------------------------------------------------	

public class DroppedItem : LgButton {
	// sprite associated with this dropped item
	public UISprite sprite;
	
	// the item that this object represents
	private Item dataItem;
	
	// state of this dropped item
	private DroppedItemStates eState = DroppedItemStates.UnInit;
	private void SetState( DroppedItemStates eState ) {
		this.eState = eState;	
	}
	public DroppedItemStates GetState() {
		return eState;	
	}
	
	//---------------------------------------------------
	// Init()
	// Inits this dropped item with 
	//---------------------------------------------------	
	public void Init( Item item ) {
		// set the state of this item to dropped
		SetState( DroppedItemStates.Dropped );
		
		// set the texture of this dropped item
		if ( sprite != null ) {
			string strSprite = item.TextureName;
			sprite.spriteName = strSprite;
			
			dataItem = item;
		}
		else
			Debug.Log("No sprite", gameObject);
		
		// also listen for when the inventory logic is being destroyed
		InventoryLogic.Instance.OnInventoryBeingDestroyed += OnInventoryBeingDestroyed;
	}
	
	//---------------------------------------------------
	// OnInventoryBeingDestroyed()
	// Callback sent from the inventory logic because it
	// is being destroyed (likely because the scene is
	// changing).
	//---------------------------------------------------	
	private void OnInventoryBeingDestroyed( object sender, EventArgs args ) {
		// if the inventory is being destroyed, but this dropped item has not yet been awarded, award it
		DroppedItemStates eState = GetState();
		if ( eState != DroppedItemStates.Awarded )
			AwardItem();
	}
	
	//---------------------------------------------------
	// Burst()
	// Call this function when you want this item to
	// burst out of wherever it currently is.
	//---------------------------------------------------	
	public void Burst() {
		// get constants that control the burst
		int nRangeX = Constants.GetConstant<int>( "ItemBoxBurst_RangeX" );
		int nRangeY = Constants.GetConstant<int>( "ItemBoxBurst_RangeY" );	
		float fTime = Constants.GetConstant<float>( "ItemBoxBurst_Time" );
		
		// the burst is actually a lean tween move along a path
		
		// the starting location is the object's current location
		Vector3 vStart = gameObject.transform.position;
		
		// the end location is some random X length away
		float fEndX = UnityEngine.Random.Range(-nRangeX, nRangeX);
		Vector3 vEnd = new Vector3( vStart.x + fEndX, vStart.y, vStart.z );
		
		// the midpoint for the path is basically just 1/2 the x movement and some Y height
		float fY = UnityEngine.Random.Range(0, nRangeY);
		Vector3 vMid = new Vector3( vStart.x + ( fEndX / 2 ), vStart.y + fY, vStart.z );
		
		// set the path
		Vector3[] path = new Vector3[4];
		path[0] = gameObject.transform.position;
		path[1] = vMid;
		path[2] = vMid;
		path[3] = vEnd;
		
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.linear);		
		
		// and send the object on its way!
		LeanTweenUtils.MoveAlongPathWithSpeed( gameObject, path, fTime, optional );		
		
		/* // saving this for now just in case we want to go back to it, so I don't have to rewrite it...
		// tried bursting with add force and rigidbody...
		int nRangeX = Constants.GetConstant<int>( "ItemBoxBurst_RangeX_Gravity" );
		int nRangeY = Constants.GetConstant<int>( "ItemBoxBurst_RangeY_Gravity" );
		
		int nForceX = UnityEngine.Random.Range(-nRangeX, nRangeX);
		int nForceY = UnityEngine.Random.Range(0, nRangeY);
		
		Vector3 vForce = new Vector3( nForceX, nForceY, 0 );
		
		gameObject.rigidbody.isKinematic = false;
		gameObject.rigidbody.AddForce( vForce );
		
		Debug.Log("Bursitng " + gameObject + " with " + vForce, gameObject);*/
		
	}
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------		
	protected override void ProcessClick() {
		// only do something if the user hasn't already started picking up this item
		DroppedItemStates eState = GetState();
		if ( eState == DroppedItemStates.Dropped ) {
			SetState( DroppedItemStates.PickedUp );
			
			// animate the object by applying a rotation, translation, and fade
			float fTime = Constants.GetConstant<float>( "ItemPickup_Time" );
			float fUp = Constants.GetConstant<float>( "ItemPickup_UpY" );
			Vector3 vSpin = Constants.GetConstant<Vector3>( "ItemPickup_Spin" );

	        Hashtable optional = new Hashtable();
			optional.Add ("onComplete", "OnDoneAnimating");			
			
	        LeanTween.moveY(gameObject, fUp, fTime, optional);			
			TweenAlpha.Begin( gameObject, fTime, 0 );
			LeanTween.rotate(gameObject, vSpin, fTime);
			
		}
	}
	
	//---------------------------------------------------
	// OnDoneAnimating()
	// The object is done doing its little pickup anim.
	//---------------------------------------------------	
	private void OnDoneAnimating() {
		// animation done -- award the item
		AwardItem();
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------		
	private void OnDestroy() {
		// if this dropped item is being destroyed, has not yet been awarded, AND the inventory still exists, award it
		DroppedItemStates eState = GetState();
		if ( eState != DroppedItemStates.Awarded && InventoryLogic.Instance != null )
			AwardItem();
	}
	
	//---------------------------------------------------
	// AwardItem()
	// Places the item in the player's inventory and
	// then destroys itself.
	//---------------------------------------------------		
	private void AwardItem() {
		DroppedItemStates eState = GetState();
		
		// if this item isn't in the right state, return
		if ( eState != DroppedItemStates.Dropped && eState != DroppedItemStates.PickedUp )
			return;

		// set the state to being awarded
		SetState( DroppedItemStates.Awarded );
		
		// add it to the player's inventory
		InventoryLogic.Instance.AddItem( dataItem.ID, 1);
		
		// destroy the object
		Destroy( gameObject );		
	}
}
