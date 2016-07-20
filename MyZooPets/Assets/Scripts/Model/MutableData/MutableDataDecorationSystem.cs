using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DecorationSystemData
// Save data script for the decoration system.
//---------------------------------------------------

public class MutableDataDecorationSystem{
	// Dictionary of placed decorations - Key: node ID, Value: item ID
    public Dictionary<string, string> PlacedDecorations {get; set;}
	public int capsuleMachineLevel;
	// Save refresh cycle types for farm decorations
	public Dictionary<string, DateTime> FarmDecorationTimes {get; set;}

    public MutableDataDecorationSystem(){
        Init();    
    }

    private void Init(){
        PlacedDecorations = new Dictionary<string, string>();
		FarmDecorationTimes = new Dictionary<string, DateTime>();
		capsuleMachineLevel = 0;
    }
}
