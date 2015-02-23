using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ImmutableDataPartition{

	private string id;
	public string Id{
		get{ return id; }
	}

	private ZoneTypes zone;
	public ZoneTypes Zone{
		get{ return zone; }
	}

	private Vector3 position;
	public Vector3 Position {
		get { return position; }
	}

	private List<MinigameTypes> minigameList;
	public List<MinigameTypes> MinigameList{
		get{ return minigameList; }
	}

	// This will be set manually on awake from partition manager
	private Vector3 basePosition;
	public Vector3 BasePosition{
		get{ return basePosition; }
		set{ basePosition = value; }
	}

	public ImmutableDataPartition(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		this.id = id;
		zone = (ZoneTypes)Enum.Parse(typeof(ZoneTypes), XMLUtils.GetString(hashElements["Zone"] as IXMLNode, null, error));
		position = StringUtils.ParseVector3(XMLUtils.GetString(hashElements["Position"] as IXMLNode, null, error));

		// get the minigames in the partition(optional)
		if(hashElements.ContainsKey("Minigame")){
			minigameList = new List<MinigameTypes>();
			string strAmounts = XMLUtils.GetString(hashElements["Minigame"] as IXMLNode);
			string[] arrayAmounts = strAmounts.Split(","[0]);
			for(int i = 0; i < arrayAmounts.Length; ++i){
				minigameList.Add((MinigameTypes)Enum.Parse(typeof(MinigameTypes), arrayAmounts[i]));
			}
		}
	}
}
