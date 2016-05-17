using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public struct StatPair{
	public HUDElementType eType;
	public int nPoints;
	public Vector3 vOrigin;
	public string strSound;
	
	public StatPair(HUDElementType eType, int nPoints, Vector3 vOrigin, string strSound = null){
		this.eType = eType;
		this.nPoints = nPoints;
		this.vOrigin = vOrigin;
		this.strSound = strSound;
	}
}

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

public class HUDAnimator : MonoBehaviour{
	//================Events================
	//call when the pet levels up. used this to level up UI components
	public static EventHandler<EventArgs> OnLevelUp;

	public static EventHandler<EventArgs> OnStatsAnimationDone;

	//========================================

	#region public variables
	//public bool isDebug = false;
	// sounds for animations
	public float soundFadeTime;
	public string soundStars;
	public string soundXP;
	public UIAtlas commonAtlas;
	public AnimationCurve customEaseCurve;
	#endregion

	#region private variables
	private int nextLevelPoints; //the minimum requirement for next level up
	private Level lastLevel; //pet's last level

	// Icon pulsing
	private AnimationControl healthIconAnim;
	private AnimationControl moodIconAnim;
	private AnimationControl starIconAnim;
	private AnimationControl xpIconAnim;
	
	private GameObject toDestroy;
	
	// stores elements for easy access
	private Dictionary<HUDElementType, int> hashDisplays = new Dictionary<HUDElementType, int>();
	private Dictionary<HUDElementType, AnimationControl> hashAnimControls = new Dictionary<HUDElementType, AnimationControl>();
	 
	// list of UI sprite objects that may have been spawned
	private List<GameObject> listMovingSprites = new List<GameObject>();

	// there may be an override for how many of a given sprite we spawn when animating the HUD
	private float fModifierOverride = 0;

	private bool isAnimating = false;
	public bool IsAnimating{
		get{
			return isAnimating;
		}
	}

	#endregion

	#region Getter/Setters
	public int NextLevelPoints{
		get{ return nextLevelPoints;}
	}
	
	public Level LastLevel{
		get{ return lastLevel;}
	}

	public bool AreSpawnedSprites(){
		bool b = listMovingSprites.Count > 0;
		return b;
	}

	public void SetModifierOverride(float fOverride){
		fModifierOverride = fOverride;	
	}

	public float GetModifierOverride(){
		return fModifierOverride;	
	}
	#endregion

	private float GetModifier(HUDElementType eType){
		float fModifier = Constants.GetConstant<float>(eType + "_Modifier");
		
		// if there is an override, use that instead
		float fOverride = GetModifierOverride();
		if(fOverride > 0)
			fModifier = fOverride;
		
		return fModifier;
	}
	
	void Start(){
		healthIconAnim = HUDUIManager.Instance.animHealth;
		moodIconAnim = HUDUIManager.Instance.animMood;
		starIconAnim = HUDUIManager.Instance.animMoney;
		xpIconAnim = HUDUIManager.Instance.animXP;
		
		// store all the relevant elements in hashes...kind of annoying
		hashAnimControls[HUDElementType.Points] = xpIconAnim.GetComponent<AnimationControl>();
		hashAnimControls[HUDElementType.Stars] = starIconAnim.GetComponent<AnimationControl>();
		hashAnimControls[HUDElementType.Health] = healthIconAnim.GetComponent<AnimationControl>();
		hashAnimControls[HUDElementType.Mood] = moodIconAnim.GetComponent<AnimationControl>();		
		
		// Model > View, exception!
		hashDisplays[HUDElementType.Points] = DataManager.Instance.GameData.Stats.Points;
		hashDisplays[HUDElementType.Stars] = DataManager.Instance.GameData.Stats.Stars;
		hashDisplays[HUDElementType.Health] = DataManager.Instance.GameData.Stats.Health;
		hashDisplays[HUDElementType.Mood] = DataManager.Instance.GameData.Stats.Mood;
		
		
		lastLevel = LevelLogic.Instance.CurrentLevel; 
		nextLevelPoints = LevelLogic.Instance.NextLevelPoints();
	}

	void FixedUpdate(){
		LevelUpEventCheck(); // Check if progress bar reach level max
	}
	
	//---------------------------------------------------
	// GetDisplayValue()
	//---------------------------------------------------		
	public int GetDisplayValue(HUDElementType eType){
		return hashDisplays[eType];
	}

