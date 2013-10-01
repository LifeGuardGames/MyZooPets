using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataGateLoader
// Loads gate data from xml.
//---------------------------------------------------

public class DataGateLoader {

    private static Dictionary<string, DataGate> dictData = new Dictionary<string, DataGate>();
    private static bool dataLoaded = false; //Prohibit double loading data

    // Get monster with incoming id
    public static DataGate GetData(string id){
        DataGate data = null;

        if(dictData.ContainsKey(id))
            data = dictData[id];
		else
			Debug.Log("No such gate with id " + id + " -- creating one with default values");

        return data;
	}
	
	public static Dictionary<string, DataGate> GetAllData() {
		return dictData;	
	}

    public static void SetupData(){
        if(dataLoaded) return; //Don't load from xml if data already loaded
		
        //Load all data xml files
         UnityEngine.Object[] files = Resources.LoadAll("Gates", typeof(TextAsset));
         foreach(TextAsset file in files){
            string xmlString = file.text;
			
			// error message
			string strError = "Error in file " + file.name;			

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
				strError += "(" + id + "): ";
				
				DataGate data = new DataGate( id, hashAttr, strError );
				
	           	// store the data
				if ( dictData.ContainsKey( id ) )
					Debug.Log(strError + "Duplicate keys!");
				else
	            	dictData.Add(id, data);		
            }
         }
         dataLoaded = true;
    }
}

