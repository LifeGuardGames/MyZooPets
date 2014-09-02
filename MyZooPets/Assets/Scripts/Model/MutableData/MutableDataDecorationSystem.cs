using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DecorationSystemData
// Save data script for the decoration system.
// Mutable data.
//---------------------------------------------------

public class MutableDataDecorationSystem{
    public Dictionary<string, string> PlacedDecorations {get; set;} // dictionary of placed decorations; Key: node ID, Value: item ID

    //=======================Initialization==================
    public MutableDataDecorationSystem(){
        Init();    
    }

    //Populate with dummy data
    private void Init(){
        PlacedDecorations = new Dictionary<string, string>();
    }
}
