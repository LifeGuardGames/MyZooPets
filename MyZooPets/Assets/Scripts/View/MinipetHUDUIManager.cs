using UnityEngine;
using System.Collections;

public class MinipetHUDUIManager : SingletonUI<MinipetHUDUIManager> {

	protected override void _OpenUI(){

		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		HUDUIManager.Instance.HidePanel();
		EditDecosUIManager.Instance.HideNavButton();
		InventoryUIManager.Instance.ShowPanel();
	}

	protected override void _CloseUI(){

		//Show other UI Objects
		NavigationUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		EditDecosUIManager.Instance.ShowNavButton();
	}
}
