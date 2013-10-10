using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// event arguments for game state callback
public class GameStateArgs : EventArgs{
	private MinigameStates eState;
	public MinigameStates GetGameState() {
		return eState;	
	}

	public GameStateArgs( MinigameStates eState){
		this.eState = eState;
	}
}	
	

//---------------------------------------------------
// MinigameManager
// Generic manager of a minigame.
//---------------------------------------------------

public class MinigameManager<T> : Singleton<T> where T : MonoBehaviour {
		
	// UIs
	public GameObject goOpening;
	public GameObject goGameOver;
	public GameObject goPaused;
	
	// scene transition
	public SceneTransition scriptTransition;

	// player score
	public UILabel labelScore;
	private int nScore;	
	
	// player lives
	public UILabel labelLives;
	private int nLives;
	public int nStartingLives;
	
	// tutorial stuff
	private MinigameTutorial tutorial;
	public bool IsTutorial() {
		return tutorial != null;
	}
	protected void SetTutorial( MinigameTutorial tutorial ) {
		this.tutorial = tutorial;
		
		this.tutorial.OnTutorialEnd += TutorialEnded;
	}	
	protected MinigameTutorial GetTutorial() {
		return tutorial;	
	}
	
	// the key of this minigame
	protected virtual string GetMinigameKey() {
		// children should implement this
		return null;	
	}
	
	// is there a tutorial override? i.e. tutorial has already been played but the user wants to see it again
	private bool bTutorialOverride;
	protected bool IsTutorialOverride() {
		return bTutorialOverride;	
	}
	protected void SetTutorialOverride( bool bOverride ) {
		bTutorialOverride = bOverride;
	}	
	
	// the state of this minigame
	private MinigameStates eCurrentState = MinigameStates.Opening;
	private void SetGameState( MinigameStates eNewState ) {
		if ( eCurrentState == eNewState ) {
			Debug.Log("Minigame is getting set to a state it's already at: " + eNewState);
			return;
		}
		
		MinigameStates eOldState = eCurrentState;
		eCurrentState = eNewState;
		
		// if the game is being paused, let the audio manager know so it can pause sounds
		if ( eCurrentState == MinigameStates.Paused )
			AudioManager.Instance.Pause(true);
		else if ( eCurrentState == MinigameStates.Playing && eOldState == MinigameStates.Paused )
			AudioManager.Instance.Pause(false);
		
		// send out a message to everything that cares about the game state
		if ( OnStateChanged != null )
			OnStateChanged( this, new GameStateArgs(eCurrentState) );
	}
	protected MinigameStates GetGameState() {
		return eCurrentState;
	}
	
	//=======================Events========================
	public static EventHandler<GameStateArgs> OnStateChanged; 	//when the game state changes
	public static EventHandler<EventArgs> OnNewGame; 		//when a new game starts
	//=====================================================	
	
	//---------------------------------------------------
	// Start()
	// Only stuff that should be done ONCE should be done
	// in this function.  See NewGame() for stuff that
	// should be done whenever the minigame starts.
	//---------------------------------------------------
	IEnumerator Start () {
		// have to yield at start because popup UIs need to run Start()
		yield return 0;
		
		// show the opening UI
		ToggleUI( goOpening, true );
		
		// reset labels
		ResetLabels();
		
		// show the cutscene for the game if it has not yet been viewed
		string strKey = GetMinigameKey();
		if ( HasCutscene() && DataManager.Instance.GameData.Cutscenes.ListViewed.Contains("Cutscene_" + strKey) == false )
			ShowCutscene();		
		
		_Start();
	}
	
