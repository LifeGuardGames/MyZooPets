using UnityEngine;
using System.Collections;

///////////////////////////////////////////
// DestroyThis
// Simple script for destroying things on
// a timer; this should be used for
// everything so that pausing can be an
// easy and universal task.
///////////////////////////////////////////
/// 


//NEEDS refactoring.. WTF is all these underscores and hungarian syntax doing

public class DestroyThis : MonoBehaviour{

	public GameObject optionalParent;

	// has this had life set on it yet?
	private bool m_bSet = false;

	private bool IsSet(){
		return m_bSet;
	}
	
	// the life of whatever this is
	private float m_fLife = 0;
	public float m_fStartLife = 0;

	public void SetLife(float i_float){
		if(IsSet()){
			Debug.LogError("Life already set on DestroyThis...not intended.");
			return;
		}
		
		m_fLife = i_float;
		m_bSet = true;
	}
	
	void Start(){
		// shortcut -- if the start life is negative, it means just destroy this object right away.
		// used as a way to keep things from getting into the build that aren't totally ready
		if(m_fStartLife < 0){
			Destroy(gameObject);
			return;
		}
		
		// life may be set on the script itself
		if(!IsSet() && m_fStartLife > 0)
			SetLife(m_fStartLife);
	}
	
	void Update(){
		// if combat isn't playing, we don't want to do any updating
		//if ( CombatManager.Exists() && CombatManager.instance.GetCombatState() != CombatStates.PLAYING ) 
		//	return;	
	
		// if life has been set, let the countdown begin
		if(IsSet()){
			float fDelta = Time.deltaTime;
			
			m_fLife -= fDelta;
			
			if(m_fLife <= 0){
				if(optionalParent != null){
					Destroy(optionalParent);
				}
				else{
					Destroy(gameObject);
				}
			}
		}
	}
}
