using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// CutsceneData
// Save data script for cutscenes.
// Mutable data.
//---------------------------------------------------

public class CutsceneData{
    public List<string> ListViewed {get; set;}

    //=======================Initialization==================
    public CutsceneData(){
       Init(); 
    }

    //Populate with dummy data
    private void Init(){
        ListViewed = new List<string>();
    }
}
