using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public struct StatPair {
	public StatType statType;
	public int value;
	public Vector3 posOrigin;
	public string soundKey;

	public StatPair(StatType _statType, int _value, Vector3 _posOrigin, string _soundKey = null) {
		statType = _statType;
		value = _value;
		posOrigin = _posOrigin;
		soundKey = _soundKey;
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
public class HUDAnimator : MonoBehaviour {
	//call when the pet levels up. used this to level up UI components
	public static EventHandler<EventArgs> OnLevelUp;
	public static EventHandler<EventArgs> OnStatsAnimationDone;

	public Animator anim;           // Controls all the animations for a single stat

	public GameObject tweenSpritePrefab;
	public AnimationCurve customEaseCurve;

	private int nextLevelPoints;    // Requirement for next level up
	public int NextLevelPoints {
		get { return nextLevelPoints; }
	}

	private Level lastLevel;        // Pet's last level
	public Level LastLevel {
		get { return lastLevel; }
	}

	// stores elements for easy access
	private Dictionary<StatType, int> hashDisplays = new Dictionary<StatType, int>();

	private bool isAnimating = false;
	public bool IsAnimating {
		get { return isAnimating; }
	}

	private bool isXpAnimating = false;
	private bool isHealthAnimating = false;
	private bool isHungerAnimating = false;
	private bool isCoinAnimating = false;

	void Start() {
		// Model > View, exception!
		hashDisplays.Add(StatType.Xp, StatsManager.Instance.GetStat(StatType.Xp));
		hashDisplays.Add(StatType.Coin, StatsManager.Instance.GetStat(StatType.Coin));
		hashDisplays.Add(StatType.Health, StatsManager.Instance.GetStat(StatType.Health));
		hashDisplays.Add(StatType.Hunger, StatsManager.Instance.GetStat(StatType.Hunger));

		lastLevel = LevelLogic.Instance.CurrentLevel;
		nextLevelPoints = LevelLogic.Instance.NextLevelPoints();
	}

	void FixedUpdate() {
		LevelUpEventCheck(); // Check if progress bar reach level max
	}

	//---------------------------------------------------
	// GetDisplayValue()
	//---------------------------------------------------		
	public int GetDisplayValue(StatType eType) {
		return hashDisplays[eType];
	}

	//Check if the points progress bar has reached the level requirement
	//if it does call on event listeners and reset the exp points progress bar
	private void LevelUpEventCheck() {
		if(hashDisplays[StatType.Xp] >= nextLevelPoints) { //logic for when progress bar reaches level requirement
			int remainderPoints = DataManager.Instance.GameData.Stats.Points - nextLevelPoints; //points to be added after leveling up

			// increment level
			LevelLogic.Instance.IncrementLevel();

			//Check for Unlock badge
			BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.Level, (int)LevelLogic.Instance.CurrentLevel, true);

			if(OnLevelUp != null) {
				OnLevelUp(this, EventArgs.Empty); //Level up. call the UI event listeners
			}

			//reset the progress bar for next level
			DataManager.Instance.GameData.Stats.ResetCurrentLevelXp();
			nextLevelPoints = LevelLogic.Instance.NextLevelPoints(); //set the requirement for nxt level
			StatsManager.Instance.ChangeStats(xpDelta: remainderPoints);
			hashDisplays[StatType.Xp] = 0;
			lastLevel = LevelLogic.Instance.CurrentLevel;
		}
	}

	/// <summary>
	/// For each piece of data in the incoming list, this function will start a curve of visual elements for it.
	/// </summary>
	/// <returns>The curve stats.</returns>
	/// <param name="statsTypeList">Stats type list.</param>
	/// <param name="isFloaty">If set to <c>true</c> is floaty.</param>
	public IEnumerator StartCurveStats(List<StatPair> statsTypeList, bool isFloaty, float animDelay) {
		isAnimating = true;
		yield return new WaitForSeconds(animDelay);
		// One loop for each TYPE of stat (Coin, Stars, etc)
		for(int i = 0; i < statsTypeList.Count; ++i) {
			StatPair pair = statsTypeList[i];
			StatType statType = pair.statType;

			// If it is a floaty text, just increment values instantaneously
			if(isFloaty) {
				// this code will instead animate the bar
				StatTick(statType);
			}
			// If not floaty text, play tween sprite animation to HUD
			else {
				Vector3 vOrigin = new Vector3(pair.posOrigin.x, pair.posOrigin.y, 0);
				Debug.Log(pair.value);
				StartCurve(statType, pair.value, vOrigin, pair.soundKey);

				float fModifier = GetSpawnCountModifier(statType);
				yield return new WaitForSeconds(fModifier * pair.value);
			}

			// Soft check for detecting when anim is done, its so complicated we are just going to estimate here
			if(i == statsTypeList.Count - 1) {
				Invoke("CallFinishedAnimation", 0.8f);
			}
		}
	}

	private void CallFinishedAnimation() {
		isAnimating = false;
		if(OnStatsAnimationDone != null) {
			OnStatsAnimationDone(this, EventArgs.Empty);
		}
	}

	/// <summary>
	/// Starts a curve for one particular stat.
	/// </summary>
	private void StartCurve(StatType type, int amount, Vector3 originPoint, string sound = null) {
		TweenMoveToPoint(type, amount, originPoint, sound);
	}

	/// <summary>
	/// For one stat, actually prepare and spawn the visuals.
	/// </summary>
	private void TweenMoveToPoint(StatType statType, int amount, Vector3 fromPos, string sound) {
		Vector3 toPos;
		float duration = 1f;
        float modifier = 3f;			// How many to spawn for each change
		bool isPlusAnimation = false;	// Used for "adding" animation otherwise "substracting" animation
		float fModifier = GetSpawnCountModifier(statType);

		GameObject tweenParent = null;	//HUDUIManager.Instance.GetTweenParent(Constants.GetConstant<String>(type + "_Anchor"));	// Check which anchor this is in

		modifier = Math.Abs(fModifier * amount);
		if(amount > 0) {
			toPos = HUDUIManager.Instance.GetStatEndPositions(statType);
			isPlusAnimation = true;
		}
		else {
			fromPos = Vector3.zero;
			Vector3 vOffset = new Vector3(0, -75, 0);
			toPos = fromPos + vOffset;
			isPlusAnimation = false;
		}

		// spawns the individual visual elements
		for(float i = 0f; i < modifier; i += 0.1f) {
			// On its own thread
			Hashtable hashSoundOverrides = new Hashtable();
			hashSoundOverrides["Pitch"] = 1.0f + i;
			StartCoroutine(SpawnOneSprite(tweenParent, i, statType, fromPos, toPos, duration, isPlusAnimation, sound, hashSoundOverrides));
		}
	}

	/// <summary>
	/// Spawns one sprite for the visual curve.
	/// </summary>
	IEnumerator SpawnOneSprite(GameObject tweenParent, float delay, StatType statType,
							   Vector3 fromPos, Vector3 toPos, float duration, bool isPlusAnimation,
							   string strSound, Hashtable hashSoundOverrides) {

		yield return new WaitForSeconds(delay);

		if(!string.IsNullOrEmpty(strSound)) {
			AudioManager.Instance.PlayClip(strSound, option: hashSoundOverrides);
		}

		// Modify some tweening behaviors based on adding or subtracting a stat
		if(isPlusAnimation) {
			GameObject go = GameObjectUtils.AddChildGUI(gameObject, tweenSpritePrefab);
			go.transform.localPosition = fromPos;
			go.GetComponent<Image>().sprite = SpriteCacheManager.GetHudTweenIcon(statType);

			// Addition tweening behavior
			Vector3[] path = new Vector3[4];
			path[0] = go.transform.localPosition;
			Vector3 randomPoint = GameObjectUtils.GetRandomPointOnCircumference(go.transform.localPosition, 200f);
			path[1] = randomPoint;
			path[2] = path[1];
			path[3] = toPos;

			LeanTween.moveLocal(go, path, duration)
				.setEase(customEaseCurve)
				.setOnComplete(StatTick).setOnCompleteParam(statType)
				.setDestroyOnComplete(true);
		}
		else {
			// Negative anim, just jump to tick and bar animate
			StatTick(statType);
		}
	}

	/// <summary>
	/// When stat reaches the bar
	/// We use object here because of leantween parameters
	/// </summary>
	private void StatTick(object obj) {
		StatType statType = (StatType)obj;

		// Play 1 pulse animation on bar
		PlayTickAnimation(statType);

		if(!GetStatAnimating(statType)) {
			SetStatAnimating(statType, true);
			StartCoroutine(StatBarAnimate(statType));
		}
	}

	/// <summary>
	/// Toggle a stat bar to animate
	/// </summary>
	private IEnumerator StatBarAnimate(StatType statType) {
		Debug.Log("Pinging stat " + statType.ToString());
		// Calculate a step
		int step = GetStepModifier(statType);
		int target = StatsManager.Instance.GetStat(statType);
		if(hashDisplays[statType] > target) {
			step *= -1;
		}

		Debug.Log(hashDisplays[statType] + " " + target);
		// While the display number is not where we want to be...
		while(hashDisplays[statType] != target) {
			// animate by altering the display amount, but don't go over/under the target
			if(step > 0) {
				hashDisplays[statType] = Mathf.Min(hashDisplays[statType] + step, target);
			}
			else {
				hashDisplays[statType] = Mathf.Max(hashDisplays[statType] + step, target);
			}

			yield return 0;

			// update our target in case it changed
			target = StatsManager.Instance.GetStat(statType);
		}

		if(hashDisplays[statType] == target) {
			SetStatAnimating(statType, false);
		}
	}

	// ==================================================

	public void PlayNeedCoinAnimation() {
		anim.SetTrigger("CoinRequired");
	}

	public void PlayTickAnimation(StatType statType) {
		switch(statType) {
			case StatType.Xp:
				anim.SetTrigger("LevelTick");
				return;
			case StatType.Health:
				anim.SetTrigger("HealthTick");
				return;
			case StatType.Hunger:
				anim.SetTrigger("HungerTick");
				return;
			case StatType.Coin:
				anim.SetTrigger("CoinTick");
				return;
			default:
				Debug.LogError("Invalid stat type for anim " + statType.ToString());
				return;
		}
	}

	#region modifiers
	private float GetSpawnCountModifier(StatType statType) {
		switch(statType) {
			case StatType.Xp:
				return 0.02f;
			case StatType.Health:
				return 0.02f;
			case StatType.Hunger:
				return 0.02f;
			case StatType.Coin:
				return 0.03f;
			default:
				Debug.LogError("Invalid stat type for count modifier " + statType.ToString());
				return 0f;
		}
	}

	private int GetStepModifier(StatType statType) {
		switch(statType) {
			case StatType.Xp:
				return 3;
			case StatType.Health:
				return 1;
			case StatType.Hunger:
				return 1;
			case StatType.Coin:
				return 1;
			default:
				Debug.LogError("Invalid stat type for step modifier " + statType.ToString());
				return 0;
		}
	}

	private void SetStatAnimating(StatType statType, bool isAnimating) {
		switch(statType) {
			case StatType.Xp:
				isXpAnimating = isAnimating;
				break;
			case StatType.Health:
				isHealthAnimating = isAnimating;
				break;
			case StatType.Hunger:
				isHungerAnimating = isAnimating;
				break;
			case StatType.Coin:
				isCoinAnimating = isAnimating;
				break;
			default:
				Debug.LogError("Invalid stat type for set stat animating " + statType.ToString());
				return;
		}
	}

	private bool GetStatAnimating(StatType statType) {
		switch(statType) {
			case StatType.Xp:
				return isXpAnimating;
			case StatType.Health:
				return isHealthAnimating;
			case StatType.Hunger:
				return isHungerAnimating;
			case StatType.Coin:
				return isCoinAnimating;
			default:
				Debug.LogError("Invalid stat type for get stat animating " + statType.ToString());
				return false;
		}
	}
	#endregion
}
