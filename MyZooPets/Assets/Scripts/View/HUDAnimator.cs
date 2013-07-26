using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Room GUI animator.
/// This is the middleman proxy class that listens to the statLogic to display animation for HUD.
///
/// this animator will also have a series of events. These events will be raised
/// when special things are happening to the changing stats. Client classes will need
/// to provide listeners to handle these events.
/// For example, Event will be raised when the level up progress bar reached the leveling up point
/// If any other class wants to display UI elements when this event happens, a listener will need to
/// be provided.
/// </summary>

public enum HUDElementType{
	points, stars, health, mood
}

public class HUDAnimator : MonoBehaviour {
	public bool isDebug = false;

	//================Events================
	//call when the pet levels up. used this to level up UI components
    public static EventHandler<EventArgs> OnLevelUp;
    //========================================

    public int DisplayPoints{
    	get{return displayPoints;}
    }

    public int DisplayStars{
    	get{return displayStars;}
    }

    public int DisplayHealth{
    	get{return displayHealth;}
    }

    public int DisplayMood{
    	get{return displayMood;}
    }

    public int NextLevelPoints{
    	get{return nextLevelPoints;}
    }

    public Level LastLevel{
    	get{return lastLevel;}
    }

    private int dataPoints, dataStars, dataHealth, dataMood, dataHunger;
	private int displayPoints, displayStars, displayHealth, displayMood, displayHunger;
	private int nextLevelPoints; //the minimum requirement for next level up
	private Level lastLevel; //pet's last level
	// private HUD hud;

	// Icon pulsing
	public GameObject healthIconAnim;
	public GameObject moodIconAnim;
	public GameObject starIconAnim;
	private AnimationControl starAnimControl;
	private AnimationControl healthAnimControl;
	private AnimationControl moodAnimControl;

	// Tweening
	public UIAtlas commonAtlas;
	private GameObject toDestroy;
	private bool isFirstTweenPointsDone = false;
	private bool isFirstTweenPointsCall = true;
	private bool isAnimatePointsBar = false;

	private bool isFirstTweenStarsDone = false;
	private bool isFirstTweenStarsCall = true;
	private bool isAnimateStarsBar = false;

	private bool isFirstTweenHealthDone = false;
	private bool isFirstTweenHealthCall = true;
	private bool isAnimateHealthBar = false;

	private bool isFirstTweenMoodDone = false;
	private bool isFirstTweenMoodCall = true;
	private bool isAnimateMoodBar = false;

	// Parent for tweening
	public GameObject tweenParent;

	void Awake(){
		starAnimControl = starIconAnim.GetComponent<AnimationControl>();
		healthAnimControl = healthIconAnim.GetComponent<AnimationControl>();
		moodAnimControl = moodIconAnim.GetComponent<AnimationControl>();
	}

	void Start(){
		// TODO-j Is this Initialization still valid??!!
		dataPoints = DataManager.Points;
		dataStars = DataManager.Stars;
		dataHealth = DataManager.Health;
		dataMood = DataManager.Mood;
		dataHunger = DataManager.Hunger;

		displayPoints = DataManager.Points;
		displayStars = DataManager.Stars;
		displayHealth = DataManager.Health;
		displayMood = DataManager.Mood;

		lastLevel = DataManager.CurrentLevel;
		nextLevelPoints = LevelUpLogic.NextLevelPoints();
	}

	void FixedUpdate(){
		if(isAnimatePointsBar){
			PointsAnimation();
		}
		if(isAnimateStarsBar){
			StarsAnimation();
		}
		if(isAnimateHealthBar){
			HealthAnimation();
		}
		if(isAnimateMoodBar){
			MoodAnimation();
		}

		if(!LoadLevelManager.IsPaused){
			LevelUpEventCheck(); // Check if progress bar reach level max
		}
	}

	//==================GUI Animation=========================
	private void StarsAnimation(){
		if(dataStars != DataManager.Stars){
			if(displayStars > DataManager.Stars){
				displayStars--;
				starAnimControl.Play();
			}else if(displayStars < DataManager.Stars){
				displayStars++;
				starAnimControl.Play();
			}else{
				dataStars = DataManager.Stars;

				// Stop grow & shrink, reset icon size
				starAnimControl.Stop();

				// Reset the bar animation flag
				isAnimateStarsBar = false;
				isFirstTweenStarsCall = true;
			}
		}
	}

	private void PointsAnimation(){
		if(dataPoints != DataManager.Points){
			if(displayPoints < DataManager.Points){ //animate
				if(displayPoints + 3 <= DataManager.Points){
					displayPoints += 3;
				}else{
					displayPoints += DataManager.Points - displayPoints;
				}
			}else{ //animation done
				dataPoints = DataManager.Points;

				// Reset the bar animation flag
				isAnimatePointsBar = false;
				isFirstTweenPointsCall = true;
			}
		}
	}

