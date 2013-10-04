using UnityEngine;
using System.Collections;
using System;

public class DegradData{
    public int ID {get; set;}
    public int PositionId {get; set;}
    public int PrefabId {get; set;}

    public DegradData(){}

    public DegradData(int id, int positionId, int prefabId){
        ID = id;
        PositionId = positionId;
        PrefabId = prefabId;
    }
}