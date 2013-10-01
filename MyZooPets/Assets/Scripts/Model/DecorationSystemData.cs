using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DecorationSystemData
// Save data script for the decoration system.
//---------------------------------------------------

[DoNotSerializePublic]
public class DecorationSystemData{
    
	[SerializeThis]
    private Dictionary<string, string> dictPlacedDecorations; // dictionary of placed decorations; Key: node ID, Value: item ID
	
    //===============Getters & Setters=================
    public Dictionary<string, string> PlacedDecorations {
        get{return dictPlacedDecorations;}
        set{dictPlacedDecorations = value;}
    }

    //=======================Initialization==================
    public DecorationSystemData(){}

    //Populate with dummy data
    public void Init(){
        dictPlacedDecorations = new Dictionary<string, string>();
    }
}
