using UnityEngine;
using System.Collections;

///////////////////////////////////////////
// LgAudioSource
// Obfuscating playing audio a little bit,
// so that audio can listen to and respond
// to messages.
///////////////////////////////////////////

public class LgAudioSource : MonoBehaviour {

	private AudioSource audioSource;
	
	public void Init( AudioClip sound, Transform tf, float fVolume ) {
		//Messenger.instance.Listen( LibraryMessageTypes.PAUSE, this );	// listen for combat being paused
		
		// create the audio source	
		audioSource = gameObject.AddComponent<AudioSource>(); 
		audioSource.clip = sound; 
		audioSource.volume = fVolume; 
		audioSource.pitch = 1.0f;
		gameObject.transform.parent = tf;
		gameObject.transform.position = tf.position;
		audioSource.Play();
		
		// add destroy script
		DestroyThis scriptDestroy = gameObject.AddComponent<DestroyThis>();
		scriptDestroy.SetLife( sound.length + 0.1f );
		
		//StartCoroutine( FadeOut() );	
	}
	
	///////////////////////////////////////////
	// FadeOut()
	// Fades out this sound over fTime.
	///////////////////////////////////////////	
	public IEnumerator FadeOut( float fTime ) {
		for (int i = 9; i >= 0; i--) {
			audioSource.volume = i * .1f;
			yield return new WaitForSeconds(fTime / 10);
		}			
		
		//Debug.Log(audioSource.name + " should be completely faded out now");
	}
	
	// TO DO: implement pause functionality
	/*
	///////////////////////////////////////////
	// _SetPauseState()
	// If the game is pausing or unpausing.
	///////////////////////////////////////////
	function _SetPauseState( i_msg:MessageSetPauseState )
	{	
		var eState:PauseStates = i_msg.GetState();	

		if ( eState == PauseStates.ON )
			m_audioSource.Pause();
		else
			m_audioSource.Play();
	}
	*/
}
