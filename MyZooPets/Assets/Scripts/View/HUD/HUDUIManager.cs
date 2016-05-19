using UnityEngine;
using UnityEngine.UI;

public class HUDUIManager : Singleton<HUDUIManager>{
	public TweenToggleDemux panelTween;
	public HUDAnimator hudAnimator;

	public UISlider healthSlider;
	public UILabel healthLabel;
	public UILabel levelNumber;
	public UILabel levelFraction;
	public UISlider levelSlider;
	public UISlider moodSlider;
	public UILabel moodLabel;
	public UILabel starLabel;

	private float points;
	private float mood;
	private float health;
	private string level;
	private string levelText;
	private int nextLevelPoints;
	private string starCount;

	void Start() {
		ToggleLabels(false);
	}

	public void ShowPanel() {
		panelTween.Show();
	}

	public void HidePanel() {
		panelTween.Hide();
	}
	
	void Update(){
		//Data reading from Data Manager
		points = hudAnimator.GetDisplayValue(HUDElementType.Xp);
		mood = hudAnimator.GetDisplayValue(HUDElementType.Hunger);
		health = hudAnimator.GetDisplayValue(HUDElementType.Health);

		//points progress bar data
		level = ((int)hudAnimator.LastLevel).ToString();
		nextLevelPoints = hudAnimator.NextLevelPoints;
		
		if(LevelLogic.Instance && LevelLogic.Instance.IsAtMaxLevel())
			levelText = Localization.Localize("MAX_LEVEL");
		else
			levelText = points + "/" + nextLevelPoints;

		//Star data
		starCount = hudAnimator.GetDisplayValue(HUDElementType.Coin).ToString();

		levelSlider.sliderValue = points / nextLevelPoints;
		levelNumber.text = level;
		levelFraction.text = levelText;
		moodSlider.sliderValue = mood / 100;
		moodLabel.text = mood.ToString() + "%";
		healthSlider.sliderValue = health / 100;
		healthLabel.text = health.ToString() + "%";
		starLabel.text = starCount;
	}
	
	public void ToggleLabels(bool isShow){
		levelFraction.gameObject.SetActive(isShow);
		healthLabel.gameObject.SetActive(isShow);
		moodLabel.gameObject.SetActive(isShow);
	}

	public void PlayNeedCoinAnimation(){
		hudAnimator.PlayNeedCoinAnimation();
    }

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "test")){
//			PlayNeedMoneyAnimation();
//		}
//	}
}
