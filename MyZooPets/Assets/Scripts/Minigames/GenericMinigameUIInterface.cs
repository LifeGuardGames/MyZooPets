using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Funnel for generic calls from UI, singleton does now really work well with inheritance so
/// we are pouring all generic calls here so we can keep things nice and clean
/// 
/// Managed by GenericMinigameUI
/// </summary>
public class GenericMinigameUIInterface : MonoBehaviour{
	private string sceneName;

	void Awake(){
		sceneName = SceneManager.GetActiveScene().name;
	}

	public string GetMinigameKey(){
		if(sceneName == SceneUtils.DOCTORMATCH){
			return DoctorMatchGameManager.Instance.MinigameKey;
		}
		else if(sceneName == SceneUtils.MEMORY){
			return MemoryGameManager.Instance.MinigameKey;
		}
		else if(sceneName == SceneUtils.RUNNER){
			return RunnerGameManager.Instance.MinigameKey;
		}
		else if(sceneName == SceneUtils.SHOOTER){
			return ShooterGameManager.Instance.MinigameKey;
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA){
			return NinjaGameManager.Instance.MinigameKey;
		}
		else if(sceneName == SceneUtils.MICROMIX){
			return MicroMixManager.Instance.MinigameKey;
		}
		else{
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
			return null;
		}
	}

	public void OnPause(){
		if(sceneName == SceneUtils.DOCTORMATCH){
			DoctorMatchGameManager.Instance.PauseGame();
		}
		else if(sceneName == SceneUtils.MEMORY){
			MemoryGameManager.Instance.PauseGame();
		}
		else if(sceneName == SceneUtils.RUNNER){
			RunnerGameManager.Instance.PauseGame();
		}
		else if(sceneName == SceneUtils.SHOOTER){
			ShooterGameManager.Instance.PauseGame();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA){
			NinjaGameManager.Instance.PauseGame();
		}
		else if(sceneName == SceneUtils.MICROMIX){
			MicroMixManager.Instance.PauseGame();
		}
		else{
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnResume(){
		if(sceneName == SceneUtils.DOCTORMATCH){
			DoctorMatchGameManager.Instance.ResumeGame();
		}
		else if(sceneName == SceneUtils.MEMORY){
			MemoryGameManager.Instance.ResumeGame();
		}
		else if(sceneName == SceneUtils.RUNNER){
			RunnerGameManager.Instance.ResumeGame();
		}
		else if(sceneName == SceneUtils.SHOOTER){
			ShooterGameManager.Instance.ResumeGame();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA){
			NinjaGameManager.Instance.ResumeGame();
		}
		else if(sceneName == SceneUtils.MICROMIX){
			MicroMixManager.Instance.ResumeGame();
		}
		else{
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnTutorial(){
		if(sceneName == SceneUtils.DOCTORMATCH){
			DoctorMatchGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.MEMORY){
			MemoryGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.RUNNER){
			RunnerGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.SHOOTER){
			ShooterGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA){
			NinjaGameManager.Instance.StartTutorial();
		}
		else if(sceneName == SceneUtils.MICROMIX){
			MicroMixManager.Instance.StartTutorial();
		}
		else{
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnContinue(){
		if(sceneName == SceneUtils.DOCTORMATCH){
			DoctorMatchGameManager.Instance.ContinueGame();
		}
		else if(sceneName == SceneUtils.MEMORY){
			// Continues not allowed for this minigame
		}
		else if(sceneName == SceneUtils.RUNNER){
			RunnerGameManager.Instance.ContinueGame();
		}
		else if(sceneName == SceneUtils.SHOOTER){
			ShooterGameManager.Instance.ContinueGame();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA){
			//NinjaManager.Instance.ContinueGame();
		} 
		else if(sceneName == SceneUtils.MICROMIX){
			MicroMixManager.Instance.ContinueGame();
		}
		else{
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnRestart(){
		if(sceneName == SceneUtils.DOCTORMATCH){
			DoctorMatchGameManager.Instance.NewGame();
		}
		else if(sceneName == SceneUtils.MEMORY){
			MemoryGameManager.Instance.NewGame();
		}
		else if(sceneName == SceneUtils.RUNNER){
			RunnerGameManager.Instance.NewGame();
		}
		else if(sceneName == SceneUtils.SHOOTER){
			ShooterGameManager.Instance.NewGame();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA){
			NinjaGameManager.Instance.NewGame();
		}
		else if(sceneName == SceneUtils.MICROMIX){
			MicroMixManager.Instance.Restart();
		}
		else{ //TODO: Add SceneUtils.MEMORY to OnResume, OnRestart, and QuitGame
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void QuitGame(){
		if(sceneName == SceneUtils.DOCTORMATCH){
			DoctorMatchGameManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.MEMORY){
			MemoryGameManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.RUNNER){
			RunnerGameManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.SHOOTER){
			ShooterGameManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.TRIGGERNINJA){
			NinjaGameManager.Instance.QuitGame();
		}
		else if(sceneName == SceneUtils.MICROMIX){
			MicroMixManager.Instance.QuitGame();
		}
		else{
			Debug.LogError("Invalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}
}
