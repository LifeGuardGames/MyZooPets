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
	
	// the sprite icon of this button
	public GameObject goIcon;
	
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
