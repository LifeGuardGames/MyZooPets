public class ButtonMicroMixEntrance : ButtonChangeScene {
	void Start () {
		// Hide the object if you have not beat the game yet
		if(!DataManager.Instance.GameData.MicroMix.hasWon) {
			gameObject.SetActive(false);
		}
	}
}
