using UnityEngine;
using System.Collections;

/// <summary>
/// Takes a particle and plays the animation in a list of different locations set by a delay time in between
/// </summary>
public class ParticleLocationBurstList : MonoBehaviour {
	public ParticleSystem particleSystem;
	public float delayBetween;
	public Transform[] locationList;

	public void Play(){
		StartCoroutine(PlayHelper());
	}

	IEnumerator PlayHelper(){
		foreach(Transform location in locationList){
			particleSystem.transform.localPosition = location.transform.localPosition;
			particleSystem.Play();
			yield return new WaitForSeconds(delayBetween);
		}
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "start")){
//			Play();
//		}
//	}
}
