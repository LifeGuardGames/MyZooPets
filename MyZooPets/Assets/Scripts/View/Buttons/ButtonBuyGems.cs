using UnityEngine;
using System.Collections;

public class ButtonBuyGems : LgButton {
	protected override void ProcessClick(){
		StoreUIManager.Instance.OpenToSubCategoryPremiumWithLockAndCallBack();
	}
}
