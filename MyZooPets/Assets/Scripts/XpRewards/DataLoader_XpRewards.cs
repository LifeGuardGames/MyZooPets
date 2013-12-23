using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataLoader_XpRewards
// Loads xp rewards from XML.
//---------------------------------------------------

public class DataLoader_XpRewards {
	// hashtable that contains all reward data
	private static Hashtable hashData;
	
	//---------------------------------------------------
	// GetXP()
	// For the incoming parameters, returns how much xp
	// the user should get.
	//---------------------------------------------------
	public static int GetXP( string strKey, Hashtable hashBonusData ) {
		// the total xp will be calculated by the data
		int nXP = 0;
		
		// set up data if it hasn't been loaded yet
		if ( hashData == null )
			SetupData();
		
		// get the data for the incoming key
		if ( hashData.ContainsKey( strKey ) ) {
			Data_XpReward data = (Data_XpReward) hashData[strKey];
			nXP = data.CalculateXP( hashBonusData );
		}
		else
			Debug.Log("No such xp data for " + strKey);			
		
		return nXP;
	}

    public static void SetupData(){
        if(hashData != null) return; //Don't load from xml if data already loaded
		
		hashData = new Hashtable();
		
        //Load all item xml files
         UnityEngine.Object[] files = Resources.LoadAll("XP_Rewards", typeof(TextAsset));
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
				
				Data_XpReward data = new Data_XpReward( id, childNode, strError );
				
				if ( hashData.ContainsKey( id ) )
					Debug.Log("Duplicate xp reward id: " + id);
				else
					hashData[id] = data;
            }
         }
    }
}

