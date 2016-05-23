using UnityEngine;
using UnityEngine.UI;

public class HUDUIManager : Singleton<HUDUIManager>{
	public TweenToggleDemux panelTween;

	public HUDAnimator hudAnimator;
	public HUDAnimator HudAnimator {
		get { return hudAnimator; }
	}

	public Text levelNumber;
	public Text levelFraction;
	public Image levelBar;
	public Text healthLabel;
	public Image healthBar;
	public Image hungerBar;
	public Text hungerLabel;
	public Text coinLabel;

	private float levelBarWidth;
	private float healthBarWidth;
	private float hungerBarWidth;
	
	private string levelText;
	private int nextLevelPoints;
	private string starCount;

	void Start() {
		ToggleLabels(false);

		// Save initial data so image bar can be populated properly
		levelBarWidth = levelBar.rectTransform.sizeDelta.x;
		healthBarWidth = healthBar.rectTransform.sizeDelta.x;
		hungerBarWidth = hungerBar.rectTransform.sizeDelta.x;
    }

	public void ShowPanel() {
		panelTween.Show();
	}

	public void HidePanel() {
		panelTween.Hide();
	}
	
	void Update(){
		// Data reading from HUDAnimator
		int xp = hudAnimator.GetDisplayValue(StatType.Xp);
		int hunger = hudAnimator.GetDisplayValue(StatType.Hunger);
		int health = hudAnimator.GetDisplayValue(StatType.Health);

		// Points progress bar data
		nextLevelPoints = hudAnimator.NextLevelPoints;
		
		if(LevelLogic.Instance && LevelLogic.Instance.IsAtMaxLevel()) {
			levelFraction.text = Localization.Localize("MAX_LEVEL");
		}
		else {
			levelFraction.text = xp + "/" + nextLevelPoints;
		}

		levelBar.rectTransform.sizeDelta = new Vector2((xp / (float)nextLevelPoints) * levelBarWidth, 31f);
		levelNumber.text = ((int)hudAnimator.LastLevel).ToString();
		healthBar.rectTransform.sizeDelta = new Vector2((health / 100f) * healthBarWidth, 31f);
		healthLabel.text = health.ToString() + "%";
		hungerBar.rectTransform.sizeDelta = new Vector2((hunger / 100f) * hungerBarWidth, 31f);
		hungerLabel.text = hunger.ToString() + "%";
		coinLabel.text = hudAnimator.GetDisplayValue(StatType.Coin).ToString();
	}
	
	public void ToggleLabels(bool isShow){
		levelFraction.gameObject.SetActive(isShow);
		healthLabel.gameObject.SetActive(isShow);
		hungerLabel.gameObject.SetActive(isShow);
	}

	public void PlayNeedCoinAnimation(){
		hudAnimator.PlayNeedCoinAnimation();
    }

	//void OnGUI() {
	//	if(GUI.Button(new Rect(100, 100, 100, 100), "test")) {
	//		PlayNeedMoneyAnimation();
	//	}
	//}
}
