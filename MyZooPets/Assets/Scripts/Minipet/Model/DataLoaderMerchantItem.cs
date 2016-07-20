using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderMerchantItem : XMLLoaderGeneric<DataLoaderMerchantItem> {
	public static ImmutableDataMerchantItem GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataMerchantItem>(id);
	}

	public static List<ImmutableDataMerchantItem> GetDataList(){
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataMerchantItem>();
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataMerchantItem data = new ImmutableDataMerchantItem(id, xmlNode, errorMessage);
		// Store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data);
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "SecretMerchantItems";
	}
}
