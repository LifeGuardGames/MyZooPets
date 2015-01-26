﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Fire crystal user interface manager.
/// The player will get a fire crystal when the meter is filled
/// Note: there is no overflow reward for the player, so the player will
/// 	lose whatever extra they are currently getting after max has been reached
/// </summary>
public class FireCrystalUIManager : SingletonUI<FireCrystalUIManager>{
	public UISprite spriteFireFill;
	public TweenToggle tweenToggle;
	public GameObject shardSpritePrefab;
	public GameObject shardParent;
	public float delayBetweenShards = 0.5f;

	private int totalSubdivisions;
	private float step = 0.001f;
	private float currentPercentage; // In terms of 0.0 -> 1.0
	private float targetPercentage;

	void Awake(){
		eModeType = UIModeTypes.FireCrystal;
	}

	void Start(){
		currentPercentage = 0; // TODO populate this from data
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
			OpenUI();
			StartCoroutine(StartFlyingShards(numberOfShards, 1.0f));
		}
	}

	protected override void _OpenUI(){
		tweenToggle.Show();
	}

	protected override void _CloseUI(){
		tweenToggle.Hide();
	}

	/// <summary>
	/// Starts the flying shards. This will also call fill fire sprite on first tween finish
	/// </summary>
	/// <param name="numberOfShards">Number of shards.</param>
	private IEnumerator StartFlyingShards(int numberOfShards, float delay){
		if(IsOpen()){
			// Wait before starting
			yield return new WaitForSeconds(delay);

			for(int i = 0; i < numberOfShards; i++){
				GameObject shardObject = GameObjectUtils.AddChild(shardParent, shardSpritePrefab);
				// Place the shard object on a random point on a circle around center
				shardObject.transform.localPosition = 
					GameObjectUtils.GetRandomPointOnCircumference(Vector3.zero, Random.Range(300f, 400f));
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
		}
	}

	private void RewardFireCrystal(){
		Debug.Log("Fire full check passed! Getting fire crystal");
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Fire reward")){
//			PopupAndRewardShards(10);
//		}
//	}
}
