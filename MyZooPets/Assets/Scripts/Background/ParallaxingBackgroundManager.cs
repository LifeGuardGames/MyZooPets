using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Parallaxing background manager.
/// Handles the transitions of backgrounds, holds the list of all backgrounds
/// and keeps track of the current and next bg's.
/// </summary>
public class ParallaxingBackgroundManager : Singleton<ParallaxingBackgroundManager>{
	public Vector3 BackgroundCameraOffset;
	public Camera BackgroundCamera;
	public GameObject BackgroundParent;
	public LevelGroup startingLevelGroup;
	public float TransitionLengthSeconds = 1.0f;

	private float mTransitionPulse;
	private ParallaxingBackgroundGroup mCurrentBackground = null;
	private ParallaxingBackgroundGroup mNextBackground = null;
	private Queue<ParallaxingBackgroundGroup> mNextTransition = new Queue<ParallaxingBackgroundGroup>();

	void Start(){
		TransitionToBackground(startingLevelGroup.parallaxBackgroundPrefab);
	}

	void LateUpdate(){
		//if there is a next group already start counting down for transition	
		if(mNextBackground != null){
			mTransitionPulse -= Time.deltaTime / Time.timeScale;

			//When timer is zero. Transition background to a new background
			if(mTransitionPulse <= 0){
				SetNextGroupAsCurrentAndDeleteCurrent();

				//If there is a queue of background waiting, get it from the queue
				if(mNextTransition.Count > 0)
					SpawnAndSetNextBackground(mNextTransition.Dequeue());
			}
			else{
				float currentAlpha = mTransitionPulse / TransitionLengthSeconds;
				float inverseAlpha = 1f - currentAlpha;
                
				if(mCurrentBackground != null)
					mCurrentBackground.SetAlpha(currentAlpha);
				if(mNextBackground != null)
					mNextBackground.SetAlpha(inverseAlpha);
			}
		}
	}

	public void Reset(){
		TransitionToBackground(startingLevelGroup.parallaxBackgroundPrefab);
	}

	public void TransitionToBackground(ParallaxingBackgroundGroup parallaxingBackgroundPrefab){
		if(parallaxingBackgroundPrefab != null){
			if(mNextTransition.Count == 0){
				SpawnAndSetNextBackground(parallaxingBackgroundPrefab);
			}
			else{
				mNextTransition.Enqueue(parallaxingBackgroundPrefab);
			}
		}
	}
	public void PlayParallax() {
		mCurrentBackground.PlayParallax();
	}
	public void PauseParallax() {
		mCurrentBackground.PauseParallax();
	}
	private void SpawnAndSetNextBackground(ParallaxingBackgroundGroup nextBackground){
		mTransitionPulse = TransitionLengthSeconds;
		mNextBackground = (ParallaxingBackgroundGroup)GameObject.Instantiate(nextBackground);
		mNextBackground.transform.parent = BackgroundParent.transform;
		mNextBackground.transform.position = BackgroundParent.transform.position + BackgroundCameraOffset;
	}

	private void SetNextGroupAsCurrentAndDeleteCurrent(){
		if(mCurrentBackground != null)
			GameObject.Destroy(mCurrentBackground.gameObject);

		mCurrentBackground = mNextBackground;
		mCurrentBackground.SetAlpha(1f);
		mNextBackground = null;
	}
}