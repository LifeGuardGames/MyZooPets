using UnityEngine;

public class FireButton : MonoBehaviour {
	public FireMeter scriptFireMeter;		// fire meter script

	private AttackGate attackScript;		// attack gate script
	private Gate gate;						// the gate that this button is for
	private bool isLegal;					// is this button being pressed legally? Mainly used as a stop gap for now

	public void SetGate(Gate gate){
		this.gate = gate;
	}
	
	/// <summary>
	/// When the user presses down on the fire meter button. This will begin
	/// some pet animation prep and start to fill the attached meter
	/// </summary>
	public void OnClick(){
		isLegal = false;
		bool canBreathFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();

		// if can breathe fire, attack the gate!!
		if(canBreathFire){
			if(SceneUtils.CurrentScene != SceneUtils.YARD) {
				isLegal = true;

				// kick off the attack script
				int damage = GetDamage();
				attackScript = PetAnimationManager.Instance.gameObject.AddComponent<AttackGate>();
				attackScript.Init(gate, damage);

				PetAnimationManager.Instance.StartFireBlow();

				// turn the fire meter on
				scriptFireMeter.StartFilling();
			}
			else {
				LoadLevelManager.Instance.StartLoadTransition(SceneUtils.MICROMIX);
			}
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

	public void OnButtonReleased(){
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
