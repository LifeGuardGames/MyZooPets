using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RareDecoMachine : MonoBehaviour {

	public int level;
	private int levelCost = 2000;
	public PositionTweenToggle openingScreen;
	public PositionTweenToggle rewardScreen;
	public PositionTweenToggle levelUpScreen;
	public Text levelText;
	public Text oldLevelText;
	public Text newLevelText;
	public Image decoImage;

	void Start() {
		level = DataManager.Instance.GameData.Decorations.capsuleMachineLevel;
		levelText.text = level.ToString();

    }

	public void OnUse() {
		openingScreen.Show();
	}

	public void OnBuyButton() {
		openingScreen.Hide();
		ImmutableDataRareDeco capsule = DataLoaderRareDeco.GetDecoAtTier(level);
		decoImage.sprite = SpriteCacheManager.GetItemSprite(capsule.ItemId);
		rewardScreen.Show();
		InventoryManager.Instance.AddItemToInventory(capsule.ItemId);
	}

	public void OnLevelUpButton() {
		openingScreen.Hide();
		levelUpScreen.Show();
	}

	public void Upgrade() {
		if(DataManager.Instance.GameData.Stats.Stars < levelCost) {
			StatsManager.Instance.ChangeStats(coinsDelta: levelCost);
			level++;
			levelText.text = level.ToString();
			DataManager.Instance.GameData.Decorations.capsuleMachineLevel++;
			levelUpScreen.Hide();
			openingScreen.Show();
		}
	}

	public void OnExitButtonLevel() {
		levelUpScreen.Hide();
		openingScreen.Hide();
	}

	public void OnAcceptButtonReward() {
		rewardScreen.Hide();
	}
}
