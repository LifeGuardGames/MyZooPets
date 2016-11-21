using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ImmutableDataFoodPreferences {
#pragma warning disable 0414
	private string id;
#pragma warning restore 0414
	private Dictionary<Level, string> foodPreferences;

	/// <summary>
	/// Gets the food preference.
	/// </summary>
	/// <returns>The food preference.</returns>
	/// <param name="level">Level.</param>
	public string GetFoodPreference(Level level){
		string retVal = "";

		if(foodPreferences.ContainsKey(level)){
			retVal = foodPreferences[level];
		}
		else{
			Debug.LogError("Level in food preference: " + level + "doesn't exist");
		}

		return retVal;
	}

	public ImmutableDataFoodPreferences(string id, IXMLNode xmlNode, string errorMsg){
		foodPreferences = new Dictionary<Level, string>();
		List<IXMLNode> elements = XMLUtils.GetChildrenList(xmlNode);

		this.id = id;

		foreach(IXMLNode node in elements){
			Hashtable attributes = XMLUtils.GetAttributes(node);

			string rawLevel = HashUtils.GetHashValue<string>(attributes, "Level", "", errorMsg);
			Level level = (Level)Enum.Parse(typeof(Level), rawLevel);

			string foodID = HashUtils.GetHashValue<string>(attributes, "FoodID", "", errorMsg);

			if(!foodPreferences.ContainsKey(level)){
				foodPreferences.Add(level, foodID);
			}
			else{
				Debug.LogError(errorMsg + "duplicate level in food preferences");
			}
		}
	}
}
