using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DGTZone
// A zone that characters travel to in the diagnose
// game (tracks).  Each zone represents a stage of
// asthma on the chart.
//---------------------------------------------------

public class DGTZone : LgButton {

	// what stage of asthma is this zone for?
	public AsthmaStage eStage;
	public AsthmaStage GetStage() {
		return eStage;	
	}
	
	//---------------------------------------------------
	// ProcessClick()
	//---------------------------------------------------	
	protected override void ProcessClick() {
		// if the tutorial is going on, we may not be able to select this zone
		if ( DGTManager.Instance.IsZoneLocked( GetStage() ) )
			return;
		
		// let the manager know the player has selected a new zone
		DGTManager.Instance.SetSelectedZone( gameObject );
		
		AnimationControl anim = GetComponent<AnimationControl>();
		if(anim != null){
			anim.Stop();
			anim.Play();
		}
	}	
}
