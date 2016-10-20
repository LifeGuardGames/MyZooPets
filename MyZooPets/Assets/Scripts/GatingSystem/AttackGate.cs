using System;
using System.Collections;

/// <summary>
/// Attack gate. Script put on a pet when it is ready to attack a gate
/// </summary>
public class AttackGate : Singleton<AttackGate>{

	private Gate gateTarget; // gate to attack
	//private int damage; // damage to deal

	void Start(){
		PetAnimationManager.OnBreathEnded += ExecutePostAttackLogic;
	}

	void OnDestroy(){
		PetAnimationManager.OnBreathEnded -= ExecutePostAttackLogic;
	}

	public void Init(Gate gateTarget){
		this.gateTarget = gateTarget;
		//this.damage = 1;
	}

	/// <summary>
	/// Cancel attack so clean up.
	/// </summary>
	public void Cancel(){
		PetAnimationManager.Instance.AbortFireBlow();
	
		//FireButtonUIManager.Instance.FireButtonCollider.enabled = true;

		//release lock if fire breathing lock was called previously
		UIModeTypes currentLockMode = ClickManager.Instance.CurrentMode;
		if(currentLockMode == UIModeTypes.FireBreathing) {
			ClickManager.Instance.ReleaseLock();
		}
		
		Destroy(this);
	}

	/// <summary>
	/// Finishs the attack. Button is charged so blow out and finish the attack
	/// </summary>
	public void FinishAttack(){
		PetAnimationManager.Instance.FinishFireBlow();
		ClickManager.Instance.Lock(mode:UIModeTypes.FireBreathing);
	}

	/// <summary>
	/// Executes the post attack logic.
	/// </summary>
	public void ExecutePostAttackLogic(object sender, EventArgs args){
		StartCoroutine(PostAttackLogic());
	}

	/// <summary>
	/// Pet done attacking. Call the appropriate classes/functions to damage
	/// smog monster, decrement fire breaths count, and release click manager lock
	/// </summary> 
	private IEnumerator PostAttackLogic(){
		// and decrement the user's fire breaths
		StatsManager.Instance.UpdateFireBreaths(-1);

		// damage the gate
		gateTarget.DamageGate();

		// also mark the player as having attack the monster (for wellapad tasks)
		WellapadMissionController.Instance.TaskCompleted("FightMonster");
		
		// wait a frame to do our other stuff because the fire breathing animation is still technically playing
		yield return 0;
		
		// release fire breathing lock
		ClickManager.Instance.ReleaseLock();

		// then we're done -- destroy ourselves
		Destroy(this);		
	}
}
