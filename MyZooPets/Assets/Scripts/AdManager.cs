using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : Singleton<AdManager> {
	[SerializeField] string iOSGameID = "1077661";
	[SerializeField] string androidGameID = "1077662";

	public delegate void AdCallback<T>(T value); // Populated by callers
	private bool isTestMode = false;

	void Awake(){
		#if UNITY_EDITOR
		isTestMode = true;
		Debug.LogWarning("Editor Mode - turning ads testing mode ON");
		#endif
		#if UNITY_ANDROID
		Advertisement.Initialize(androidGameID, isTestMode);
		#endif
		#if UNITY_IOS
		Advertisement.Initialize(iOSGameID, isTestMode);
		#endif
	}

	// Determine preliminary check
	public bool IsAdReady(string zone = "rewardedVideoZone"){
		return Advertisement.IsReady(zone);
	}

	public void ShowAd(AdCallback<bool> onVideoPlayed, string zone = "rewardedVideoZone"){
		if(DataManager.Instance.IsAdsEnabled && IsAdReady()) {
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
}
