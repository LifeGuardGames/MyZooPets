using UnityEngine;
using System.Collections;
using System;

[DoNotSerializePublic]
public class DegradData{
    [SerializeThis]
    private int id;
    [SerializeThis]
    private int positionId;
    [SerializeThis]
    private int prefabId;

    //================Getters & Setters
    public int ID{
        get{return id;}
    }

    public int PositionId{
        get{return positionId;}
    }

    public int PrefabId{
        get{return prefabId;}
    }

    public DegradData(int id, int positionId, int prefabId){
        this.id = id;
        this.positionId = positionId;
        this.prefabId = prefabId;
    }
    public DegradData(){}
}