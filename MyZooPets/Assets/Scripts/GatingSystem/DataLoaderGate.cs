using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Data loader gate.
/// Hash -- Key: GateID, Value: ImmutableDataGate
/// </summary>
public class DataLoaderGate : XMLLoaderGeneric<DataLoaderGate>{
	//Key: area name (Bedroom, yard), Value: Dictionary
	//Key: room partition number, Value: instance of ImmutableDataGate
	private static Hashtable areaPartitionGates;
	
	/// <summary>
	/// Gets the gate data.
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="id">Identifier.</param>
	public static ImmutableDataGate GetData(string id){
		instance.InitXMLLoader();

		return instance.GetData<ImmutableDataGate>(id);;
	}

	/// <summary>
	/// Gets the gate data. Will be null if there is no gate
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="area">Area.</param>
	/// <param name="roomPartition">Room partition.</param>
	public static ImmutableDataGate GetData(string areaID, int roomPartition){
		instance.InitXMLLoader();
		instance.ForceSetup();
		
		ImmutableDataGate dataGate = null;
		
		if(areaPartitionGates.ContainsKey(areaID)){
			Hashtable area = (Hashtable) areaPartitionGates[areaID];
			if(area.ContainsKey(roomPartition))
				dataGate = (ImmutableDataGate) area[roomPartition];
		}
		
		return dataGate;
	}

	/// <summary>
	/// Gets the area gates.
	/// </summary>
	/// <returns>The area gates.</returns>
	/// <param name="area">Area.</param>
	public static Hashtable GetAreaGates(string areaID){
		Hashtable area = new Hashtable();
		
		if(areaPartitionGates.ContainsKey(areaID))
			area = (Hashtable) areaPartitionGates[areaID];
		else
			Debug.LogError("No such area in the gates hash: " + areaID);
		
		return area;
	}
	
	public static List<ImmutableDataGate> GetAllData(){
		instance.InitXMLLoader();
		
		return instance.GetDataList<ImmutableDataGate>();	
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataGate data = new ImmutableDataGate(id, xmlNode, errorMessage);

		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else{
			// add to dictionary of all gates
			hashData.Add(id, data);	
			
			// we also want to store the gates in a more elaborate hashtable for easy access
			SortGate(data);
		}
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "Gates";
	}

	/// <summary>
	/// Stores the gate in a hash of areas to partition 
	/// ids to the actual data.
	/// </summary>
	/// <param name="dataGate">Data gate.</param>
	private void SortGate(ImmutableDataGate dataGate){
		if(areaPartitionGates == null)
			areaPartitionGates = new Hashtable();

		string areaID = dataGate.GetArea();
		int roomPartition = dataGate.GetPartition();
		
		// if the area isn't in the hash yet, create it
		if(!areaPartitionGates.ContainsKey(areaID))
			areaPartitionGates[areaID] = new Hashtable();
		
		Hashtable area = (Hashtable) areaPartitionGates[areaID];
		
		if(area.ContainsKey(roomPartition))
			Debug.LogError("Duplicate gate for room " + roomPartition + " in area " + areaID);
		else
			area[roomPartition] = dataGate;
	}
}

