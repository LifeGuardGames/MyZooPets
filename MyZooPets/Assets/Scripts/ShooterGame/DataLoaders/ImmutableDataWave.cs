using UnityEngine;
using System.Collections;

public class ImmutableDataWave {

	private string waveID;
	public string Wave{
	get{ return waveID;}
	}

	private int totalEnemies;
	public int TotalEnemies{
		get{ return totalEnemies;}
	}

	private int begEnemiesCount;
	public int BegEnemiesCount{
		get{ return begEnemiesCount;}
	}

	private int mediumEnemiesCount;
	public int MediumEnemiesCount{
		get{ return mediumEnemiesCount;}
	}

	private int hardEnemiesCount;
	public int HardEnemiesCount{
		get{ return hardEnemiesCount;}
	}

	private int powerUpCount;
	public int PowerUpCount{
		get{ return powerUpCount;}
	}
	private bool bossRound;
	public bool BossRound{
		get{return bossRound;;}
	}

	public ImmutableDataWave(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		this.waveID = id;
		totalEnemies = XMLUtils.GetInt(hashElements["NumOfEnemies"] as IXMLNode, 0, error);
		begEnemiesCount = XMLUtils.GetInt(hashElements["BasicEnemies"] as IXMLNode, 0, error);
		mediumEnemiesCount = XMLUtils.GetInt(hashElements["MediumEnemies"] as IXMLNode, 0, error);
		hardEnemiesCount = XMLUtils.GetInt(hashElements["HardEnemies"] as IXMLNode, 0, error);
		powerUpCount = XMLUtils.GetInt(hashElements["PowerUps"] as IXMLNode, 0, error);
		bossRound = XMLUtils.GetBool(hashElements["Bossround"] as IXMLNode);
	}
}
