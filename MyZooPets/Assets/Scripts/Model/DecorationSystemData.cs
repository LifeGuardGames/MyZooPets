using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DecorationSystemData
// Save data script for the decoration system.
//---------------------------------------------------

public class DecorationSystemData{
    public Dictionary<string, string> PlacedDecorations {get; set;} // dictionary of placed decorations; Key: node ID, Value: item ID
	

    //=======================Initialization==================
    public DecorationSystemData(){}

    //Populate with dummy data
    public void Init(){
        PlacedDecorations = new Dictionary<string, string>();
    }
}
