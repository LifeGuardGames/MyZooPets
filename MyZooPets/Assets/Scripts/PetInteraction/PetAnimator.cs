using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
	
	// queue of animations the pet should be doing
	private Queue<DataPetAnimation> queueAnims = new Queue<DataPetAnimation>();
	
	// pet's animation state; used to decide what to do after an animation finishes
	private PetAnimStates eAnimState;
	private void SetAnimState( PetAnimStates eState ) {
		eAnimState = eState;	
	}
	public PetAnimStates GetAnimState() {
		return eAnimState;
	}
	
	// just for testing and seeing what anim is play
	private bool bTesting = false;
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	void Start() {
		// load up our animation data from xml
		DataLoaderPetAnimations.SetupData();
		
		// then go through all the anim data and create out clip list from that
		SetClipList();
		
		// set the LWFAnimator loading data based on the pet's attributes
		string strSpecies = GetSpeciesKey();
		string strColor = GetColorKey();
		animName = strSpecies + strColor;
		folderPath = "LWF/" + animName + "/";
		
		// only call this AFTER we have set our loading data
		base.Start();	
		
		// the animator starts off empty, so immediately pick the animation we want to play (idle)
		Idle();
		
		// testing
		/*
		DataPetAnimation data = DataLoaderPetAnimations.GetRestrictedData( "Idle" );
		data = DataLoaderPetAnimations.GetRestrictedData( "Idle" );
		data = DataLoaderPetAnimations.GetRestrictedData( "Idle" );
		data = DataLoaderPetAnimations.GetRestrictedData( "Idle" );
		data = DataLoaderPetAnimations.GetRestrictedData( "Walk" );
		*/
	}
	
	//---------------------------------------------------
	// SetClipList()
	// Sets the clip list for the LWF animator based on
	// the animation xml data.
	//---------------------------------------------------	
	private void SetClipList() {
		// clear the one we have set up through the inspector
		clips = new List<FlashMovieClip>();
		
		// get all the animations
		Hashtable hashData = DataLoaderPetAnimations.GetAllData();
		
		foreach ( DictionaryEntry entry in hashData ) {
			DataPetAnimation dataAnim = (DataPetAnimation) entry.Value;
			clips.Add( new FlashMovieClip( dataAnim ) );
		}
	}
	
	//---------------------------------------------------
	// Transition()
	//---------------------------------------------------	
	public void Transition( string strTransitionType ) {
		// get a random transition based on the type
		DataPetAnimation dataAnim = DataLoaderPetAnimations.GetRestrictedData( strTransitionType );
		
		// queue the anim
		QueueAnim( dataAnim );	
	}
	
	//---------------------------------------------------
	// QueueAnim()
	//---------------------------------------------------	
	private void QueueAnim( DataPetAnimation dataAnim ) {
		// don't queue if the anim is null
		if ( dataAnim != null )
			queueAnims.Enqueue( dataAnim );			
	}

	//---------------------------------------------------
	// Idle()
	//---------------------------------------------------
	private void Idle() {
		// if the pet's animation queue is not empty, we should not kick off an idle
		if ( queueAnims.Count > 0 )
			return;
		
		// get a random idle based on pet's attributes
		DataPetAnimation dataAnim = DataLoaderPetAnimations.GetRestrictedData( "Idle" );
		
		// queue the anim
		QueueAnim( dataAnim );
	}
	
	//---------------------------------------------------
	// StartMoving()
	//---------------------------------------------------		
	public void StartMoving() {
		// get a random walk based on pet's attributes
		DataPetAnimation dataAnim = DataLoaderPetAnimations.GetRestrictedData( "Walk" );
		
		// then start playing the anim immediately
		PlayAnimation( dataAnim );
	}
	
	//---------------------------------------------------
	// StopMoving()
	//---------------------------------------------------		
	public void StopMoving() {
		// we have to do this ourselves here because walking is on a loop...kind of annoying, but so is LWF
		SetAnimating( false );
		
		// now that we're done moving, idle
		Idle();
	}
	
	//---------------------------------------------------
	// PlayAnimation()
	// Plays the incoming animation.
	//---------------------------------------------------	
	private void PlayAnimation( DataPetAnimation dataAnim ) {
		// there is a slim possibility that the data was set up wrong and the incoming animation is null
		if ( dataAnim == null ) {
			Debug.Log("Trying to play a null animation on the pet!");
			return;
		}
		
		// change the pet's state if appropriate
		PetAnimStates eNewState = dataAnim.GetAnimState();
		PetAnimStates eOldState = GetAnimState();
		if ( eOldState != eNewState )
			SetAnimState( eNewState );		
		
		// get the clip name and play it!
		string strClip = dataAnim.GetClipName();
		
		if ( bTesting )
			Debug.Log("Playing clip " + strClip);	
		
		PlayClip( strClip );		
	}
	
	//---------------------------------------------------
	// Update()
	//---------------------------------------------------		
	void Update() {
		// call base update so that LWF can do it's thing
		base.Update();
		
#if UNITY_EDITOR
    	CheckForAnimTest();
#endif 		
		
		// if we have a queued animation, play it
		if ( !IsAnimating() && queueAnims.Count > 0 ) {
			DataPetAnimation dataAnim = queueAnims.Dequeue();
						
			PlayAnimation( dataAnim );
			
			// put the anim system back to normal if we are done testing
			if ( queueAnims.Count == 0 && bTesting )
				bTesting = false;
		}
	}
	
	//---------------------------------------------------
	// CheckForAnimTest()
	//---------------------------------------------------		
	private void CheckForAnimTest() {
        if( Input.GetKeyDown( KeyCode.Space ) ) {
			// just queue up every anim
			Hashtable hashData = DataLoaderPetAnimations.GetAllData();
			
			foreach ( DictionaryEntry entry in hashData ) {
				DataPetAnimation dataAnim = (DataPetAnimation) entry.Value;
				QueueAnim( dataAnim );
			}			
			bTesting = true;
		}
	}
	
	//---------------------------------------------------
	// _ClipFinished()
	//---------------------------------------------------		
	protected override void _ClipFinished() {
		PetAnimStates eState = GetAnimState();
		
		// don't do anything if we are in the middle of testing the anims
		if ( bTesting )
			return;
		
		switch ( eState ) {
			case PetAnimStates.Idling:
				Idle();
				break;				
			case PetAnimStates.Walking:
				// do nothing; the anim will loop
				// but we actually have to mark that we are still animating, because parent object set it to false
				SetAnimating( true );
				break;
			case PetAnimStates.Transitioning:
				Idle();
				break;
			default:
				Debug.Log("Unhandled pet anim state finishing: " + eState);
				Idle();
				break;
		}
	}
}
