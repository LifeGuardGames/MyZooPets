using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// ButtonMonster
// Button that is on a gating monster.
//---------------------------------------------------

public class ButtonMonster : LgButtonHold {
	public int nDamage;
	
	// attack gate script
	private AttackGate scriptAttack;
	
	// fire meter script
	private FireMeter scriptFireMeter;
	
	// actual fire meter object created when this button is pressed
	public GameObject goFireMeter;
	
	// the gate that this button is for
	private Gate gate;
	
	// is this button being pressed legally?  Mainly used as a stopgap for now
	private bool bLegal;
	
	//---------------------------------------------------
	// _Start()
	//---------------------------------------------------		
	protected override void _Start() {
		PanToMoveCamera scriptPan = CameraManager.Instance.GetPanScript();
		scriptPan.OnPartitionChanging += OnPartitionChanging;
	}
	
	//---------------------------------------------------
	// _OnDestroy()
	//---------------------------------------------------		
	protected override void _OnDestroy() {
		PanToMoveCamera scriptPan = CameraManager.Instance.GetPanScript();
		scriptPan.OnPartitionChanging -= OnPartitionChanging;		
	}
	
	//---------------------------------------------------
	// OnPartitionChanged()
	//---------------------------------------------------	
	public void OnPartitionChanging( object sender, PartitionChangedArgs args ) {
		// if the partition is changing at all, destroy this UI
		Destroy( gameObject );
	}
	
	//---------------------------------------------------
	// SetGate()
	//---------------------------------------------------	
	public void SetGate( Gate gate ) {
		this.gate = gate;
	}
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {	
		bLegal = false;
		
		PetAnimator scriptPetAnimator = GatingManager.Instance.GetPlayerPetAnimator();
		
		// if the pet is currently busy, forgetaboutit
		if ( scriptPetAnimator.IsBusy() || scriptPetAnimator.GetAnimState() == PetAnimStates.Walking )
			return;
		
		bLegal = true;
		
		// get the gate for this monster
		Gate scriptGate = gate;		
		
		// otherwise, add the attack script
		scriptAttack = scriptPetAnimator.gameObject.AddComponent<AttackGate>();
		scriptAttack.Init( scriptPetAnimator, scriptGate, nDamage );
		
		// create the fire meter
		//GameObject resourceFireMeter = Resources.Load( "FireMeter" ) as GameObject;
		GameObject goFireMeter = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), this.goFireMeter );
		scriptFireMeter = goFireMeter.GetComponent<FireMeter>();
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
			if ( gate.GetGateHP() == nDamage )
				Destroy( gameObject );
		}
		else {
			// if the meter was not full, cancel the attack
			scriptAttack.Cancel();
		}	
		
		// either way, the meter should be destroyed
		Destroy( scriptFireMeter.gameObject );
	}
}
