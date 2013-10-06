using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// CutsceneData
// Save data script for cutscenes.
//---------------------------------------------------

public class CutsceneData{
    public List<string> ListViewed {get; set;}

    //=======================Initialization==================
    public CutsceneData(){}

    //Populate with dummy data
    public void Init(){
        ListViewed = new List<string>();
    }
}
