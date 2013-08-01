using UnityEngine;
using System.Collections;

public class HUDManager : MonoBehaviour {
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
    public UISlider moodSlider;
    public UISlider levelSlider;
    public UILabel starLabel;
    public UILabel levelLabel;
    public UILabel levelTextLabel;
    public UILabel healthLabel;
    public UILabel moodLabel;
    
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
        level = "Lv " + (int)hudAnimator.LastLevel;
        nextLevelPoints = hudAnimator.NextLevelPoints;
        levelText = hudAnimator.DisplayPoints + "/" + nextLevelPoints;

        //Star data
        starCount = hudAnimator.DisplayStars.ToString();

        levelSlider.sliderValue = points/nextLevelPoints;
        levelLabel.text = level;
        levelTextLabel.text = levelText;
        moodSlider.sliderValue = mood/100;
        moodLabel.text = mood.ToString();
        healthSlider.sliderValue = health/100;
        healthLabel.text = health.ToString();
        starLabel.text = starCount;
	}
}
