using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public struct StatPair {
	public HUDElementType eType;
	public int nPoints;
	public Vector3 vOrigin;
	public string strSound;
	
	public StatPair( HUDElementType eType, int nPoints, Vector3 vOrigin, string strSound = null ) {
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

public class HUDAnimator : MonoBehaviour {
	public bool isDebug = false;

	//================Events================
	//call when the pet levels up. used this to level up UI components
    public static EventHandler<EventArgs> OnLevelUp;
    //========================================

    public int NextLevelPoints{
    	get{return nextLevelPoints;}
    }

    public Level LastLevel{
    	get{return lastLevel;}
    }

	private int nextLevelPoints; //the minimum requirement for next level up
	private Level lastLevel; //pet's last level
	// private HUD hud;

	// Icon pulsing
	public GameObject healthIconAnim;
	public GameObject moodIconAnim;
	public GameObject starIconAnim;

	// Tweening
	public UIAtlas commonAtlas;
	private GameObject toDestroy;

	// Parent for tweening
	public GameObject tweenParent;
	
	// sounds for animations
	public float fSoundFadeTime;
	public string strSoundStars;
	public string strSoundXP;
	
	// stores elements for easy access
	private Dictionary<HUDElementType, int> hashDisplays = new Dictionary<HUDElementType, int>();
	private Dictionary<HUDElementType, AnimationControl> hashAnimControls = new Dictionary<HUDElementType, AnimationControl>();
	 
	// list of UI sprite objects that may have been spawned
	private List<GameObject> listMovingSprites = new List<GameObject>();
	public bool AreSpawnedSprites() {
		bool b = listMovingSprites.Count > 0;
		return b;
	}

	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Start(){		
		// store all the relevant elements in hashes...kind of annoying
		
		hashAnimControls[HUDElementType.Points] = null;
		hashAnimControls[HUDElementType.Stars] = starIconAnim.GetComponent<AnimationControl>();
		hashAnimControls[HUDElementType.Health] = healthIconAnim.GetComponent<AnimationControl>();
		hashAnimControls[HUDElementType.Mood] = moodIconAnim.GetComponent<AnimationControl>();		
		
		hashDisplays[HUDElementType.Points] = DataManager.Instance.GameData.Stats.Points;
		hashDisplays[HUDElementType.Stars] = DataManager.Instance.GameData.Stats.Stars;
		hashDisplays[HUDElementType.Health] = DataManager.Instance.GameData.Stats.Health;
		hashDisplays[HUDElementType.Mood] = DataManager.Instance.GameData.Stats.Mood;
		
		
		lastLevel = DataManager.Instance.GameData.Level.CurrentLevel;
		nextLevelPoints = LevelUpLogic.Instance.NextLevelPoints();
	}

	void FixedUpdate(){
		LevelUpEventCheck(); // Check if progress bar reach level max
	}
	
	//---------------------------------------------------
	// GetDisplayValue()
	//---------------------------------------------------		
	public int GetDisplayValue( HUDElementType eType ) {
		return hashDisplays[eType];	
	}
	
	//---------------------------------------------------
	// AnimateStatBar()
	// Animates the stat bar for eStat.
	//---------------------------------------------------	
	private IEnumerator AnimateStatBar( HUDElementType eStat, float fDelay ) {
		// wait X seconds
		yield return new WaitForSeconds(fDelay);
		
		// required data for animating the bar
		int nStep = Constants.GetConstant<int>( eStat + "_Step" );
		AnimationControl anim = hashAnimControls[eStat];
		int nTarget = DataManager.Instance.GameData.Stats.GetStat( eStat );
		
		// while the display number is not where we want to be...
		while( hashDisplays[eStat] != nTarget ){
			// add proper signage to the step
			if ( hashDisplays[eStat] > nTarget )
				nStep *= -1;			
			
			// animate by altering the display amount, but don't go over/under the target
			if ( nStep > 0 )
				hashDisplays[eStat] = Mathf.Min( hashDisplays[eStat] + nStep, nTarget );
			else
				hashDisplays[eStat] = Mathf.Max( hashDisplays[eStat] + nStep, nTarget );
				
			// if there is a controller, play it
			if ( anim )
				anim.Play();
			
			// wait one frame
			yield return 0;
			
			// update our target in case it changed
			nTarget = DataManager.Instance.GameData.Stats.GetStat( eStat );
		}
		
		// animating is finished, so stop the control if it exists
		if ( anim )
			anim.Stop();
	}

    //Check if the points progress bar has reached the level requirement
	//if it does call on event listeners and reset the exp points progress bar
	private void LevelUpEventCheck(){
		if(hashDisplays[HUDElementType.Points] >= nextLevelPoints){ //logic for when progress bar reaches level requirement
			int remainderPoints = DataManager.Instance.GameData.Stats.Points - nextLevelPoints; //points to be added after leveling up

			if(OnLevelUp != null)
                OnLevelUp(this, EventArgs.Empty); //Level up. call the UI event listeners

			//reset the progress bar for next level
			DataManager.Instance.GameData.Stats.ResetPoints();
			nextLevelPoints = LevelUpLogic.Instance.NextLevelPoints(); //set the requirement for nxt level
			StatsController.Instance.ChangeStats(remainderPoints, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero, false);
			hashDisplays[HUDElementType.Points] = 0;
			lastLevel = DataManager.Instance.GameData.Level.CurrentLevel;
		}
	}
	
	//---------------------------------------------------
	// StartCurveStats()
	// For each piece of data in the incoming list, this
	// function will start a curve of visual elements for
	// it.
	//---------------------------------------------------	
	public IEnumerator StartCurveStats( List<StatPair> listStat, bool bPlaySounds ){
		for ( int i = 0; i < listStat.Count; ++i ) {
			StatPair pair = listStat[i];
			HUDElementType eType = pair.eType;
			
			//Default spawn from top if zero, otherwise remove z component, since we are in NGUI
			Vector3 vHUD = Constants.GetConstant<Vector3>( eType + "_HUD" );
			Vector3 vOrigin = (pair.vOrigin == Vector3.zero) ? vHUD : new Vector3(pair.vOrigin.x, pair.vOrigin.y - 800, 0);
			StartCurve( eType, pair.nPoints, vOrigin, pair.strSound );
			
			float fModifier = Constants.GetConstant<float>( eType + "_Modifier" );
			yield return new WaitForSeconds( fModifier * pair.nPoints );			
		}
	}
	
	//---------------------------------------------------
	// StartCurve()
	// Starts a curve for one particular stat.
	//---------------------------------------------------	
	public void StartCurve( HUDElementType eType, int nDelta, Vector3 vPointsOrigin, string strSound = null ) {
		TweenMoveToPoint( eType, nDelta, vPointsOrigin, strSound );	
	}

	// Using Linear move for now, LeanTween does not have moveLocal curve path
	private void TweenMoveToPoint(HUDElementType type, int amount, Vector3 originPoint){
		TweenMoveToPoint( type, amount, originPoint, null );	
	}
	
	//---------------------------------------------------
	// TweenMoveToPoint()
	// For one stat, actually prepare and spawn the
	// visuals.
	//---------------------------------------------------	
	private void TweenMoveToPoint(HUDElementType type, int amount, Vector3 originPoint, string strSound){
		float duration = .7f;
		String imageName = null;
		Vector3 endPosition = Vector3.zero;
		float modifier = 3f;	// How many to spawn for each change
		bool isScaleUpDown = false;	// Used for adding animation
		bool isFade = false;	// Used for subtracting animation
		
		string strImageUp = Constants.GetConstant<string>( type + "_Up" );
		string strImageDown = Constants.GetConstant<string>( type + "_Down" );
		float fModifier = Constants.GetConstant<float>( type + "_Modifier" );
		Vector3 vHUD = Constants.GetConstant<Vector3>( type + "_HUD" );
		
		modifier = Math.Abs( fModifier * amount );
		if ( amount > 0 ) {
			endPosition = vHUD;
			isScaleUpDown = true;
			imageName = strImageUp;
		}
		else {
			originPoint = vHUD;
			Vector3 vOffset = Constants.GetConstant<Vector3>( "Below_HUD" );
			endPosition = originPoint + vOffset;
			isFade = true;
			imageName = strImageDown;
		}
		
		// spawns the individual visual elements
		for(float i = 0f; i < modifier; i += 0.1f){
			// On its own thread, asynchronous
			Hashtable hashSoundOverrides = new Hashtable();
			hashSoundOverrides["Pitch"] = 1.0f + i;
			StartCoroutine(SpawnOneSprite(i, type, imageName, originPoint, endPosition, duration, isScaleUpDown, isFade, strSound, hashSoundOverrides));
		}
	}

	//---------------------------------------------------
	// SpawnOneSprite()
	// Spawns one sprite for the visual curve.
	//---------------------------------------------------	
	IEnumerator SpawnOneSprite(float waitTime, HUDElementType type, string imageName, Vector3 fromPos, Vector3 toPos, float duration, bool isScaleUpDown, bool isFade, string strSound, Hashtable hashSoundOverrides){
		yield return new WaitForSeconds(waitTime);
		
		if ( !string.IsNullOrEmpty(strSound) )
			AudioManager.Instance.PlayClip( strSound, hashSoundOverrides );

		// Create the tween image
		UISprite sprite = NGUITools.AddSprite(tweenParent, commonAtlas, imageName);
		sprite.depth = 30;	// TODO-s make this dynamic?
		GameObject go = sprite.gameObject;
		go.transform.localPosition = fromPos;
		go.transform.localScale = new Vector3(33f, 33f, 1f);
		go.AddComponent<DestroyOnCall>();
		if(isScaleUpDown){
			ScaleTweenUpDown scaleScript = go.AddComponent<ScaleTweenUpDown>();
			scaleScript.scaleDelta = new Vector3(4f, 4f, 1f);
			scaleScript.duration = 0.7f;
		}
		if(isFade){
			NGUIAlphaTween alphaScript = go.AddComponent<NGUIAlphaTween>();
			alphaScript.startAlpha = 1f;
			alphaScript.endAlpha = 0f;
			alphaScript.duration = 0.7f;
		}
		
		// also add a component that keeps track of this UI element (if it's xp or starts)
		if ( type == HUDElementType.Stars || type == HUDElementType.Points ) {
			TrackObject scriptTrack = go.AddComponent<TrackObject>();
			scriptTrack.Init( listMovingSprites );
		}
		
		
		Hashtable optional = new Hashtable();
		optional.Add("ease", LeanTweenType.easeInOutQuad);
		optional.Add("onCompleteTarget", go);
		optional.Add("onComplete", "DestroySelf");
		LeanTween.moveLocal(go, toPos, duration, optional);

		// Enables the progress bar/count to animate after duration (when the first tween image touches the bar)
		// Hopefully Invoke matches up with LeanTween time
		if ( waitTime == 0 )
			StartCoroutine( AnimateStatBar( type, duration ) );
	}
}
