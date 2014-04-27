/* 
 * ParallaxingBackgroundManager.cs
 * Description:
 * Handles the transitions of backgrounds, holds the list of a;ll backgrounds, and keeps track
 * of the current and next bg's.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxingBackgroundManager : Singleton<ParallaxingBackgroundManager> {
	public Vector3 BackgroundCameraOffset;
	public Camera BackgroundCamera;
    public GameObject BackgroundParent;
	// public string FirstGroupID = "";
	public LevelGroup startingLevelGroup;

	public float TransitionLengthSeconds = 1.0f;

	// public List<GameObject> ParralaxingGroups = new List<GameObject>();

	private float mTransitionPulse;

	private ParallaxingBackgroundGroup mCurrentBackground = null;
	private ParallaxingBackgroundGroup mNextBackground = null;

	private Queue<ParallaxingBackgroundGroup> mNextTransition = new Queue<ParallaxingBackgroundGroup>();

	// Use this for initialization
	void Start() {
		TransitionToBackground(startingLevelGroup.parallaxBackgroundPrefab);
	}
	
	// Update is called once per frame
    void LateUpdate() {
    	
    	//if there is a next group already start counting down for transition	
		if (mNextBackground != null) {
            mTransitionPulse -= Time.deltaTime / Time.timeScale;

            //When timer is zero. Transition background to a new background
			if (mTransitionPulse <= 0) {
				SetNextGroupAsCurrentAndDeleteCurrent();

				//If there is a queue of background waiting, get it from the queue
				if (mNextTransition.Count > 0)
					SpawnAndSetNextBackground(mNextTransition.Dequeue());

			} else {
				float currentAlpha = mTransitionPulse / TransitionLengthSeconds;
				float inverseAlpha = 1f - currentAlpha;
                
				if (mCurrentBackground != null)
                    mCurrentBackground.SetAlpha(currentAlpha);
				if (mNextBackground != null)
                    mNextBackground.SetAlpha(inverseAlpha);
			}
		}
	}

    public void Reset() {
        TransitionToBackground(startingLevelGroup.parallaxBackgroundPrefab);
    }

	public void TransitionToBackground(ParallaxingBackgroundGroup parallaxingBackgroundPrefab) {
		// ParallaxingBackgroundGroup nextGroup = null;
		// foreach (ParallaxingBackgroundGroup currentGroup in ParralaxingGroups) {
		// 	if (currentGroup.GroupID == inGroupID) {
		// 		nextGroup = currentGroup;
		// 		break;
		// 	}
		// }

		if (parallaxingBackgroundPrefab != null) {
			if (mNextTransition.Count == 0) {
				SpawnAndSetNextBackground(parallaxingBackgroundPrefab);
			} else {
				mNextTransition.Enqueue(parallaxingBackgroundPrefab);
			}
		} 

		// else
		// 	Debug.LogError("No parralaxing bg named " + inGroupID);
	}

	private void SpawnAndSetNextBackground(ParallaxingBackgroundGroup nextBackground) {
		mTransitionPulse = TransitionLengthSeconds;
		mNextBackground = (ParallaxingBackgroundGroup)GameObject.Instantiate(nextBackground);
        mNextBackground.transform.parent = BackgroundParent.transform;
        mNextBackground.transform.position = BackgroundParent.transform.position + BackgroundCameraOffset;
	}

	private void SetNextGroupAsCurrentAndDeleteCurrent() {
		if (mCurrentBackground != null)
			GameObject.Destroy(mCurrentBackground.gameObject);

		mCurrentBackground = mNextBackground;
		mCurrentBackground.SetAlpha(1f);
		mNextBackground = null;
	}
}