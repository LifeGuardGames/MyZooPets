using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonMonster
// Button that is on a gating monster.
//---------------------------------------------------

public class ButtonMonster : LgButton {
	public int nDamage;
	
	// attack gate script
	private AttackGate scriptAttack;
	
	// fire meter script
	private FireMeter scriptFireMeter;
	
	// actual fire meter object
	public GameObject goFireMeter;
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		PetAnimator scriptPetAnimator = GatingManager.Instance.GetPlayerPetAnimator();
		Gate scriptGate = GetComponent<Gate>();
		
		// if the pet is currently busy, forgetaboutit
		if ( scriptPetAnimator.IsBusy() || scriptPetAnimator.GetAnimState() == PetAnimStates.Walking )
			return;
		
		// otherwise, add the attack script
		scriptAttack = scriptPetAnimator.gameObject.AddComponent<AttackGate>();
		scriptAttack.Init( scriptPetAnimator, scriptGate, nDamage );
		
		// create the fire meter
		//GameObject resourceFireMeter = Resources.Load( "FireMeter" ) as GameObject;
		GameObject goFireMeter = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Top"), this.goFireMeter );	
		scriptFireMeter = goFireMeter.GetComponent<FireMeter>();
	}
	
	protected override void ButtonReleased() {
		if ( scriptFireMeter.IsFull() ) {
			scriptAttack.FinishAttack();
			Destroy( scriptFireMeter.gameObject );
		}
		else {
			scriptAttack.Cancel();
			Destroy( scriptFireMeter.gameObject );
		}	
	}
}
