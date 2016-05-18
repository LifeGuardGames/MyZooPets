using UnityEngine;

public class HUDUIManager : Singleton<HUDUIManager>{
	//public bool isDebug;
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

	// Parent for tweening
	public GameObject tweenParent;
	public GameObject anchorTop;

	// Icon pulsing
	public AnimationControl animHealth;
	public AnimationControl animMood;
	public AnimationControl animMoney;
	public AnimationControl animXP;

	public Animation needMoneyAnimation;

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

	/// <summary>
	/// Gets the tween parent.
	/// </summary>
	/// <returns>The tween parent.</returns>
	//public GameObject GetTweenParent(){
	//	return tweenParent;	
	//}

	//public GameObject GetTweenParent(string anchor){
	//	if(anchor == "Top"){
	//		return anchorTop;
	//	}
	//	else{
	//		Debug.LogError("Bad anchor specified for HUD tween");
	//		return null;
	//	}
	//}
	
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

	public void PlayNeedMoneyAnimation(){
		needMoneyAnimation.wrapMode = WrapMode.Once;
		needMoneyAnimation.Play("moneyRequired");
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "test")){
//			PlayNeedMoneyAnimation();
//		}
//	}
}
