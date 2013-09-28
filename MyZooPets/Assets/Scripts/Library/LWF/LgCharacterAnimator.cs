using UnityEngine;
using System.Collections;

//---------------------------------------------------
// LgAnimator
//---------------------------------------------------

public class LgCharacterAnimator : LWFAnimator {
	//---------------------------------------------------
	// IsBusy()
	// Is this character busy?  i.e. is it playing an
	// un-interruptable clip.
	//---------------------------------------------------	
	public bool IsBusy() {
		bool bBusy = false;
		
		if ( clipCurrent != null )
			bBusy = !clipCurrent.bCanInterrupt;
		
		return bBusy;
	}
	
	//---------------------------------------------------
	// PlayClip()
	// I highly recommend NOT calling this from the
	// animation callback or else you will run into
	// the tree removal issues.
	//---------------------------------------------------	
	public override void PlayClip(string _clipName) {
		// if the character is already playing the incoming animation, return -- otherwise we get blinking issues
		if ( clipCurrent != null && clipCurrent.clipName == _clipName )
			return;
		
		// a final failsafe; before we play a clip, if there is a clip currently playing and it is uninterruptable, bail out.
		// show a debug message because this should not be happening...the responsibility of the caller is to check first.
		if ( IsBusy() ) {
			Debug.Log("Trying to play clip " + _clipName + " but " + clipCurrent.clipName + " is not interruptable.  Check first.");
			return;
		}
		
		// characters should only be playing one movie at a time
		DetachAllMovies();		
		
		base.PlayClip( _clipName );
	}
	
	//---------------------------------------------------
	// PlayClip()
	//---------------------------------------------------		
	public override void PlayClip(string _clipName, bool _stopAllPrevMovies){
		Debug.Log("LgAnimator does not support this kind of clip playing.");
	}
	
	//---------------------------------------------------
	// Flip()
	// For when the character needs to change directions.
	// Assumes that the parent object of this LWF animator
	// is the highest level object for the character.
	//---------------------------------------------------		
	public void Flip( bool bFlip ) {
		if ( bFlip )
			transform.parent.localScale = new Vector3(-1f, 1f, 1f);	
		else
			transform.parent.localScale = new Vector3(1f, 1f, 1f);	
	}	
}
