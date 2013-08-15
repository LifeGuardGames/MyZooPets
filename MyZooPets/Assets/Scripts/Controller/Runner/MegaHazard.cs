using UnityEngine;
using System.Collections;

public class MegaHazard : MonoBehaviour {
    public float ZDefaultDistanceFromPlayer = 5f;
    public float DistanceDivisor = 2.0f;
    public float DistanceRegainIncrement = 0.1f;
    public float DistanceRegainTime = 1f;

    private float mDistanceRegainPulse = 0f;
    private float mCurrentDistanceFromPlayer = 0f;
    private PlayerRunner mPlayerRunner;

	// Use this for initialization
	void Start() {
        mCurrentDistanceFromPlayer = ZDefaultDistanceFromPlayer;
        mDistanceRegainPulse = DistanceRegainTime;

        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null) {
            mPlayerRunner = playerObject.GetComponent<PlayerRunner>();
            transform.position = mPlayerRunner.transform.position;
            UpdatePositionRelativeToPlayer();
        } else
            Debug.LogError("No player exists");
	}
	
	// Update is called once per frame
	void Update() {
        UpdatePositionRelativeToPlayer();

        if (mCurrentDistanceFromPlayer < ZDefaultDistanceFromPlayer) {
            mDistanceRegainPulse -= Time.deltaTime;
            if (mDistanceRegainPulse <= 0f) {
                mDistanceRegainPulse = DistanceRegainTime;
                mCurrentDistanceFromPlayer += DistanceRegainIncrement;
            }
        }

	}

    void OnTriggerEnter(Collider inOther) {
        if (inOther.gameObject.tag == "Player") {
            Debug.Log("Smoke monster ahhh");
    
            RunnerGameManager gameManager = ((GameObject)GameObject.FindGameObjectWithTag("GameManager")).GetComponent<RunnerGameManager>();
            gameManager.ActivateGameOver();
        }
    }

    public void TriggerPlayerSlowdown() {
        mCurrentDistanceFromPlayer -= (ZDefaultDistanceFromPlayer / DistanceDivisor);
    }

    private void UpdatePositionRelativeToPlayer() {
        Vector3 myPos = transform.position;
        myPos.z = mPlayerRunner.transform.position.z + mCurrentDistanceFromPlayer;
        transform.position = myPos;
    }
}
