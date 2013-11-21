using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComicPlayer : MonoBehaviour{
    public List<string> pages = new List<string>(); //name of the prefab of the comic pages

    private List<GameObject> spawnedPages; //keeps track of all the spawned comic pages
    private int currentPage = 1;
    private int totalNumOfPages;
    private GameObject centerAnchor;

    void Awake(){
        totalNumOfPages = pages.Count;
        spawnedPages = new List<GameObject>();
        centerAnchor = GameObject.Find("Anchor-Center"); //TO-DO: don't like using GO.Find...expensive
    }

    void Start(){

    }

    private GameObject LoadComicPage(int pageIndex){
        GameObject loadedPage = null; 

        try{
            string pageName = pages[pageIndex];
            GameObject pagePrefab = (GameObject) Resources.Load("pageName");
            loadedPage = NGUITools.AddChild(centerAnchor, pagePrefab);
        }
        catch(ArgumentOutOfRangeException outOfRange){
            Debug.Log("page index out of range");
        }

        return loadedPage;
    }

    public void NextPage(){
        //first page
        if(currentPage == 1){
            GameObject comicPage = LoadComicPage(currentPage - 1);
            //find the action button. set it to call next page again.

        }
        //last page
        else if(currentPage == totalNumOfPages){

        }
        //pages in between (unlikely to happen since the comics are short)
        else{

        }
        //check if last page



        //check next page is not out of index

        //Hide current page

        //load next page prefab
        //set the action button

        currentPage++;
    }

    public void DestroyComic(){


    }
}
