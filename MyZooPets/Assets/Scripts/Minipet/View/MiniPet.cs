using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Script to control Minipet and contains the basic properties of a minipet.
/// Should attach this script to the highest parent in the minipet prefab
/// </summary>
public abstract class MiniPet : MonoBehaviour, IDropInventoryTarget {

	protected string minipetId;
	public string MinipetId{
		get{ return minipetId; }
	}

	protected MiniPetTypes minipetType;
	public MiniPetTypes MinipetType{
		get{ return minipetType; }
	}

	public MiniPetAnimationManager animationManager;
	public MiniPetSpeechAI miniPetSpeechAI;
	public GameObject flippable;
	public GameObject eggParent;
	public Animation eggAnimation;
	public MinipetEggClickController eggClickController;

	public Vector3 zoomPositionOffset = new Vector3(-3, 4, -11);
	public Vector3 zoomRotation = new Vector3(12, 0, 0);
	private bool isVisible;
	private bool isHatchedAux;

	public ParticleSystem getAccessoryParticle;
	public GameObject accessory1;
	public GameObject accessory2;
	public GameObject accessory3;
	public Transform petPosition;

	private bool isMiniPetColliderLocked = false; //use this to disable click on minipet when zooming in
	public bool isFinishEating = false; //F: Need to finish the eating logic after camera zooms in
	private string invItemID; //local reference to the item that is dropped on the minipet

	public EventHandler<EventArgs> OnTutorialMinipetClicked;    // event that fires when the user clicks on pet during tutorial
	public bool isAdded;

	/// <summary>
	/// Pass in the immutable data so this specific MiniPet instantiate can be instantiated with the proper information.
	/// </summary>
	/// <param name="minipetData">ImmutableDataMiniPet.</param>
	public void Init(ImmutableDataMiniPet minipetData){
		minipetId = minipetData.ID;
		name = minipetData.Name;
	}

	protected virtual void Start(){		
		MiniPetHUDUIManager.Instance.OnManagerOpen += ShouldPauseIdleAnimations;
		
		isFinishEating = MiniPetManager.Instance.IsPetFinishedEating(minipetId);

		AccessoriesCheck(false);
		RefreshUnlockState();
	}

	// Check to see if you want to display pet or egg
	public void RefreshUnlockState(){
		ToggleHatched(DataManager.Instance.GameData.MiniPets.GetHatched(minipetId));
	}

	public void ToggleHatched(bool isHatched){
		isHatchedAux = isHatched;
		if(isHatched){
			eggParent.SetActive(false);
			eggAnimation.GetComponent<Animation>().Stop();
			flippable.SetActive(true);
			gameObject.GetComponent<Collider>().enabled = true;
			if(eggClickController != null){		// Remove unused components on the egg parent
				Destroy(eggClickController);
				Destroy(eggClickController.GetComponent<Collider>());
			}
			isVisible = true;
		}
		else{	// Not hatched yet
			eggParent.SetActive(true);
			eggAnimation.GetComponent<Animation>().Play();
			flippable.SetActive(false);
			gameObject.GetComponent<Collider>().enabled = false;
			isVisible = false;
		}
	}
	
	void OnDestroy(){
		if(MiniPetHUDUIManager.Instance){
			MiniPetHUDUIManager.Instance.OnManagerOpen -= ShouldPauseIdleAnimations;
		}
	}

	private void OnTap(TapGesture gesture){
		Debug.LogWarning("NGUI REMOVE CHANGED - CORRECT CODE HERE");
		//if(!IsTouchingNGUI(gesture.Position)){
			if(ClickManager.Instance.stackPeek != "MiniPet"){
				Analytics.Instance.MiniPetVisited(minipetId);
				if(!isFinishEating){
					ShowFoodPreferenceMessage();
				}
				bool isUIOpened = MiniPetHUDUIManager.Instance.IsOpen();
				bool isModeLockEmpty = ClickManager.Instance.IsModeLockEmpty;

				if(!isMiniPetColliderLocked){
					if(LoadLevelManager.Instance.GetCurrentSceneName() == SceneUtils.BEDROOM){
						if(TutorialManagerBedroom.Instance == null || TutorialManagerBedroom.Instance.IsTutorialActive()){
							if(OnTutorialMinipetClicked != null){
								OnTutorialMinipetClicked(this, EventArgs.Empty);
							}
							return;
						}
					}
					if(!isUIOpened && isModeLockEmpty && !isAdded) {
						AudioManager.Instance.PlayClip("talkMinipet");
						PetMovement.Instance.MovePet(petPosition.position);
						isAdded = true;
						PetMovement.Instance.OnReachedDest += ZoomInToMiniPet;
						//ZoomInToMiniPet();
						//OpenChildUI();	// Further child UI calls
					}
				}
			}
		//}
	}

