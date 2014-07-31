using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data loader monster.
/// Hashtable -- Key: MonsterID, Value: ImmutableDataMonster
/// </summary>
public class DataLoaderMonster : XMLLoaderGeneric<DataLoaderMonster>{

	/// <summary>
	/// Gets Monster data.
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="id">Identifier.</param>
	public static ImmutableDataMonster GetData(string id){
		instance.InitXMLLoader();

		return instance.GetData<ImmutableDataMonster>(id);
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataMonster data = new ImmutableDataMonster(id, xmlNode, errorMessage);

		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data);	
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "Monsters";
	}
}

