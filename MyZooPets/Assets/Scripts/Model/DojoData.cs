using UnityEngine;
using System.Collections;

[DoNotSerializePublic]
public class DojoData{
    [SerializeThis]
    private int numOfPurchasedSkills;
    [SerializeThis]
    private bool[] purchasedSkills; // store the data for purchased skills - enum tells us which is which

    //===============Getters & Setters=================
    public int NumOfPurchasedSkills{
        get{return numOfPurchasedSkills;}
        set{numOfPurchasedSkills = value;}
    }
    public bool[] PurchasedSkills{
        get{return purchasedSkills;}
        set{purchasedSkills = value;}
    }

    //================Initialization============
    public DojoData(){}

    public void Init(){
        numOfPurchasedSkills = 0;
        purchasedSkills = new bool[DojoLogic.MAX_SKILLS_COUNT];
        for(int i=0; i<DojoLogic.MAX_SKILLS_COUNT; i++){
            purchasedSkills[i] = false;
        }
    }
}