	private void OnMouseDown() {
		RaycastHit hitObject;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hitObject)) {
			if(hitObject.transform == transform) {
				Analytics.Instance.MiniPetVisited(minipetId);
				if(!isFinishEating) {
					ShowFoodPreferenceMessage();
				}
				bool isUIOpened = MiniPetHUDUIManager.Instance.IsOpen();
				bool isModeLockEmpty = ClickManager.Instance.IsModeLockEmpty;

				if(!isMiniPetColliderLocked) {
                    if(LoadLevelManager.Instance.GetCurrentSceneName() == SceneUtils.BEDROOM) {
						if(TutorialManagerBedroom.Instance == null || TutorialManagerBedroom.Instance.IsTutorialActive()) {
							if(OnTutorialMinipetClicked != null) {
								OnTutorialMinipetClicked(this, EventArgs.Empty);
							}
							return;
						}
					}
					if(!isUIOpened && isModeLockEmpty) {
						AudioManager.Instance.PlayClip("talkMinipet");
						//ZoomInToMiniPet();
					}
				}
			}
		}
	}
	// Further code to run in children if tapped and zoom into minipet
	protected abstract void OpenChildUI();


	private void ZoomInToMiniPet(System.Object sender,EventArgs args){
		isAdded = false;
		OpenChildUI();  // Further child UI calls
						//if pet not finish eating yet. finish eating logic
		Vector3 position = this.transform.position + zoomPositionOffset;

		isMiniPetColliderLocked = true;

		CameraManager.Callback cameraDoneFunction = delegate(){
			CameraMoveDone();
		};
		CameraManager.Instance.ZoomToTarget(position, zoomRotation, 0.5f, cameraDoneFunction);
		PetMovement.Instance.OnReachedDest -= ZoomInToMiniPet;
	}

	/// <summary>
	/// Logic to run after the camera has zoomed into the minipet
	/// </summary>
	private void CameraMoveDone(){
		isMiniPetColliderLocked = false;
		MiniPetHUDUIManager.Instance.SelectedMiniPetID = minipetId;
		MiniPetHUDUIManager.Instance.SelectedMiniPetName = name;
		MiniPetHUDUIManager.Instance.SelectedMiniPetGameObject = this.gameObject;
		MiniPetHUDUIManager.Instance.OpenUI();
		
		if(!isFinishEating){
			//InventoryLogic.Instance.UseMiniPetItem(invItemID);
			//MiniPetManager.Instance.IncreaseXP(id);
			//animationManager.Eat();
		}
		//else check if tickle and cleaning is done. if both done 
		else{
			if(TutorialManagerBedroom.Instance == null || !TutorialManagerBedroom.Instance.IsTutorialActive()){
				Invoke("ShowFoodPreferenceMessage", 1f);
			}
		}
		//}
	}

	private void ShouldPauseIdleAnimations(object sender, UIManagerEventArgs args){
		// Check if the minipet is hatched first
		if(isHatchedAux){
			if(args.Opening)
				animationManager.IsRunningIdleAnimations = false;
			else
				animationManager.IsRunningIdleAnimations = true;
		}
	}
	public virtual void FinishEating(){
		// If it is finished eating already, dont change anything when fed more
		if(isFinishEating){
			return;
		}
		else{
			isFinishEating = true;
			DataManager.Instance.GameData.MiniPets.SaveHunger(minipetId, isFinishEating);
			DataManager.Instance.GameData.MiniPets.SetMiniPetFoodChoice(minipetId, null);
        }
	}

	public void OnItemDropped(InventoryItem itemData) {
		if(MiniPetHUDUIManager.Instance.IsOpen()) {
			//check if minipet wants this food
			if(MiniPetManager.Instance.GetFoodPreference(minipetId) == itemData.ItemID) {
				InventoryManager.Instance.UseMiniPetItem(itemData.ItemID);    // Tell inventory logic item is used -> remove
				FinishEating();
				animationManager.Eat();
			}
			// show notification that the mp wants a specific food
			else{
				ShowFoodPreferenceMessage();
			}
		}
	}

	protected virtual void ShowFoodPreferenceMessage(){
		if(!isFinishEating){
			string preferredFoodID = MiniPetManager.Instance.GetFoodPreference(minipetId);
			Item item = DataLoaderItems.GetItem(preferredFoodID);
			miniPetSpeechAI.ShowFoodPreferenceMsg(item.TextureName);
		}
	}

	public void ToggleVisibility(bool isVisible){
		this.isVisible = isVisible;
		if(this.isVisible){
			ToggleHatched(isHatchedAux);	// Keep track of it internally already DWID
		}
		else{
			flippable.SetActive(false);
			eggParent.SetActive(false);
			gameObject.GetComponent<Collider>().enabled = false;
		}
	}

	/*
	//True: if finger touches NGUI 
	/// <summary>
	/// Determines whether if the touch is touching NGUI element
	/// </summary>
	/// <returns><c>true</c> if this instance is touching NGUI; otherwise, <c>false</c>.</returns>
	/// <param name="screenPos">Screen position.</param>
	protected bool IsTouchingNGUI(Vector2 screenPos){
		Ray ray = nguiCamera.ScreenPointToRay(screenPos);
		RaycastHit hit;
		int layerMask = 1 << 10; 
		bool isOnNGUILayer = false;
		
		// Raycast
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
			isOnNGUILayer = true;
		}
		return isOnNGUILayer;
	}
	*/

	// Called from minipet manager, handle animations and ui effects here
	public void GainedExperience(){
		animationManager.Cheer();
	}

	// Called from minipet manager, handle animations and ui effects here
	public void GainedLevel(){
		AccessoriesCheck(true);
		Invoke("GainedLevelHelper", 0.5f);
	}

	private void GainedLevelHelper(){
		animationManager.Cheer();
	}

	public virtual void OnItemDropped(InventoryItem item) {
		 string preferredFoodID = MiniPetManager.Instance.GetFoodPreference(minipetId);

		//check if minipet wants this food
		if(preferredFoodID == item.ItemID) {
			//use item if so
			InventoryManager.Instance.UseMiniPetItem(item.ItemID);    // Tell inventory logic item is used -> remove
			FinishEating();
			animationManager.Eat();
		}
		// show notification that the mp wants a specific food
		else {
			ShowFoodPreferenceMessage();
		}
	}

	/// <summary>
	/// Check enable the correct accessories according to the pet level
	/// </summary>
	/// <param name="isJustGained">If set to <c>true</c> is just gained from getting experience, play some fancy particles</param>
	private void AccessoriesCheck(bool isJustGained){
		Level currentLvl = MiniPetManager.Instance.GetCurrentLevel(minipetId);
		switch(currentLvl){
		case Level.Level1:
			accessory1.SetActive(false);
			accessory2.SetActive(false);
			accessory3.SetActive(false);
			break;
		case Level.Level2:
			accessory1.SetActive(true);
			accessory2.SetActive(false);
			accessory3.SetActive(false);
			if(isJustGained){
				getAccessoryParticle.Play();
			}
			break;
		case Level.Level3:
			accessory1.SetActive(true);
			accessory2.SetActive(true);
			accessory3.SetActive(false);
			if(isJustGained){
				getAccessoryParticle.Play();
			}
			break;
		case Level.Level4:
			accessory1.SetActive(true);
			accessory2.SetActive(true);
			accessory3.SetActive(true);
			if(isJustGained){
				getAccessoryParticle.Play();
			}
			break;
		default:
			Debug.LogError("Level limit exceeded, something wrong with minipets");
			break;
		}
	}
}
