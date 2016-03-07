using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class AdManager : Singleton<AdManager> {
	[SerializeField] string iOSGameID = "105899";
	[SerializeField] string androidGameID = "105908";

	public delegate void AdCallback<T>(T value); // Populated by callers

	void Awake(){
		#if UNITY_ANDROID
		Advertisement.Initialize(androidGameID, true);
		#endif
		#if UNITY_IOS
		Advertisement.Initialize(iOSGameID, true);
		#endif
	}

	// Determine preliminary check
	public bool IsAdReady(string zone = "rewardedVideoZone"){
		return Advertisement.isReady(zone);
	}

	public void ShowAd(AdCallback<bool> onVideoPlayed, string zone = "rewardedVideoZone"){
		if(DataManager.Instance.IsAdsEnabled && Advertisement.isReady(zone)){
			
			//		#if UNITY_EDITOR		// Used for debugging
			//		StartCoroutine(WaitForAd());
			//		#endif

			if(string.Equals(zone, "")) {
				zone = null;
			}

			ShowOptions options = new ShowOptions();
			options.resultCallback = result => {
				switch (result) {
				case ShowResult.Finished:
					onVideoPlayed(true);
					break;
				case ShowResult.Skipped:
					onVideoPlayed(false);
					break;
				case ShowResult.Failed:
					onVideoPlayed(false);
					break;
				}
			};
			Advertisement.Show(zone, options);
		}
	}



	// Coroutine to simulate app pausing in editor
//	IEnumerator WaitForAd(){
//		float currentTimeScale = Time.timeScale;
//		Time.timeScale = 0f;
//		yield return null;
//
//		while(Advertisement.isShowing) {
//			yield return null;
//		}
//
//		Time.timeScale = currentTimeScale;
//	}


//	void OnGUI(){
//		if (GUI.Button (new Rect (100f, 100f, 100f, 100f), "Test")) {
//			ShowAd ("rewardedVideoZone");
//		}
//	}
}
