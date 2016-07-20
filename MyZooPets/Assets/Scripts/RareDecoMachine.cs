using UnityEngine;
using System.Collections;

public class RareDecoMachine : MonoBehaviour {

	public int level;
	public PositionTweenToggle openingScreen;
	public PositionTweenToggle outPutScreen;
	public PositionTweenToggle levelUpScreen;

	void Start() {
		level = DataManager.Instance.GameData.Decorations.capsuleMachineLevel;
    }

	public void OnUse() {
		openingScreen.Show();
	}

	public void RollOnDeco() {
		openingScreen.Hide();
		ImmutableDataRareDeco capsule = DataLoaderRareDeco.GetDecoAtTier(level);
		InventoryManager.Instance.AddItemToInventory(capsule.ID);
	}

	public void Upgrade() {
		level++;
		DataManager.Instance.GameData.Decorations.capsuleMachineLevel++;
	}
}
