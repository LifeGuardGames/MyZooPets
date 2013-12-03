using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataLoader_LootTables
// Loads loot tables from XML.
//---------------------------------------------------

public class DataLoader_LootTables {
	// hashtable that contains all loot table data
	private static Hashtable hashData;
	
	//---------------------------------------------------
	// GetLootTable()
	// Returns loot table data for the incoming id.
	//---------------------------------------------------	
	public static Data_LootTable GetLootTable( string strID ) {
		if ( hashData == null ) 
			SetupData();
		
		Data_LootTable dataTable = null;
		
		if ( hashData.ContainsKey( strID ) ) 
			dataTable = (Data_LootTable) hashData[strID];
		else
			Debug.Log("No such loot table with id: " + strID);
		
		return dataTable;
	}

    public static void SetupData(){
        if(hashData != null) return; //Don't load from xml if data already loaded
		
		hashData = new Hashtable();

        //Load all item xml files
         UnityEngine.Object[] files = Resources.LoadAll("LootTables", typeof(TextAsset));
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
				
                // Get children from xml node
                List<IXMLNode> listChildren = XMLUtils.GetChildrenList(childNode);				
				
				Data_LootTable data = new Data_LootTable( id, hashAttr, listChildren, strError );
				
				if ( hashData.ContainsKey( id ) )
					Debug.Log("Duplicate loot table id: " + id);
				else
					hashData[id] = data;
            }
         }
    }
}

