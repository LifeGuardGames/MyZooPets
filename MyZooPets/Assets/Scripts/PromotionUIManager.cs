using System.Collections.Generic;

public class PromotionUIManager : Singleton<PromotionUIManager> {

	public List<TweenToggleDemux> promoDemux;

	public void TryShow() {
		if(DataManager.Instance.GameData.AdViews.SeanAdViews >= 3) {
			promoDemux.RemoveAt(0);
		}
		if(promoDemux.Count > 0 && CheckIosToggle()) {
			Analytics.Instance.ShowAd(promoDemux[0].name);
			promoDemux[0].Show();
			DataManager.Instance.GameData.AdViews.SeanAdViews++;
		}
		else {
			OnContinueButton();
		}
	}

	public void OnContinueButton() {
		QuestionaireManager.Instance.ContinueLoading(true);
	}

	/// <summary>
	/// Returns true all the time if it is Android
	/// </summary>
	public bool CheckIosToggle() {
#if UNITY_IOS && !UNITY_EDITOR
		bool toggleAux = DataManager.Instance.GameData.AdViews.SeanAdViewsIosAlternateToggle;
		DataManager.Instance.GameData.AdViews.SeanAdViewsIosAlternateToggle = !DataManager.Instance.GameData.AdViews.SeanAdViewsIosAlternateToggle;
		return toggleAux;
#endif
#if UNITY_ANDROID
		return true;
#endif
	}
}
