using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataPetAnimation
// Individual piece of pet anim data as loaded from xml.
// Considered to be immutable.
//---------------------------------------------------

public class DataPetAnimation {
	// id for the animation
	private string strID;
	public string GetID() {
		return strID;	
	}
	
	// clip name of the animation; used to play the LWF
	private string strClipName;
	public string GetClipName() {
		return strClipName;	
	}
	
	// can this clip be interrupted?
	private bool bCanInterrupt;
	public bool CanInterrupt() {
		return bCanInterrupt;	
	}
	
	// the animation state that this pet is put into when the animation plays
	private PetAnimStates eAnimState;
	public PetAnimStates GetAnimState() {
		return eAnimState;	
	}
	
	// weight of the animation; used when randomly picking an animation
	private int nWeight;
	public int GetWeight() {
		return nWeight;	
	}
	
	// mood associated with this animation
	private PetMoods eMood;
	public PetMoods GetMood() {
		return eMood;	
	}
	
	// health associated with this animation
	private PetHealthStates eHealthState;
	public PetHealthStates GetHealth() {
		return eHealthState;	
	}
	
	// categories this animation belongs to
	private List<string> listCategories = new List<string>();
	public List<string> GetCategories() {
		return listCategories;	
	}

	public DataPetAnimation( string id, Hashtable hashAttr, Hashtable hashData, string strError ) {
		// set id
		strID = id;
		
		// get the animation state that this animation reflects
		string strState = HashUtils.GetHashValue<string>( hashAttr, "state", "Idling", strError );
		eAnimState = (PetAnimStates) System.Enum.Parse( typeof( PetAnimStates ), strState );
		
		// get the clip name
		strClipName = XMLUtils.GetString(hashData["Clip"] as IXMLNode, id);
		
		// get whether or not this animation can be interrupted
		bCanInterrupt = XMLUtils.GetBool(hashData["CanInterrupt"] as IXMLNode, true);
		
		// get the weight
		nWeight = XMLUtils.GetInt(hashData["Weight"] as IXMLNode, 1, strError);
		
		// get the health state
		string strHealth = XMLUtils.GetString( hashData["Health"] as IXMLNode, "Healthy", strError );
		eHealthState = (PetHealthStates) System.Enum.Parse( typeof( PetHealthStates ), strHealth );			
		
		// get the mood
		// NOTE if the health for this anim is NOT set to healthy, the mood should be "Any"
		if ( eHealthState != PetHealthStates.Healthy )
			eMood = PetMoods.Any;
		else {
			string strMood = XMLUtils.GetString( hashData["Mood"] as IXMLNode, "Happy", strError );
			eMood = (PetMoods) System.Enum.Parse( typeof( PetMoods ), strMood );
		}
		
		//Debug.Log("Loading pet anim data: " + strID + " - " + eAnimState + " - " + strClipName + " - " + nWeight + " - " + eMood + " - " + eHealthState);
		
		// get the list of categories
		string strCats = XMLUtils.GetString( hashData["Categories"] as IXMLNode, "Idle", strError );
		string[] arrayCats = strCats.Split( ","[0] );
		for ( int i = 0; i < arrayCats.Length; ++i )
			listCategories.Add( arrayCats[i] );
	}
}
