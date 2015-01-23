using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComicPlayer : MonoBehaviour{
    public static EventHandler<EventArgs> OnComicPlayerDone; //event fire when comic player played through all pages
	public List<GameObject> pages = new List<GameObject>(); //name of the prefab of the comic pages
	public string pagePrefabPrefix = "ComicPage";
	public ComicPageTransition transition;

    private int currentPageNum;
    private int totalNumOfPages = 3;
    private GameObject currentComicPage; //keeps track of the spawned comic page

	void Start(){
		currentPageNum = 0;
	}

	public void StartComic(){
		currentPageNum = 0;
		StartTransitionAndCallNextPage();
	}

	public void StartTransitionAndCallNextPage(){
		transition.Darken();
	}

    //---------------------------------------------------
    // NextPage()
    // Setup the callback function name.
    //---------------------------------------------------
    public void NextPage(){
		currentPageNum++;
		// First page
		if(currentPageNum == 1){
            LoadComicPages();
        }
        else if(currentPageNum <= totalNumOfPages){
			// Turn off all the other pages that isnt the current page
			pages[0].GetComponent<ComicPage>().ToggleActive(false);
			for(int i = 1; i < totalNumOfPages; i++){
				Debug.Log("Loading page " + i);
				pages[i].GetComponent<ComicPage>().ToggleActive(i == (currentPageNum - 1) ? true : false);
			}
		}
		else{
			Debug.Log("DONE");
		}
    }

    /// <summary>
    /// Load comic pages into pages list, and make sure only the first one shows
    /// </summary>
    private void LoadComicPages(){
        string petSpecies = DataManager.Instance.GameData.PetInfo.PetSpecies;
        string petColor = DataManager.Instance.GameData.PetInfo.PetColor;

		for(int i = 1; i <= totalNumOfPages; i++){
			Debug.Log("Loading " + pagePrefabPrefix + i.ToString());
			GameObject pagePrefab = Resources.Load(pagePrefabPrefix + i.ToString()) as GameObject;
			GameObject pageGO = GameObjectUtils.AddChild(gameObject, pagePrefab);
			ComicPage pageScript = pageGO.GetComponent<ComicPage>();
			pageScript.Init(petColor, this);

			// Turn off everything but the first page
			if(i == 1){
				pageScript.ToggleActive(true);	// Takes care of play as well
			}
			else{
				pageScript.ToggleActive(false);	// Hide all and dont play
			}

			pages.Add(pageGO);
		}
    }

	void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 100, 100), "Play")){
			StartComic();
		}
	}
}
