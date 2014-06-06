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
	// private string UnlockAtGateID; // at which gate should the miniPet be unlocked
	private string spawnLocation; // the location that the miniPet should be spawned at

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

	// public string UnlockAtGateID{
	// 	get{ return UnlockAtGateID;}
	// }

	public Vector3 SpawnLocation{
		get{ return Constants.ParseVector3(spawnLocation);}
	}

//	private Dictionary<Level, int> levelUpConditions;
	//rewards? in a dictionary maybe?

	public ImmutableDataMiniPet(string id, Hashtable hashElements, string error){
		this.id = id;

		name = XMLUtils.GetString(hashElements["Name"] as IXMLNode, null, error);

		type = (MiniPetTypes)System.Enum.Parse(typeof(MiniPetTypes),
                XMLUtils.GetString(hashElements["Type"] as IXMLNode, null, error));

		prefabName = XMLUtils.GetString(hashElements["PrefabName"] as IXMLNode, null, error);

		// UnlockAtGateID = XMLUtils.GetInt(hashElements["UnlockAtGateID"] as IXMLNode,
		//                                     0, error);

		spawnLocation = XMLUtils.GetString(hashElements["SpawnLocation"] as IXMLNode, null, error);
	}
}
