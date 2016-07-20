using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComicPlayer : MonoBehaviour{
    public static EventHandler<EventArgs> OnComicPlayerDone; 	//event fire when comic player played through all pages
	public string pagePrefabPrefix = "IntroComicPage";			//name of the prefab of the comic pages
	public List<GameObject> pages = new List<GameObject>(); 
	public ComicPageTransition transition;

    private int currentPageNum;
    private GameObject currentComicPage; //keeps track of the spawned comic page

	void Start(){
		currentPageNum = 0;
	}

	public void Init(string petColor){
		// Hide all and dont play
		for(int i = 0; i < pages.Count; i++){
			ComicPage page = pages[i].GetComponent<ComicPage>();
			// Set the pet color
			page.Init(petColor);

			page.ToggleActive(false);
		}

		// Call on next frame, wait for tween toggle to init!!!
		StartCoroutine(StartComic());
	}

	public IEnumerator StartComic(){
		yield return 0;
		currentPageNum = 0;
		StartTransitionAndCallNextPage();
	}

	public void StartTransitionAndCallNextPage(){
		bool isLastPage = currentPageNum == pages.Count ? true : false;

		// Call finish comic directly instead of waiting for callback from tween
		if(isLastPage){
			transition.Darken(true);
			NextPage();
		}
		// Regular transition with callback on finish tween
		else{
			transition.Darken(false);
		}
	}
	
    // Setup the callback function name
    public void NextPage(){
		if(currentPageNum < pages.Count){
			// Turn off all the other pages that isnt the current page
			for(int i = 0; i < pages.Count; i++){
				pages[i].GetComponent<ComicPage>().ToggleActive(i == (currentPageNum) ? true : false);
			}
			currentPageNum++;
		}
		else{
			if(OnComicPlayerDone != null){
				EventArgs args = new EventArgs();
				OnComicPlayerDone(this, args);
			}
		}
    }
}
