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
	
	// the animation state that this pet is put into when the animation plays
	private PetAnimStates eAnimState;
	
	// weight of the animation; used when randomly picking an animation
	private float fWeight;
	
	// mood associated with this animation
	private PetMoods eMood;
	
	// health associated with this animation
	private PetHealthStates eHealthState;
	
	// categories this animation belongs to
	private List<string> listCategories;

	public DataPetAnimation( string id, Hashtable hashAttr, Hashtable hashData ) {
		// set id
		strID = id;
		
		// get the animation state that this animation reflects
		
	}
}
