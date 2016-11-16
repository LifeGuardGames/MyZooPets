using System.Collections.Generic;
using UnityEngine;

public class PromotionUIManager : Singleton<PromotionUIManager> {

	public List<TweenToggleDemux> promoDemux;

	public void Show() {
		Debug.Log("SDF");
		if(DataManager.Instance.GameData.AdViews.SeanAdViews >= 3) {
			promoDemux.RemoveAt(0);
		}
		if(promoDemux.Count > 0) {
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
}
