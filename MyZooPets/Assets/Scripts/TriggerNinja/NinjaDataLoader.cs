using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// NinjaDataLoader
// Loads ninja game data (patterns) from xml.
//---------------------------------------------------

public class NinjaDataLoader {
	
	// hash of data
    private static Hashtable hashData = new Hashtable();
	
    private static bool dataLoaded = false; //Prohibit double loading data
	
	//---------------------------------------------------
	// GetGroupToSpawn()
	// Will return the data for a group to spawn given
	// a mode and a scoring key.
	//---------------------------------------------------	
	public static NinjaData GetGroupToSpawn( NinjaModes eMode, NinjaScoring eScoring ) {
		NinjaData data = null;
		
		// get the mode data
		if ( hashData.Contains( eMode ) ) {
			Hashtable hashMode = (Hashtable) hashData[eMode];
			
			// get the list of entries for the scoring key
			if ( hashMode.Contains( eScoring ) ) {
				List<NinjaData> listData = (List<NinjaData>) hashMode[eScoring];
				data = GetRandomData( listData );
			}
		}
		
		if ( data == null )
			Debug.Log("Something going wrong with picking a group for the ninja game to spawn");
		
		return data;
	}
	
	//---------------------------------------------------
	// GetRandomData()
	// Creates a weighted list with the incoming list of
	// data and then returns a random entry from it.
	//---------------------------------------------------	
	private static NinjaData GetRandomData( List<NinjaData> listData ) {
		List<NinjaData> listWeighted = new List<NinjaData>();
		
		// create the weighted list
		for ( int i = 0; i < listData.Count; ++i ) {
			NinjaData data = listData[i];
			int nWeight = data.GetWeight();
			for ( int j = 0; j < nWeight; ++j )
				listWeighted.Add( data );
		}
		
		// pick a random element from the weighted list
		int nRandom = UnityEngine.Random.Range(0, listWeighted.Count);
		NinjaData dataRandom = listWeighted[nRandom];
		return dataRandom;
	}

    public static void SetupData(){
        if(dataLoaded) return; //Don't load from xml if data already loaded
		
        //Load all data xml files
         UnityEngine.Object[] files = Resources.LoadAll("Ninja", typeof(TextAsset));
         foreach(TextAsset file in files){
            string xmlString = file.text;
			
			// error message
			string strErrorFile = "Error in file " + file.name;			

            //Create XMLParser instance
            XMLParser xmlParser = new XMLParser(xmlString);

            //Call the parser to build the IXMLNode objects
            XMLElement xmlElement = xmlParser.Parse();
			
			// we store the data per mode of the ninja game.  the name of the file is the mode.
			NinjaModes eMode = (NinjaModes) System.Enum.Parse( typeof( NinjaModes ), file.name );
			hashData[eMode] = new Hashtable();
			Hashtable hashMode = (Hashtable) hashData[eMode];

            //Go through all child node of xmlElement (the parent of the file)
            for(int i=0; i<xmlElement.Children.Count; i++){
                IXMLNode childNode = xmlElement.Children[i];

                // Get id
                Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
                string id = (string)hashAttr["ID"];
				string strError = strErrorFile + "(" + id + "): ";
				
				NinjaData data = new NinjaData( id, hashAttr, childNode, strError );
				
				// we want to stuff the data in each of its scoring categories
				List<NinjaScoring> listScoring = data.GetScoringCategories();
				for ( int j = 0; j < listScoring.Count; ++j ) {
					NinjaScoring eScoring = listScoring[j];
					
					// if the mode doesn't contain this scoring key yet, add it
					if ( !hashMode.ContainsKey( eScoring ) )
						hashMode[eScoring] = new List<NinjaData>();
					
					// add the data to the list of data for this mode/scoring key
					List<NinjaData> listData = (List<NinjaData>) hashMode[eScoring];
					listData.Add( data );
					
				}
            }
         }
         dataLoaded = true;
    }
}

