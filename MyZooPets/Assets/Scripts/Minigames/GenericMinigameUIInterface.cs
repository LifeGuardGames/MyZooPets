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
			return MemoryGameManager.Instance.MinigameKey;
		}
		else if(sceneName == SceneUtils.RUNNER) {
			return RunnerGameManager.Instance.MinigameKey;
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			return ShooterGameManager.Instance.GetMinigameKey();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			return NinjaManager.Instance.MinigameKey;
		}
		else {
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
			return null;
		}
	}

	public void PauseToggle(bool isShow) {
		if(sceneName == SceneUtils.DOCTORMATCH) {
			DoctorMatchManager.Instance.PauseGame(isShow);
		}
		else if(sceneName == SceneUtils.MEMORY) {
			Debug.LogWarning("PauseGame not set up for Memory");
		}
		else if(sceneName == SceneUtils.RUNNER) {
			RunnerGameManager.Instance.PauseGame(isShow);
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			ShooterGameManager.Instance.PauseGame(isShow);
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			NinjaManager.Instance.PauseGame(isShow);
		}
		else {
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnTutorial() { //These should/could be Coroutines
		if(sceneName == SceneUtils.DOCTORMATCH) {
			//TODO: Determine if coroutine is necessary for DoctorMatch.
			//The one thing holding it back is the fact that Initialize under
			//AssemblyLineController is a coroutine, but it may not actually need to be.
			//That would simply this considerably.
			StartCoroutine(DoctorMatchManager.Instance.StartTutorial());    //This one needs to be a coroutine
		}
		else if(sceneName == SceneUtils.MEMORY) {
			//MemoryGameManager.Instance.StartTutorial();
		} else if (sceneName == SceneUtils.RUNNER) {
			StartCoroutine(RunnerGameManager.Instance.StartTutorial());
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
		}
		else if(sceneName == SceneUtils.MEMORY) {
		}
		else if(sceneName == SceneUtils.RUNNER) {
		}
		else if(sceneName == SceneUtils.SHOOTER) {
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
		}
		else {
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnContinue() {
		if(sceneName == SceneUtils.DOCTORMATCH) {
			DoctorMatchManager.Instance.ContinueGame();
		}
		else if(sceneName == SceneUtils.MEMORY) {
			// Continues not allowed for this minigame
		}
		else if(sceneName == SceneUtils.RUNNER) {
			RunnerGameManager.Instance.ContinueGame();
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			ShooterGameManager.Instance.ContinueGame();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			//NinjaManager.Instance.ContinueGame();
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
		}
		else if(sceneName == SceneUtils.RUNNER) {
			RunnerGameManager.Instance.NewGame();
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			ShooterGameManager.Instance.NewGame();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			NinjaManager.Instance.NewGame();
		}
		else { //TODO: Add SceneUtils.MEMORY to OnResume, OnRestart, and QuitGame
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
			RunnerGameManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.SHOOTER) {
			ShooterGameManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA) {
			NinjaManager.Instance.QuitGame();
		}
		else {
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}
}
