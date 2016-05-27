using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// event arguments for game state callback
public class GameStateArgs : EventArgs{
	private MinigameStates eState;

	public MinigameStates GetGameState(){
		return eState;	
	}

	public GameStateArgs(MinigameStates eState){
		this.eState = eState;
	}
}	

// event arguments for lives updated callback
public class LivesChangedArgs : EventArgs{
	private int nChange;

	public int GetChange(){
		return nChange;	
	}
	
	public LivesChangedArgs(int nChange){
		this.nChange = nChange;
	}
}

//---------------------------------------------------
// MinigameManager
// Generic manager of a minigame.
//---------------------------------------------------

public abstract class MinigameManager<T> : Singleton<T> where T : MonoBehaviour{	
	//----------------- Abstract -----------------------------
	public abstract int GetReward(MinigameRewardTypes eType);       // returns the reward the player got for playing this minigame
	public abstract string GetMinigameKey();						// returns string key for this minigame
	protected abstract void _Start();								// when the manager is started

	//=======================Events========================
	public static EventHandler<GameStateArgs> OnStateChanged; 		// when the game state changes
	public static EventHandler<EventArgs> OnNewGame; 				// when a new game starts
	public static EventHandler<LivesChangedArgs> OnLivesChanged;	// when lives are changed
	
	public MinigameUI ui; // reference to the UI controller
	public int nStartingLives;
	public string gameOverAudioClip = "minigameGameOver";

	private int score;	// player score
	public int lives; // player lives
	private bool tutorialOverride; // is there a tutorial override? 
	//i.e. tutorial has already been played but the user wants to see it again
	private MinigameStates currentState = MinigameStates.Opening; // the state of this minigame
	private MinigameTutorial tutorial; // Reference to the tutorial. Null when tutorial is not active

	protected string quitGameScene = null;	// Over write this in child on awake
	public bool IsNewGameAd = false;		// Used to show ads only once per new game

	//Return player score
	public virtual int GetScore(){
		return score;	
	}

	//Return player lives	
	public int GetLives(){
		return lives;	
	}

	//T: tutorial is active, F: tutorial is not running
	public bool IsTutorialRunning(){
		return tutorial != null;
	}

	//Set reference to the tutorial
	protected void SetTutorial(MinigameTutorial tutorial){
		this.tutorial = tutorial;
		this.tutorial.OnTutorialEnd += TutorialEnded;
	}	

	//Return the reference to tutorial
	protected MinigameTutorial GetTutorial(){
		return tutorial;	
	}

	//T: Tutorial is on so play tutorial, F: Tutorial off so don't play tutorial
	protected virtual bool IsTutorialOn(){
		return true;	
	}

	//T: Play tutorial again even if it has already been played	
	protected bool IsTutorialOverride(){
		return tutorialOverride;	
	}

	//Toggle bTutorialOverride
	protected void SetTutorialOverride(bool bOverride){
		tutorialOverride = bOverride;
	}

	//Change the game state	
	private void SetGameState(MinigameStates eNewState){
//		Debug.Log("Setting game state " + eNewState.ToString());
		if(currentState == eNewState){
			Debug.LogError("Minigame(" + GetMinigameKey() + ") is getting set to a state it's already at: " + eNewState);
			return;
		}
		
		MinigameStates eOldState = currentState;
		currentState = eNewState;
		
		// if the game is being paused, let the audio manager know so it can pause sounds
		if(currentState == MinigameStates.Paused)
			AudioManager.Instance.PauseBackground(true);
		else if(currentState == MinigameStates.Playing && eOldState == MinigameStates.Paused)
			AudioManager.Instance.PauseBackground(false);
		
		// send out a message to everything that cares about the game state
		if(OnStateChanged != null)
			OnStateChanged(this, new GameStateArgs(currentState));
	}

	//Return the game state
	public MinigameStates GetGameState(){
		return currentState;
	}

	//---------------------------------------------------
	// Start()
	// Only stuff that should be done ONCE should be done
	// in this function.  See NewGame() for stuff that
	// should be done whenever the minigame starts.
	//---------------------------------------------------
	IEnumerator Start(){
		// have to yield at start because popup UIs need to run Start()
		yield return 0;
		
		// show the opening UI
		ui.TogglePopup(MinigamePopups.Opening, true);
		
		// reset labels
		ResetLabels();
		
		_Start();
	}
	
	//---------------------------------------------------
	// OnDestroy()
	//---------------------------------------------------	
	void OnDestroy(){
		_OnDestroy();	
	}
	
	protected virtual void _OnDestroy(){
		// children implement
	}
	
	//---------------------------------------------------
	// Update()
	// Only call children update if the game is playing.
	//---------------------------------------------------	
	void Update(){
		MinigameStates eState = GetGameState();
		if(eState == MinigameStates.Playing)
			_Update();
	}
	
	protected virtual void _Update(){
		// children implement this
	}
	
