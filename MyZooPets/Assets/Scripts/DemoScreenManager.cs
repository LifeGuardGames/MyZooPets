using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DemoScreenManager : MonoBehaviour {

	public void OnRunnerButton() {
		SceneManager.LoadScene("Runner");
	}

	public void OnShooterButton() {
		SceneManager.LoadScene("ShooterGame");
	}
}
