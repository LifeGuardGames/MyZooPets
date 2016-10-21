using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ImmutableDataMiniPetLevelUpConditions{
#pragma warning disable 0414
	private string id;
#pragma warning restore 0414
	private Dictionary<Level, int> levelUpConditions;

	/// <summary>
	/// Gets the level up condition. Returns -1 if no level up condition found
	/// </summary>
	/// <returns>The level up condition.</returns>
	/// <param name="level">Level.</param>
	public int GetXpNeededForLevel(Level level){
		int retVal = -1;

		if(levelUpConditions.ContainsKey(level)){
			retVal = levelUpConditions[level];
		}
		else{
			Debug.LogError("Level: " + level + " doesn't exist");
		}

		return retVal;
	}

	public ImmutableDataMiniPetLevelUpConditions(string id, IXMLNode xmlNode, string errorMsg){
		levelUpConditions = new Dictionary<Level, int>();
		List<IXMLNode> elements = XMLUtils.GetChildrenList(xmlNode);

		this.id = id;

		foreach(IXMLNode node in elements){
			Hashtable attributes = XMLUtils.GetAttributes(node);

			string rawLevel = HashUtils.GetHashValue<string>(attributes, "Level", "", errorMsg);
			Level level = (Level)Enum.Parse(typeof(Level), rawLevel);

			int condition = int.Parse(HashUtils.GetHashValue<string>(attributes, "Value", "-1", errorMsg));

			levelUpConditions.Add(level, condition);
		}
	}
}
