using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PetAnimArgs : EventArgs{
	private PetAnimStates eState;
	public PetAnimStates GetAnimState() {
		return eState;	
	}

	public PetAnimArgs( PetAnimStates eState){
		this.eState = eState;
	}
}

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
	
	// related to fire blowing
	public GameObject goBlow;		// where to parent the fire particle
	private GameObject goFire;		// actual fire particle game object
	private FireBlowParticleController scriptFire;
	 
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
	
	//=======================Events========================
	public static EventHandler<PetAnimArgs> OnAnimDone; 	// when the pet finishes an anim
	public static EventHandler<EventArgs> OnBreathStarted;	// when pet starts to breath fire
	//=====================================================		

	//---------------------------------------------------
	// Start()
	//---------------------------------------------------	
	new void Start() {
		// load up our animation data from xml
		DataLoaderPetAnimations.SetupData();
		
		// then go through all the anim data and create out clip list from that
		SetClipList();
		
		// set the LWFAnimator loading data based on the pet's attributes
		string strSpecies = DataManager.Instance.GameData.PetInfo.PetSpecies; 

		// string strColor = GetColorKey();
		string strColor = DataManager.Instance.GameData.PetInfo.PetColor;

		animName = strSpecies + strColor;
		folderPath = "LWF/" + animName + "/";
		
		// only call this AFTER we have set our loading data
		base.Start();	
		
		// the animator starts off empty, so immediately pick the animation we want to play (idle)
		Idle();
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
	// PlayUnrestrictedAnim()
	// Play animation in a category without consideration 
	// of the pet mood or health stats
	//---------------------------------------------------
	public void PlayUnrestrictedAnim(string strCat, bool bImmediate = false){
		DataPetAnimation dataAnim = DataLoaderPetAnimations.GetUnrestrictedData(strCat);

		if(bImmediate)	
			 PlayAnimation(dataAnim);
		else
			QueueAnim(dataAnim);
	}

	//---------------------------------------------------
	// PlayRestrictedAnim
	// Play anim based on pet's attributes
	//---------------------------------------------------
	public void PlayRestrictedAnim(string strCat, bool bImmediate = false){
		// get anim based on pet's attributes
		DataPetAnimation dataAnim = DataLoaderPetAnimations.GetRestrictedData(strCat);
	
		if(bImmediate)	
			PlayAnimation(dataAnim);
		else
			QueueAnim(dataAnim);
	}
	
	//---------------------------------------------------
	// BreathFire()
	// The player is attacking a gate!
	//---------------------------------------------------	
	public void BreathFire() {
		PlayRestrictedAnim("Fire", true);
		
		// spawn the particle effect
		Skill curSkill = FlameLevelLogic.Instance.GetCurrentSkill();
		string strResource = curSkill.FlameResource;
		GameObject resource = Resources.Load( strResource ) as GameObject;
		goFire = Instantiate( resource, new Vector3(0,0,0), resource.transform.rotation ) as GameObject;
		
		// parent it to the right position
		goFire.transform.parent = goBlow.transform;				
		goFire.transform.localPosition = new Vector3(0,0,0);
		
		// actually kick off the effect
		scriptFire = goFire.GetComponent<FireBlowParticleController>();
		
		// start a coroutine to pause the animation, for timing purposes.
		// it is resumed from the FireMeter script.
		float fFireWait = Constants.GetConstant<float>( "FireInhale" );
		StartCoroutine( PauseAnim( fFireWait ) );
	}
	
	//---------------------------------------------------
	// PauseAnim()
	// A helper function that basically just pauses the
	// current animation after a short duration.  Used
	// for timing purposes.
	//---------------------------------------------------		
	private IEnumerator PauseAnim( float fWait ) {
		yield return new WaitForSeconds( fWait );
		Pause();
	}
	
	//---------------------------------------------------
	// FinishFire()
	// The pet is stopped mid-animation when breathing
	// fire.  This function will finish the animation.
	//---------------------------------------------------		
	public IEnumerator FinishFire() {
		// resume the animation
		Resume();
		
		// process any callbacks for when the pet starts to breath fire
		if ( OnBreathStarted != null )
			OnBreathStarted( this, EventArgs.Empty );
		
		// the pet is actually breathing fire, so play the fire breathing sound
		LgAudioSource audioFire = AudioManager.Instance.PlayClip( "petFire" );
		
		// pause it again once the pet begins to exhale
		float fUntilExhale = Constants.GetConstant<float>( "UntilExhale" );
		yield return new WaitForSeconds( fUntilExhale );
		
		// the pet is now exhaling, so pause
		Pause();
		
		// play the actual particle effect
		float fFireWait = Constants.GetConstant<float>( "FireInhale" );
		float fFireDelay = Constants.GetConstant<float>( "FireDelay" );
		StartCoroutine( scriptFire.PlayAfterDelay( fFireDelay - fFireWait - fUntilExhale ) );		
		
		// then wait another amount of time
		float fHoldExhale = Constants.GetConstant<float>( "HoldExhale" );
		yield return new WaitForSeconds( fHoldExhale );
		
		// done holding exhale
		Resume();
		
		// stop the game object of the fire
		FireBlowParticleController script = goFire.GetComponent<FireBlowParticleController>();
		script.Stop();		
		
		// fire breathing portion of the animation is over, so fade out the sound
		float fFade = Constants.GetConstant<float>( "FireSoundFadeTime" );
		StartCoroutine( audioFire.FadeOut( fFade ) );
	}
	
	//---------------------------------------------------
	// DoneBreathingFire()
	//---------------------------------------------------	
	public void DoneBreathingFire( bool bFinished ) {
		if ( !bFinished )
			Resume();
		else{
			//If the smoke monster has been defeated play celebrate animation, else
			//go back to idle
			if(!GatingManager.Instance.IsInGatedRoom()){
				PlayRestrictedAnim("Celebrate");

			}
			else
				Idle( !bFinished );
		}	
	}
	
	//---------------------------------------------------
	// CancelFire()
	// Cancels the whole fire breathing animation.
	//---------------------------------------------------		
	public void CancelFire() {
		DoneBreathingFire( false );
	}

	//---------------------------------------------------
	// Idle()
	//---------------------------------------------------
	private void Idle( bool bImmediate = false ) {
		// if the pet's animation queue is not empty, we should not kick off an idle
		if ( queueAnims.Count > 0 )
			return;
		
		// get a random idle based on pet's attributes
		DataPetAnimation dataAnim = DataLoaderPetAnimations.GetRestrictedData( "Idle" );
		
		if ( bImmediate )
			PlayAnimation( dataAnim );
		else {
			// queue the anim
			QueueAnim( dataAnim );
		}
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
		
		// I hate to do this here, but there seems to be a bug where the walk animation is finishing precisely as the pet is
		// supposed to stop moving, and so the pet enters an endless walk-cycle.  This is definitely hacky.
		SetAnimState( PetAnimStates.Idling );
		
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
	new void Update() {
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
			int counter = 0;
			foreach ( DictionaryEntry entry in hashData ) {
				print(counter++);
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
			case PetAnimStates.BreathingFire:
				DoneBreathingFire( true );
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
		
		// send out a message to anyone listening that the anim is done
		if ( OnAnimDone != null )
			OnAnimDone( this, new PetAnimArgs(eState) );		
	}
}
