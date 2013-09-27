﻿using UnityEngine;
using System.Collections;

//---------------------------------------------------
// PetAnimator
// Script that controls all the pet's animations.
//---------------------------------------------------

public class PetAnimator : LgCharacterAnimator {
	
	// key of the pet's "species" -- i.e. what kind of pet it is
	// this will eventually be set in save data probably
	public string strKeySpecies;
	public string GetSpeciesKey() {
		return strKeySpecies;	
	}
	
	// key of the pet's color
	// this will eventually be set in save data probably
	public string strKeyColor;
	public string GetColorKey() {
		return strKeyColor;	
	}
	
	void Start() {
		// set the LWFAnimator loading data based on the pet's attributes
		string strSpecies = GetSpeciesKey();
		string strColor = GetColorKey();
		animName = strSpecies + strColor;
		folderPath = animName + "/";
		
		// only call this AFTER we have set our loading data
		base.Start();	
		
		// the animator starts off empty, so immediately pick the animation we want to play
		PlayClip( "happyIdle" );
	}
}
