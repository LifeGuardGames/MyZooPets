using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComicPlayer : MonoBehaviour{
    public static EventHandler<EventArgs> OnComicPlayerDone; //event fire when comic player played through all pages
    public List<string> pages = new List<string>(); //name of the prefab of the comic pages

    private int currentPageNum;
    private int totalNumOfPages;
    private GameObject currentComicPage; //keeps track of the spawned comic page

    void Awake(){
        totalNumOfPages = pages.Count;
        currentPageNum = 1;
        currentComicPage = null;
    }

    void Start(){
        NextPage();
    }

    //---------------------------------------------------
    // NextPage()
    // Setup the callback function name.
    //---------------------------------------------------
    public void NextPage(){
        //first page
        if(currentPageNum == 1){
            LoadComicPage();
        }
        //last page
        else if(currentPageNum == totalNumOfPages){
            if(currentComicPage != null){
                TweenToggleDemux tweenToggleDemux = currentComicPage.GetComponent<TweenToggleDemux>();
                tweenToggleDemux.HideTarget = this.gameObject;
                tweenToggleDemux.HideFunctionName = "LoadComicPage";

                tweenToggleDemux.Hide();
            }
        }
        //no more pages left. destroy player
        else{
            if(currentComicPage != null){
                TweenToggleDemux tweenToggleDemux = currentComicPage.GetComponent<TweenToggleDemux>();
                tweenToggleDemux.HideTarget = this.gameObject;
                tweenToggleDemux.HideFunctionName = "DestroyComicPage";

                if(OnComicPlayerDone != null)
                    OnComicPlayerDone(this, EventArgs.Empty);

                tweenToggleDemux.Hide();
            }
        }
    }

    //---------------------------------------------------
    // LoadComicPage()
    // Load comic page from resources and Instantiate
    //---------------------------------------------------
    public void LoadComicPage(){
        GameObject loadedPage = null; 
        int pageIndex = currentPageNum - 1;

        string petSpecies = DataManager.Instance.GameData.PetInfo.PetSpecies;
        string petColor = DataManager.Instance.GameData.PetInfo.PetColor;

        try{
            //Get the comic page name
            string pageName = petSpecies + petColor + pages[pageIndex];
            //Load comic prefab from resource and instantiate
            GameObject pagePrefab = (GameObject) Resources.Load(pageName);
            loadedPage = NGUITools.AddChild(this.gameObject, pagePrefab);

            LgButtonMessage buttonMessage = loadedPage.transform.Find("NextButton").GetComponent<LgButtonMessage>();
            buttonMessage.target = this.gameObject;
            buttonMessage.functionName = "NextPage";

            //display loaded comic page
            loadedPage.GetComponent<TweenToggleDemux>().Show();

            //store reference to current comic page
            currentComicPage = loadedPage;
            currentPageNum++;
        }
        catch(ArgumentOutOfRangeException outOfRange){
            Debug.LogError("page index out of range " + outOfRange.Message);
        }
    }

    public void DestroyComicPage(){
        Destroy(this.gameObject);
    }
}
