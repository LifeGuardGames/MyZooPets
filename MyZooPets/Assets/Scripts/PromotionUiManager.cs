using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PromotionUiManager : Singleton<PromotionUiManager> {

	public List<PositionTweenToggle> adPromo;

	public void Show() {
		int rand = Random.Range(0, adPromo.Count);
		adPromo[rand].Show();
	}

	public void OnContinueButton() {
		LoadLevelManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
	}

}
