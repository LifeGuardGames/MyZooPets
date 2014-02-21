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
	public string FirstGroupID = "";
	public float TransitionLengthSeconds = 1.0f;
	public List<ParallaxingBackgroundGroup> ParralaxingGroups = new List<ParallaxingBackgroundGroup>();

	private float mTransitionPulse;
	private ParallaxingBackgroundGroup mCurrentGroup = null;
	private ParallaxingBackgroundGroup mNextGroup = null;
	private Queue<ParallaxingBackgroundGroup> mNextTransition = new Queue<ParallaxingBackgroundGroup>();

	// Use this for initialization
	void Start() {
		TransitionToGroup(FirstGroupID);
	}
	
	// Update is called once per frame
    void LateUpdate() {
    	
    	//if there is a next group already start counting down for transition	
		if (mNextGroup != null) {
            mTransitionPulse -= Time.deltaTime / Time.timeScale;

            //When timer is zero. Transition background to a new background
			if (mTransitionPulse <= 0) {
				SetNextGroupAsCurrentAndDeleteCurrent();

				//If there is a queue of background waiting, get it from the queue
				if (mNextTransition.Count > 0)
					SpawnAndSetNextGroup(mNextTransition.Dequeue());
			} else {
				float currentAlpha = mTransitionPulse / TransitionLengthSeconds;
				float inverseAlpha = 1f - currentAlpha;
                
				if (mCurrentGroup != null)
                    mCurrentGroup.SetAlpha(currentAlpha);
				if (mNextGroup != null)
                    mNextGroup.SetAlpha(inverseAlpha);
			}
		}
	}

    public void Reset() {
        TransitionToGroup(FirstGroupID);
    }

	public void TransitionToGroup(string inGroupID) {
		ParallaxingBackgroundGroup nextGroup = null;
		foreach (ParallaxingBackgroundGroup currentGroup in ParralaxingGroups) {
			if (currentGroup.GroupID == inGroupID) {
				nextGroup = currentGroup;
				break;
			}
		}

		if (nextGroup != null) {
			if (mNextTransition.Count == 0) {
				SpawnAndSetNextGroup(nextGroup);
			} else {
				mNextTransition.Enqueue(nextGroup);
			}
		} else
			Debug.LogError("No parralaxing bg named " + inGroupID);
	}

	private void SpawnAndSetNextGroup(ParallaxingBackgroundGroup inNextGroup) {
		mTransitionPulse = TransitionLengthSeconds;
		mNextGroup = (ParallaxingBackgroundGroup)GameObject.Instantiate(inNextGroup);
        mNextGroup.transform.parent = BackgroundParent.transform;
        mNextGroup.transform.position = BackgroundParent.transform.position + BackgroundCameraOffset;
	}

	private void SetNextGroupAsCurrentAndDeleteCurrent() {
		if (mCurrentGroup != null)
			GameObject.Destroy(mCurrentGroup.gameObject);
		mCurrentGroup = mNextGroup;
		mCurrentGroup.SetAlpha(1f);
		mNextGroup = null;
	}
}