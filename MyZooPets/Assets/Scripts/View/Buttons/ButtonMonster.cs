using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ButtonMonster
// Button that is on a gating monster.
//---------------------------------------------------

public class ButtonMonster : LgButtonHold {
	// attack gate script
	private AttackGate scriptAttack;
	
	// fire meter script
	public FireMeter scriptFireMeter;
	
	// the sprite icon of this button
	public GameObject goIcon;
	
	// The parent panel for this button, to be destroyed when needed
	public GameObject parentPanel;
	
	// the gate that this button is for
	private Gate gate;
	
	// is this button being pressed legally?  Mainly used as a stopgap for now
	private bool bLegal;
	
	// const variable for the name of the fire button
	public const string FIRE_BUTTON = "FireButtonPanel";
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------		
	protected override void _Start() {
		PanToMoveCamera scriptPan = CameraManager.Instance.GetPanScript();
		scriptPan.OnPartitionChanging += OnPartitionChanging;
	}
	
	//---------------------------------------------------
	// _Update()
	//---------------------------------------------------		
	protected override void _Update() {
		if ( !CameraManager.Instance )
			return;
		
		bool bActive = NGUITools.GetActive( goIcon );
		bool bCamMoving = CameraManager.Instance.IsCameraMoving();

		if ( bActive && bCamMoving ) {
			NGUITools.SetActive( goIcon, false );	// if the button is on and the camera is moving, deactivate it
			Debug.Log("off");
		}
		else if ( !bActive && !bCamMoving ) {
			Debug.Log("On");
			NGUITools.SetActive( goIcon, true );	// if the button is off and the camera isn't moving, activate it
		}
	}	
	
	//---------------------------------------------------
	// _OnDestroy()
	//---------------------------------------------------		
	protected override void _OnDestroy() {
		if ( CameraManager.Instance ) {
			PanToMoveCamera scriptPan = CameraManager.Instance.GetPanScript();
			scriptPan.OnPartitionChanging -= OnPartitionChanging;	
		}
	}
	
	//---------------------------------------------------
	// OnPartitionChanged()
	//---------------------------------------------------	
	public void OnPartitionChanging( object sender, PartitionChangedArgs args ) {
		// if the partition is changing at all, destroy this UI
		Destroy( parentPanel );
	}
	
	//---------------------------------------------------
	// SetGate()
	//---------------------------------------------------	
	public void SetGate( Gate gate ) {
		this.gate = gate;
	}
	
	//---------------------------------------------------
	// ProcessClick()
	// When the user presses down on the fire meter button.
	// This will begin some pet animation prep and start
	// to fill the attached meter.
	//---------------------------------------------------	
	protected override void ProcessClick() {	
		bLegal = false;
		
		PetAnimator scriptPetAnimator = PetMovement.Instance.GetPetAnimatorScript();
		
		// if the pet is currently busy, forgetaboutit
		if ( scriptPetAnimator.IsBusy() || scriptPetAnimator.GetAnimState() == PetAnimStates.Walking )
			return;
		
		bLegal = true;
		
		// get the gate for this monster
		Gate scriptGate = gate;		
		
		// kick off the attack script
		int nDamage = GetDamage();
		scriptAttack = scriptPetAnimator.gameObject.AddComponent<AttackGate>();
		scriptAttack.Init( scriptPetAnimator, scriptGate, nDamage );
		
		// turn the fire meter on
		scriptFireMeter.StartFilling();
	}
	
	//---------------------------------------------------
	// GetDamage()
	// Returns the amount of damage the pet will currently
	// attack with.
	//---------------------------------------------------		
	private int GetDamage() {
		Skill curSkill = FlameLevelLogic.Instance.GetCurrentSkill();
		int nDamage = curSkill.DamagePoint;
		return nDamage;
	}
	
	//---------------------------------------------------
	// ButtonReleased()
	//---------------------------------------------------	
	protected override void ButtonReleased() {
		if ( !bLegal )  {
			Debug.Log("Something going wrong with the fire button.  Aborting");
			return;
		}
		
		if ( scriptFireMeter.IsFull() ) {
			// if the meter was full on release, complete the attack!
			scriptAttack.FinishAttack();			
			
			// just kind of a hack for now until the gating system is complete
			// if the monster is dead or the pet can't breathe fire any more, destroy the button
			int nDamage = GetDamage();
			if ( gate.GetGateHP() <= nDamage || !DataManager.Instance.GameData.PetInfo.CanBreathFire() )
				Destroy( parentPanel );		
		}
		else {
			// if the meter was not full, cancel the attack
			scriptAttack.Cancel();
		}	
		
		// regardless we want to empty the meter
		scriptFireMeter.Empty();
	}
}
