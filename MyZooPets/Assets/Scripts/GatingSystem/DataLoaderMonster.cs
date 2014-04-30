using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataMonsterLoader
// Loads monster data from xml.
//---------------------------------------------------

public class DataLoaderMonster{

	private static Dictionary<string, ImmutableDataMonster> dictData;

	// Get monster with incoming id
	public static ImmutableDataMonster GetData(string id){
		if(dictData == null)
			SetupData();
		
		ImmutableDataMonster data = null;

		if(dictData.ContainsKey(id))
			data = dictData[id];
		else
			Debug.LogError("No such monster with id " + id + " -- creating one with default values");

		return data;
	}

	public static void SetupData(){
		dictData = new Dictionary<string, ImmutableDataMonster>();
		
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

				// Get  properties from xml node
				Hashtable hashElements = XMLUtils.GetChildren(childNode);
				
				ImmutableDataMonster data = new ImmutableDataMonster(id, hashElements, strError);
				
				// store the data
				if(dictData.ContainsKey(id))
					Debug.LogError(strError + "Duplicate keys!");
				else
					dictData.Add(id, data);		
			}
		}
	}
}

