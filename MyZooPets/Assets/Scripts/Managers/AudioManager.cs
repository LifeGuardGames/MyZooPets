using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : LgAudioManager<AudioManager> {
	public bool isMusicOn = true;
	public string backgroundMusic;

	private AudioSource backgroundSource;
	public string lastLoadedScene;

	protected override void Awake() {
		base.Awake();
		backgroundSource = GetComponent<AudioSource>();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	protected override void Start() {
		// Dont play anything if its in loading scene
		if(SceneUtils.CurrentScene == SceneUtils.LOADING) {
			return;
		}
		base.Start();
	}

	private IEnumerator PlayBackground() {
		yield return new WaitForSeconds(0.5f);
		if(isMusicOn) {
			AudioClip backgroundClip = null;
			if(backgroundMusic != null) {
				backgroundClip = Resources.Load(backgroundMusic) as AudioClip;
			}

			if(backgroundClip != null) {
				backgroundSource.clip = backgroundClip;
				backgroundSource.Play();
			}
		}
	}

	public void LowerBackgroundVolume(float newVolume) {
		backgroundSource.volume = newVolume;
	}

	// Pass in null if don't want new music
	public void FadeOutPlayNewBackground(string newAudioClipName, bool isLoop = true) {
		StartCoroutine(FadeOutPlayNewBackgroundHelper(newAudioClipName, isLoop));
	}

	private IEnumerator FadeOutPlayNewBackgroundHelper(string newAudioClipName, bool isLoop) {
		for(int i = 9; i >= 0; i--) {
			backgroundSource.volume = i * .1f;
			yield return new WaitForSeconds(.01f);
		}
		if(newAudioClipName != null) {
			ImmediatelyPlayNewBackground(newAudioClipName, isLoop);
		}
		else {
			backgroundSource.Stop();
		}
	}

	private void ImmediatelyPlayNewBackground(string newAudioClipName, bool isLoop) {
		backgroundSource.Stop();
		backgroundSource.volume = 0.6f;
		backgroundMusic = newAudioClipName;
		backgroundSource.loop = isLoop;

		StartCoroutine(PlayBackground());
	}

	/// <summary>
	/// Pause all audio sources.
	/// </summary>
	/// <param name="isPaused">If set to <c>true</c> is paused.</param>
	public void PauseBackground(bool isPaused) {
		if(isPaused) {
			backgroundSource.Pause();
		}
		else {
			backgroundSource.Play();
		}
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		string currentScene = SceneUtils.CurrentScene;
		if(currentScene == SceneUtils.LOADING) {
			isMusicOn = true;
		}
		if(currentScene == SceneUtils.INHALERGAME) {
			backgroundMusic = "bgInhaler";
			FadeOutPlayNewBackground(backgroundMusic);
		}
		else if(currentScene == SceneUtils.MENU) {
			backgroundMusic = "bgMenuScene";
			FadeOutPlayNewBackground(backgroundMusic);
		}
		else if(currentScene == SceneUtils.BEDROOM) {
			backgroundMusic = "bgBedroom";
			if(lastLoadedScene == SceneUtils.MENU) {		// Edge case, dont allow comic music to loop
				ImmediatelyPlayNewBackground(backgroundMusic, true);
			}
			else {
				FadeOutPlayNewBackground(backgroundMusic);
			}
		}
		else if(currentScene == SceneUtils.YARD) {
			backgroundMusic = "bgYard";
			FadeOutPlayNewBackground(backgroundMusic);
		}
		else if(currentScene == SceneUtils.TRIGGERNINJA) {
			backgroundMusic = "bgTriggerNinja";
			FadeOutPlayNewBackground(backgroundMusic);
		}
		else if(currentScene == SceneUtils.DOCTORMATCH) {
			backgroundMusic = "bgClinic";
			FadeOutPlayNewBackground(backgroundMusic);
		}
		else if(currentScene == SceneUtils.MEMORY) {
			backgroundMusic = "bgMemory";
			FadeOutPlayNewBackground(backgroundMusic);
		}
		else if(currentScene == SceneUtils.SHOOTER) {
			backgroundMusic = "bgShooter";
			FadeOutPlayNewBackground(backgroundMusic);
		}
		else if(currentScene == SceneUtils.RUNNER) {
			backgroundMusic = "bgRunner";
			FadeOutPlayNewBackground(backgroundMusic);
		}
		else if(currentScene == SceneUtils.MICROMIX) {
			backgroundMusic = "bgRunner";
			FadeOutPlayNewBackground(backgroundMusic);
		}
		lastLoadedScene = currentScene;
	}
}
