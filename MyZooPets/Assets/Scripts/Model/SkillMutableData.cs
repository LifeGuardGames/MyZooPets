using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// SkillMutableData 
// Save the data for skills/dojo. Mutable data
//---------------------------------------------------
public class SkillMutableData{
    public class Status{
        public bool IsUnlocked {get; set;}
        public bool IsPurchased {get; set;}

        public Status(){}

        public Status(bool isUnlocked, bool isPurchased){
            IsUnlocked = isUnlocked;
            IsPurchased = isPurchased;
        }
    }

    public Dictionary<string, Status> SkillStatus {get; set;} //Key: Skill ID, Value: Instance of Status
    public string CurrentSkillID {get; set;} //The current skill that the pet has


    public void UpdateSkillStatus(string skillID, bool isUnlocked, bool isPurchased){
        if(SkillStatus.ContainsKey(skillID)){
            Status status = SkillStatus[skillID];
            status.IsUnlocked = isUnlocked;
            status.IsPurchased = isPurchased;
            SkillStatus[skillID] = status;
        }else{
            Status status = new Status(isUnlocked, isPurchased);
            SkillStatus.Add(skillID, status);
        }
    }

    public bool GetIsUnlocked(string skillID){
        bool retVal = false;
        if(SkillStatus.ContainsKey(skillID)){
            retVal = SkillStatus[skillID].IsUnlocked;
        }
        return retVal;
    }

    public bool GetIsPurchased(string skillID){
        bool retVal = false;
        if(SkillStatus.ContainsKey(skillID)){
            retVal = SkillStatus[skillID].IsPurchased;
        }
        return retVal;
    }

    //========================Initialization===================================
    public SkillMutableData(){}

    public void Init(){
        SkillStatus = new Dictionary<string, Status>();
        UpdateSkillStatus("Skill0", true, false);
    }
}