using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataGateLoader
// Loads gate data from xml.
//---------------------------------------------------

public class DataLoaderGate{
	
	// dictionary of all gates
	private static Dictionary<string, ImmutableDataGate> dictData;
	
	// hash of areas to rooms to gates
	private static Hashtable hashData;

	//---------------------------------------------------
	// GetData()
	// Returns the gate with incoming id.  This probably
	// isn't very useful.
	//---------------------------------------------------
	public static ImmutableDataGate GetData(string id){
		Dictionary<string, ImmutableDataGate> dictData = GetAllData();
		
		ImmutableDataGate data = null;

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
	public static ImmutableDataGate GetData(string area, int roomPartition){
		if(hashData == null)
			SetupData();
		
		ImmutableDataGate dataGate = null;
		
		if(hashData.ContainsKey(area)){
			Hashtable hashArea = (Hashtable)hashData[area];
			if(hashArea.ContainsKey(roomPartition))
				dataGate = (ImmutableDataGate)hashArea[roomPartition];
		}
		
		return dataGate;
	}
	
	//---------------------------------------------------
	// IsActiveGate()
	// Returns whether or not the incoming room in the
	// incoming area has an active gate.
	//---------------------------------------------------	
//	public static bool HasActiveGate(string area, int roomPartition){
//		bool isActive = false;
//		
//		ImmutableDataGate data = GetData(area, roomPartition);
//		if(data != null) 
//			isActive = DataManager.Instance.GameData.GatingProgress.IsGateActive(data.GetGateID());
//		
//		return isActive;
//	}
	
	//---------------------------------------------------
	// GetAreaGates()
	// Returns all the gates for the incoming area.
	//---------------------------------------------------
	//
	public static Hashtable GetAreaGates(string area){
		Hashtable hashGates = new Hashtable();
		
		if(hashData.ContainsKey(area))
			hashGates = (Hashtable)hashData[area];
		else
			Debug.LogError("No such area in the gates hash: " + area);
		
		return hashGates;
	}
	
	public static Dictionary<string, ImmutableDataGate> GetAllData(){
		if(dictData == null)
			SetupData();
		
		return dictData;	
	}

	public static void SetupData(){
		dictData = new Dictionary<string, ImmutableDataGate>();
		hashData = new Hashtable();
		
		//Load all data xml files
		UnityEngine.Object[] files = Resources.LoadAll("Gates", typeof(TextAsset));
		foreach(TextAsset file in files){
			string xmlString = file.text;
			
			// error message
			string errorMessage = "Error in file " + file.name;			

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
				string strError = errorMessage + "(" + id + "): ";
				
				ImmutableDataGate data = new ImmutableDataGate(id, hashElements, strError);
				
				// store the data
				if(dictData.ContainsKey(id))
					Debug.LogError(strError + "Duplicate keys!");
				else{
					// add to dictionary of all gates
					dictData.Add(id, data);	
					
					// we also want to store the gates in a more elaborate hashtable for easy access
					StoreGate(data);
				}
			}
		}
	}

	/// <summary>
	/// tores the gate in a hash of areas to partition 
	/// ids to the actual data.
	/// </summary>
	/// <param name="dataGate">Data gate.</param>
	private static void StoreGate(ImmutableDataGate dataGate){
		string area = dataGate.GetArea();
		int roomPartition = dataGate.GetPartition();
		
		// if the area isn't in the hash yet, create it
		if(!hashData.ContainsKey(area))
			hashData[area] = new Hashtable();
		
		Hashtable hashArea = (Hashtable)hashData[area];
		
		if(hashArea.ContainsKey(roomPartition))
			Debug.LogError("Duplicate gate for room " + roomPartition + " in area " + area);
		else
			hashArea[roomPartition] = dataGate;
	}
}

