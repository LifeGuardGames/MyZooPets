using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashSceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine("WaitAFew");
	}

	IEnumerator WaitAFew() {
		yield return new WaitForSeconds(5);
		ContinueLoading(true);
	}

	public void ContinueLoading(bool doTransition) {
		if(DataManager.Instance.GameData.PetInfo.IsHatched) {
			if(doTransition) {
				LoadLevelManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
			}
			else {
				PromotionUIManager.Instance.TryShow();
				//SceneManager.LoadScene(SceneUtils.BEDROOM);
			}
		}
		else {
			if(doTransition) {
				LoadLevelManager.Instance.StartLoadTransition(SceneUtils.MENU);
			}
			else {
				SceneManager.LoadScene(SceneUtils.MENU);
			}
		}
	}
}
