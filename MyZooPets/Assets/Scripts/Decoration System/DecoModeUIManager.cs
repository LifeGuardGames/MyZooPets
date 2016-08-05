using System;
using UnityEngine;

public class DecoModeUIManager : SingletonUI<DecoModeUIManager> {
	public TweenToggleDemux tweenDemux;
	public Animation shopButtonPulseAnim;
	public GameObject shopButtonSunbeam;

	private bool isActive = false;

	protected override void Awake() {
		base.Awake();
		eModeType = UIModeTypes.EditDecos;
	}

	protected override void Start() {
		base.Start();
	}

	protected override void OnDestroy() {
		base.OnDestroy();
	}

	protected override void _OpenUI() {
		if(!isActive) {
			isActive = true;

			tweenDemux.Show();
			DecoInventoryUIManager.Instance.ShowPanel();
			RoomArrowsUIManager.Instance.ShowPanel();

			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();

			// Hide the pet/minipet so it doesn't get in the way
			PetAnimationManager.Instance.DisableVisibility();
			if(MiniPetManager.Instance) {
				MiniPetManager.Instance.ToggleAllMinipetVisilibity(false);
			}

			if(InventoryManager.Instance.AllDecoInventoryItems.Count == 0) {
				TogglePulseShopButton(true);
			}
		}
	}

	//The back button on the left top corner is clicked to zoom out of the highscore board
	protected override void _CloseUI() {
		if(isActive) {
			isActive = false;

			tweenDemux.Hide();
			DecoInventoryUIManager.Instance.HidePanel();

			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			RoomArrowsUIManager.Instance.ShowPanel();

			// Show the pet/minipet again
			PetAnimationManager.Instance.EnableVisibility();
			if(MiniPetManager.Instance) {
				MiniPetManager.Instance.ToggleAllMinipetVisilibity(true);
			}

			TogglePulseShopButton(false);
		}
	}

	/// <summary>
	/// Open store directly to decoration category
	/// </summary>
	public void OnDecoShopButton() {
		// hide swipe arrow because not needed in shop mode
		RoomArrowsUIManager.Instance.HidePanel();

		// push the shop mode type onto the click manager stack
		ClickManager.Instance.Lock(UIModeTypes.Store);
		CloseUI();
		// open the shop
		StoreUIManager.OnShortcutModeEnd += ReopenChooseMenu;
		StoreUIManager.Instance.OpenToSubCategory("Decorations", true, StoreShortcutType.DecorationUIStoreButton);
	}

	/// <summary>
	/// This function is called from the store UI when the store closes and the user had opened the store from the deco system.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void ReopenChooseMenu(object sender, EventArgs args) {
		// show swipe arrows
		RoomArrowsUIManager.Instance.ShowPanel();

		// pop the mode we pushed earlier from the click manager
		ClickManager.Instance.ReleaseLock();
		OpenUI();
		StoreUIManager.OnShortcutModeEnd -= ReopenChooseMenu;
	}

	private void TogglePulseShopButton(bool isPulseShopButton) {
		if(isPulseShopButton) {
			shopButtonPulseAnim.Play();
			shopButtonSunbeam.SetActive(true);
		}
		else {
			shopButtonPulseAnim.Stop();
			GameObjectUtils.ResetLocalScale(shopButtonPulseAnim.gameObject);
			shopButtonSunbeam.SetActive(false);
		}
	}
}