	private void HealthAnimation(){
		if(dataHealth != DataManager.Health){
			if(displayHealth > DataManager.Health){
				displayHealth--;
				healthAnimControl.Play();
			}else if(displayHealth < DataManager.Health){
				displayHealth++;
				healthAnimControl.Play();
			}else{
				dataHealth = DataManager.Health;

				// Stop grow & shrink. reset icon size
				healthAnimControl.Stop();

				// Reset the bar animation flag
				isAnimateHealthBar = false;
				isFirstTweenHealthCall = true;
			}
		}
	}

	private void MoodAnimation(){
		if(dataMood != DataManager.Mood){
			if(displayMood > DataManager.Mood){
				displayMood--;
				moodAnimControl.Play();
			}else if(displayMood < DataManager.Mood){
				displayMood++;
				moodAnimControl.Play();
			}else{
				dataMood = DataManager.Mood;

				// Stop grow & shrink. reset icon size
				moodAnimControl.Stop();

				// Reset the bar animation flag
				isAnimateMoodBar = false;
				isFirstTweenMoodCall = true;
			}
		}

	}

	//================================================================

    //Check if the points progress bar has reached the level requirement
	//if it does call on event listeners and reset the exp points progress bar
	private void LevelUpEventCheck(){
		if(displayPoints >= nextLevelPoints){ //logic for when progress bar reaches level requirement
			int remainderPoints = DataManager.Points - nextLevelPoints; //points to be added after leveling up


			if(OnLevelUp != null){
                OnLevelUp(this, EventArgs.Empty); //Level up. call the UI event listeners
            }else{
                Debug.LogError("OnLevelUp listener is null");
            }

			//reset the progress bar for next level
			DataManager.ResetPoints();
			nextLevelPoints = LevelUpLogic.NextLevelPoints(); //set the requirement for nxt level
			StatsController.Instance.ChangeStats(remainderPoints, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);
			displayPoints = 0;
			dataPoints = 0;
			lastLevel = DataManager.CurrentLevel;
		}
	}

	//================================================================
	// TWEEN ANIMATION
	//========================

