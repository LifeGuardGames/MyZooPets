using UnityEngine;
using System;

public class FireButtonManager : Singleton<FireButtonManager> {
	public enum FireButtonState {
		Empty,
		ActivatingButton,
		ReadyForPress,
		HoldingButton,
		BlowingFire
	}

	private FireButtonState buttonState = FireButtonState.Empty;    // Current state of the structure

	public static EventHandler<EventArgs> FireButtonActive;

	public GameObject toggleParent;

	public GameObject fireButtonObject;
	public GameObject FireButtonObject {
		get { return fireButtonObject; }
	}

	public FireButtonAnimHelper animHelper;
	public FireMeter fireMeterScript;

	private bool isActive = false;
	public bool IsActive {
		get { return isActive; }
	}

	void Start() {
		toggleParent.SetActive(false);
		CameraManager.Instance.PanScript.OnPartitionChanging += OnPartitionChanging;
	}

	void OnDestroy() {
		if(CameraManager.Instance) {
			CameraManager.Instance.PanScript.OnPartitionChanging -= OnPartitionChanging;
		}
	}

	public void OnPartitionChanging(object sender, PartitionChangedArgs args) {
		// if the partition is changing at all, destroy this UI
		Deactivate();
	}

	// Called when the pet reaches a smoke monster room
	public void Activate() {
		// The fire button will always be spawned at the pet's location
		GameObject pet = GameObject.Find("Pet");
		transform.position = pet.transform.position;
		Debug.Log(pet.transform.position);

		isActive = true;
		toggleParent.SetActive(true);

		bool canBreatheFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();
		if(!canBreatheFire) {
			Debug.Log("Fire effect off");
			TurnFireEffectOff();
		}
		else {
			Debug.Log("Fire effect on");
			TurnFireEffectOn();
			buttonState = FireButtonState.ReadyForPress;
        }
	}

	// Called when the pet leaves a smoke monster room
	public void Deactivate() {
		isActive = false;
		toggleParent.SetActive(false);
	}

	public void TurnFireEffectOff() {
		animHelper.FireEffectOff();
	}

	// This is called from start, and from FireButtonAnimHelper (looped back in)
	public void TurnFireEffectOn() {
		animHelper.FireEffectOn();
		Debug.Log("FIRE BUTTON CALL");
		if(FireButtonActive != null) {  // Used for tutorials
			FireButtonActive(this, EventArgs.Empty);
		}
	}

	// Called from FireButtonHelper - Fire orb is dropped onto button
	public void Step1_SetButtonActiveWithItem(InventoryItem itemData) {
		if(buttonState == FireButtonState.Empty) {
			bool canBreatheFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();

			// Only works if item is flame crystal and pet can't breathe fire yet
			if(itemData.ItemID == "Usable1" && !canBreatheFire) {
				// Check to make sure the item can be used
				if(ItemManager.Instance.CanUseItem(itemData.ItemID)) {
					// Notify inventory logic that this item is being used
					InventoryManager.Instance.UsePetItem(itemData.ItemID);

					// Pass, move on
					buttonState = FireButtonState.ActivatingButton;
					animHelper.StartFireButtonAnimation();
				}
			}
		}
		else {
			Debug.LogWarning("Button state error " + buttonState.ToString());
		}
	}

	// Called from FireButtonAnimHelper
	public void Step2_AnimationComplete() {
		if(buttonState == FireButtonState.ActivatingButton) {
			buttonState = FireButtonState.ReadyForPress;
		}
		else {
			Debug.LogWarning("Button state error " + buttonState.ToString());
		}
	}

	// Called from FireButtonHelper
	public void Step3_ChargeFire() {
		if(buttonState == FireButtonState.ReadyForPress) {
			bool canBreathFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();

			// if can breathe fire, attack the gate!!
			if(canBreathFire) {
				if(SceneUtils.CurrentScene != SceneUtils.YARD) {
					buttonState = FireButtonState.HoldingButton;

					// kick off the attack script
					attackScript = PetAnimationManager.Instance.gameObject.AddComponent<AttackGate>();
					attackScript.Init(gate);

					PetAnimationManager.Instance.StartFireBlow();

					// turn the fire meter on
					fireMeterScript.StartFilling();
				}
				else {
					LoadLevelManager.Instance.StartLoadTransition(SceneUtils.MICROMIX);
				}
			}
			// else can't breathe fire. explain why
			else {
				if(!TutorialManager.Instance.IsTutorialActive()) {
					GatingManager.Instance.IndicateNoFire();
				}
			}
		}
		else {
			Debug.LogWarning("Button state error " + buttonState.ToString());
		}
	}

	// Call from FireButtonHelper
	public void Step4_ReleaseCharge() {
		if(buttonState == FireButtonState.HoldingButton) {
			if(fireMeterScript.IsMeterFull()) {
				buttonState = FireButtonState.BlowingFire;

				// if the meter was full on release, complete the attack!
				attackScript.FinishAttack();

				// because the user can only ever breath fire once, the only time we don't want to destroy the fire button is when the infinite
				// fire mode cheat is active and the gate is still alive
				if(gate.GetGateHP() <= 1) {
					Deactivate();
				}
				else {
					//disable button
					TurnFireEffectOff();
				}
			}
			else {
				// if the meter was not full, cancel the attack
				attackScript.Cancel();
				buttonState = FireButtonState.ReadyForPress;
			}
			// regardless we want to empty the meter
			fireMeterScript.Reset();
		}
		else {
			Debug.LogWarning("Button state error " + buttonState.ToString());
		}
	}

	public void Step5_FinishBlowingFire() {

	}






	private AttackGate attackScript;        // attack gate script
	private Gate gate;                      // the gate that this button is for

	public void SetGate(Gate gate) {
		this.gate = gate;
	}
}
