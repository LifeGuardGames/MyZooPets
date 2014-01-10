using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataGateLoader
// Loads gate data from xml.
//---------------------------------------------------

public class DataGateLoader {
	
	// dictionary of all gates
    private static Dictionary<string, DataGate> dictData;
	
	// hash of areas to rooms to gates
	private static Hashtable hashData;

	//---------------------------------------------------
	// GetData()
	// Returns the gate with incoming id.  This probably
	// isn't very useful.
	//---------------------------------------------------
    public static DataGate GetData(string id){
		Dictionary<string, DataGate> dictData = GetAllData();
		
        DataGate data = null;

        if(dictData.ContainsKey(id))
            data = dictData[id];
		else
			Debug.LogError("No such gate with id " + id + " -- creating one with default values");

        return data;
	}
	
	//---------------------------------------------------
	// GetData()
	// Returns the gate data for a given area & room.
	// Will be null if there is no gate.
	//---------------------------------------------------
	public static DataGate GetData( string strArea, int nRoom ) {
		if ( hashData == null )
			SetupData();
		
		DataGate dataGate = null;
		
		if ( hashData.ContainsKey( strArea ) ) {
			Hashtable hashArea = (Hashtable) hashData[strArea];
			if ( hashArea.ContainsKey( nRoom ) )
				dataGate = (DataGate) hashArea[nRoom];
		}
		
		return dataGate;
	}
	
	//---------------------------------------------------
	// IsActiveGate()
	// Returns whether or not the incoming room in the
	// incoming area has an active gate.
	//---------------------------------------------------	
	public static bool HasActiveGate( string strArea, int nRoom ) {
		bool bHas = false;
		
		DataGate data = GetData( strArea, nRoom );
		if ( data != null ) 
			bHas = DataManager.Instance.GameData.GatingProgress.IsGateActive( data.GetGateID() );
		
		return bHas;
	}
	
	//---------------------------------------------------
	// GetAreaGates()
	// Returns all the gates for the incoming area.
	//---------------------------------------------------	
	public static Hashtable GetAreaGates( string strArea ) {
		Hashtable hashGates = new Hashtable();
		
		if ( hashData.ContainsKey( strArea ) )
			hashGates = (Hashtable) hashData[strArea];
		else
			Debug.LogError("No such area in the gates hash: " + strArea);
		
		return hashGates;
	}
	
	public static Dictionary<string, DataGate> GetAllData() {
		if ( dictData == null )
			SetupData();
		
		return dictData;	
	}

    public static void SetupData(){
		dictData = new Dictionary<string, DataGate>();
		hashData = new Hashtable();
		
        //Load all data xml files
         UnityEngine.Object[] files = Resources.LoadAll("Gates", typeof(TextAsset));
         foreach(TextAsset file in files){
            string xmlString = file.text;
			
			// error message
			string strErrorFile = "Error in file " + file.name;			

            //Create XMLParser instance
            XMLParser xmlParser = new XMLParser(xmlString);

            //Call the parser to build the IXMLNode objects
            XMLElement xmlElement = xmlParser.Parse();

            //Go through all child node of xmlElement (the parent of the file)
            for(int i=0; i<xmlElement.Children.Count; i++){
                IXMLNode childNode = xmlElement.Children[i];
				
             	// Get  properties from xml node
                Hashtable hashElements = XMLUtils.GetChildren(childNode);				
				
                //Get id
                Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
                string id = (string)hashAttr["ID"];
				string strError = strErrorFile + "(" + id + "): ";
				
				DataGate data = new DataGate( id, hashAttr, hashElements, strError );
				
	           	// store the data
				if ( dictData.ContainsKey( id ) )
					Debug.LogError(strError + "Duplicate keys!");
				else {
					// add to dictionary of all gates
	            	dictData.Add(id, data);	
					
					// we also want to store the gates in a more elaborate hashtable for easy access
					StoreGate( data );
				}
            }
         }
    }
	
	//---------------------------------------------------
	// StoreGate()
	// Stores the gate in a hash of areas to partition 
	// ids to the actual data.
	//---------------------------------------------------	
	private static void StoreGate( DataGate dataGate ) {
		string strArea = dataGate.GetArea();
		int nRoom = dataGate.GetPartition();
		
		// if the area isn't in the hash yet, create it
		if ( !hashData.ContainsKey( strArea ) )
			hashData[strArea] = new Hashtable();
		
		Hashtable hashArea = (Hashtable) hashData[strArea];
		
		if ( hashArea.ContainsKey( nRoom ) )
			Debug.LogError("Duplicate gate for room " + nRoom + " in area " + strArea);
		else
			hashArea[nRoom] = dataGate;
	}
}

