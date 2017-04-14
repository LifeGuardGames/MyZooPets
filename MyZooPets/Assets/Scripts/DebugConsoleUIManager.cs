using UnityEngine;
using UnityEngine.UI;

public class DebugConsoleUIManager : SingletonUI<DebugConsoleUIManager> {
	public TweenToggle tween;
	public InputField input;

	protected override void Awake() {
		base.Awake();
		eModeType = UIModeTypes.Debug;
	}

	public void OnTextUpdate(string text) {
		input.text = text;
	}

	public void OnEndEdit(string text) {
		Debug.Log(text);
		if(text == "FIR3UNLOCK") {
			InventoryManager.Instance.AddItemToInventory("Usable1", count:20);
			InventoryUIManager.Instance.PulseInventory();
			InventoryUIManager.Instance.RefreshPage();
		}
		CloseUI();
	}

	protected override void _OpenUI() {
		tween.Show();
	}

	protected override void _CloseUI() {
		tween.Hide();
	}
}
