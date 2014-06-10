using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Immutable data mini pet. Data loaded from XML
/// </summary>
public class ImmutableDataMiniPet{

	private string id; // id of miniPet
	private string name; // name of miniPet
	private MiniPetTypes type; // type of miniPet
	private string prefabName; // prefab to spawn for the miniPet
	private Vector3 spawnLocation; // the location that the miniPet should be spawned at
	private string levelUpConditionID; //key: level, value: condition to lv up. # of food required

	public string ID{
		get{ return id;}
	}

	public string Name{
		get{ return Localization.Localize(name);}
	}

	public MiniPetTypes Type{
		get{ return type;}
	}

	public string PrefabName{
		get{ return prefabName;}
	}

	public Vector3 SpawnLocation{
		get{ return spawnLocation;}
	}

	public string LevelUpConditionID{
		get{ return levelUpConditionID;}
	}

	//rewards? in a dictionary maybe?

	public ImmutableDataMiniPet(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);

		this.id = id;

		name = XMLUtils.GetString(hashElements["Name"] as IXMLNode, null, error);

		type = (MiniPetTypes)System.Enum.Parse(typeof(MiniPetTypes),
                XMLUtils.GetString(hashElements["Type"] as IXMLNode, null, error));

		prefabName = XMLUtils.GetString(hashElements["PrefabName"] as IXMLNode, null, error);

		string rawLocation = XMLUtils.GetString(hashElements["SpawnLocation"] as IXMLNode, null, error);
		spawnLocation = Constants.ParseVector3(rawLocation);
	}
}