	void OnGUI(){
		if(isDebug){
			if(GUI.Button(new Rect(100, 100, 100, 50), "add points")){
				StatsController.Instance.ChangeStats(100, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);
			}
			if(GUI.Button(new Rect(100, 200, 100, 50), "add stars")){
				StatsController.Instance.ChangeStats(0, Vector3.zero, 60, Vector3.zero, 0, Vector3.zero, 0, new Vector3(0, 0, 0));
			}
			if(GUI.Button(new Rect(100, 300, 100, 50), "add health")){
				DataManager.SubtractHealth(100);
				dataHealth = 0;
				displayHealth = 0;
				StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 27, Vector3.zero, 0, new Vector3(0, 0, 0));
			}
			if(GUI.Button(new Rect(100, 400, 100, 50), "add mood")){
				DataManager.SubtractMood(100);
				dataMood = 0;
				displayMood = 0;
				StatsController.Instance.ChangeStats(0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 85, new Vector3(0, 0, 0));
			}
			if(GUI.Button(new Rect(100, 500, 100, 50), "KABOOYA")){
				DataManager.SubtractMood(100);
				dataMood = 0;
				displayMood = 0;
				DataManager.SubtractHealth(100);
				dataHealth = 0;
				displayHealth = 0;
				StatsController.Instance.ChangeStats(200, Vector3.zero, 100, Vector3.zero, 73, Vector3.zero, 85, new Vector3(0, 0, 0));
			}
		}
	}

	// Parse data and make effects serial
	public void StartCoroutineCurveStats(int deltaPoints, Vector3 pointsOrigin, int deltaStars, Vector3 starsOrigin,
		int deltaHealth, Vector3 healthOrigin, int deltaMood, Vector3 moodOrigin){
		StartCoroutine(StartCurveStats(deltaPoints, pointsOrigin, deltaStars, starsOrigin, deltaHealth, healthOrigin, deltaMood, moodOrigin));
	}

	// Helper function for StartCoroutineCurveStats
	IEnumerator StartCurveStats(int deltaPoints, Vector3 pointsOrigin, int deltaStars, Vector3 starsOrigin,
		int deltaHealth, Vector3 healthOrigin, int deltaMood, Vector3 moodOrigin){

		if(deltaPoints != 0){
			//Default spawn from top if zero, otherwise remove z component, since we are in NGUI
			pointsOrigin = (pointsOrigin == Vector3.zero) ? new Vector3(130f, 500f, 0f) : new Vector3(pointsOrigin.x, pointsOrigin.y - 800, 0);
			StartCurvePoints(deltaPoints, pointsOrigin);
			yield return new WaitForSeconds(1.3f / 200f * deltaPoints);
		}
		if(deltaStars != 0){
			starsOrigin = (starsOrigin == Vector3.zero) ? new Vector3(514f, 500f, 0f) : new Vector3(starsOrigin.x, starsOrigin.y - 800, 0);
			StartCurveStars(deltaStars, starsOrigin);
			yield return new WaitForSeconds(4f / 200f * deltaStars);
		}
		if(deltaHealth != 0){
			healthOrigin = (healthOrigin == Vector3.zero) ? new Vector3(730f, 500f, 0f) : new Vector3(healthOrigin.x, healthOrigin.y - 800, 0);
			StartCurveHealth(deltaHealth, healthOrigin);
			yield return new WaitForSeconds(1.6f / 80f * deltaHealth);
		}
		if(deltaMood != 0){
			moodOrigin = (moodOrigin == Vector3.zero) ? new Vector3(1010f, 500f, 0f) : new Vector3(moodOrigin.x, moodOrigin.y - 800, 0);
			StartCurveMood(deltaMood, moodOrigin);
			yield return new WaitForSeconds(1.6f / 80f * deltaMood);
		}
	}

	public void StartCurvePoints(int deltaPoints, Vector3 pointsOrigin){
		TweenMoveToPoint(HUDElementType.points, deltaPoints, pointsOrigin);
	}

	public void StartCurveStars(int deltaStars, Vector3 starsOrigin){
		TweenMoveToPoint(HUDElementType.stars, deltaStars, starsOrigin);
	}

	public void StartCurveHealth(int deltaHealth, Vector3 healthOrigin){
		TweenMoveToPoint(HUDElementType.health, deltaHealth, healthOrigin);
	}

	public void StartCurveMood(int deltaMood, Vector3 moodOrigin){
		TweenMoveToPoint(HUDElementType.mood, deltaMood, moodOrigin);
	}

	// Using Linear move for now, LeanTween does not have moveLocal curve path
	private void TweenMoveToPoint(HUDElementType type, int amount, Vector3 originPoint){
		float duration = 1f;
		String imageName = null;
		Vector3 endPosition = Vector3.zero;
		float modifier = 3f;	// How many to spawn for each change

		//Testing
		duration = .7f;

		switch(type){
			case(HUDElementType.points):
				imageName = "tweenPoints";
				modifier = Math.Abs(1.3f / 200f * amount);
				endPosition = new Vector3(130f, -25f, 0); //Points
				break;
			case(HUDElementType.stars):
				imageName = "tweenStars";
				modifier = Math.Abs(4f / 200f * amount);
				endPosition = new Vector3(514f, -25f, 0);	//Stars
				break;
			case(HUDElementType.health):
				imageName = "tweenHealthUp";
				modifier = Math.Abs(1.6f / 80f * amount);
				endPosition = new Vector3(730f, -23f, 0);	//Health
				break;
			case(HUDElementType.mood):
				imageName = "tweenMoodUp";
				modifier = Math.Abs(1.6f / 80f * amount);
				endPosition = new Vector3(1010f, -23f, 0); //Mood
				break;
		}

		for(float i = 0f; i < modifier; i += 0.1f){
			// On its own thread, asynchronous
			StartCoroutine(SpawnOneSprite(i, type, imageName, originPoint, endPosition, duration));
		}
	}

	IEnumerator SpawnOneSprite(float waitTime, HUDElementType type, string imageName, Vector3 fromPos, Vector3 toPos, float duration){
		yield return new WaitForSeconds(waitTime);

		// Create the tween image
		UISprite sprite = NGUITools.AddSprite(tweenParent, commonAtlas, imageName);
		sprite.depth = 30;	// TODO-s make this dynamic?
		GameObject go = sprite.gameObject;
		go.transform.localPosition = fromPos;
		go.transform.localScale = new Vector3(33f, 33f, 1f);
		go.AddComponent<DestroyOnCall>();
		ScaleTweenUpDown scaleScript = go.AddComponent<ScaleTweenUpDown>();
		scaleScript.scaleDelta = new Vector3(4f, 4f, 1f);
		scaleScript.duration = .7f;

		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		optional.Add("onCompleteTarget", go);
		optional.Add("onComplete", "DestroySelf");
		LeanTween.moveLocal(go, toPos, duration, optional);

		// Enables the progress bar/count to animate after duration (when the first tween image touches the bar)
		// Hopefully Invoke matches up with LeanTween time
		//TODO sloppy logic speed and order, fix later
		switch(type){
			case(HUDElementType.points):
				if(isFirstTweenPointsCall){
					isFirstTweenPointsCall = false;
					Invoke("StartBarAnimationPoints", duration);
				}
				break;
			case(HUDElementType.stars):
				if(isFirstTweenStarsCall){
					isFirstTweenStarsCall = false;
					Invoke("StartBarAnimationStars", duration);
				}
				break;
			case(HUDElementType.health):
				if(isFirstTweenHealthCall){
					isFirstTweenHealthCall = false;
					Invoke("StartBarAnimationHealth", duration);
				}
				break;
			case(HUDElementType.mood):
				if(isFirstTweenMoodCall){
					isFirstTweenMoodCall = false;
					Invoke("StartBarAnimationMood", duration);
				}
				break;
		}
	}

	public void StartBarAnimationPoints(){
		isAnimatePointsBar = true;
	}

	public void StartBarAnimationStars(){
		isAnimateStarsBar = true;
	}

	public void StartBarAnimationHealth(){
		isAnimateHealthBar = true;
	}

	public void StartBarAnimationMood(){
		isAnimateMoodBar = true;
	}
}
