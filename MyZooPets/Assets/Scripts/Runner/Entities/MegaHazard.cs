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
		transform.position = Vector3.Lerp(transform.position, mDestinationPosition, Time.deltaTime);
	}

	//When megahazard collides with the player. End the game
	void OnTriggerEnter(Collider inOther){
		if(inOther.gameObject.tag == "Player") {
			RunnerGameManager gameManager = RunnerGameManager.Instance;
			gameManager.ActivateGameOver();
		}
	}

	public void Reset() {
		mCurrentDistanceFromPlayer = XDefaultDistanceFromPlayer;
		mDistanceRegainPulse = DistanceRegainTime;
		mDistanceUntilTarget = 0f;

		//Set the hazards y position to the players y position
		transform.position = new Vector3(PlayerController.Instance.transform.position.x, transform.position.y, transform.position.z);
		mDestinationPosition = transform.position;

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
		transform.position = new Vector3(mDestinationPosition.x, transform.position.y, transform.position.z);

		// Update the Y distance
		// Vector3 currentPosition = transform.position;
		// currentPosition.y = PlayerController.Instance.transform.position.y + 5;
		// transform.position = currentPosition;
	}
}
