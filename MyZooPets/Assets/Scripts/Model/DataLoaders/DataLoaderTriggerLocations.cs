using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data loader trigger locations.
/// Hash of a hash -- scenes to trigger locations for that scene
/// Hash -- Key: scene name, Value: (Hash -- Key: TriggerID, Value: ImmutableDataTriggerLocation)
/// </summary>
public class DataLoaderTriggerLocations : XMLLoaderGeneric<DataLoaderTriggerLocations>{

	/// <summary>
	/// Gets the trigger location.
	/// </summary>
	/// <returns>The trigger location.</returns>
	/// <param name="id">Identifier.</param>
	/// <param name="scene">Scene.</param>
	public static ImmutableDataTriggerLocation GetTriggerLocation(string id, string scene){
		instance.InitXMLLoader();
		
		ImmutableDataTriggerLocation data = null;
		Hashtable hashScene = instance.GetData<Hashtable>(scene);

		if(hashScene.ContainsKey(id))
			data = (ImmutableDataTriggerLocation) hashScene[id];
		else
			Debug.LogError("No such trigger id " + id + " for scene " + scene);

		return data;		
	}

	/// <summary>
	/// Gets the available trigger locations for a scene.
	/// </summary>
	/// <returns>The available trigger locations.</returns>
	/// <param name="scene">Scene.</param>
	public static List<ImmutableDataTriggerLocation> GetAvailableTriggerLocations(string scene){
		instance.InitXMLLoader();
		
		List<ImmutableDataTriggerLocation> list = new List<ImmutableDataTriggerLocation>();
		
		Hashtable hashScene = instance.GetData<Hashtable>(scene);
		
		foreach(DictionaryEntry entry in hashScene){
			ImmutableDataTriggerLocation location = (ImmutableDataTriggerLocation)entry.Value;
			
			// check to make sure the partition of this trigger is unlocked; if it is, it's okay to add to the list
			int partition = location.Partition;
			if(GatingManager.Instance.HasActiveGate(partition) == false) {
				list.Add(location);
			}
		}
		
		return list;		
	}	

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataTriggerLocation data = new ImmutableDataTriggerLocation(id, xmlNode, errorMessage);

		string scene = data.Scene;
		if(!hashData.ContainsKey(scene))
			hashData[scene] = new Hashtable();

		Hashtable hashScenes = (Hashtable) hashData[scene];
		if(hashScenes.ContainsKey(id))
			Debug.LogError("Duplicate trigger location id: " + id + " for " + scene);
		else
			hashScenes[id] = data;
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "TriggerLocations";
	}
}

