using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Funnel for generic calls from UI, singleton does now really work well with inheritance so
/// we are pouring all generic calls here so we can keep things nice and clean
/// 
/// Managed by GenericMinigameUI
/// </summary>
public class GenericMinigameUIInterface : MonoBehaviour {
	private string sceneName;

	void Awake() {
		sceneName = SceneManager.GetActiveScene().name;
    }

	public string GetMinigameKey() {
		if(sceneName == SceneUtils.DOCTORMATCH) {
			return DoctorMatchManager.Instance.MinigameKey;
        }
		else if(sceneName == SceneUtils.MEMORY) {
			return MemoryGameManager.Instance.GetMinigameKey();	// ...
		}
		else if(sceneName == SceneUtils.RUNNER) {
			return RunnerGameManager.Instance.GetMinigameKey();
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			return ShooterGameManager.Instance.GetMinigameKey();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			return NinjaManager.Instance.GetMinigameKey();
		}
		else {
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
			return null;
		}
	}

	public void PauseToggle(bool isShow) {
		if(sceneName == SceneUtils.DOCTORMATCH) {
			DoctorMatchManager.Instance.PauseGame(isShow);
			//DoctorMatchManager.Instance.GetMinigameKey();
		}
		else if(sceneName == SceneUtils.MEMORY) {
			//MemoryGameManager.Instance.GetMinigameKey();
		}
		else if(sceneName == SceneUtils.RUNNER) {
			//RunnerGameManager.Instance.GetMinigameKey();
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			//ShooterGameManager.Instance.GetMinigameKey();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			//NinjaManager.Instance.GetMinigameKey();
		}
		else {
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnTutorial() {
		if(sceneName == SceneUtils.DOCTORMATCH) {
			DoctorMatchManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.MEMORY) {
			//MemoryGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.RUNNER) {
			//RunnerGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			//ShooterGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			//NinjaManager.Instance.StartTutorial();
		}
		else {
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnResume() {
		if(sceneName == SceneUtils.DOCTORMATCH) {
			//DoctorMatchManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.MEMORY) {
			//MemoryGameManager.Instance.GetMinigameKey();
		}
		else if(sceneName == SceneUtils.RUNNER) {
			//RunnerGameManager.Instance.GetMinigameKey();
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			//ShooterGameManager.Instance.GetMinigameKey();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			//NinjaManager.Instance.GetMinigameKey();
		}
		else {
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnRestart() {
		if(sceneName == SceneUtils.DOCTORMATCH) {
			DoctorMatchManager.Instance.NewGame();
		}
		else if(sceneName == SceneUtils.MEMORY) {
			//MemoryGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.RUNNER) {
			//RunnerGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			//ShooterGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			//NinjaManager.Instance.StartTutorial();
		}
		else {
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void QuitGame() {
		if(sceneName == SceneUtils.DOCTORMATCH) {
			DoctorMatchManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.MEMORY) {
			//MemoryGameManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.RUNNER) {
			//RunnerGameManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			//ShooterGameManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			//NinjaManager.Instance.QuitGame();
		}
		else {
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}
}
