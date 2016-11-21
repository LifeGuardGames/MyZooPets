using UnityEngine;

public class NinjaTriggerBomb : NinjaTrigger {
	// how much damage does this trigger do when cut?
	public int nDamage;
	public int GetDamageValue() {
		return nDamage;	
	}
			
	protected override void _OnCut() {
		//if(!NinjaManager.Instance.isTutorialRunning){
			// take lives from the player
			int nLives = GetDamageValue();
			NinjaGameManager.Instance.UpdateLife( -nLives );
			NinjaGameManager.Instance.ResetChain();
		//}
		// then destroy the object
		Destroy(gameObject);		
	}

	void OnBecameVisible() {
		this.GetComponent<Rigidbody>().detectCollisions = true;
	}
}
