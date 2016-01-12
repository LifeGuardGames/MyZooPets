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
	public float totalTimeTween = 1.5f;
	public Animation crystalAnimation;
	public ParticleSystem getGemParticle;
	public GameObject clickableFireCrystalPrefab;
	public UIAnchor parentAnchor;	// Changes depending on which scene we are in
	public TweenToggle parentTween;	// Changes depending on which scene we are in
	public UISprite parentBG;
	public UISprite backdrop;

	private int totalSubdivisions = 100;
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

	protected override void Awake(){
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

	protected override void Start(){
		// Initalizaing from data
		currentPercentage = (float)DataManager.Instance.GameData.Stats.Shards/(float)totalSubdivisions;
		currentPercentage = Mathf.Min(currentPercentage, 1.0f);
		spriteFireFill.fillAmount = currentPercentage;
		backdrop.enabled = true; 	// backdrop enabled by default
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

			OpenUIBasedOnScene();

			StartCoroutine(StartFlyingShards(numberOfShards, 0.5f));
		}
	}

	public void OpenUIBasedOnScene(){
		if((Application.loadedLevelName == SceneUtils.BEDROOM) || Application.loadedLevelName == SceneUtils.YARD){
			backdrop.enabled = false;	// Use the wellapad backdrop due to panel layering issues
			WellapadUIManager.Instance.OpenUI();
		}
		else{
			backdrop.enabled = true;
			OpenUI();
		}
	}

	public void CloseUIBasedOnScene(){
		if((Application.loadedLevelName == SceneUtils.BEDROOM) || Application.loadedLevelName == SceneUtils.YARD){
			WellapadUIManager.Instance.CloseUI();

			if(FinishedAnimatingCallback != null){
				FinishedAnimatingCallback();
			}
		}
		else{
			CloseUI();
		}
	}

	protected override void _OpenUI(){
		tweenToggle.Show();
	}

	protected override void _CloseUI(){
		tweenToggle.Hide();

		// Launch any finished callback
		if(FinishedAnimatingCallback != null){
			FinishedAnimatingCallback();
		}
	}

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
		//	if(numberOfShards == 1){
		//		numberOfShards = 2;
		//	}
			// Wait before starting
			yield return new WaitForSeconds(delay);

			// 100 shards is too much... cap at 15
			float numberOfShardsToShow = numberOfShards > 15 ? 15f : (float)numberOfShards;
			float delayBetweenShards = totalTimeTween / numberOfShardsToShow;

			for(float i = 0; i < numberOfShardsToShow; i++){
				GameObject shardObject = GameObjectUtils.AddChild(shardParent, shardSpritePrefab);
				// Place the shard object on a random point on a circle around center
				shardObject.transform.localPosition = 
					GameObjectUtils.GetRandomPointOnCircumference(Vector3.zero, UnityEngine.Random.Range(300f, 400f));
				FireShardController shardController = shardObject.GetComponent<FireShardController>();

				float pitchCount = 1f + (i / 8.0f);

				Vector3 endPoint = GameObjectUtils.GetRandomPointOnCircumference(Vector3.zero, UnityEngine.Random.Range(0, 40f));

				if(i == 0){
					// Move the shard into the center and call start filling sprite, first tween
					shardController.StartMoving(endPoint, 0.8f, pitchCount, isFirstSprite: true);
				}
				else{
					// Move the shard into the center
					shardController.StartMoving(endPoint, 0.8f, pitchCount);
				}
				yield return new WaitForSeconds(delayBetweenShards);
			}
		}
	}

	/// <summary>
	/// Starts the one-off fire animation, only should be called ONCE at start animating!
	/// </summary>
	public void StartFillFireSprite(){
		LeanTween.cancel(gameObject);
		LeanTween.value(gameObject, UpdateValueCallback, currentPercentage, targetPercentage, totalTimeTween)
			.setOnComplete(FinishedFillSpriteCallback).setEase(LeanTweenType.easeOutCubic);
	}

	// Helper function for the value leantween
	private void UpdateValueCallback(float value){
		currentPercentage = value;
		spriteFireFill.fillAmount = currentPercentage;
	}

	// Helper function for value leantween done
	private void FinishedFillSpriteCallback(){
		// Do check if full
		if(currentPercentage == 1.0f){
			RewardFireCrystal();
		}
		else{
			Invoke("CloseUIBasedOnScene", 1f);
		}
	}

	private void RewardFireCrystal(){
		crystalAnimation.Play();
	}

	// Event callback from the crystal animation CrystalPop
	public void CrystalPopDone(){
		InventoryUIManager.Instance.ShowPanel();

		AudioManager.Instance.PlayClip("fireGemGet");

		// Spawn a prefab that the user can click on and obtain
		GameObjectUtils.AddChild(shardParent, clickableFireCrystalPrefab);
	}

	public void PlayGetGemParticle(){
		getGemParticle.Play();
	}

	public void ResetFrontSprite(){
		currentPercentage = 0;
		spriteFireFill.fillAmount = 0;
		FireCrystalManager.Instance.ResetShards();
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Fire reward")){
//			FireCrystalManager.Instance.RewardShards(100);
//		}
//		if(GUI.Button(new Rect(200, 100, 100, 100), "Fire reward")){
//			FireCrystalManager.Instance.RewardShards(30);
//		}
//	}
}
