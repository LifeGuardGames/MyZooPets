using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// CutsceneData
// Save data script for cutscenes.
// Mutable data.
//---------------------------------------------------

public class MutableDataCutscene{
    public List<string> ListViewed {get; set;}

    //=======================Initialization==================
    public MutableDataCutscene(){
       Init(); 
    }

    //Populate with dummy data
    private void Init(){
        ListViewed = new List<string>();
    }
}
