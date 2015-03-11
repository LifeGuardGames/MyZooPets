using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DataLoaderShooterArmy : XMLLoaderGeneric<DataLoaderShooterArmy>{

	public static ImmutableDataShooterArmy GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataShooterArmy>(id);
	}

	public static List<ImmutableDataShooterArmy> GetDataList(){
		instance.InitXMLLoader();

		List<ImmutableDataShooterArmy> armyList = instance.GetDataList<ImmutableDataShooterArmy>();
		return (from army in armyList orderby army.Id ascending select army).ToList();
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataShooterArmy data = new ImmutableDataShooterArmy(id, xmlNode, errorMessage);
		// Store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data);
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "Shooter/TriggerShooter";
	}
}
