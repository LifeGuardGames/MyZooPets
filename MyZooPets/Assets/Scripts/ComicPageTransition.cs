using UnityEngine;
using System.Collections;

/// <summary>
/// Comic page transition.
/// Used for callback transitioning between comic pages, fades in and out
/// </summary>
public class ComicPageTransition : MonoBehaviour {
	public AlphaTweenToggle alphaTween;
	public ComicPlayer comicPlayer;

	public void StartTransition(){
		Darken();
	}

	public void Darken(){
		alphaTween.Show();
	}

	public void BrightenAndNextPage(){
		comicPlayer.NextPage();
		alphaTween.Hide();
	}
}
