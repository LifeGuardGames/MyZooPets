using UnityEngine;
using System.Collections;

public class HUDUIManager : Singleton<HUDUIManager> {

	public GameObject HUDPanel;

    private HUDAnimator hudAnimator;
    private float points;
    private float mood;
    private float health;
    private string level;
    private string levelText;
    private int nextLevelPoints;
    private string starCount;

    public bool isDebug;
	public UISlider healthSlider;
	public UILabel healthLabel;
	
	public UILabel levelNumber;
	public UILabel levelFraction;
	public UISlider levelSlider;

	public UISlider moodSlider;
	public UILabel moodLabel;

	public UILabel starLabel;
    
	// Use this for initialization
	void Start () {
	  hudAnimator = GetComponent<HUDAnimator>();
	}
	
	// Update is called once per frame
	void Update () {

        //Data reading from Data Manager
        points = hudAnimator.DisplayPoints;
        mood = hudAnimator.DisplayMood;
        health = hudAnimator.DisplayHealth;

        //points progress bar data
        level = ((int)hudAnimator.LastLevel).ToString();
        nextLevelPoints = hudAnimator.NextLevelPoints;
        levelText = hudAnimator.DisplayPoints + "/" + nextLevelPoints;

        //Star data
        starCount = hudAnimator.DisplayStars.ToString();

        levelSlider.sliderValue = points/nextLevelPoints;
        levelNumber.text = level;
        levelFraction.text = levelText;
        moodSlider.sliderValue = mood/100;
        moodLabel.text = mood.ToString() + "%";
        healthSlider.sliderValue = health/100;
        healthLabel.text = health.ToString() + "%";
        starLabel.text = starCount;
	}

	public void ShowPanel(){
		HUDPanel.GetComponent<TweenToggleDemux>().Show();
	}

	public void HidePanel(){
		HUDPanel.GetComponent<TweenToggleDemux>().Hide();
	}
}
