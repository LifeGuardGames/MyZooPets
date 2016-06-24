using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ButtonFireButton : LgButtonHold{
	public FireMeter scriptFireMeter;		// fire meter script
	public GameObject goIcon;				// the sprite icon of this button

	private AttackGate attackScript;		// attack gate script
	private Gate gate;						// the gate that this button is for
	private bool isLegal;					// is this button being pressed legally?  Mainly used as a stopgap for now

	protected override void _Update(){
		if(!CameraManager.Instance){
			return;
		}
		
		bool isActive = NGUITools.GetActive(goIcon);
		bool isCamMoving = CameraManager.Instance.IsCameraMoving();

		if(isActive && isCamMoving){
			goIcon.SetActive(false);	// if the button is on and the camera is moving, deactivate it
		}
		else if(!isActive && !isCamMoving){
			goIcon.SetActive(true);	// if the button is off and the camera isn't moving, activate it
		}
	}

	public void SetGate(Gate gate){
		this.gate = gate;
	}

	public void FireButtonClicked() {
		ProcessClick();
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
			if(!TutorialManager.Instance.IsTutorialActive()){
				GatingManager.Instance.IndicateNoFire();
			}
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

	public void FireButtonReleased() {
		ButtonReleased();
	}


	protected override void ButtonReleased(){
		if(!isLegal){
//			Debug.Log("Something going wrong with the fire button.  Aborting");
			return;
		}

		if(scriptFireMeter.IsMeterFull()){
			// if the meter was full on release, complete the attack!
			attackScript.FinishAttack();
			
			// because the user can only ever breath fire once, the only time we don't want to destroy the fire button is when the infinite
			// fire mode cheat is active and the gate is still alive
			int damage = GetDamage();
			if(gate.GetGateHP() <= damage){
				FireButtonUIManager.Instance.Deactivate();
			}
			else{
				//disable button
				FireButtonUIManager.Instance.FireEffectOff();
			}
		}
		else{
			// if the meter was not full, cancel the attack
			attackScript.Cancel();
		}
		// regardless we want to empty the meter
		scriptFireMeter.Empty();
	}
}
