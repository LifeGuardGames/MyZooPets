using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Data loader gate.
/// </summary>
public class DataLoaderGate{

	private static Dictionary<string, ImmutableDataGate> allGates; //Key: gateID, Value: ImmutableDataGate
	private static Hashtable hashData; // hash of areas to rooms to gates
	
	/// <summary>
	/// Gets the gate data.
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="id">Identifier.</param>
	public static ImmutableDataGate GetData(string id){
		Dictionary<string, ImmutableDataGate> dictData = GetAllData();
		
		ImmutableDataGate data = null;

		if(dictData.ContainsKey(id))
			data = dictData[id];
		else
			Debug.LogError("No such gate with id " + id + " -- creating one with default values");

		return data;
	}

	/// <summary>
	/// Gets the gate data. Will be null if there is no gate
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="area">Area.</param>
	/// <param name="roomPartition">Room partition.</param>
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

	/// <summary>
	/// Gets the area gates.
	/// </summary>
	/// <returns>The area gates.</returns>
	/// <param name="area">Area.</param>
	public static Hashtable GetAreaGates(string area){
		Hashtable hashGates = new Hashtable();
		
		if(hashData.ContainsKey(area))
			hashGates = (Hashtable)hashData[area];
		else
			Debug.LogError("No such area in the gates hash: " + area);
		
		return hashGates;
	}
	
	public static Dictionary<string, ImmutableDataGate> GetAllData(){
		if(allGates == null)
			SetupData();
		
		return allGates;	
	}

	public static void SetupData(){
		allGates = new Dictionary<string, ImmutableDataGate>();
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
				if(allGates.ContainsKey(id))
					Debug.LogError(strError + "Duplicate keys!");
				else{
					// add to dictionary of all gates
					allGates.Add(id, data);	
					
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

