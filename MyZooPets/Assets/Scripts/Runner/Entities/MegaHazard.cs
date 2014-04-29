/* 
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
	public ParticleSystem hazardParticle;
	public ParticleSystem hazardParticle2;
	
	private float mSlowDownStayPulse = 0f;
	private float mDistanceUntilTarget = 0f;
	private float mDistanceRegainPulse = 0f;
	private float mCurrentDistanceFromPlayer = 0f;
	private Vector3 mDestinationPosition = Vector3.zero;

	private float animationSmoothing = 2.0f;

	void Start() {
		RunnerGameManager.OnStateChanged += GameStateChanged;
	}

	void OnDestroy(){
		RunnerGameManager.OnStateChanged -= GameStateChanged;
	}
	
	// Update is called once per frame
	void Update() {
		if(!RunnerGameManager.Instance.GameRunning)
			return;
			
		UpdatePositionRelativeToPlayer();

		//Smoothly move mega hazard to its new position
//		transform.position = Vector3.Lerp(transform.position, mDestinationPosition, animationSmoothing * Time.deltaTime);
	}

	//When megahazard collides with the player. End the game
	void OnTriggerEnter(Collider inOther){
		if(inOther.gameObject.tag == "Player") {
			GameOver();
//			Invoke("GameOver", 1.0f);
		}
	}

	private void GameOver(){
		RunnerGameManager.Instance.ActivateGameOver();
	}

	/// <summary>
	/// Reset the distance between the player and the mega hazard to default
	/// </summary>
	public void Reset() {
		mCurrentDistanceFromPlayer = XDefaultDistanceFromPlayer;
		mDistanceRegainPulse = DistanceRegainTime;
		mDistanceUntilTarget = 0f;


//		mDestinationPosition = transform.position;
//		float currentDistance = GetCurrentOffsetDistance();
//		mDestinationPosition.x = PlayerController.Instance.transform.position.x + currentDistance;
//
//		//Set the hazard to be a default distance away from the player
//		transform.position = new Vector3(mDestinationPosition.x, transform.position.y, transform.position.z);

		UpdatePositionRelativeToPlayer();
	}

    //---------------------------------------------------
    // TriggerPlayerSlowdown()
    // The idea behind this function seems to be to
	// move the megahazard closer to the player.
    //---------------------------------------------------	
	public void TriggerPlayerSlowdown() {
		mSlowDownStayPulse = SlowDownStayDuration;
		mDistanceUntilTarget -= (XDefaultDistanceFromPlayer / DistanceDivisor);
		mCurrentDistanceFromPlayer += mDistanceUntilTarget;
	}

	public float GetCurrentOffsetDistance() {
		return mCurrentDistanceFromPlayer - mDistanceUntilTarget;
	}

	private void GameStateChanged(object sender, GameStateArgs args){
		MinigameStates gameState = args.GetGameState();
		if(gameState == MinigameStates.Paused){
			hazardParticle.Pause();
			hazardParticle2.Pause();
		}else if(gameState == MinigameStates.Playing){
			hazardParticle.Play();
			hazardParticle2.Play();
		}
	}

	//Make hazard move with the player
	private void UpdatePositionRelativeToPlayer() {
		// Update the X distance
		if (mDistanceUntilTarget > 0)
			mDistanceUntilTarget -= GapClosingIncrement;
		else {
			if (mSlowDownStayPulse > 0f) {
				// When we get hit, we must stay at our slowed-down locaiton until the time elapses.
				mSlowDownStayPulse -= Time.deltaTime;         
			} else if (mCurrentDistanceFromPlayer > XDefaultDistanceFromPlayer) {
				mDistanceRegainPulse -= Time.deltaTime;
				if (mDistanceRegainPulse <= 0f) {
					mDistanceRegainPulse = DistanceRegainTime;
					mCurrentDistanceFromPlayer += DistanceRegainIncrement;
				}
			}
		}

		float currentDistance = GetCurrentOffsetDistance();
		mDestinationPosition.x = PlayerController.Instance.transform.position.x + currentDistance;

		//Set the hazard to be a default distance away from the player
		transform.position = new Vector3(mDestinationPosition.x, transform.position.y, transform.position.z);
	}
}
