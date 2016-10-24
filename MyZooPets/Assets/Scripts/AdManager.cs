using UnityEngine.Advertisements;

public class AdManager : Singleton<AdManager> {
	public delegate void AdCallback<T>(T value); // Populated by callers

	// Determine preliminary check
	public bool IsAdReady(string zone = "rewardedVideo") {
		return Advertisement.IsReady(zone);
	}

	public void ShowAd(AdCallback<bool> onVideoPlayed, string zone = "rewardedVideo") {
		if(DataManager.Instance.IsAdsEnabled && IsAdReady()) {
			if(string.Equals(zone, "")) {
				zone = null;
			}
			ShowOptions options = new ShowOptions();
			options.resultCallback = result => {
				switch(result) {
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
