using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// CutsceneData
// Save data script for cutscenes.
//---------------------------------------------------

[DoNotSerializePublic]
public class CutsceneData{
    
    [SerializeThis]
    private List<string> listViewed;
	
    //===============Getters & Setters=================
    public List<string> ListViewed {
        get{return listViewed;}
        set{listViewed = value;}
    }

    //=======================Initialization==================
    public CutsceneData(){}

    //Populate with dummy data
    public void Init(){
        listViewed = new List<string>();
    }
}
