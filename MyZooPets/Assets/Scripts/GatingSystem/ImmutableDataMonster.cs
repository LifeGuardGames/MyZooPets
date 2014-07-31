using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DataMonster
// Individual piece of monster data as loaded from xml.
// This is considered immutable.
//---------------------------------------------------

public class ImmutableDataMonster{
	private string monsterID; // id of the monster
	private int monsterHP; // hp of the monster
	private string resourceKey; // key for the monster, used to buid resources

	public string MonsterID{
		get{ return monsterID;}
	}
	
	public int MonsterHealth{
		get{ return monsterHP;}
	}
	
	public string ResourceKey{
		get{ return resourceKey;}
	}
	
	public ImmutableDataMonster(string id, IXMLNode xmlNode, string errorMsg){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);

		monsterID = id;	

		// get monster hp
		monsterHP = XMLUtils.GetInt(hashElements["Health"] as IXMLNode, 100, errorMsg);
		
		// get prefab that this monster spawns
		resourceKey = XMLUtils.GetString(hashElements["PrefabName"] as IXMLNode, "SmokeMonster", errorMsg);
	}
}
