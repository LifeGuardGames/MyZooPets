using UnityEngine;
using System.Collections;

//---------------------------------------------------
// NinjaTriggerTarget
// This is like a piece of fruit from Fruit Ninja;
// it is a positive object that the player wants to
// destroy.
//---------------------------------------------------	

public class NinjaTriggerTarget : NinjaTrigger {
	// how much is this trigger worth when the player cuts it?
	public int nPoints;
	public int GetPointValue() {
		return nPoints;	
	}
	
	// renderer for cockroach face
	public Renderer rendererFace;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	protected override void Start() {
		base.Start();	
		
		// pick a face for this roach
		int nFaces = Constants.GetConstant<int> ("Ninja_NumFaces" );
		string strFaceKey = Constants.GetConstant<string> ("Ninja_FaceKey" );
		int nFace = Random.Range( 1, nFaces ); // faces index starts at 1, so get 1-max inclusive
		string strFace = strFaceKey + nFace;
		SetFace( strFace );
	}
	
	//---------------------------------------------------
	// SetFace()
	// Sets this roach's face to the incoming string
	// referenced material.
	//---------------------------------------------------	
	private void SetFace( string strFace ) {
		Material matPrefab = Resources.Load(strFace) as Material;
		
		if ( matPrefab != null )
			rendererFace.material = matPrefab;	
		else
			Debug.LogError( "Attempting to set cockroach face to non-existant material with face " + strFace );
	}
	
	//---------------------------------------------------
	// _OnCut()
	//---------------------------------------------------		
	protected override void _OnCut() {
		// award points
		int nVal = GetPointValue();
		NinjaManager.Instance.UpdateScore( nVal );	
		
		// increase the player's combo
		NinjaManager.Instance.IncreaseCombo( 1 );
		
		// then launch the trigger into the air
		
		// get proper forces
		int nForceRangeX = Constants.GetConstant<int>("CutForceRangeX");
		int nForceX = UnityEngine.Random.Range( -nForceRangeX, nForceRangeX );
		int nForceY = Constants.GetConstant<int>("CutForceY");
		Vector3 vForce = new Vector3( nForceX, nForceY, 0 );
		
		// apply said force
		gameObject.rigidbody.AddForce( vForce );
		
		// set the cockroach's face to dead
		string strFaceKey = Constants.GetConstant<string>( "Ninja_FaceKey" );
		SetFace( strFaceKey + "Dead" );	
	}
	
	//---------------------------------------------------
	// _OnMissed()
	//---------------------------------------------------	
	protected override void _OnMissed() {
		// the player loses a life
		NinjaManager.Instance.UpdateLives( -1 );	
	}	
}
