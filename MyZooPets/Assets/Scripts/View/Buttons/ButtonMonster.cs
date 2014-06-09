using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ButtonMonster
// Button that is on a gating monster.
//---------------------------------------------------

public class ButtonMonster : LgButtonHold{
	// const variable for the name of the fire button
	public const string FIRE_BUTTON = "FireButtonPanel";

	public FireMeter scriptFireMeter; // fire meter script
	public GameObject goIcon; // the sprite icon of this button
	public GameObject parentPanel; // The parent panel for this button, to be destroyed when needed
	
	private AttackGate scriptAttack; // attack gate script
	private Gate gate; // the gate that this button is for
	private bool isLegal; // is this button being pressed legally?  Mainly used as a stopgap for now

	protected override void _Start(){
		PanToMoveCamera scriptPan = CameraManager.Instance.GetPanScript();
		scriptPan.OnPartitionChanging += OnPartitionChanging;
	}

	protected override void _Update(){
		if(!CameraManager.Instance)
			return;
		
		bool isActive = NGUITools.GetActive(goIcon);
		bool isCamMoving = CameraManager.Instance.IsCameraMoving();

		if(isActive && isCamMoving){
			NGUITools.SetActive(goIcon, false);	// if the button is on and the camera is moving, deactivate it
			Debug.Log("off");
		}
		else if(!isActive && !isCamMoving){
			Debug.Log("On");
			NGUITools.SetActive(goIcon, true);	// if the button is off and the camera isn't moving, activate it
		}
	}	
	
	//---------------------------------------------------
	// _OnDestroy()
	//---------------------------------------------------		
	protected override void _OnDestroy(){
		if(CameraManager.Instance){
			PanToMoveCamera scriptPan = CameraManager.Instance.GetPanScript();
			scriptPan.OnPartitionChanging -= OnPartitionChanging;	
		}
	}
	
	//---------------------------------------------------
	// OnPartitionChanged()
	//---------------------------------------------------	
	public void OnPartitionChanging(object sender, PartitionChangedArgs args){
		// if the partition is changing at all, destroy this UI
		Destroy(parentPanel);
	}
	
	//---------------------------------------------------
	// SetGate()
	//---------------------------------------------------	
	public void SetGate(Gate gate){
		this.gate = gate;
	}
	
	//---------------------------------------------------
	// ProcessClick()
	// When the user presses down on the fire meter button.
	// This will begin some pet animation prep and start
	// to fill the attached meter.
	//---------------------------------------------------	
	protected override void ProcessClick(){	
		isLegal = false;
		
		PetAnimator scriptPetAnimator = PetMovement.Instance.GetPetAnimatorScript();
		
		// if the pet is currently busy, forgetaboutit
		if(scriptPetAnimator.IsBusy() || scriptPetAnimator.GetAnimState() == PetAnimStates.Walking)
			return;
		
		isLegal = true;
		
		// get the gate for this monster
		Gate scriptGate = gate;		
		
		// kick off the attack script
		int damage = GetDamage();
		scriptAttack = scriptPetAnimator.gameObject.AddComponent<AttackGate>();
		scriptAttack.Init(scriptPetAnimator, scriptGate, damage);
		
		// turn the fire meter on
		scriptFireMeter.StartFilling();
	}
	
	//---------------------------------------------------
	// GetDamage()
	// Returns the amount of damage the pet will currently
	// attack with.
	//---------------------------------------------------		
	private int GetDamage(){
		Skill curSkill = FlameLevelLogic.Instance.GetCurrentSkill();
		int damage = curSkill.DamagePoint;
		return damage;
	}
	
	//---------------------------------------------------
	// ButtonReleased()
	//---------------------------------------------------	
	protected override void ButtonReleased(){
		if(!isLegal){
			Debug.Log("Something going wrong with the fire button.  Aborting");
			return;
		}
		
		if(scriptFireMeter.IsFull()){
			// if the meter was full on release, complete the attack!
			scriptAttack.FinishAttack();			
			
			// because the user can only ever breath fire once, the only time we don't want to destroy the fire button is when the infinite
			// fire mode cheat is active and the gate is still alive
			int damage = GetDamage();
			if(gate.GetGateHP() <= damage || !DataManager.Instance.GameData.PetInfo.IsInfiniteFire())
				Destroy(parentPanel);		
		}
		else{
			// if the meter was not full, cancel the attack
			scriptAttack.Cancel();
		}	
		
		// regardless we want to empty the meter
		scriptFireMeter.Empty();
	}
}
