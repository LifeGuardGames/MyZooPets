using UnityEngine;
using System.Collections;

///////////////////////////////////////////
// DestroyThis
// Simple script for destroying things on
// a timer; this should be used for
// everything so that pausing can be an
// easy and universal task.
///////////////////////////////////////////

public class DestroyThis : MonoBehaviour {

	// has this had life set on it yet?
	private bool m_bSet = false;
	private bool IsSet() {
		return m_bSet;
	}
	
	// the life of whatever this is
	private float m_fLife = 0;
	public float m_fStartLife = 0;
	public void SetLife( float i_float ) {
		if ( IsSet() )
		{
			Debug.Log("Life already set on DestroyThis...not intended.");
			return;
		}
		
		m_fLife = i_float;
		m_bSet = true;
	}
	
	void Start() {
		// life may be set on the script itself
		if ( !IsSet() && m_fStartLife > 0 )
			SetLife( m_fStartLife );
	}
	
	void Update() {
		// if combat isn't playing, we don't want to do any updating
		//if ( CombatManager.Exists() && CombatManager.instance.GetCombatState() != CombatStates.PLAYING ) 
		//	return;	
	
		// if life has been set, let the countdown begin
		if ( IsSet() )
		{
			float fDelta = Time.deltaTime;
			
			m_fLife -= fDelta;
			
			if ( m_fLife <= 0 )
				Destroy(gameObject);
		}
	}
}
