using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Item. Immutable data
/// </summary>
public class Item{
	private string id; //id of item
	private string name; //name of item
	private string textureName; //name of texture in the atlas
	private ItemType type;
	private double cost; //cost of item
	private CurrencyTypes currencyType;
	protected string description;
	private int unlockAtLevel = 0; //the level when item is unlocked
	private bool itemBoxOnly = false; //T: only available from item box (dropped by smog monster)
	private bool isSecretItem = false;
	private int sortCategory;	// Options, use for category sorting (ie. accessories)

	//F: available in store as well
	//default to false
	private string soundUsed; // sound item makes when it is used

	public string ID{
		get{ return id;}
	}

	public string Name{ 
		get{ return Localization.Localize(name);} 
	}

	public string TextureName{
		get{ return textureName;}
	}

	public ItemType Type{
		get{ return type;}
	}

	public double Cost{
		get{ return cost;}
	}

	public CurrencyTypes CurrencyType{
		get{ return currencyType;}
	}

	public virtual string Description{
		get{
			string retVal = "";

			if(!String.IsNullOrEmpty(description))
				retVal = Localization.Localize(description);

			return retVal;
		}
	}

	public int UnlockAtLevel{
		get{ return unlockAtLevel;}
	}

	public string SoundUsed{
		get{ return soundUsed;}	
	}

	public bool IsSecretItem{
		get{ return isSecretItem;}
	}

	public int SortCategory{
		get{ return sortCategory;}
	}
	
	public bool IsLocked(){
		bool isLocked = true;
		int lockLevel = UnlockAtLevel;
		int currentPetLevel = (int)(LevelLogic.Instance.CurrentLevel);

		isLocked = lockLevel > 0 && lockLevel > currentPetLevel;

		return isLocked;
	}

	public Item(string id, ItemType type, Hashtable hashItemData){
		this.id = id;
		this.type = type;

		name = XMLUtils.GetString(hashItemData["Name"] as IXMLNode);
		textureName = XMLUtils.GetString(hashItemData["TextureName"] as IXMLNode);

		Hashtable costAttributes = XMLUtils.GetAttributes(hashItemData["Cost"] as IXMLNode);

//		cost = XMLUtils.GetInt(hashItemData["Cost"] as IXMLNode);

		currencyType = (CurrencyTypes)Enum.Parse(typeof(CurrencyTypes), 
		                  HashUtils.GetHashValue<string>(costAttributes, "CurrencyType", ""));

		cost = (double)Double.Parse(HashUtils.GetHashValue<string>(costAttributes, "Amount", ""));

		description = XMLUtils.GetString(hashItemData["Desc"] as IXMLNode, "");
    
		// optional for now
		if(hashItemData.Contains("UnlockAtLevel")){
			unlockAtLevel = XMLUtils.GetInt(hashItemData["UnlockAtLevel"] as IXMLNode, 0);
		}
		if(hashItemData.Contains("SortCategory")){
			sortCategory = XMLUtils.GetInt(hashItemData["SortCategory"] as IXMLNode, 0);
		}
		if(hashItemData.Contains("Sound")){
			soundUsed = XMLUtils.GetString(hashItemData["Sound"] as IXMLNode, "");
		}
		if(hashItemData.Contains("IsSecretItem")){
			isSecretItem = XMLUtils.GetBool(hashItemData["IsSecretItem"] as IXMLNode, false);
		}
	}
	
	/// <summary>
	/// Loads the stats. Retunrs all attributes of all children in a hashtable
	/// </summary>
	/// <returns>The stats.</returns>
	/// <param name="node">Node.</param>
	protected Dictionary<StatType, int> LoadStats(IXMLNode node){
		Dictionary<StatType, int> statsDict = new Dictionary<StatType, int>();
		List<IXMLNode> childrenList = XMLUtils.GetChildrenList(node);

		foreach(IXMLNode xmlNode in childrenList){
			Hashtable attrHash = XMLUtils.GetAttributes(xmlNode);
			StatType statType = (StatType)Enum.Parse(typeof(StatType), (string)attrHash["ID"]);
			int amount = Convert.ToInt32(attrHash["Amount"]);
			statsDict.Add(statType, amount);
		}
		return statsDict;
	}
}
