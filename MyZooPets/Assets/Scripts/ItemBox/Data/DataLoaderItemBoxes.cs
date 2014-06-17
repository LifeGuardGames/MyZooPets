using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data loader item boxes.
/// Hashtable -- Key: ItemBoxID, Value: ImmutableDataItemBox
/// </summary>
public class DataLoaderItemBoxes : XMLLoaderGeneric<DataLoaderItemBoxes>{
	
	/// <summary>
	/// Gets the item box.
	/// </summary>
	/// <returns>The item box.</returns>
	/// <param name="id">Identifier.</param>
	public static ImmutableDataItemBox GetItemBox(string id){
		instance.InitXMLLoader();

		return instance.GetData<ImmutableDataItemBox>(id);
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataItemBox data = new ImmutableDataItemBox(id, xmlNode, errorMessage);

		if(hashData.ContainsKey(id))
			Debug.LogError("Duplicate item box id: " + id);
		else
			hashData[id] = data;
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "ItemBoxes";
	}
}

