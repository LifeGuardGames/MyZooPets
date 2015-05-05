using UnityEngine;
using System.Collections;

public class ImmutableDataGate{
	
	private string gateID; // id of the gate
	private int gateNumber ;// sequential order of the gates, used for gate comparison
	private string zone; // location of the gate
	private float screenPercentage; //stronger gate covers more screen space //DEPRECATED
	private int partition; // partition id of the gate
	private int partitionZoneOffset; //Offset for calculating partitions in different scenes
	private string monsterID; // id of the monster at this gate
	private RoomDirection swipeDirection; // the swipe direction that this monster is blocking
	private string[] taskUnlocks; // list of wellapad unlocks removing this makes available
	private bool isRecurring; // is this gate recurring? i.e. comes back to life after a set amount of time
	private string miniPetID; // id of the miniPet that will be unlocked when this gate is destroyed

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
	/// Gets the gate zone
	/// </summary>
	/// <value>The gate area/zone</value>
	public string Zone{
		get{ return zone; }
	}

	public float ScreenPercentage{
		get{ return screenPercentage / 100f; }
	}

	public int Partition{
		get{ return partition; }
	}

	public int PartitionZoneOffset{
		get{ return partitionZoneOffset; }
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

	public ImmutableDataGate(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);

		gateID = id;

		// get gate number
		gateNumber = XMLUtils.GetInt(hashElements["GateNumber"] as IXMLNode, -1, error);

		// get location
		zone = XMLUtils.GetString(hashElements["Zone"] as IXMLNode, null, error);
		
		// get partition
		partition = XMLUtils.GetInt(hashElements["Partition"] as IXMLNode, 0, error);

		// get partition zone offset
		if(hashElements.ContainsKey("PartitionZoneOffset")){
			partitionZoneOffset = XMLUtils.GetInt(hashElements["PartitionZoneOffset"] as IXMLNode, -1, error);
		}
		else{
			partitionZoneOffset = -1;
		}

		// get screen percentage
		screenPercentage = XMLUtils.GetFloat(hashElements["ScreenPercentage"] as IXMLNode, 30, error);
		
		// get monster
		monsterID = XMLUtils.GetString(hashElements["MonsterID"] as IXMLNode, null, error);
		
		// get whether or not the gate recurs
		isRecurring = XMLUtils.GetBool(hashElements["Recurring"] as IXMLNode, false);

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
