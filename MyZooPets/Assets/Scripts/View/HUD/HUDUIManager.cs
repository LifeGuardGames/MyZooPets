using UnityEngine;
using System.Collections;

public class HUDUIManager : Singleton<HUDUIManager>{
	public HUDAnimator hudAnimator;
	public bool isDebug;
	public UISlider healthSlider;
	public UILabel healthLabel;
	public UILabel levelNumber;
	public UILabel levelFraction;
	public UISlider levelSlider;
	public UISlider moodSlider;
	public UILabel moodLabel;
	public UILabel starLabel;

	public UILabel gemLabel;

	// Parent for tweening
	public GameObject tweenParent;

	// Icon pulsing
	public AnimationControl animHealth;
	public AnimationControl animMood;
	public AnimationControl animMoney;
	public AnimationControl animXP;
	public ParticleSystemController animFire;

	public AnimationControl animGem;

	private float points;
	private float mood;
	private float health;
	private string level;
	private string levelText;
	private int nextLevelPoints;
	private string starCount;

	private string gemCount;
	
	/// <summary>
	/// Gets the tween parent.
	/// </summary>
	/// <returns>The tween parent.</returns>
	public GameObject GetTweenParent(){
		return tweenParent;	
	}
    
	// Use this for initialization
	void Awake(){
		hudAnimator = GetComponent<HUDAnimator>();
	}
	
	// Update is called once per frame
	void Update(){

		//Data reading from Data Manager
		points = hudAnimator.GetDisplayValue(HUDElementType.Points);
		mood = hudAnimator.GetDisplayValue(HUDElementType.Mood);
		health = hudAnimator.GetDisplayValue(HUDElementType.Health);

		//points progress bar data
		level = ((int)hudAnimator.LastLevel).ToString();
		nextLevelPoints = hudAnimator.NextLevelPoints;
		
		if(LevelLogic.Instance && LevelLogic.Instance.IsAtMaxLevel())
			levelText = Localization.Localize("MAX_LEVEL");
		else
			levelText = points + "/" + nextLevelPoints;

		//Star data
		starCount = hudAnimator.GetDisplayValue(HUDElementType.Stars).ToString();

		gemCount = hudAnimator.GetDisplayValue(HUDElementType.Gems).ToString();

		levelSlider.sliderValue = points / nextLevelPoints;
		levelNumber.text = level;
		levelFraction.text = levelText;
		moodSlider.sliderValue = mood / 100;
		moodLabel.text = mood.ToString() + "%";
		healthSlider.sliderValue = health / 100;
		healthLabel.text = health.ToString() + "%";
		starLabel.text = starCount;

		gemLabel.text = gemCount;
	}

	public void ShowPanel(){
		gameObject.GetComponent<TweenToggleDemux>().Show();
	}

	public void HidePanel(){
		gameObject.GetComponent<TweenToggleDemux>().Hide();
	}
}
