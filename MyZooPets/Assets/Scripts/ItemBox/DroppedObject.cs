using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DroppedObject
// This is an object that is on the ground, in the
// 3D world (although it may be 2D) that the player
// can pick up to obtain.
//---------------------------------------------------	

public abstract class DroppedObject : LgButton {
	// --------------- Pure Abstract ---------------------------
	protected abstract void _ObtainObject();			// give the user the object
	protected abstract void OnObjectDestroyed();		// when this game object is destroyed
	protected abstract void _OnManagerDestroyed();		// when related managers are destroyed
	// ---------------------------------------------------------
	
	// sprite associated with this dropped object
	public UISprite sprite;
	
	// state of this dropped item
	private DroppedItemStates eState = DroppedItemStates.UnInit;
	protected void SetState( DroppedItemStates eState ) {
		this.eState = eState;	
	}
	public DroppedItemStates GetState() {
		return eState;	
	}
	
	void Start() {
		DataManager.Instance.OnBeingDestroyed += OnManagerDestroyed;
	}
	
	//---------------------------------------------------
	// OnManagerDestroyed()
	// This is a generic function that is called when a
	// dependent manager is destroyed.  It basically 
	// forces the item to be picked up right away, because
	// the item requires the dependent manager in order to
	// actual process the picking up.
	//---------------------------------------------------	
	protected void OnManagerDestroyed( object sender, EventArgs args ) {
		_OnManagerDestroyed();
	}		
	
	//---------------------------------------------------
	// Appear()
	// This function sets the object's alpha to 0, and then
	// fades it in.  It's an attempt at a bit more stable
	// and controllable than bursting it.
	//---------------------------------------------------	
	public void Appear() {
		GameObject go = GetGameObject();
			
		// turn the object completely invis
		TweenAlpha.Begin( go, 0, 0 );
		
		// then fade in over time
		float fInTime = Constants.GetConstant<float>( "ItemBoxAppear_Time" );
		TweenAlpha.Begin( go, fInTime, 1 );
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
		GameObject go = GetGameObject();
		Vector3 vStart = go.transform.position;
		
		// the end location is some random X length away
		float fEndX = UnityEngine.Random.Range(-nRangeX, nRangeX);
		Vector3 vEnd = new Vector3( vStart.x + fEndX, vStart.y, vStart.z );
		
		// the midpoint for the path is basically just 1/2 the x movement and some Y height
		float fY = UnityEngine.Random.Range(0, nRangeY);
		Vector3 vMid = new Vector3( vStart.x + ( fEndX / 2 ), vStart.y + fY, vStart.z );
		
		// set the path
		Vector3[] path = new Vector3[4];
		path[0] = go.transform.position;
		path[1] = vMid;
		path[2] = vMid;
		path[3] = vEnd;
		
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.linear);		
		
		// and send the object on its way!
		LeanTweenUtils.MoveAlongPathWithSpeed( go, path, fTime, optional );
		
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
	// GetGameObject()
	// This function is necessary because some dropped
	// objects have different hierarchies.  This function
	// should return the upper most level of the dropped
	// object's game object, to destory, apply movement
	// to, etc.
	//---------------------------------------------------		
	protected virtual GameObject GetGameObject() {
		return gameObject;
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
			
			GameObject go = GetGameObject();
	        LeanTween.moveY(go, fUp, fTime, optional);			
			TweenAlpha.Begin( go, fTime, 0 );
			LeanTween.rotate(go, vSpin, fTime);
			
		}
	}
	
	//---------------------------------------------------
	// OnDoneAnimating()
	// The object is done doing its little pickup anim.
	//---------------------------------------------------	
	private void OnDoneAnimating() {
		// animation done -- award the object
		ObtainObject();
	}
	
	//---------------------------------------------------
	// _OnDestroy()
	//---------------------------------------------------		
	protected override void _OnDestroy() {
		OnObjectDestroyed();
	}
	
	//---------------------------------------------------
	// ObtainObject()
	// Gives the user the object.
	//---------------------------------------------------
	protected void ObtainObject() {
		_ObtainObject();	
	}
	
	//---------------------------------------------------
	// OnApplicationPause()
	// Unity callback function.
	//---------------------------------------------------		
	void OnApplicationPause( bool bPaused ) {
		if ( bPaused ) {
			// if the game is pausing, obtain this item
			ObtainObject();
		}
	}	
}
