using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MiniPetHUDUIManager : SingletonUI<MiniPetHUDUIManager> {
	public Text nameLabel;

	public TweenToggle storeTweenParent;
	private TweenToggle contentTweenParent;

	public Animation storeButtonPulseAnim;
	public GameObject storeButtonSunbeam;

	public GameObject contentParent;
	public GameObject petReference;

	public ParticleSystem experienceParticle;
	public ParticleSystem levelUpParticle;

	private GameObject content;
	private MiniPetTypes minipetType = MiniPetTypes.None;

	/// <summary>
	/// Gets or sets the selected mini pet ID. Need to be set before the HUD
	/// panel is opened.
	/// </summary>
	/// <value>The selected mini pet ID.</value>
	public string SelectedMiniPetID { get; set; }
	public string SelectedMiniPetName { get; set; }
	public GameObject SelectedMiniPetGameObject { get; set; }
	public MonoBehaviour SelectedMiniPetContentUIScript { get; set; }

	protected override void Awake() {
		base.Awake();
		eModeType = UIModeTypes.MiniPet;
	}

	// Called from minipet script children themselves
	public void OpenUIMinipetType(MiniPetTypes type, Hashtable hash, MonoBehaviour baseScript) {
		GameObject contentPrefab;
		switch(type) {
			case MiniPetTypes.Retention:
				minipetType = MiniPetTypes.Retention;
				//Debug.Log(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(hash[0].ToString()));
				if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(hash[0].ToString())) {
					//					Debug.Log(DataManager.Instance.GameData.Wellapad.CurrentTasks[hash[0].ToString()].RewardStatus);
					if(DataManager.Instance.GameData.Wellapad.CurrentTasks[hash[0].ToString()].isReward == RewardStatuses.Unclaimed
						  || DataManager.Instance.GameData.Wellapad.CurrentTasks[hash[0].ToString()].isReward == RewardStatuses.Unearned) {

						contentPrefab = Resources.Load("ContentParentRetention") as GameObject;
						content = GameObjectUtils.AddChildWithPositionAndScale(contentParent, contentPrefab);
						content.GetComponentInChildren<Button>().onClick.AddListener(() => MiniPetManager.Instance.MiniPetTable["MiniPet0"].GetComponent<MiniPetRetentionPet>().OnTurnInButton());
						MiniPetRetentionUIController minipetRetentionUIController = content.GetComponent<MiniPetRetentionUIController>();
						minipetRetentionUIController.InitializeContent(hash[0].ToString(), (MiniPetRetentionPet)baseScript);
						SelectedMiniPetContentUIScript = minipetRetentionUIController;

						if(TutorialManager.Instance == null || !TutorialManager.Instance.IsTutorialActive()) {
							contentTweenParent = content.GetComponent<TweenToggle>();
							//if(IsOpen() && (contentTweenParent != null)){	// Pet just finished eating, show asap HACK
							if(contentTweenParent != null) {
								StartCoroutine(ShowContentHelper());
							}
						}
					}
				}
				break;

			case MiniPetTypes.GameMaster:
				minipetType = MiniPetTypes.GameMaster;
				if(DataManager.Instance.GameData.Wellapad.CurrentTasks[hash[0].ToString()].isReward == RewardStatuses.Unclaimed
					  || DataManager.Instance.GameData.Wellapad.CurrentTasks[hash[0].ToString()].isReward == RewardStatuses.Unearned) {

					contentPrefab = Resources.Load("ContentParentGameMaster") as GameObject;
					content = GameObjectUtils.AddChildWithPositionAndScale(contentParent, contentPrefab);
					MinigameTypes minigameType = (MinigameTypes)Enum.Parse(typeof(MinigameTypes), hash[1].ToString());
					content.GetComponentInChildren<Button>().onClick.AddListener(() => MiniPetManager.Instance.MiniPetTable["MiniPet1"].GetComponent<MiniPetGameMaster>().OnTurnInButton());
					MiniPetGameMasterUIController minipetGameMasterUIController = content.GetComponent<MiniPetGameMasterUIController>();
					minipetGameMasterUIController.InitializeContent(hash[0].ToString(), minigameType, (MiniPetGameMaster)baseScript);
					SelectedMiniPetContentUIScript = minipetGameMasterUIController;

					if(TutorialManager.Instance == null || !TutorialManager.Instance.IsTutorialActive()) {
						contentTweenParent = content.GetComponent<TweenToggle>();
						//if(IsOpen() && (contentTweenParent != null)){	// Pet just finished eating, show asap HACK
						if(contentTweenParent != null) {
							StartCoroutine(ShowContentHelper());
						}
					}
				}
				break;

			case MiniPetTypes.Merchant:
				minipetType = MiniPetTypes.Merchant;
				contentPrefab = Resources.Load("ContentParentMerchant") as GameObject;
				content = GameObjectUtils.AddChildWithPositionAndScale(contentParent, contentPrefab);
				ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), hash[1].ToString());
				MiniPetMerchantUIController minipetMerchantUIController = content.GetComponent<MiniPetMerchantUIController>();
				minipetMerchantUIController.InitializeContent(hash[0].ToString(), false, itemType, (MiniPetMerchant)baseScript);
				SelectedMiniPetContentUIScript = minipetMerchantUIController;

				if(TutorialManager.Instance == null || !TutorialManager.Instance.IsTutorialActive()) {
					contentTweenParent = content.GetComponent<TweenToggle>();
					//if(IsOpen() && (contentTweenParent != null)){	// Pet just finished eating, show asap HACK
					if(contentTweenParent != null) {
						StartCoroutine(ShowContentHelper());
					}
				}
				break;

			default:
				minipetType = MiniPetTypes.None;
				Debug.LogError("No minipet type found: " + type.ToString());
				return;
		}
	}


	private IEnumerator ShowContentHelper() {
		yield return new WaitForSeconds(1.0f);
		if(contentTweenParent != null) {
			contentTweenParent.Show();
			StartCoroutine(HidePet());
		}
	}

	IEnumerator HidePet() {
		yield return new WaitForSeconds(0.5f);
		PetAnimationManager.Instance.DisableVisibility();
		PetAudioManager.Instance.EnableSound = false;
	}

	#region Overridden functions
	protected override void _OpenUI() {
		nameLabel.text = SelectedMiniPetName;
		this.GetComponent<TweenToggleDemux>().Show();
		storeTweenParent.Show();

		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		HUDUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.HidePanel();

	}

	protected override void _CloseUI() {
		this.GetComponent<TweenToggleDemux>().Hide();
		minipetType = MiniPetTypes.None;

		storeTweenParent.Hide();
		CheckStoreButtonPulse();
		SelectedMiniPetGameObject.GetComponent<MiniPetSpeechAI>().BeQuiet();
		SelectedMiniPetGameObject.GetComponent<MiniPetSpeechAI>().PetSpeechZoomToggle(false);

		//Show other UI Objects
		NavigationUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.ShowPanel();
		PetAnimationManager.Instance.EnableVisibility();
		PetAudioManager.Instance.EnableSound = true;
		DecoInventoryUIManager.Instance.HidePanel();
		if(content != null) {
			Destroy(content.gameObject);
		}
		CameraManager.Instance.ZoomOutMove();
	}
	#endregion

	/// <summary>
	/// Checks the store button pulse.
	/// This does all the check by itself so dont worry when calling this
	/// </summary>
	public void CheckStoreButtonPulse() {
		Item neededItem = DataLoaderItems.GetItem(MiniPetManager.Instance.GetFoodPreference(SelectedMiniPetID));
		bool isNeedItem = !DataManager.Instance.GameData.Inventory.InventoryItems.ContainsKey(neededItem.ID);
		bool isPetFed = !DataManager.Instance.GameData.MiniPets.IsPetFinishedEating(SelectedMiniPetID);

		if(isNeedItem && IsOpen && isPetFed) {
			storeButtonPulseAnim.Play();
			storeButtonSunbeam.SetActive(true);
		}
		else {
			storeButtonPulseAnim.Stop();
			GameObjectUtils.ResetLocalScale(storeButtonPulseAnim.gameObject);
			storeButtonSunbeam.SetActive(false);
		}
	}

	/// <summary>
	/// Opens the shop. Store button calls this function
	/// </summary>
	public void OnMiniPetShopButton() {
		this.GetComponent<TweenToggleDemux>().Hide();
		storeTweenParent.Hide();

		//sometimes this function will be called in a different mode, so we need
		//to make sure the UIs are handled appropriately
		bool isModeLockEmpty = ClickManager.Instance.IsModeStackEmpty;
		if(isModeLockEmpty) {
			NavigationUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
		}

		ClickManager.Instance.Lock(UIModeTypes.Store);
		Invoke("OpenFoodShopAfterWaiting", 0.2f);
	}

	/// <summary>
	/// Opens the shop after waiting. So the MiniPetHUDUI can hide first before the
	/// store UI shows up
	/// </summary>
	private void OpenFoodShopAfterWaiting() {
		StoreUIManager.OnShortcutModeEnd += CloseShop;
		StoreUIManager.Instance.OpenToSubCategory("Food", true, StoreShortcutType.MinipetUIStoreButton);
		if(content != null) {
			content.GetComponent<TweenToggle>().Hide();
		}
	}

	private void CloseShop(object sender, EventArgs args) {
		StoreUIManager.OnShortcutModeEnd -= CloseShop;
		HUDUIManager.Instance.ToggleLabels(false);
		ClickManager.Instance.ReleaseLock();

		UIModeTypes currentMode = ClickManager.Instance.CurrentMode;
		if(currentMode == UIModeTypes.MiniPet) {
			this.GetComponent<TweenToggleDemux>().Show();
			if(content != null && minipetType == MiniPetTypes.Merchant) {   // Keep the hud and open deco inventory
				content.GetComponent<TweenToggle>().Show();
				MiniPetMerchantUIController merchantUI = (MiniPetMerchantUIController)SelectedMiniPetContentUIScript;
				merchantUI.ShowDecoInventoryHelper();
			}
			else if(content != null) {
				content.GetComponent<TweenToggle>().Show();
				HUDUIManager.Instance.HidePanel();
			}
			else {
				HUDUIManager.Instance.HidePanel();
			}
			storeTweenParent.Show();
		}
		CheckStoreButtonPulse();
	}

	public bool HasContent() {
		return content == null ? false : true;
	}

	public void GainedExperience() {
		experienceParticle.Play();
	}

	public void GainedLevel() {
		levelUpParticle.Play();
	}
}
