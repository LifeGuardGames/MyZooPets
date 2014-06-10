using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ImmutableDataMiniPetLevelUpConditions{
	private string id;
	private Dictionary<Level, int> levelUpConditions;

	public ImmutableDataMiniPetLevelUpConditions(string id, IXMLNode xmlNode, string errorMsg){
		List<IXMLNode> elements = XMLUtils.GetChildrenList(xmlNode);

		this.id = id;

		foreach(IXMLNode node in elements){
			Hashtable attributes = XMLUtils.GetAttributes(node);

			string rawLevel = HashUtils.GetHashValue<string>(attributes, "Level", "", errorMsg);
			Level level = (Level)Enum.Parse(typeof(Level), rawLevel);

			int condition = HashUtils.GetHashValue<int>(attributes, "Value", -1, errorMsg);

			levelUpConditions.Add(level, condition);
		}
	}
}
