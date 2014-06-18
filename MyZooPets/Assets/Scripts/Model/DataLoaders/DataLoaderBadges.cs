using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data loader badges.
/// Hash -- Key: BadgeID, Value: Badge
/// </summary>
public class DataLoaderBadges : XMLLoaderGeneric<DataLoaderBadges>{

    /// <summary>
    /// Gets the data list.
    /// </summary>
    /// <returns>The data list.</returns>
    public static List<Badge> GetDataList(){
		instance.InitXMLLoader();
        return instance.GetDataList<Badge>();
    }

    /// <summary>
    /// Gets the data.
    /// </summary>
    /// <returns>The data.</returns>
    /// <param name="badgeID">Badge ID.</param>
    public static Badge GetData(string badgeID){
		instance.InitXMLLoader();
        return instance.GetData<Badge>(badgeID);
    }

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		Badge data = new Badge(id, xmlNode, errorMessage);

		// store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data); 
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "Badges";
	}
}