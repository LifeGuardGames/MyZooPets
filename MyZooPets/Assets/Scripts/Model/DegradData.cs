using UnityEngine;
using System.Collections;
using System;

[DoNotSerializePublic]
public class DegradData{
    [SerializeThis]
    private int id;
    [SerializeThis]
    private Vector3 position;

    //================Getters & Setters
    public int ID{
        get{return id;}
    }

    public Vector3 Position{
        get{return position;}
    }

    public DegradData(int id, Vector3 position){
        this.id = id;
        this.position = position;
    }
    public DegradData(){}
}