using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[DoNotSerializePublic]
public class SkillMutableData{
    private struct Status{
        public bool isUnlocked;
        public bool isPurchased;

        public Status(bool isUnlocked, bool isPurchased){
            this.isUnlocked = isUnlocked;
            this.isPurchased = isPurchased;
        }
    }

    [SerializeThis]
    private Dictionary<string, Status> skillStatus; //Key: Skill ID, Value: Instance of Status
    [SerializeThis]
    private string currentSkillID; //The current skill that the pet has

    public string CurrentSkillID{
        get{return currentSkillID;}
    }

    public void UpdateSkillStatus(string skillID, bool isUnlocked, bool isPurchased){
        if(skillStatus.ContainsKey(skillID)){
            Status status = skillStatus[skillID];
            status.isUnlocked = isUnlocked;
            status.isPurchased = isPurchased;
            skillStatus[skillID] = status;
        }else{
            Status status = new Status(isUnlocked, isPurchased);
            skillStatus.Add(skillID, status);
        }
    }

    public bool GetIsUnlocked(string skillID){
        bool retVal = false;
        if(skillStatus.ContainsKey(skillID)){
            retVal = skillStatus[skillID].isUnlocked;
        }
        return retVal;
    }

    public bool GetIsPurchased(string skillID){
        bool retVal = false;
        if(skillStatus.ContainsKey(skillID)){
            retVal = skillStatus[skillID].isPurchased;
        }
        return retVal;
    }

    //========================Initialization===================================
    public SkillMutableData(){}

    public void Init(){
        skillStatus = new Dictionary<string, Status>();
        UpdateSkillStatus("Skill0", true, false);
    }
}