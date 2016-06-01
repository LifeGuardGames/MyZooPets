using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderBadges : XMLLoaderGeneric<DataLoaderBadges> {
	public static List<ImmutableDataBadge> GetDataList() {
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataBadge>();
	}

	public static ImmutableDataBadge GetData(string badgeID) {
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataBadge>(badgeID);
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage) {
		ImmutableDataBadge data = new ImmutableDataBadge(id, xmlNode, errorMessage);

		// store the data
		if(hashData.ContainsKey(id)) {
			Debug.LogError(errorMessage + "Duplicate keys!");
		}
		else {
			hashData.Add(id, data);
		}
	}

	protected override void InitXMLLoader() {
		xmlFileFolderPath = "Badges";
	}
}