	//---------------------------------------------------
	// NewGame()
	// Game init stuff should go here.
	//---------------------------------------------------	
	private void NewGame(){
		// alert anything that may care about a new game starting
		if(OnNewGame != null){
			OnNewGame(this, EventArgs.Empty);
		}

		IsNewGameAd = true;

		// wait one frame so that cleanup from the previous game can happen properly
		StartCoroutine(NewGameAfterFrame());
	}
	
	private IEnumerator NewGameAfterFrame(){
		yield return 0;	

		// Want to hide inventory if we got fire crystal and restart again
		if(InventoryUIManager.Instance){
			InventoryUIManager.Instance.HidePanel();
		}

		// reset labels
		ResetLabels();
		
		// the game is now playing!
		// this is potentially not the best place to put this...
		// right now I'd say NewGame() means the game is starting
		SetGameState(MinigameStates.Playing);
		StatsManager.Instance.ChangeStats(hungerDelta: -5, isInternal: true);
		_NewGame();		
	}
	
	protected virtual void _NewGame(){
		// children implement this
	}
	
	//---------------------------------------------------
	// ResetLabels()
	// Resets UI labels to their base value.
	//---------------------------------------------------		
	private void ResetLabels(){
		// reset score
		SetScore(0);
		
		// reset levels
		SetLives(nStartingLives);		
	}
	
	//---------------------------------------------------
	// StartGame()
	// This comes from clicking a button.
	//---------------------------------------------------	
	public void StartGame(){
		// minigame is starting, so hide the opening
		ui.TogglePopup(MinigamePopups.Opening, false);
		
		// Analytics.Instance.StartPlayTimeTracker();		

		// init stuff
		if(currentState != MinigameStates.Playing){
			NewGame();
		}
	}

	//---------------------------------------------------
	// RestartGame()
	// This comes from clicking a button.
	//---------------------------------------------------	
	public void RestartGame(){
		RestartGame(minusMood:true);
	}
		
	public void RestartGame(bool minusMood = true){
		if(minusMood){
			StatsManager.Instance.ChangeStats(hungerDelta: -10, isInternal: true);
		}
		AudioManager.Instance.PauseBackground(false);

		// this is a little messy...the way the UI Button Message works, we don't really know where this is coming from
		if(ui.IsPopupShowing(MinigamePopups.GameOver))
			ui.TogglePopup(MinigamePopups.GameOver, false);
		
		if(ui.IsPopupShowing(MinigamePopups.Pause))
			ui.TogglePopup(MinigamePopups.Pause, false);
		
		if(IsTutorialRunning()){
			tutorial.Abort();
			tutorial = null;	
		}

		SetGameState(MinigameStates.Restarting);
		NewGame();
	}
	
	//---------------------------------------------------
	// TutorialEnded()
	// When the tutorial for this minigame ends.
	//---------------------------------------------------	
	private void TutorialEnded(object sender, TutorialEndEventArgs args){
		// if the tutorial did not get finished; don't do anything 
		if(!args.DidFinish())
			return;
		
		// set the game to over so that it restarts properly
		// cheat a little bit and DONT use SetGameState() because we don't want the usual stuff to happen
		currentState = MinigameStates.GameOver;
		
		// set the tutorial to null
		tutorial = null;
		
		// turn the override off (in case it was on)
		SetTutorialOverride(false);
		
		// then just restart the game
		RestartGame(minusMood:false);
	}	
	
