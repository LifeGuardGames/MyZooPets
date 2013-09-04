using UnityEngine;
using System;
using System.Collections;

[DoNotSerializePublic]
public class DecorationSystemData{

    [SerializeThis]
    private int decoTest;
	
    //===============Getters & Setters=================
    public int DecoTest{
        get{return decoTest;}
        set{decoTest = value;}
    }	
	
    public bool UseDummyData{get; set;} //initialize with test data

    //=======================Initialization==================
    public DecorationSystemData(){}

    //Populate with dummy data
    public void Init(){
        decoTest = 111;
    }
}
