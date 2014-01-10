using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataMonsterLoader
// Loads monster data from xml.
//---------------------------------------------------

public class DataMonsterLoader {

    private static Dictionary<string, DataMonster> dictData;

    // Get monster with incoming id
    public static DataMonster GetData(string id){
		if ( dictData == null )
			SetupData();
		
        DataMonster data = null;

        if(dictData.ContainsKey(id))
            data = dictData[id];
		else
			Debug.LogError("No such monster with id " + id + " -- creating one with default values");

        return data;
	}

    public static void SetupData(){
        dictData = new Dictionary<string, DataMonster>();
		
        //Load all data xml files
         UnityEngine.Object[] files = Resources.LoadAll("Monsters", typeof(TextAsset));
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

                //Get id
                Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
                string id = (string)hashAttr["ID"];
				string strError = strErrorFile + "(" + id + "): ";
				
				DataMonster data = new DataMonster( id, hashAttr, strError );
				
	           	// store the data
				if ( dictData.ContainsKey( id ) )
					Debug.LogError(strError + "Duplicate keys!");
				else
	            	dictData.Add(id, data);		
            }
         }
    }
}

