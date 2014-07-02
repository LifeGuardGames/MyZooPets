using UnityEngine;
using System.Collections;

public class ImmutableDataGate{
	
	private string gateID; // id of the gate
	private string gateArea; // location of the gate
	private float screenPercentage; //stronger gate covers more screen space
	private int partition; // partition id of the gate
	private string monsterID; // id of the monster at this gate
	private RoomDirection swipeDirection; // the swipe direction that this monster is blocking
	private string[] taskUnlocks; // list of wellapad unlocks removing this makes available
	private bool isRecurring; // is this gate recurring? i.e. comes back to life after a set amount of time
	private string itemBoxID; // item box id this gate leaves behind once destroyed
	private float itemBoxPositionOffset; // offset from the position of the gate
	private string miniPetID; // id of the miniPet that will be unlocked when this gate is destroyed

	public string GetGateID(){
		return gateID;	
	}

	public string GetMiniPetID(){
		return miniPetID;
	}

	/// <summary>
	/// Gets the area.
	/// </summary>
	/// <returns>area that the gate is in</returns>
	public string GetArea(){
		return gateArea;	
	}

	public float GetScreenPercentage(){
		return screenPercentage / 100f;
	}

	/// <summary>
	/// Gets the partition.
	/// </summary>
	/// <returns>The room partition.</returns>
	public int GetPartition(){
		return partition;	
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
	
	public string[] GetTaskUnlocks(){
		return taskUnlocks;	
	}

	/// <summary>
	/// Determines whether this gate is recurring.
	/// </summary>
	/// <returns><c>true</c> if this instance is recurring; otherwise, <c>false</c>.</returns>
	public bool IsRecurring(){
		return isRecurring;	
	}

	/// <summary>
	/// Gets the item box ID.
	/// </summary>
	/// <returns>The item box ID.</returns>
	public string GetItemBoxID(){
		return itemBoxID;	
	}

	public float GetItemBoxPositionOffset(){
		return itemBoxPositionOffset;
	}
	
	public ImmutableDataGate(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);

		gateID = id;	

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
		itemBoxID = XMLUtils.GetString(hashElements["ItemBoxID"] as IXMLNode, null, error);

		itemBoxPositionOffset = XMLUtils.GetFloat(hashElements["ItemBoxPositionOffset"] as IXMLNode, 0, error);
		
		// get the direction the gate is blocking
		swipeDirection = (RoomDirection)System.Enum.Parse(typeof(RoomDirection), 
		                  	XMLUtils.GetString(hashElements["Blocking"] as IXMLNode, null, error));

		// get list of wellapad unlocks
		if(hashElements.ContainsKey("TaskUnlocks")){
			string strUnlocks = XMLUtils.GetString(hashElements["TaskUnlocks"] as IXMLNode);
			taskUnlocks = strUnlocks.Split(","[0]);
		}

		if(hashElements.ContainsKey("MiniPetID"))
			miniPetID = XMLUtils.GetString(hashElements["MiniPetID"] as IXMLNode, null, error);
		
		//Debug.Log("Loading gate " + strID + " in loc " + strArea + " in partition " + nPartition + " and monster " + strMonsterID);
	}
}
