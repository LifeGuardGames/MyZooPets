using UnityEngine;

public class WizdyDinerPromo : MonoBehaviour {
	public TweenToggleDemux tweenToggle;

	public void OnOkButton() {
		Analytics.Instance.ClickedAd("WizdyDinerPromo");
		tweenToggle.Hide();
		PromotionUIManager.Instance.OnContinueButton();
#if UNITY_ANDROID
		Application.OpenURL("market://details?id=com.LifeGuardGames.FoodAllergyAndroid");
#elif UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/id1059091444");
#endif
	}

	public void OnExitButton() {
		Analytics.Instance.ExitAd("WizdyDinerPromo");
		tweenToggle.Hide();
		PromotionUIManager.Instance.OnContinueButton();
	}
}
