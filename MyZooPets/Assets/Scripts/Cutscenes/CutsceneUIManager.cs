using UnityEngine;
using System.Collections;

/// <summary>
/// Cutscene user interface manager.
/// Need CutsceneManager? probably not yet...
/// </summary>
public class CutsceneUIManager : SingletonUI<CutsceneUIManager> {

	public FadeTweener background;

	public float fadeInDuration = 0.5f;				// Time for screen to black out for cutscene
	public float pauseBeforePlayDuration = 0.3f;	// Time to wait after black out to playing animations
	public float pauseAfterFinishDuration = 0.3f;	// Time to wait after all the clips has finished before fading
	public float fadeOutDuration = 0.5f;			// Time for screen to turn back to normal

	private string currentCutscene = null;

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.Cutscene;
		background.gameObject.SetActive(false);
	}

	/// <summary>
	/// Plays the cutscene.
	/// NOTE: Use this instead of OpenUI()
	/// </summary>
	/// <param name="cutscenePrefabName">Cutscene prefab name.</param>
	public void PlayCutscene(string cutscenePrefabName){
		if(currentCutscene == null){
			currentCutscene = cutscenePrefabName;
			background.gameObject.SetActive(true);
			OpenUI();	// Fade in the black background
			Invoke("StartAnimation", fadeInDuration + pauseBeforePlayDuration);
		}
		else{
			Debug.LogError("Cutscene already playing");
		}
	}

	private void StartAnimation(){
		GameObject loadedObject = Resources.Load(currentCutscene) as GameObject;
		GameObject go = Instantiate(loadedObject) as GameObject;
		go.transform.parent = this.transform;
		GameObjectUtils.ResetLocalTransform(go);
		go.GetComponent<CutsceneController>().Play();
	}

	/// <summary>
	/// Finish cutscene callback
	/// This should be called from CutsceneControllers to notify manager to close
	/// </summary>
	public void FinishAnimation(){
		Invoke("CloseUI", pauseAfterFinishDuration);
	}

	protected override void _OpenUI(){
		background.BlackOutScreen();
	}

	protected override void _CloseUI(){
		background.LightsUpScreen();
		Invoke("DisableBackground", fadeOutDuration + 0.5f);
	}

	private void DisableBackground(){
		currentCutscene = null; // HACK Not sure if best place to do this or callback
		background.gameObject.SetActive(false);
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "open")){
//			PlayCutscene("CutsceneMinipetHatchFireBird");
//		}
//		if(GUI.Button(new Rect(200, 100, 100, 100), "close")){
//			CloseUI();
//		}
//	}
}
