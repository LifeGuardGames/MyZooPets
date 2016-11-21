using UnityEngine;

/// <summary>
/// Comic page transition.
/// Used for callback transitioning between comic pages, fades in and out
/// </summary>
public class ComicPageTransition : MonoBehaviour {
	public AlphaTweenToggle alphaTween;
	public ComicPlayer comicPlayer;

	public void StartTransition(){
		Darken(false);
	}

	public void Darken(bool isLastPage){
		// Remove the callback is it is the last page
		if(isLastPage){
			alphaTween.ShowTarget = null;
			alphaTween.ShowFunctionName = null;
		}
		alphaTween.Show();
	}

	public void BrightenAndNextPage(){
		alphaTween.Hide();
		comicPlayer.NextPage();
	}
}