	//Check if the points progress bar has reached the level requirement
	//if it does call on event listeners and reset the exp points progress bar
	private void LevelUpEventCheck(){
		if(hashDisplays[HUDElementType.Points] >= nextLevelPoints){ //logic for when progress bar reaches level requirement
			int remainderPoints = DataManager.Instance.GameData.Stats.Points - nextLevelPoints; //points to be added after leveling up

			// increment level
			LevelLogic.Instance.IncrementLevel();

			//Check for Unlock badge
			BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Level, (int)LevelLogic.Instance.CurrentLevel, true);
			
			if(OnLevelUp != null){
				OnLevelUp(this, EventArgs.Empty); //Level up. call the UI event listeners
			}

			//reset the progress bar for next level
			DataManager.Instance.GameData.Stats.ResetPoints();
			nextLevelPoints = LevelLogic.Instance.NextLevelPoints(); //set the requirement for nxt level
			StatsController.Instance.ChangeStats(deltaPoints: remainderPoints, isPlaySounds: false);
			hashDisplays[HUDElementType.Points] = 0;
			lastLevel = LevelLogic.Instance.CurrentLevel;
		}
	}
	
	/// <summary>
	/// For each piece of data in the incoming list, this function will start a curve of visual elements for it.
	/// </summary>
	/// <returns>The curve stats.</returns>
	/// <param name="statsTypeList">Stats type list.</param>
	/// <param name="isPlaySounds">If set to <c>true</c> is play sounds.</param>
	/// <param name="isAllAtOnce">If set to <c>true</c> is all at once.</param>
	/// <param name="isFloaty">If set to <c>true</c> is floaty.</param>
	public IEnumerator StartCurveStats(List<StatPair> statsTypeList, bool isPlaySounds, bool isAllAtOnce, bool isFloaty, float animDelay){
		isAnimating = true;
		yield return new WaitForSeconds(animDelay);
		// One loop for each TYPE of stat (Coin, Stars, etc)
		for(int i = 0; i < statsTypeList.Count; ++i){
			StatPair pair = statsTypeList[i];
			HUDElementType eType = pair.eType;
			
			// If it is a floaty text, just increment values instantaneously
			if(isFloaty){
				// this code will instead animate the bar
				// leaving old code in since this is one of the last things I do; in case things backfire, just uncomment and delete
				StartCoroutine(AnimateStatBar(eType, 0));	// TODO why is there no delay?
			}
			// If not floaty text, play tween sprite animation to HUD
			else{
				//Default spawn from top if zero, otherwise remove z component, since we are in NGUI
				Vector3 vHUD = Constants.GetConstant<Vector3>(eType + "_HUD");
				Vector3 vOrigin = (pair.vOrigin == Vector3.zero) ? vHUD : new Vector3(pair.vOrigin.x, pair.vOrigin.y, 0);

				if(isPlaySounds)
					StartCurve(eType, pair.nPoints, vOrigin, pair.strSound);
				else
					StartCurve(eType, pair.nPoints, vOrigin);
				
				// some places might want to do all the stat curves at once
				if(isAllAtOnce == false){
					float fModifier = GetModifier(eType);
					yield return new WaitForSeconds(fModifier * pair.nPoints);	
				}
			}

			// Soft check for detecting when anim is done, its so complicated we are just going to estimate here
			if(i == statsTypeList.Count - 1){
				Invoke("CallFinishedAnimation", 0.8f);
			}
		}
	}

	private void CallFinishedAnimation(){
		if(OnStatsAnimationDone != null){
			OnStatsAnimationDone(this, EventArgs.Empty);
		}
	}
	
	/// <summary>
	/// Starts a curve for one particular stat.
	/// </summary>
	/// <param name="type">HUDElementType type</param>
	/// <param name="amount">Amount</param>
	/// <param name="originPoint">Origin point</param>
	/// <param name="sound">Sound optional</param>
	private void StartCurve(HUDElementType type, int amount, Vector3 originPoint, string sound = null){
		TweenMoveToPoint(type, amount, originPoint, sound);	
	}
	
	//---------------------------------------------------
	// TweenMoveToPoint()
	// For one stat, actually prepare and spawn the
	// visuals.
	//---------------------------------------------------	
	private void TweenMoveToPoint(HUDElementType type, int amount, Vector3 originPoint, string sound){
		float duration = Constants.GetConstant<float>("HudCurveDuration");
		String imageName = null;
		Vector3 endPosition = Vector3.zero;
		float modifier = 3f;	// How many to spawn for each change
		bool isPlusAnimation = false;	// Used for "adding" animation otherwise "substracting" animation
		string strImageUp = Constants.GetConstant<string>(type + "_Up");
		string strImageDown = Constants.GetConstant<string>(type + "_Down");
		float fModifier = GetModifier(type);
		Vector3 vHUD = Constants.GetConstant<Vector3>(type + "_HUD");

		GameObject tweenParent = null;//HUDUIManager.Instance.GetTweenParent(Constants.GetConstant<String>(type + "_Anchor"));	// Check which anchor this is in

		modifier = Math.Abs(fModifier * amount);
		if(amount > 0){
			endPosition = vHUD;
			isPlusAnimation = true;
			imageName = strImageUp;
		}
		else{
			originPoint = vHUD;
			Vector3 vOffset = Constants.GetConstant<Vector3>("Below_HUD");
			endPosition = originPoint + vOffset;
			isPlusAnimation = false;
			imageName = strImageDown;
		}
		
		// spawns the individual visual elements
		for(float i = 0f; i < modifier; i += 0.1f){
			// On its own thread
			Hashtable hashSoundOverrides = new Hashtable();
			hashSoundOverrides["Pitch"] = 1.0f + i;
			StartCoroutine(SpawnOneSprite(tweenParent, i, type, imageName, originPoint, endPosition, duration, isPlusAnimation, sound, hashSoundOverrides));
		}
	}

	//---------------------------------------------------
	// SpawnOneSprite()
	// Spawns one sprite for the visual curve.
	//---------------------------------------------------	
	IEnumerator SpawnOneSprite(GameObject tweenParent, float waitTime, HUDElementType type, string imageName,
	                           Vector3 fromPos, Vector3 toPos, float duration, bool isPlusAnimation,
	                           string strSound, Hashtable hashSoundOverrides){

		yield return new WaitForSeconds(waitTime);

		if(!string.IsNullOrEmpty(strSound))
			AudioManager.Instance.PlayClip(strSound, option: hashSoundOverrides);

		// Modify some tweening behaviors based on adding or subtracting a stat
		if(isPlusAnimation){
			// Create the tween image
			UISprite sprite = NGUITools.AddSprite(tweenParent, commonAtlas, imageName);
			sprite.depth = 30;	// TODO-s make this dynamic?
			GameObject go = sprite.gameObject;
			go.transform.localPosition = fromPos;
			go.transform.localScale = new Vector3(10f, 10f, 1f);

			RotateAroundCenter rotateScript = go.AddComponent<RotateAroundCenter>();
			rotateScript.speed = 500;
			rotateScript.Play();

			ScaleTweenUpDown scaleScript = go.AddComponent<ScaleTweenUpDown>();
			scaleScript.scaleFactor = Constants.GetConstant<Vector3>("HudCurveAddScale");
			scaleScript.duration = Constants.GetConstant<float>("HudCurveDuration");

			// Addition tweening behavior
			Vector3[] path = new Vector3[4];
			path[0] = go.transform.localPosition;
			Vector3 randomPoint = GameObjectUtils.GetRandomPointOnCircumference(go.transform.localPosition, 200f);
			path[1] = randomPoint;
			path[2] = path[1];
			path[3] = toPos;

			LeanTween.moveLocal(go, path, duration).setEase(customEaseCurve).setDestroyOnComplete(true);
		}
		else{
			// Add subtraction tweening logic here

			// Disable curve wait time for stat penalty
			duration = 0;
		}

		// Enables the progress bar/count to animate after duration (when the first tween image touches the bar)
		// Hopefully Invoke matches up with LeanTween time
		if(waitTime == 0){
			StartCoroutine(AnimateStatBar(type, duration));
		}
	}

	//---------------------------------------------------
	// AnimateStatBar()
	// Animates the stat bar for eStat.
	//---------------------------------------------------	
	private IEnumerator AnimateStatBar(HUDElementType eStat, float delay){
		// wait X seconds
		yield return new WaitForSeconds(delay);
		
		// required data for animating the bar
		int step = Constants.GetConstant<int>(eStat + "_Step");
		AnimationControl anim = hashAnimControls[eStat];
		int target = StatsController.Instance.GetStat(eStat);

		if(hashDisplays[eStat] > target){
			step *= -1;
		}

		// while the display number is not where we want to be...
		while(hashDisplays[eStat] != target){

			// animate by altering the display amount, but don't go over/under the target
			if(step > 0){
				hashDisplays[eStat] = Mathf.Min(hashDisplays[eStat] + step, target);
			}
			else{
				hashDisplays[eStat] = Mathf.Max(hashDisplays[eStat] + step, target);
			}

			// if there is a controller, play it
			if(anim){
				bool auxPlayParticle = (step > 0) ? true : false;
				anim.Play(auxPlayParticle);
			}

			// wait one frame
			yield return 0;
			
			// update our target in case it changed
			target = StatsController.Instance.GetStat(eStat);
		}

		// animating is finished, so stop the control if it exists
		if(anim){
			anim.Stop();
		}
	}
}
