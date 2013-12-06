using UnityEngine;
using System.Collections;
using System;

public class DegradData{
    public int ID {get; set;}
    public int PrefabId {get; set;}
	
	private string strPosition;
	public Vector3 GetPosition() {
		return Constants.ParseVector3( strPosition );	
	}
	
    public DegradData(){}

    public DegradData(int id, string position, int prefabId){
        ID = id;
        strPosition = position;
        PrefabId = prefabId;
    }
}