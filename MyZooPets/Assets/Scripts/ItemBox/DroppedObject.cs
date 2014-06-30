using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This is an object that is on the ground, in the
/// 3D world (although it may be 2D) that the player
/// can pick up to obtain.
/// </summary>
 public abstract class DroppedObject : LgButton{
	// --------------- Pure Abstract ---------------------------
	protected abstract void ObtainObject();			// give the user the object
	protected abstract void CollectAndDestroyAutomatically();	// collect the dropped object and destroy itself 
	// ---------------------------------------------------------
	
	// sprite associated with this dropped object
	public UISprite sprite;

	protected float timeBeforeAutoCollect = 5.0f;
	private DroppedItemStates eState = DroppedItemStates.UnInit; // state of this dropped item
	private float timer = 0;
	private bool runTimer = true;


	public DroppedItemStates GetState(){
		return eState;	
	}

	protected void SetState(DroppedItemStates eState){
		this.eState = eState;	
	}
	
	void Start(){
		ButtonChangeScene.OnChangeScene += OnChangeScene;
	}

	void Update(){
		if(!runTimer) return;

		timer += Time.deltaTime;
		if(timer > timeBeforeAutoCollect){
			ProcessClick(); //by pass the click manager, so the droppedobject self collect regardless
			runTimer = false;
		}
	}

	void OnDestroy(){
		ButtonChangeScene.OnChangeScene -= OnChangeScene;
	}
			
	void OnApplicationPause(bool isPaused){
		if(isPaused){
			// if the game is pausing, obtain this item
			ObtainObject();
		}
	}	

	/// <summary>
	/// his function sets the object's alpha to 0, and then
	/// fades it in.  It's an attempt at a bit more stable
	/// and controllable than bursting it.
	/// </summary>
	public void Appear(){
		GameObject go = GetGameObject();
			
		// turn the object completely invis
		TweenAlpha.Begin(go, 0, 0);
		
		// then fade in over time
		float fInTime = Constants.GetConstant<float>("ItemBoxAppear_Time");
		TweenAlpha.Begin(go, fInTime, 1);
	}

	/// <summary>
	/// Call this function when you want this item to
	/// burst out of wherever it currently is.
	/// </summary>
	/// <param name="burstToLeftOnly">If set to <c>true</c> burst to left only.</param>
	public void Burst(bool burstToLeftOnly = false){
		// get constants that control the burst
		int nRangeX = Constants.GetConstant<int>("ItemBoxBurst_RangeX");
		int nRangeY = Constants.GetConstant<int>("ItemBoxBurst_RangeY");	
		float fTime = Constants.GetConstant<float>("ItemBoxBurst_Time");
	
		// the burst is actually a lean tween move along a path
		
		// the starting location is the object's current location
		GameObject go = GetGameObject();
		Vector3 vStart = go.transform.position;
		
		// the end location is some random X length away
		int positiveRangeX = nRangeX;
		if(burstToLeftOnly)
			positiveRangeX = 0;
		float fEndX = UnityEngine.Random.Range(-nRangeX, positiveRangeX);
		Vector3 vEnd = new Vector3(vStart.x + fEndX, vStart.y, vStart.z);
		
		// the midpoint for the path is basically just 1/2 the x movement and some Y height
		float fY = UnityEngine.Random.Range(0, nRangeY);
		Vector3 vMid = new Vector3(vStart.x + (fEndX / 2), vStart.y + fY, vStart.z);
		
		// set the path
		Vector3[] path = new Vector3[4];
		path[0] = go.transform.position;
		path[1] = vMid;
		path[2] = vMid;
		path[3] = vEnd;
		
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.linear);		
		
		// and send the object on its way!
		LeanTweenUtils.MoveAlongPathWithSpeed(go, path, fTime, optional);
		
		/* // saving this for now just in case we want to go back to it, so I don't have to rewrite it...
		// tried bursting with add force and rigidbody...
		int nRangeX = Constants.GetConstant<int>( "ItemBoxBurst_RangeX_Gravity" );
		int nRangeY = Constants.GetConstant<int>( "ItemBoxBurst_RangeY_Gravity" );
		
		int nForceX = UnityEngine.Random.Range(-nRangeX, nRangeX);
		int nForceY = UnityEngine.Random.Range(0, nRangeY);
		
		Vector3 vForce = new Vector3( nForceX, nForceY, 0 );
		
		gameObject.rigidbody.isKinematic = false;
		gameObject.rigidbody.AddForce( vForce );
		
		Debug.LogError("Bursitng " + gameObject + " with " + vForce, gameObject);*/
		
	}

	/// <summary>
	/// Gets the game object.
	/// This function is necessary because some dropped
	/// objects have different hierarchies.  This function
	/// should return the upper most level of the dropped
	/// object's game object, to destory, apply movement
	/// to, etc.
	/// </summary>
	/// <returns>The game object.</returns>
	protected virtual GameObject GetGameObject(){
		return gameObject;
	}

	protected override void ProcessClick(){
		// only do something if the user hasn't already started picking up this item
		DroppedItemStates eState = GetState();
		if(eState == DroppedItemStates.Dropped){
			SetState(DroppedItemStates.PickedUp);

			// play pick up audio
			AudioManager.Instance.PlayClip(buttonSound);
			
			// animate the object by applying a rotation, translation, and fade
			float fTime = Constants.GetConstant<float>("ItemPickup_Time");
			float fUp = Constants.GetConstant<float>("ItemPickup_UpY");
			Vector3 vSpin = Constants.GetConstant<Vector3>("ItemPickup_Spin");

			Hashtable optional = new Hashtable();
			optional.Add("onComplete", "OnDoneAnimating");			
			
			GameObject go = GetGameObject();
			LeanTween.moveY(go, fUp, fTime, optional);			
			TweenAlpha.Begin(go, fTime, 0);
			LeanTween.rotate(go, vSpin, fTime);
			
		}
	}

	/// <summary>
	/// Raises the done animating event.
	/// Object done doing its pickup anim.
	/// </summary>
	private void OnDoneAnimating(){
		// animation done -- award the object
		ObtainObject();
	}
	
	/// <summary>
	/// Raises the change scene event.
	/// Force object to be picked up on scene change.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void OnChangeScene(object sender, EventArgs args){
		CollectAndDestroyAutomatically();
	}
}
