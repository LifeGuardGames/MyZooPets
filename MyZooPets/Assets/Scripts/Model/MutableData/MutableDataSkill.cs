using System.Collections.Generic;

//---------------------------------------------------
// SkillMutableData 
// Save the data for skills/dojo. Mutable data
//---------------------------------------------------
public class MutableDataSkill{
    public class Status{
        public bool IsUnlocked {get; set;}

        public Status(){}

        public Status(bool isUnlocked){
            IsUnlocked = isUnlocked;
        }
    }

    public Dictionary<string, Status> SkillStatus {get; set;} //Key: Skill ID, Value: Instance of Status
    public string CurrentSkillID {get; set;} //The current skill that the pet has

    //---------------------------------------------------
    // GetIsUnlocked()
    // Check if skill with skillID has been unlocked 
    //---------------------------------------------------
    public bool GetIsUnlocked(string skillID){
        bool retVal = false;
        if(SkillStatus.ContainsKey(skillID)){
            retVal = SkillStatus[skillID].IsUnlocked;
        }
        return retVal;
    }

    //---------------------------------------------------
    // UpdateSkillStatus
    // Change skill with skillID from locked to unlocked
    //---------------------------------------------------
    public void UpdateSkillStatus(string skillID, bool isUnlocked){
        if(SkillStatus.ContainsKey(skillID)){
            Status status = SkillStatus[skillID];
            status.IsUnlocked = isUnlocked;
            SkillStatus[skillID] = status;
        }else{
            Status status = new Status(isUnlocked);
            SkillStatus.Add(skillID, status);
        }
    }


    //========================Initialization===================================
    public MutableDataSkill(){
        Init();
    }

    private void Init(){
        SkillStatus = new Dictionary<string, Status>();
        UpdateSkillStatus("Flame_1", true);
        CurrentSkillID = "Flame_1";
    }
}