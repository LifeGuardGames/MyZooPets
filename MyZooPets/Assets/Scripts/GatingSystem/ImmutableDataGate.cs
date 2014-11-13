using UnityEngine;
using System.Collections;

public class ImmutableDataGate{
	
	private string gateID; // id of the gate
	private int gateNumber ;// sequential order of the gates, used for gate comparison
	private string gateArea; // location of the gate
	private float screenPercentage; //stronger gate covers more screen space //DEPRECATED
	private int partition; // partition id of the gate
	private string monsterID; // id of the monster at this gate
	private RoomDirection swipeDirection; // the swipe direction that this monster is blocking
	private string[] taskUnlocks; // list of wellapad unlocks removing this makes available
	private bool isRecurring; // is this gate recurring? i.e. comes back to life after a set amount of time
	private string itemBoxID; // item box id this gate leaves behind once destroyed
	private float itemBoxPositionOffset; // offset from the position of the gate
	private string miniPetID; // id of the miniPet that will be unlocked when this gate is destroyed
	private string[] decoCategoriesStore;	// Categories of deco items to unlock in store if this is latest gate

	public string GateID{
		get{ return gateID; }
	}

	public int GateNumber{
		get{ return gateNumber; }
	}

	public string MiniPetID{
		get{ return miniPetID; }
	}

	/// <summary>
	/// Gets the gate area/zone
	/// </summary>
	/// <value>The gate area/zone</value>
	public string GateArea{
		get{ return gateArea; }
	}

	public float ScreenPercentage{
		get{ return screenPercentage / 100f; }
	}

	public int Partition{
		get{ return partition; }
	}

	/// <summary>
	/// Gets the monster.
	/// </summary>
	/// <returns>The monster data.</returns>
	public ImmutableDataMonster GetMonster(){
		return DataLoaderMonster.GetData(monsterID);	
	}
	
	public bool DoesBlock(RoomDirection eSwipeDirection){
		return this.swipeDirection == eSwipeDirection;
	}

	public string[] TaskUnlocks{
		get{ return taskUnlocks; }
	}

	public bool IsRecurring{
		get{ return isRecurring; }
	}

	public string ItemBoxID{
		get{ return itemBoxID; }
	}

	public float ItemBoxPositionOffset{
		get{ return itemBoxPositionOffset; }
	}

	public string[] DecoCategoriesStore{
		get{ return decoCategoriesStore; }
	}
	
	public ImmutableDataGate(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);

		gateID = id;

		// get gate number
		gateNumber = XMLUtils.GetInt(hashElements["GateNumber"] as IXMLNode, -1, error);

		// get location
		gateArea = XMLUtils.GetString(hashElements["Location"] as IXMLNode, null, error);
		
		// get partition
		partition = XMLUtils.GetInt(hashElements["Partition"] as IXMLNode, 0, error);

		// get screen percentage
		screenPercentage = XMLUtils.GetFloat(hashElements["ScreenPercentage"] as IXMLNode, 30, error);
		
		// get monster
		monsterID = XMLUtils.GetString(hashElements["MonsterID"] as IXMLNode, null, error);
		
		// get whether or not the gate recurs
		isRecurring = XMLUtils.GetBool(hashElements["Recurring"] as IXMLNode, false);
		
		// get an item box id gate leaves behind (if any)
		if(hashElements.ContainsKey("ItemBoxID"))
			itemBoxID = XMLUtils.GetString(hashElements["ItemBoxID"] as IXMLNode, null, error);

		if(hashElements.ContainsKey("ItemBoxPositionOffset"))
			itemBoxPositionOffset = XMLUtils.GetFloat(hashElements["ItemBoxPositionOffset"] as IXMLNode, 0, error);
		
		// get the direction the gate is blocking
		swipeDirection = (RoomDirection)System.Enum.Parse(typeof(RoomDirection), 
		                  	XMLUtils.GetString(hashElements["Blocking"] as IXMLNode, null, error));

		// get list of wellapad unlocks
		if(hashElements.ContainsKey("TaskUnlocks")){
			string strUnlocks = XMLUtils.GetString(hashElements["TaskUnlocks"] as IXMLNode);
			taskUnlocks = strUnlocks.Split(","[0]);
		}

		// get list of wellapad unlocks
		if(hashElements.ContainsKey("DecoTypeAllowed")){
			string strStoreCategories = XMLUtils.GetString(hashElements["DecoTypeAllowed"] as IXMLNode);
			decoCategoriesStore = strStoreCategories.Split(","[0]);
		}

		if(hashElements.ContainsKey("MiniPetID"))
			miniPetID = XMLUtils.GetString(hashElements["MiniPetID"] as IXMLNode, null, error);
		
		//Debug.Log("Loading gate " + strID + " in loc " + strArea + " in partition " + nPartition + " and monster " + strMonsterID);
	}
}
