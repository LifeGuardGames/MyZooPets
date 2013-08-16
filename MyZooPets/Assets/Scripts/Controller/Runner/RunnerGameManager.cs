using UnityEngine;
using System.Collections;

public class RunnerGameManager : MonoBehaviour {
    private PlayerRunner mPlayerRunner;
    private LevelManager mLevelManager;
    private ParallaxingBackgroundManager mParallaxingBackgroundManager;
    private ScoreManager mScoreManager;
    private TouchDetectorManager mTouchDetectorManager;

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
	}
	
	// Update is called once per frame
	void Update() {
	}

    public void ActivateGameOver() {
        GameRunning = false;

        // Disable the player
        if (mPlayerRunner != null)
            mPlayerRunner.gameObject.SetActive(false);
    }

    void ResetGame() {
        GameRunning = true;

        // Turn player on, if it isnt
        if (mPlayerRunner != null)
            mPlayerRunner.gameObject.SetActive(true);
    }
}
