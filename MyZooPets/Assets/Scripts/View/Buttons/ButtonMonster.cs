using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ButtonMonster
// Button that is on a gating monster.
//---------------------------------------------------

public class ButtonMonster : LgButton {
	public int nDamage;
	
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
		AttackGate script = scriptPetAnimator.gameObject.AddComponent<AttackGate>();
		script.Init( scriptPetAnimator, scriptGate, nDamage );
	}
}
