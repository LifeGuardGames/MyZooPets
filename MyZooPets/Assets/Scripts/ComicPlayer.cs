using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComicPlayer : MonoBehaviour{
    public static EventHandler<EventArgs> OnComicPlayerDone; //event fire when comic player played through all pages
	public List<GameObject> pages = new List<GameObject>(); //name of the prefab of the comic pages

	public string pagePrefabPrefix = "ComicPage";

    private int currentPageNum;
    private int totalNumOfPages = 2;
    private GameObject currentComicPage; //keeps track of the spawned comic page

	void Start(){
		currentPageNum = 0;
	}

	public void StartComic(){
		NextPage();
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
        else{
			// Turn off all the other pages that isnt the current page
			for(int i = 1; i <= totalNumOfPages; i++){
				pages[i].GetComponent<ComicPage>().ToggleActive(i == currentPageNum ? true : false);
			}
		}
    }

    /// <summary>
    /// Load comic pages into pages list, and make sure only the first one shows
    /// </summary>
    private void LoadComicPages(){
        string petSpecies = DataManager.Instance.GameData.PetInfo.PetSpecies;
        string petColor = DataManager.Instance.GameData.PetInfo.PetColor;

		for(int i = 1; i <= totalNumOfPages; i++){
			GameObject page = Resources.Load(pagePrefabPrefix + i.ToString()) as GameObject;
			ComicPage pageScript = page.GetComponent<ComicPage>();
			pageScript.Init(petColor, this);

			// Turn off everything but the first page
			if(i != 1){
				pageScript.ToggleActive(false);
			}

			pages.Add(page);
		}
    }
}
