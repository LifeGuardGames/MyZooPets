using UnityEngine;
using System.Collections;

public class ImmutableDataWave {

	private string waveID;
	public string Wave{
	get{ return waveID;}
	}

	private string numOfEnemies;
	public string NumOfEnemies{
		get{ return numOfEnemies;}
	}

	private string begEnemies;
	public string BegEnemies{
		get{ return begEnemies;}
	}

	private string mediumEnemies;
	public string MediumEnemies{
		get{ return mediumEnemies;}
	}

	private string hardEnemies;
	public string HardEnemies{
		get{ return hardEnemies;}
	}

	private string powerUp;
	public string PowerUp{
		get{ return powerUp;}
	}

	public ImmutableDataWave(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		this.waveID = id;
		//wave = XMLUtils.GetString(hashElements["Wave"] as IXMLNode, null, error);
		numOfEnemies = XMLUtils.GetString(hashElements["NumOfEnemies"] as IXMLNode, null, error);
		begEnemies = XMLUtils.GetString(hashElements["BasicEnemies"] as IXMLNode, null, error);
		mediumEnemies = XMLUtils.GetString(hashElements["MediumEnemies"] as IXMLNode, null, error);
		hardEnemies = XMLUtils.GetString(hashElements["HardEnemies"] as IXMLNode, null, error);
		powerUp = XMLUtils.GetString(hashElements["PowerUps"] as IXMLNode, null, error);
	}
}
