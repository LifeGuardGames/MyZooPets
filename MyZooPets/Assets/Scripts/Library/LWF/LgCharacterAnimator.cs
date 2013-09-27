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
	
	public override void PlayClip(string _clipName) {
		// a final failsafe; before we play a clip, if there is a clip currently playing and it is uninterruptable, bail out.
		// show a debug message because this should not be happening...the responsibility of the caller is to check first.
		if ( IsBusy() ) {
			Debug.Log("Trying to play clip " + _clipName + " but " + clipCurrent.clipName + " is not interruptable.  Check first.");
			return;
		}
		
		//Debug.Log("Playing clip: " + _clipName);
		
		// stop all other movies
		DetachAllMovies();	
		
		base.PlayClip( _clipName );
	}
	
	public override void PlayClip(string _clipName, bool _stopAllPrevMovies){
		Debug.Log("LgAnimator does not support this kind of clip playing.");
	}
}