	protected virtual void _Start() {
		// children implement	
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------	
	void OnDestroy() {
		_OnDestroy();	
	}
	
	protected virtual void _OnDestroy() {
		// children implement	
	}
	
	//---------------------------------------------------
	// Update()
	// Only call children update if the game is playing.
	//---------------------------------------------------	
	void Update() {
		MinigameStates eState = GetGameState();
		if ( eState == MinigameStates.Playing )
			_Update();
	}
	
	protected virtual void _Update() {
		// children implement this
	}
	
	//---------------------------------------------------
	// NewGame()
	// Game init stuff should go here.
	//---------------------------------------------------	
	private void NewGame() {
		// alert anything that may care about a new game starting
		if ( OnNewGame != null )
			OnNewGame( this, EventArgs.Empty );
		
		// wait one frame so that cleanup from the previous game can happen properly
		StartCoroutine(NewGameAfterFrame());
	}
	
	private IEnumerator NewGameAfterFrame() {
		yield return 0;	
		
		// reset labels
		ResetLabels();
		
		// the game is now playing!
		// this is potentially not the best place to put this...
		// right now I'd say NewGame() means the game is starting
		SetGameState( MinigameStates.Playing );		
		
		_NewGame();		
	}
	
	protected virtual void _NewGame() {
		// children implement this
	}
	
	//---------------------------------------------------
	// ResetLabels()
	// Resets UI labels to their base value.
	//---------------------------------------------------		
	private void ResetLabels() {
		// reset score
		SetScore( 0 );
		
		// reset levels
		SetLives( nStartingLives );		
	}
	
	//---------------------------------------------------
	// ToggleUI()
	// Show or hide one of the UI panels for this mini game.
	//---------------------------------------------------		
	private void ToggleUI( GameObject goUI, bool bShow ) {
		if ( goUI ) {
			PositionTweenToggle script = goUI.GetComponent<PositionTweenToggle>();
			if ( bShow ) {
				script.Show();
				
				// lock clicks
				ClickManager.Instance.ClickLock();
				ClickManager.SetActiveGUIModeLock( true );
			}
			else {
				script.Hide();
				
				// clicks are ok
				ClickManager.Instance.ReleaseClickLock();
				ClickManager.SetActiveGUIModeLock( false );
			}
		}
		else
			Debug.Log("Minigame doesn't have a UI panel but it's trying to be shown!");
	}
	
	//---------------------------------------------------
	// StartGame()
	// This comes from clicking a button.
	//---------------------------------------------------	
	public void StartGame() {
		// minigame is starting, so hide the opening
		ToggleUI( goOpening, false );
		
		// init stuff
		NewGame();
	}
	
	//---------------------------------------------------
	// RestartGame()
	// This comes from clicking a button.
	//---------------------------------------------------		
	public void RestartGame() {
		// this is a little messy...the way the UI Button Message works, we don't really know where this is coming from
		if ( goGameOver.GetComponent<PositionTweenToggle>().IsShowing )
			ToggleUI( goGameOver, false );
		
		if ( goPaused.GetComponent<PositionTweenToggle>().IsShowing )
			ToggleUI( goPaused, false );
		
		if ( IsTutorial() ) {
			tutorial.Abort();
			tutorial = null;	
		}
		
		NewGame();
	}
	
	//---------------------------------------------------
	// TutorialEnded()
	// When the tutorial for this minigame ends.
	//---------------------------------------------------	
	private void TutorialEnded( object sender, TutorialEndEventArgs args ) {
		// if the tutorial did not get finished; don't do anything 
		if ( !args.DidFinish() )
				return;
		
		// set the game to over so that it restarts properly
		// cheat a little bit and DONT use SetGameState() because we don't want the usual stuff to happen
		eCurrentState = MinigameStates.GameOver;
		
		// set the tutorial to null
		tutorial = null;
		
		// turn the override off (in case it was on)
		SetTutorialOverride( false );
		
		// then just restart the game
		RestartGame();
	}	
	
	//---------------------------------------------------
	// Quit()
	// This comes from clicking a button.
	//---------------------------------------------------		
	public void QuitGame() {
		scriptTransition.StartTransition( SceneUtils.BEDROOM );
	}
	
	//---------------------------------------------------
	// SetScore()
	// Sets the player's score and updates the label.
	//---------------------------------------------------	
	private void SetScore( int num ) {
		nScore = num;
		
		if ( labelScore )
			labelScore.text = nScore.ToString();
		
		// Animate if any
		AnimationControl anim = labelScore.transform.parent.gameObject.GetComponent<AnimationControl>();
		if(anim != null){
			anim.Stop();
			anim.Play();
		}
	}
	
	//---------------------------------------------------
	// UpdateScore()
	// Adds num points to the player's current score.
	//---------------------------------------------------	
	public void UpdateScore( int num ) {
		int nNewScore = nScore + num;
		SetScore( nNewScore );
	}	
	
	//---------------------------------------------------
	// SetLives()
	// Sets the player's lives and updates the label.
	//---------------------------------------------------	
	private void SetLives( int num ) {
		nLives = num;
		
		if ( labelLives )
			labelLives.text = nLives.ToString();
		
		// check for game over
		if ( nLives <= 0 )
			GameOver();
		
		// Animate if any
		AnimationControl anim = labelLives.transform.parent.gameObject.GetComponent<AnimationControl>();
		if(anim != null){
			anim.Stop();
			anim.Play();
		}
	}
	
	//---------------------------------------------------
	// UpdateLives()
	// Adds num points to the player's current lives.
	//---------------------------------------------------	
	public void UpdateLives( int num ) {
		int nNew = nLives + num;
		SetLives( nNew );
	}		
	
	//---------------------------------------------------
	// GameOver()
	// The player lost.
	//---------------------------------------------------	
	protected void GameOver() {
		// update the game state
		SetGameState( MinigameStates.GameOver );
		
		// show the game over UI
		ToggleUI( goGameOver, true );
	}
	
	//---------------------------------------------------
	// PauseGame()
	// The game is being paused.
	//---------------------------------------------------	
	protected void PauseGame() {
		// if the game isn't playing, pause shouldn't do anything
		if ( GetGameState() != MinigameStates.Playing )
				return;
		
		// update the game state
		SetGameState( MinigameStates.Paused );
		
		// show the pause game UI
		ToggleUI( goPaused, true );
	}	
	
	//---------------------------------------------------
	// ResumeGame()
	// The game is being unpaused.
	//---------------------------------------------------	
	protected void ResumeGame() {
		// update the game state
		SetGameState( MinigameStates.Playing );
		
		// hide the game paused UI
		ToggleUI( goPaused, false );
	}	
	
	//---------------------------------------------------
	// HowToPlay()
	// The user has manually turned the tutorial back on.
	//---------------------------------------------------		
	private void HowToPlay() {
		SetTutorialOverride( true );
		StartGame();
	}
	
	//---------------------------------------------------
	// ShowCutscene()
	//---------------------------------------------------	
	private void ShowCutscene() {
		string strKey = GetMinigameKey();
		GameObject resourceMovie = Resources.Load("Cutscene_" + strKey) as GameObject;
		LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceMovie );
		CutsceneFrames.OnCutsceneDone += CutsceneDone;	
	}
	
	//---------------------------------------------------
	// CutsceneDone()
	//---------------------------------------------------		
    private void CutsceneDone(object sender, EventArgs args){
		string strKey = GetMinigameKey();
		DataManager.Instance.GameData.Cutscenes.ListViewed.Add("Cutscene_" + strKey);	
		CutsceneFrames.OnCutsceneDone -= CutsceneDone;
    }	
	
	//---------------------------------------------------
	// HasCutscene()
	//---------------------------------------------------		
	protected virtual bool HasCutscene() {
		// children implement this
		return false;
	}
}
