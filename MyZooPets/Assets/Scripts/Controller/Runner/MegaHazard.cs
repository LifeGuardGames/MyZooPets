using UnityEngine;
using System.Collections;

public class MegaHazard : MonoBehaviour {
    public float ZDefaultDistanceFromPlayer = 5f;
    public float DistanceDivisor = 2.0f;
    public float DistanceRegainIncrement = 0.1f;
    public float DistanceRegainTime = 1f;
    public float mGapClosingIncrement = 0.01f;

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
        UpdatePositionRelativeToPlayer();
        transform.position = Vector3.Lerp(transform.position, mDestinationPosition, Time.deltaTime);
	}

    void OnTriggerEnter(Collider inOther) {
        if (inOther.gameObject.tag == "Player") {
            RunnerGameManager gameManager = RunnerGameManager.GetInstance();
            gameManager.ActivateGameOver();
        }
    }

    public void Reset() {
        mCurrentDistanceFromPlayer = ZDefaultDistanceFromPlayer;
        mDistanceRegainPulse = DistanceRegainTime;

        transform.position = RunnerGameManager.GetInstance().PlayerRunner.transform.position;
        UpdatePositionRelativeToPlayer();
        mDestinationPosition = transform.position;
    }

    public void TriggerPlayerSlowdown() {
        PlayerRunner player = RunnerGameManager.GetInstance().PlayerRunner;
        if (player != null && !player.Invincible) { 
            mDistanceUntilTarget -= (ZDefaultDistanceFromPlayer / DistanceDivisor);
            mCurrentDistanceFromPlayer += mDistanceUntilTarget;
        }
    }

    private void UpdatePositionRelativeToPlayer() {
        if (mDistanceUntilTarget > 0)
            mDistanceUntilTarget -= mGapClosingIncrement;
        else if (mCurrentDistanceFromPlayer > ZDefaultDistanceFromPlayer) {
            mDistanceRegainPulse -= Time.deltaTime;
            if (mDistanceRegainPulse <= 0f) {
                mDistanceRegainPulse = DistanceRegainTime;
                mCurrentDistanceFromPlayer += DistanceRegainIncrement;
            }
        }

        float currentDistance = GetCurrentOffsetDistance();
        //Vector3 myPos = transform.position;
        PlayerRunner playerRunner = RunnerGameManager.GetInstance().PlayerRunner;
        mDestinationPosition.z = playerRunner.transform.position.z + currentDistance;
        transform.position = mDestinationPosition;
    }

    public float GetCurrentOffsetDistance() {
        return mCurrentDistanceFromPlayer - mDistanceUntilTarget;
    }
}
