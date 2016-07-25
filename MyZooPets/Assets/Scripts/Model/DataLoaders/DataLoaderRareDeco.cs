using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderRareDeco : XMLLoaderGeneric<DataLoaderRareDeco> {


	public static ImmutableDataRareDeco GetItem(string id) {
		instance.InitXMLLoader();

		return instance.GetData<ImmutableDataRareDeco>(id);

	}

	public static bool IsItemExist(string id) {
		instance.InitXMLLoader();
		return instance.IsDataExist(id);
	}

	public static List<ImmutableDataRareDeco> GetItemList() {
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataRareDeco>();
	}

	public static ImmutableDataRareDeco GetDecoAtTier(int tier) {
		List<ImmutableDataRareDeco> temp = GetItemList();
		foreach(ImmutableDataRareDeco deco in temp) {
			if(deco.Tier > tier) {
				temp.Remove(deco);
			}
		}
		int rand = UnityEngine.Random.Range(0, temp.Count);
		return temp[rand];
	}


	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage) {
		ImmutableDataRareDeco data = new ImmutableDataRareDeco(id, xmlNode, errorMessage);

		// store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data);
	}

	protected override void InitXMLLoader() {
		xmlFileFolderPath = "RareDeco";
	}
}
