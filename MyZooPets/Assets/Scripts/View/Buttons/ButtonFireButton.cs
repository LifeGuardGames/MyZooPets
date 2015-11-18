using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Button monster.
/// </summary>
public class ButtonFireButton : LgButtonHold{
	// const variable for the name of the fire button
	public const string FIRE_BUTTON = "FireButtonPanel";

	public FireMeter scriptFireMeter; // fire meter script
	public GameObject goIcon; // the sprite icon of this button
	public GameObject parentPanel; // The parent panel for this button, to be destroyed when needed
	
	private AttackGate attackScript; // attack gate script
	private Gate gate; // the gate that this button is for
	private bool isLegal; // is this button being pressed legally?  Mainly used as a stopgap for now

	protected override void _Start(){
		PanToMoveCamera scriptPan = CameraManager.Instance.PanScript;
		scriptPan.OnPartitionChanging += OnPartitionChanging;
	}

	protected override void _Update(){
		if(!CameraManager.Instance)
			return;
		
		bool isActive = NGUITools.GetActive(goIcon);
		bool isCamMoving = CameraManager.Instance.IsCameraMoving();

		if(isActive && isCamMoving){
			NGUITools.SetActive(goIcon, false);	// if the button is on and the camera is moving, deactivate it
		}
		else if(!isActive && !isCamMoving){

			NGUITools.SetActive(goIcon, true);	// if the button is off and the camera isn't moving, activate it
		}
	}	
		
	protected override void _OnDestroy(){
		if(CameraManager.Instance){
			PanToMoveCamera scriptPan = CameraManager.Instance.PanScript;
			scriptPan.OnPartitionChanging -= OnPartitionChanging;	
		}
	}

	public void OnPartitionChanging(object sender, PartitionChangedArgs args){
		// if the partition is changing at all, destroy this UI
		parentPanel.SetActive(false);
	}

	public void SetGate(Gate gate){
		this.gate = gate;
	}

	/// <summary>
	/// Processes the click.
	/// When the user presses down on the fire meter button. This will begin
	/// some pet animation prep and start to fill the attached meter
	/// </summary>
	protected override void ProcessClick(){	
		isLegal = false;
		bool canBreathFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();

		// if can breathe fire, attack the gate!!
		if(canBreathFire){
			isLegal = true;

			
			// kick off the attack script
			int damage = GetDamage();
			attackScript = PetAnimationManager.Instance.gameObject.AddComponent<AttackGate>();
			attackScript.Init(gate, damage);

			PetAnimationManager.Instance.StartFireBlow();
			
			// turn the fire meter on
			scriptFireMeter.StartFilling();
		}
		// else can't breathe fire. explain why
		else{
			if(!TutorialManager.Instance.IsTutorialActive())
				GatingManager.Instance.IndicateNoFire();
		}
	}

	/// <summary>
	/// Gets the damage the pet will currenlty attack with.
	/// </summary>
	/// <returns>The damage.</returns>
	private int GetDamage(){
		Skill curSkill = FlameLevelLogic.Instance.GetCurrentSkill();
		int damage = curSkill.DamagePoint;
		return damage;
	}

	protected override void ButtonReleased(){
		if(!isLegal){
//			Debug.Log("Something going wrong with the fire button.  Aborting");
			return;
		}

		if(scriptFireMeter.IsMeterFull()){
			// if the meter was full on release, complete the attack!
			attackScript.FinishAttack();
//			scriptAttack.Attack();
			
			// because the user can only ever breath fire once, the only time we don't want to destroy the fire button is when the infinite
			// fire mode cheat is active and the gate is still alive
			int damage = GetDamage();
			//if(gate.GetGateHP() <= damage)

				//Destroy(parentPanel);	
			//else
				//disable button
				FireButtonUIManager.Instance.TurnFireButtonEffectOff();
		}
		else{
			// if the meter was not full, cancel the attack
			attackScript.Cancel();
		}	
		
		// regardless we want to empty the meter
		scriptFireMeter.Empty();
	}
}
