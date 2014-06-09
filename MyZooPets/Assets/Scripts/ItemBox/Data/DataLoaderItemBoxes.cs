using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataLoader_ItemBoxes
// Loads item boxes from XML.
//---------------------------------------------------

public class DataLoaderItemBoxes{
	// hashtable that contains all item box data
	private static Hashtable hashData;
	
	//---------------------------------------------------
	// GetItemBox()
	// Returns an item box data with the incoming id.
	//---------------------------------------------------	
	public static ImmutableDataItemBox GetItemBox(string strID){
		if(hashData == null) 
			SetupData();
		
		ImmutableDataItemBox data = null;
		
		if(hashData.ContainsKey(strID)) 
			data = (ImmutableDataItemBox)hashData[strID];
		else
			Debug.LogError("No such item box with id: " + strID);
		
		return data;		
	}

	public static void SetupData(){
		if(hashData != null)
			return; //Don't load from xml if data already loaded
		
		hashData = new Hashtable();

		//Load all item xml files
		UnityEngine.Object[] files = Resources.LoadAll("ItemBoxes", typeof(TextAsset));
		foreach(TextAsset file in files){
			string xmlString = file.text;
			
			// error message
			string fileError = "Error in file " + file.name;				
			
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
				string errorMessage = fileError + "(" + id + "): ";
				
				// Get children from xml node
				List<IXMLNode> listChildren = XMLUtils.GetChildrenList(childNode);				
				
				ImmutableDataItemBox data = new ImmutableDataItemBox(id, hashAttr, listChildren, errorMessage);
				
				if(hashData.ContainsKey(id))
					Debug.LogError("Duplicate item box id: " + id);
				else
					hashData[id] = data;
			}
		}
	}
}

