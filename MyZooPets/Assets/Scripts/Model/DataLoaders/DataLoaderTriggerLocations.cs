using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataLoader_TriggerLocations
// Loads trigger locations from XML.
//---------------------------------------------------

public class DataLoaderTriggerLocations {
	// hashtable that contains all trigger location data
	// it's a hash of a hash -- scenes to trigger locations for that scene
	private static Hashtable hashData;
	
	//---------------------------------------------------
	// GetTriggerLocation()
	// Returns a trigger location based on an id.
	//---------------------------------------------------	
	public static Data_TriggerLocation GetTriggerLocation( string strID, string strScene ) {
		if ( hashData == null ) 
			SetupData();
		
		Data_TriggerLocation data = null;
		
		if ( hashData.ContainsKey( strScene ) ) {
			Hashtable hashScene = (Hashtable) hashData[strScene];
			if ( hashScene.ContainsKey( strID ) )
				data = (Data_TriggerLocation) hashScene[strID];
			else
				Debug.LogError("No such trigger id " + strID + " for scene " + strScene);
		}
		else
			Debug.LogError("No such scene for trigger ids: " + strScene);
		
		return data;		
	}
	
	//---------------------------------------------------
	// GetAvailableTriggerLocations()
	// Returns a list of available trigger locations for
	// a scene.
	//---------------------------------------------------	
	public static List<Data_TriggerLocation> GetAvailableTriggerLocations( string strScene ) {
		if ( hashData == null ) 
			SetupData();
		
		List<Data_TriggerLocation> list = new List<Data_TriggerLocation>();
		
		if ( hashData.ContainsKey( strScene ) ) {
			Hashtable hashScene = (Hashtable) hashData[strScene];
			foreach ( DictionaryEntry entry in hashScene ) {
				Data_TriggerLocation location = (Data_TriggerLocation) entry.Value;
				
				// check to make sure the partition of this trigger is unlocked; if it is, it's okay to add to the list
				int nPartition = location.GetPartition();
				if ( GatingManager.Instance.HasActiveGate( nPartition ) == false )
					list.Add( location );
			}
		}
		else
			Debug.LogError("No such trigger loc data for " + strScene);
		
		return list;		
	}

    public static void SetupData(){
        if(hashData != null) return; //Don't load from xml if data already loaded
		
		hashData = new Hashtable();

        //Load all item xml files
         UnityEngine.Object[] files = Resources.LoadAll("TriggerLocations", typeof(TextAsset));
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

                // Get id
                Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
                string id = (string)hashAttr["ID"];
				string strError = strErrorFile + "(" + id + "): ";
				
				Data_TriggerLocation data = new Data_TriggerLocation( id, hashAttr, strError );
				
				string strScene = data.GetScene();	
				if ( !hashData.ContainsKey( strScene ) )
					hashData[strScene] = new Hashtable();
				
				Hashtable hashScenes = (Hashtable) hashData[strScene];
				if ( hashScenes.ContainsKey( id ) )
					Debug.LogError("Duplicate trigger location id: " + id + " for " + strScene);
				else
					hashScenes[id] = data;
            }
         }
    }
}

