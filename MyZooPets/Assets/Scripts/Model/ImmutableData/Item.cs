using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Immutable item data
/// </summary>
public class Item{
	private string id;
	public string ID {
		get { return id; }
	}

	private string name;
	public string Name {
		get { return Localization.Localize(name); }
	}

	private string textureName;
	public string TextureName {
		get { return textureName; }
	}

	private ItemType type;
	public ItemType Type {
		get { return type; }
	}

	private int cost;
	public int Cost {
		get { return cost; }
	}

	private CurrencyTypes currencyType;
	public CurrencyTypes CurrencyType {
		get { return currencyType; }
	}

	protected string description;
	public virtual string Description {
		get {
			string retVal = "";
			if(!string.IsNullOrEmpty(description)) {
				retVal = Localization.Localize(description);
			}
			return retVal;
		}
	}

	private int unlockAtLevel = 0;
	public int UnlockAtLevel {
		get { return unlockAtLevel; }
	}

	private bool isSecretItem = false;
	public bool IsSecretItem {
		get { return isSecretItem; }
	}

	private int sortCategory;   // Options, use for category sorting (ie. accessories)
	public int SortCategory {
		get { return sortCategory; }
	}

	private string soundUsed; // sound item makes when it is used
	public string SoundUsed {
		get { return soundUsed; }
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
		currencyType = (CurrencyTypes)Enum.Parse(typeof(CurrencyTypes), 
		                  HashUtils.GetHashValue<string>(costAttributes, "CurrencyType", ""));
		cost = int.Parse(HashUtils.GetHashValue<string>(costAttributes, "Amount", ""));
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
