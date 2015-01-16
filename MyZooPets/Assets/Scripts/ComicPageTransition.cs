using UnityEngine;
using System.Collections;

/// <summary>
/// Comic page transition.
/// Used for callback transitioning between comic pages, fades in and out
/// </summary>
public class ComicPageTransition : MonoBehaviour {
	public SpriteRenderer transitionSprite;
	private AlphaTweenToggle alphaTween;

	void Start(){
		// Make sure we have a valid sprite
		if(transitionSprite == null){
			transitionSprite = GetComponent<SpriteRenderer>();
			if(transitionSprite == null){
				Debug.LogError("Cant find any components");
			}
		}

		alphaTween = transitionSprite.GetComponent<AlphaTweenToggle>();


	}
}
