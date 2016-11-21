using UnityEngine;
using System.Collections;

public class PitchTest : LgWorldButton {

	public float pitchStart;
	public float pitchChange;
	public AudioClip sound;
	private int count = 0;
	
	protected override void ProcessClick() {
		float pitch = pitchStart - ( pitchChange * count );
		
		
		// create the audio source	
		AudioSource audioSource = gameObject.AddComponent<AudioSource>(); 
		audioSource.clip = sound; 
		audioSource.volume = 1.0f; 
		audioSource.pitch = pitch;
		gameObject.transform.parent = transform;
		gameObject.transform.position = transform.position;
		audioSource.Play();
		
		// add destroy script
		//DestroyThis scriptDestroy = gameObject.AddComponent<DestroyThis>();
		//scriptDestroy.SetLife( sound.length + 0.1f );	
		count++;
		
		Debug.Log("Playing at pitch " + pitch + "(" + sound.length + ")");
	}
}
