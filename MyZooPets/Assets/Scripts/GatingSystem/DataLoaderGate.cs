using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Data loader gate.
/// Hash -- Key: GateID, Value: ImmutableDataGate
/// </summary>
public class DataLoaderGate : XMLLoaderGeneric<DataLoaderGate>{
	//Key: zone name (Bedroom, yard), Value: Dictionary
		//Key: room partition number, Value: instance of ImmutableDataGate
	private static Hashtable zoneDictionary;
	
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
	public static ImmutableDataGate GetData(string zoneID, int localPartition){
		instance.InitXMLLoader();
		instance.ForceSetup();
		
		ImmutableDataGate dataGate = null;
		if(zoneID != null && zoneDictionary.ContainsKey(zoneID)){
			Hashtable zone = (Hashtable) zoneDictionary[zoneID];
			if(zone.ContainsKey(localPartition)){
				dataGate = (ImmutableDataGate) zone[localPartition];
			}
		}
		return dataGate;
	}

	/// <summary>
	/// Gets the gates inside the zone(area).
	/// </summary>
	/// <returns>The area gates.</returns>
	/// <param name="area">Area.</param>
	public static Hashtable GetZoneGates(string zoneID){
		if(zoneDictionary.ContainsKey(zoneID)){
			return (Hashtable)zoneDictionary[zoneID];
		}
		else{
			Debug.LogError("No such zone in the gates hash: " + zoneID);
			return null;
		}
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
	/// Stores the gate in a hash of areas to partition ids to the actual data.
	/// </summary>
	/// <param name="dataGate">Data gate.</param>
	private void SortGate(ImmutableDataGate dataGate){
		if(zoneDictionary == null){
			zoneDictionary = new Hashtable();
		}

		string zoneID = dataGate.Zone;
		int localPartition = dataGate.LocalPartition;
		
		// if the area isn't in the hash yet, create it
		if(!zoneDictionary.ContainsKey(zoneID)){
			zoneDictionary[zoneID] = new Hashtable();
		}
		
		Hashtable zone = (Hashtable) zoneDictionary[zoneID];
		
		if(zone.ContainsKey(localPartition)){
			Debug.LogError("Duplicate gate for room " + localPartition + " in area " + zone);
		}
		else{
			zone[localPartition] = dataGate;
		}
	}
}

