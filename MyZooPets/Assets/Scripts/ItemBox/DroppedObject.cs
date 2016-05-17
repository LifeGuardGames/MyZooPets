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
	protected bool runTimer = true;

	private DroppedItemStates eState = DroppedItemStates.UnInit; // state of this dropped item
	private float timer = 0;

	public new Animation animation;

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
	/// <param name="burstToLeftOnly">If set to <c>true</c> burst to left only</param>
	/// <param name="burstStreamOrder">Burst stream order, will be multiplied with delay for multi burst effect</param>
	/// <param name="isXOverride">Check true to override x value to certain value</param>
	/// <param name="xOverride">Overrided value</param>
	public void Burst(bool isBurstToLeftOnly = false, int burstStreamOrder = -1, bool isXOverride = false, float xOverride = 0f){
		float delay = Constants.GetConstant<float>("ItemBoxBurst_DelayBetweenEach");

		// Hide this for animation first
		transform.Find("AnimationParent").localScale = new Vector3(0, 0, 1f);

		// Default case
		if(burstStreamOrder == -1){
			StartCoroutine(BurstHelper(isBurstToLeftOnly, 0f, isXOverride, xOverride));
		}
		// Multiple items stream effect
		else{
			StartCoroutine(BurstHelper(isBurstToLeftOnly, burstStreamOrder * delay, isXOverride, xOverride));
		}
	}

	private IEnumerator BurstHelper(bool isBurstToLeftOnly, float secondsToWait, bool isXOverride, float xOverride){

		// Wait if streaming
		yield return new WaitForSeconds(secondsToWait);

		animation.Play();

		// get constants that control the burst
		int nRangeX = Constants.GetConstant<int>("ItemBoxBurst_RangeX");
		float fTime = Constants.GetConstant<float>("ItemBoxBurst_Time");

		// the starting location is the object's current location
		GameObject go = GetGameObject();
		Vector3 vStart = go.transform.localPosition;


		// Use overrided x if it is on, otherwise use random values between specified range
		float fEndX;
		if(isXOverride){
			fEndX = xOverride;
		}
		else{
			// the end location is some random X length away
			int positiveRangeX = nRangeX;
			if(isBurstToLeftOnly){
				positiveRangeX = 0;
			}
			fEndX = UnityEngine.Random.Range(-nRangeX, positiveRangeX);
		}



		Vector3 vEnd = new Vector3(vStart.x + fEndX, vStart.y, vStart.z);

		// and send the object on its way!
		LeanTween.moveLocal(go, vEnd, fTime);
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
			
			GameObject go = GetGameObject();
			LeanTween.moveY(go, fUp, fTime).setOnComplete(OnDoneAnimating);			
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
