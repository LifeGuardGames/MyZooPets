/* Sean Duane
 * RunnerGameManager.cs
 * 8:26:2013   14:36
 * Description:
 * The sort of 'center' of the game.
 * However, all it really does is track the games running, handles the timescale, and acts as a cheap way to grab popular variables.
 * This singleton design, and caching out of certain game component, makes anything using this "glued" to the runner game.
 * You must 'unglue' everything you want to use elsewhere, sorry!
 * The upside is it's a bit faster.
 * 
 * Handles Resetting, Game Ending, and TimeScale.
 */

using UnityEngine;
using System.Collections;

public class RunnerGameManager : MonoBehaviour {
    private PlayerRunner mPlayerRunner;
    private LevelManager mLevelManager;
    private ParallaxingBackgroundManager mParallaxingBackgroundManager;
    private ScoreManager mScoreManager;
    private TouchDetectorManager mTouchDetectorManager;
    private MegaHazard mMegaHazard;
    private RunnerUIManager mRunnerUIManager;

    public bool GameRunning
    {
        get;
        protected set;
    }
    public PlayerRunner PlayerRunner { get { return mPlayerRunner; } }
    public LevelManager LevelManager { get { return mLevelManager; } }
    public ParallaxingBackgroundManager ParallaxingBackgroundManager { get { return mParallaxingBackgroundManager; } }
    public ScoreManager ScoreManager { get { return mScoreManager; } }
    public TouchDetectorManager TouchDetectorManager { get { return mTouchDetectorManager; } }
    public MegaHazard MegaHazard { get { return mMegaHazard; } }
    public RunnerUIManager RunnerUIManager { get { return mRunnerUIManager; } }
    
    private static RunnerGameManager sRunnerGameManagerInstance = null;
    static public RunnerGameManager GetInstance()
    {
        return sRunnerGameManagerInstance;
    }

	// Use this for initialization
	void Start() {
        if (sRunnerGameManagerInstance != null)
            Debug.LogError("There cannot be two RunnerGameManagers, it's supposed to be a SINGLEton! Please remove one.");
        sRunnerGameManagerInstance = this;

        GameObject foundObject;

        foundObject  = GameObject.Find("Player");
        if (foundObject != null)
            mPlayerRunner = foundObject.GetComponent<PlayerRunner>();
        else
            Debug.LogError("Could not find an object named 'Player'");

        foundObject = GameObject.Find("LevelManager");
        if (foundObject != null)
            mLevelManager = foundObject.GetComponent<LevelManager>();
        else
            Debug.LogError("Could not find an object named 'LevelManager'");

        foundObject = GameObject.Find("ParallaxingBGManager");
        if (foundObject != null)
            mParallaxingBackgroundManager = foundObject.GetComponent<ParallaxingBackgroundManager>();
        else
            Debug.LogError("Could not find an object named 'ParallaxingBGManager'");

        foundObject = GameObject.Find("ScoreManager");
        if (foundObject != null)
            mScoreManager = foundObject.GetComponent<ScoreManager>();
        else
            Debug.LogError("Could not find an object named 'ScoreManager'");

        foundObject = GameObject.Find("TouchDetectorManager");
        if (foundObject != null)
            mTouchDetectorManager = foundObject.GetComponent<TouchDetectorManager>();
        else
            Debug.LogError("Could not find an object named 'TouchDetectorManager'");

        foundObject = GameObject.Find("MegaHazard");
        if (foundObject != null)
            mMegaHazard = foundObject.GetComponent<MegaHazard>();
        else
            Debug.LogError("Could not find an object named 'MegaHazard'");

        foundObject = GameObject.Find("RunnerUIManager");
        if (foundObject != null)
            mRunnerUIManager = foundObject.GetComponent<RunnerUIManager>();
        else
            Debug.LogError("Could not find an object named 'RunnerUIManager'");
	}
	
	// Update is called once per frame
	void Update() {
	}

    public void ResetGame() {
        Time.timeScale = 1f;

        mRunnerUIManager.DeActivateGameOverPanel();

        GameRunning = true;

        mPlayerRunner.gameObject.SetActive(true);
        mPlayerRunner.Reset();

        mScoreManager.Reset();
        mLevelManager.Reset();
        mMegaHazard.Reset();
        mParallaxingBackgroundManager.Reset();
    }

    public void ActivateGameOver() {
        GameRunning = false;

        mRunnerUIManager.ActivateGameOverPanel();

        // Disable the player
        if (mPlayerRunner != null)
            mPlayerRunner.gameObject.SetActive(false);
    }

    public void IncreaseTimeSpeed(float inIncreaseTime) {
        Time.timeScale += inIncreaseTime;
    }

    public void SlowTimeSpeed(float inTimeDivisor) {
        Time.timeScale /= inTimeDivisor;
        if (Time.timeScale < 1f)
            Time.timeScale = 1f;
    }
}
