using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLevelManager : Singleton<LoadLevelManager> {
	private static bool isCreated;

	public TweenToggleDemux loadDemux;
	public Text loadText;
	public Image loadImage;
	public float textWait = 1.3f;        // How long to wait if there is text
	private bool isShowingImageTip = false;
	private string sceneToLoad;

	public string GetCurrentSceneName() {
		return SceneManager.GetActiveScene().name;
	}

	void Awake() {
		// Make object persistent
		if(isCreated) {
			// If There is a duplicate in the scene. delete the object and jump Awake
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		isCreated = true;
	}

	/// <summary>
	/// Call this to start the transition
	/// </summary>
	/// <param name="sceneName">Scene to be loaded</param>
	public void StartLoadTransition(string sceneName, string additionalTextKey = null, string additionalImageKey = null) {
		isShowingImageTip = false;

		// Reset everything first
		//loadText.text = "";
		//loadImage.gameObject.SetActive(false);

		if(additionalTextKey != null) {
			//loadText.text = LocalizationText.GetText(additionalTextKey);
		}
		if(additionalImageKey != null) {
			//loadImage.gameObject.SetActive(true);
			//loadImage.sprite = SpriteCacheManager.GetLoadingImageData(additionalImageKey);
			isShowingImageTip = true;
		}

		sceneToLoad = sceneName;

		AudioManager.Instance.PlayClip("loadingSlideClose");
		loadDemux.Show();
	}

	/// <summary>
	/// Call this to initiate loading (when ending tween is done), there will be a performance hit here
	/// </summary>
	public void ShowFinishedCallback() {
		if(sceneToLoad != null) {
			if(isShowingImageTip) {
				Invoke("LoadLevel", textWait);
			}
			else {
				LoadLevel();
			}
		}
		else {
			Debug.LogError("No level name specified");
		}
	}

	private void LoadLevel() {
		SceneManager.LoadScene(sceneToLoad);
	}

	/// <summary>
	/// Hide the demux when the new level is loaded
	void OnLevelWasLoaded() {
		AudioManager.Instance.PlayClip("loadingSlideOpen");
		loadDemux.Hide();
	}
}