	//---------------------------------------------------
	// Quit()
	// This comes from clicking a button.
	//---------------------------------------------------		
	public virtual void QuitGame(){
		if(quitGameScene != null){
			// this is a little messy...the way the UI Button Message works, we don't really know where this is coming from
			if(ui.IsPopupShowing(MinigamePopups.GameOver)){
				ui.TogglePopup(MinigamePopups.GameOver, false);

				LoadLevelManager.Instance.StartLoadTransition(quitGameScene);
			}
			
			//double confirm quit game
			if(ui.IsPopupShowing(MinigamePopups.Pause)){

				PopupNotificationNGUI.Callback button1Function = delegate(){
					ui.TogglePopup(MinigamePopups.Pause, false);

					LoadLevelManager.Instance.StartLoadTransition(quitGameScene);
				};

				PopupNotificationNGUI.Callback button2Function = delegate(){
				};

				Hashtable notificationEntry = new Hashtable();
				notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.MiniGameQuitCheck);
				notificationEntry.Add(NotificationPopupFields.Message, Localization.Localize("MG_DELETE_CONFIRM")); 
				notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
				notificationEntry.Add(NotificationPopupFields.Button2Callback, button2Function);

				NotificationUIManager.Instance.AddToQueue(notificationEntry);
			}
		}
		else{
			Debug.LogError("quitGameScene not set in child");
		}
	}
	
	//---------------------------------------------------
	// SetScore()
	// Sets the player's score and updates the label.
	//---------------------------------------------------	
	protected void SetScore(int num){
		score = num;
		
		// update ui
		ui.SetLabel(MinigameLabels.Score, num.ToString());
	}
	
	//---------------------------------------------------
	// UpdateScore()
	// Adds num points to the player's current score.
	//---------------------------------------------------	
	public void UpdateScore(int num){
		if(!IsTutorialRunning()){
			int nNewScore = score + num;
			SetScore(nNewScore);
		}
	}	
	
	//---------------------------------------------------
	// SetLives()
	// Sets the player's lives and updates the label.
	//---------------------------------------------------	
	private void SetLives(int num){		
		lives = num;
		
		// update ui
		ui.SetLabel(MinigameLabels.Lives, num.ToString());
		
		// check for game over
		if(lives <= 0)
			GameOver();
	}
	
	//---------------------------------------------------
	// UpdateLives()
	// Adds num points to the player's current lives.
	//---------------------------------------------------	
	public void UpdateLives(int num){
		if(!IsTutorialRunning()){
			// for now, current lives cannot go above the starting value...
			int nCurLives = GetLives();
			
			int nNew = lives + num;
			SetLives(nNew);
			
			// send callback because lives are chaning
			if(OnLivesChanged != null)
				OnLivesChanged(this, new LivesChangedArgs(num));		
		}
	}		
	
	//---------------------------------------------------
	// GameOver()
	// The player lost.
	//---------------------------------------------------	
	protected void GameOver(){
		// send out a completion task
//		WellapadMissionController.Instance.TaskCompleted("Play" + GetMinigameKey());
		AudioManager.Instance.PlayClip(gameOverAudioClip);

		// send out score task
		int nScore = GetScore();
		WellapadMissionController.Instance.TaskCompleted("Score" + GetMinigameKey(), nScore);

		// record highest score
		HighScoreManager.Instance.UpdateMinigameHighScore(GetMinigameKey(), GetScore());
		
		// update the game state
		SetGameState(MinigameStates.GameOver);
		
		// show the game over UI
		// make sure this is above _GameOver() because children  may do stuff that messes with the stats of the game
		ui.TogglePopup(MinigamePopups.GameOver, true);		
		
		// call children function
		_GameOver();
	}
	
	protected virtual void _GameOver(){
		// children implement this
	}	
	
	//---------------------------------------------------
	// PauseGame()
	// The game is being paused.  There are two functions
	// because Unity is dumb, and because we use SendMessage.
	//---------------------------------------------------	
	protected void PauseGame(){
//		Debug.Log("Pausedd");
		PauseGameWithPopup(true);	
	}

	protected void PauseGameWithPopup(bool bShowPopup){
		// if the game isn't playing, pause shouldn't do anything
		if(GetGameState() != MinigameStates.Playing)
			return;
		
		// update the game state
		SetGameState(MinigameStates.Paused);
		
		// show the pause game UI if appropriate
		if(bShowPopup) 
			ui.TogglePopup(MinigamePopups.Pause, true);
	}
	
	//---------------------------------------------------
	// ResumeGame()
	// The game is being unpaused.
	//---------------------------------------------------	
	protected void ResumeGame(){
		// update the game state, next frame due to clicking resume getting registered in game
		StartCoroutine(ResumeChangeStateNextFrame());
		
		// hide the game paused UI
		ui.TogglePopup(MinigamePopups.Pause, false);
	}

	IEnumerator ResumeChangeStateNextFrame(){
		yield return 0;
		SetGameState(MinigameStates.Playing);
	}
	
	//---------------------------------------------------
	// HowToPlay()
	// The user has manually turned the tutorial back on.
	//---------------------------------------------------		
	private void HowToPlay(){
		SetTutorialOverride(true);
		StartGame();
	}
	
	//---------------------------------------------------
	// GetStandardReward()
	// Returns the standard reward for a minigame, which
	// is the player's score divided by some constant.
	//---------------------------------------------------	
	protected int GetStandardReward(MinigameRewardTypes rewardType){
		int rewardAmount = 0;
		string modifierKey = null;
		string minigameKey = GetMinigameKey();
		
		switch(rewardType){
		case MinigameRewardTypes.Money:
			modifierKey = minigameKey + "_StandardMoney";
			break;
		case MinigameRewardTypes.Shard:
			modifierKey = minigameKey + "_StandardShard";
			break;
		case MinigameRewardTypes.XP:
			modifierKey = minigameKey + "_StandardXP";
			break;
		default:
			Debug.LogError("Unhandled minigame reward type: " + rewardType);
			break;
		}
		
		int score = GetScore();
		
		if(rewardType == MinigameRewardTypes.XP){
			// whoops, things changed, so I'm implementing this as a quick hack for now...
			// to get xp we now use another system
			Hashtable hashBonus = new Hashtable();
			hashBonus["Score"] = score.ToString();
			rewardAmount = DataLoaderXpRewards.GetXP(minigameKey, hashBonus);
		}
		else if(!string.IsNullOrEmpty(modifierKey)){
			// the standard reward is the player's score divided by some constant
			int modifier = Constants.GetConstant<int>(modifierKey);			
			rewardAmount = score / modifier;
		}
		
//		Debug.Log("Reward for " + rewardType + " is " + rewardAmount);
		
		return rewardAmount;
	}
}
