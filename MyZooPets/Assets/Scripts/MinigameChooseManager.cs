using UnityEngine;

public class MinigameChooseManager : MonoBehaviour {

	public void OnLoadSceneButton(string sceneName) {
		LoadLevelManager.Instance.StartLoadTransition(sceneName);
	}
}
