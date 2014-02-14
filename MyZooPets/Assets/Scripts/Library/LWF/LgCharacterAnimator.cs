using UnityEngine;
using System.Collections;

//---------------------------------------------------
// LgAnimator
//---------------------------------------------------

public class LgCharacterAnimator : LWFAnimator {
	// I don't trust the LWFAnimatoro OR LWFObject, so I'm creating a custom way of knowing if an animation is finished or not
	private bool bAnimating = false;
	public bool IsAnimating() {
		return bAnimating;	
	}

	protected void SetAnimating( bool b ) {
		bAnimating = b;	
	}

    // key that tells where the animation is used -- i.e runner, bedroom, trigger ninja
	public string animType;
	
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
		
		// -- Note by Joe
		// I decided to bail out on the below code because of the complexity it would introduce.  It's now up to the scripts
		// to do their own checking and controlling of this.
		
		// a final failsafe; before we play a clip, if there is a clip currently playing and it is uninterruptable, bail out.
		// show a debug message because this should not be happening...the responsibility of the caller is to check first.
		//if ( IsBusy() ) {
		//	Debug.Log("Trying to play clip " + _clipName + " but " + clipCurrent.clipName + " is not interruptable.  Check first.");
		//	return;
		//}
		
		// characters should only be playing one movie at a time
		DetachAllMovies();		
		
		// the character is now animating
		SetAnimating( true );
		
		base.PlayClip( _clipName );
	}
	
	//---------------------------------------------------
	// PlayClip()
	//---------------------------------------------------		
	public override void PlayClip(string _clipName, bool _stopAllPrevMovies){
		Debug.LogError("LgAnimator does not support this kind of clip playing.");
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


	//---------------------------------------------------		
	// Start()
	// Override start method to set the path of the animation
	// according to the pet species and pet color name
	//---------------------------------------------------		
	protected override void Start(){
		// set the LWFAnimator loading data based on the pet's attributes
		string strSpecies = DataManager.Instance.GameData.PetInfo.PetSpecies; 

		// string strColor = GetColorKey();
		string strColor = DataManager.Instance.GameData.PetInfo.PetColor;

		animName = strSpecies + strColor;
		folderPath = "LWF/" + animType + "/" + animName + "/";

		base.Start();
	}
	
	//---------------------------------------------------
	// ClipFinished()
	//---------------------------------------------------		
	protected override void ClipFinished() {
		// the clip is finished; we are done animating
		SetAnimating( false );
		
		// have children do their thing
		_ClipFinished();
	}	
	
	protected virtual void _ClipFinished() {
		// child should implement this
	}
}
