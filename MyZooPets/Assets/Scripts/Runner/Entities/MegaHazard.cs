﻿/* Sean Duane
 * MegaHazard.cs
 * 8:26:2013   14:12
 * Description:
 * The megahazard follows the player through the level, and give them a chase.
 * If the player touches the MegaHazard, they lose.
 * The hazard tries to keep a constant offset from the player. This offset can be added/recuded by the distancedivisor/distanceuntiltarget. 
 * So basically, this is what handled the players "distance"
 */

using UnityEngine;
using System.Collections;

public class MegaHazard : Singleton<MegaHazard> {
	public float XDefaultDistanceFromPlayer = 5f;
	public float DistanceDivisor = 2.0f;
	public float DistanceRegainIncrement = 0.1f;
	public float DistanceRegainTime = 1f;
	public float GapClosingIncrement = 0.01f;
	public float SlowDownStayDuration = 1.0f;

	private float mSlowDownStayPulse = 0f;
	private float mDistanceUntilTarget = 0f;
	private float mDistanceRegainPulse = 0f;
	private float mCurrentDistanceFromPlayer = 0f;
	private Vector3 mDestinationPosition = Vector3.zero;

	// Use this for initialization
	void Start() {
		Reset();
	}
	
	// Update is called once per frame
	void Update() {
		if(!RunnerGameManager.Instance.GameRunning)
			return;
			
		UpdatePositionRelativeToPlayer();
		transform.position = Vector3.Lerp(transform.position, mDestinationPosition, Time.deltaTime);
	}

	void OnTriggerEnter(Collider inOther) {
		if (inOther.gameObject.tag == "Player") {
			RunnerGameManager gameManager = RunnerGameManager.Instance;
			gameManager.ActivateGameOver();
		}
	}

	public void Reset() {
		mCurrentDistanceFromPlayer = XDefaultDistanceFromPlayer;
		mDistanceRegainPulse = DistanceRegainTime;
		mDistanceUntilTarget = 0f;

		transform.position = PlayerController.Instance.transform.position;
		UpdatePositionRelativeToPlayer();
		mDestinationPosition = transform.position;

	}

	public void TriggerPlayerSlowdown() {
		mSlowDownStayPulse = SlowDownStayDuration;
		mDistanceUntilTarget -= (XDefaultDistanceFromPlayer / DistanceDivisor);
		mCurrentDistanceFromPlayer += mDistanceUntilTarget;
	}

	private void UpdatePositionRelativeToPlayer() {
		// Update the Z distance
		if (mDistanceUntilTarget > 0)
			mDistanceUntilTarget -= GapClosingIncrement;
		else {
			if (mSlowDownStayPulse > 0f) {
				// When we get hit, we must stay at our slowed-down locaiton until the time elapses.
				mSlowDownStayPulse -= Time.deltaTime / Time.timeScale;         
			} else if (mCurrentDistanceFromPlayer > XDefaultDistanceFromPlayer) {
				mDistanceRegainPulse -= Time.deltaTime / Time.timeScale;
				if (mDistanceRegainPulse <= 0f) {
					mDistanceRegainPulse = DistanceRegainTime;
					mCurrentDistanceFromPlayer += DistanceRegainIncrement;
				}
			}
		}

		float currentDistance = GetCurrentOffsetDistance();
		mDestinationPosition.x = PlayerController.Instance.transform.position.x + currentDistance;
		transform.position = mDestinationPosition;

		// Update the Y distance
		Vector3 currentPosition = transform.position;
		currentPosition.y = PlayerController.Instance.transform.position.y;
		transform.position = currentPosition;
	}

	public float GetCurrentOffsetDistance() {
		return mCurrentDistanceFromPlayer - mDistanceUntilTarget;
	}
}
