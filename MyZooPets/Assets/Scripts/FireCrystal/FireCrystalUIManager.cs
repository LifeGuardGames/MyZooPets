using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Fire crystal user interface manager.
/// The player will get a fire crystal when the meter is filled
/// Note: there is no overflow reward for the player, so the player will
/// 	lose whatever extra they are currently getting after max has been reached
/// </summary>
public class FireCrystalUIManager : SingletonUI<FireCrystalUIManager>{

	public static EventHandler<EventArgs> OnFireCrystalUIAnimationDone;

	public UISprite spriteFireFill;
	public TweenToggle tweenToggle;
	public GameObject shardSpritePrefab;
	public GameObject shardParent;
	public float delayBetweenShards = 0.02f;
	public Animation crystalAnimation;
	public GameObject clickableFireCrystalPrefab;
	public UIAnchor parentAnchor;	// Changes depending on which scene we are in
	public TweenToggle parentTween;	// Changes depending on which scene we are in
	public UISprite parentBG;

	private int totalSubdivisions = 100;
	private float step = 0.005f;
	private float currentPercentage; // In terms of 0.0 -> 1.0
	private float targetPercentage;

	private bool isFireCrystalUIAnimating = false;
	public bool IsFireCrystalUIAnimating{		// Scenes will poll this to see if they need to wait
		get{
			return isFireCrystalUIAnimating;
		}
	}

	public delegate void Callback();
	public Callback FinishedAnimatingCallback;

	void Awake(){
		eModeType = UIModeTypes.FireCrystal;

		// If scene is the inhaler game, use right anchor and offset tweenparent
		// NOTE: Make sure script execution order is BEFORE tween toggle scripts!
		if(Application.loadedLevelName == SceneUtils.INHALERGAME){
			parentAnchor.side = UIAnchor.Side.Right;
			Vector3 currentPos = parentTween.transform.localPosition;
			parentTween.transform.localPosition = new Vector3(-300f, currentPos.y, currentPos.z);
		}
		// If scene is in bedroom or yard, combine with wellapad
		else if((Application.loadedLevelName == SceneUtils.BEDROOM) || Application.loadedLevelName == SceneUtils.YARD){
			Vector3 currentPos = parentTween.transform.localPosition;
			parentTween.transform.localPosition = new Vector3(-272, currentPos.y, currentPos.z);
			parentBG.enabled = false;
		}
	}

	void Start(){
		// Initalizaing from data
		currentPercentage = (float)DataManager.Instance.GameData.Stats.Shards/(float)totalSubdivisions;
		currentPercentage = Mathf.Min(currentPercentage, 1.0f);
		spriteFireFill.fillAmount = currentPercentage;
	}

	public void PopupAndRewardShards(int numberOfShards){
		// Only reward stuff if you have something
		if(numberOfShards > 0){
			// Get the current amount of subdivisions required
			totalSubdivisions = 100; 	// TODO implement data getter for this

			// Calculate a theoretical percentage that might spill over 1.0f
			float targetPercentageAux = currentPercentage + (float)numberOfShards/(float)totalSubdivisions;

			// Make sure that the actual percentage does not go over 1.0f
			targetPercentage = Mathf.Min(targetPercentageAux, 1.0f);

			// Lock and fire animations
			isFireCrystalUIAnimating = true;

			if((Application.loadedLevelName == SceneUtils.BEDROOM) || Application.loadedLevelName == SceneUtils.YARD){
				WellapadUIManager.Instance.OpenUI();
			}
			else{
				OpenUI();
			}

			StartCoroutine(StartFlyingShards(numberOfShards, 0.8f));
		}
	}

	protected override void _OpenUI(){
		tweenToggle.Show();
	}

	protected override void _CloseUI(){
		tweenToggle.Hide();

		// Launch any finished callback
		if(FinishedAnimatingCallback != null){	//TODO check if it is in wellapad mode here
			FinishedAnimatingCallback();
		}
	}

	// TODO Sean - inconsistent place call with other finishtweens, this is done after hide whereas others is on hide
	public void CloseFinishedHelper(){
		isFireCrystalUIAnimating = false;

		// Notify anything that is listening to this done
		if(OnFireCrystalUIAnimationDone != null){
			OnFireCrystalUIAnimationDone(this, EventArgs.Empty);
		}
	}

	/// <summary>
	/// Starts the flying shards. This will also call fill fire sprite on first tween finish
	/// </summary>
	/// <param name="numberOfShards">Number of shards.</param>
	private IEnumerator StartFlyingShards(int numberOfShards, float delay){
		if(IsOpen()){
			// Wait before starting
			yield return new WaitForSeconds(delay);

			for(int i = 0; i < numberOfShards/2; i++){
				GameObject shardObject = GameObjectUtils.AddChild(shardParent, shardSpritePrefab);
				// Place the shard object on a random point on a circle around center
				shardObject.transform.localPosition = 
					GameObjectUtils.GetRandomPointOnCircumference(Vector3.zero, UnityEngine.Random.Range(300f, 400f));
				FireShardController shardController = shardObject.GetComponent<FireShardController>();
				if(i == 0){
					// Move the shard into the center and call start filling sprite, first tween
					shardController.StartMoving(Vector3.zero, 0.8f, isFirstSprite: true);
				}
				else{
					// Move the shard into the center
					shardController.StartMoving(Vector3.zero, 0.8f);
				}
				yield return new WaitForSeconds(delayBetweenShards);
			}
		}
	}

	/// <summary>
	/// Starts the one-off fire animation, only should be called ONCE at start animating!
	/// </summary>
	public void StartFillFireSprite(){
		StartCoroutine(StartFillFireSpriteHelper());
	}

	private IEnumerator StartFillFireSpriteHelper(){
		while(currentPercentage != targetPercentage){
			// Increase by one step, but do not go over the target amount
			currentPercentage = Mathf.Min(currentPercentage + step, targetPercentage);
			// Populate sprite
			spriteFireFill.fillAmount = currentPercentage;

			// Wait one frame
			yield return 0;

			// Update our target incase it has been changed
			//TODO targetPercentage = 

			// Do a sanity check here for target
			if(currentPercentage > targetPercentage){
				Debug.LogError("The current flame percentage is higher than the target percentage");
			}
		}

		// Do check if full
		if(currentPercentage == 1.0f){
			RewardFireCrystal();
		}
		else{
			Debug.Log("Fire is not full");

			//TODO check if it is in wellapad mode here
			Invoke("CloseUI", 1f);
		}
	}

	private void RewardFireCrystal(){
		Debug.Log("Fire full check passed! Getting fire crystal");
		crystalAnimation.Play();
	}

	// Event callback from the crystal animation CrystalPop
	public void CrystalPopDone(){
		InventoryUIManager.Instance.ShowPanel();

		// Spawn a prefab that the user can click on and obtain
		GameObjectUtils.AddChild(shardParent, clickableFireCrystalPrefab);
	


		//TODO check if it is in wellapad mode here
	}

	public void ResetFrontSprite(){
		currentPercentage = 0;
		FireCrystalManager.Instance.ResetShards();
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Fire reward")){
//			FireCrystalManager.Instance.RewardShards(100);
//		}
//	}
}
