using UnityEngine;
using System.Collections;

/// <summary>
/// Cutscene controller.
/// Interface for different cutscenes, override all these fucntions!
/// </summary>
public abstract class CutsceneController : MonoBehaviour {

	public abstract void Play();

	public void Finish(){
		CutsceneUIManager.Instance.FinishAnimation();
		Invoke("DestroySelf", 1f);
	}

	private void DestroySelf(){
		Destroy(gameObject);
	}
}
