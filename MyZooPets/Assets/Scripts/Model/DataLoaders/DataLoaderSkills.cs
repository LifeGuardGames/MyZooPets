                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data loader skills.
/// Hash -- Key: SkillID, Value: Skill
/// </summary>
public class DataLoaderSkills : XMLLoaderGeneric<DataLoaderSkills>{

	/// <summary>
	/// Gets the data.
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="skillID">Skill ID.</param>
	public static Skill GetData(string skillID){
		instance.InitXMLLoader();
		
		return instance.GetData<Skill>(skillID);
	}

	/// <summary>
	/// Gets the data list.
	/// </summary>
	/// <returns>The data list.</returns>
	public static List<Skill> GetDataList(){
		instance.InitXMLLoader();

		return instance.GetDataList<Skill>();
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, 
	                               string errorMessage){
		Skill data = new Skill(id, xmlNode, errorMessage);

		// store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data); 
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "Skills";
	}
}