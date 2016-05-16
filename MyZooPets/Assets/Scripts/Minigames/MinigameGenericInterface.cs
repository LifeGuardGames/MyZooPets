using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Funnel for generic calls from UI, singleton does now really work well with inheritance so
/// we are pouring all generic calls here so we can keep things nice and clean
/// </summary>
public class MinigameGenericInterface : MonoBehaviour {
	public void PauseToggle(bool isPause) {
		if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.DOCTORMATCH)) {
			//...
		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.MEMORY)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.RUNNER)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.SHOOTER)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.TRIGGERNINJA)) {

		}
		else {
			Debug.LogError("Unvalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnTutorial() {
		if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.DOCTORMATCH)) {
			DoctorMatchManager.Instance.StartTutorial();
		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.MEMORY)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.RUNNER)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.SHOOTER)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.TRIGGERNINJA)) {

		}
		else {
			Debug.LogError("Unvalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnResume() {
		if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.DOCTORMATCH)) {
			DoctorMatchManager.Instance.StartTutorial();
		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.MEMORY)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.RUNNER)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.SHOOTER)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.TRIGGERNINJA)) {

		}
		else {
			Debug.LogError("Unvalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void OnRestart() {
		if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.DOCTORMATCH)) {
			DoctorMatchManager.Instance.StartTutorial();
		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.MEMORY)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.RUNNER)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.SHOOTER)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.TRIGGERNINJA)) {

		}
		else {
			Debug.LogError("Unvalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}

	public void QuitGame() {
		if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.DOCTORMATCH)) {
			DoctorMatchManager.Instance.QuitGame();
		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.MEMORY)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.RUNNER)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.SHOOTER)) {

		}
		else if(string.Equals(SceneManager.GetActiveScene().name, SceneUtils.TRIGGERNINJA)) {

		}
		else {
			Debug.LogError("Unvalid scene detected" + SceneManager.GetActiveScene().name);
		}
	}
}
