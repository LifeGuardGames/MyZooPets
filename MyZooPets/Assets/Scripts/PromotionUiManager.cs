using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PromotionUiManager : Singleton<PromotionUiManager> {

	public List<PositionTweenToggle> adPromo;

	public void Show() {
		if(DataManager.Instance.GameData.AdViews.SeanAdViews >= 3) {
			adPromo.RemoveAt(0);
		}
		if(adPromo.Count > 0) {
			adPromo[0].Show();
			DataManager.Instance.GameData.AdViews.SeanAdViews++;
        }
		else {
			OnContinueButton();
		}
	}

	public void OnContinueButton() {
		LoadLevelManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
	}

}
