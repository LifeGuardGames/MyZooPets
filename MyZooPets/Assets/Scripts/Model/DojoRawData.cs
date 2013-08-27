using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DoNotSerializePublic]
public class DojoRawData{
    private struct Skill{
        public bool IsUnlocked {get; set;}
        public bool IsPurchased {get; set;}

        public Skill(bool isUnlocked, bool isPurchased){
            IsUnlocked = isUnlocked;
            IsPurchased = isPurchased;
        }
    } 
    [SerializeThis]
    private Dictionary<int, Skill> skillsDict; //Key: ID of the Dojo Skill, Value: skill data 
    
    //===============Getters & Setters=================
    /*
        Get IsUnlocked field for entity with skillID
        if the entity with skillID can't be found that means it hasn't been initialized
        add the entity into the dictionary

        This is assuming that GetSkillIsUnlocked will be call first.
    */
    public bool GetSkillIsUnlocked(int skillID){
        bool retVal = false;
        Skill skill; 
        if(skillsDict.TryGetValue(skillID, out skill)){
            retVal = skill.IsUnlocked;
        }else{
            skill = new Skill(false, false);
            skillsDict.Add(skillID, skill);
            retVal = skill.IsUnlocked;
        }
        return retVal;
    }
    public bool GetSkillIsPurchased(int skillID){
        bool retVal = false;
        Skill skill;
        if(skillsDict.TryGetValue(skillID, out skill)){
            retVal = skill.IsPurchased; 
        }
        return retVal;
    }
    //Set IsUnlocked field for entity with skillID
    public void SetSkillIsUnlocked(int skillID, bool value){
        if(skillsDict.ContainsKey(skillID)){
            Skill skill = skillsDict[skillID];
            skill.IsUnlocked = value;
            skillsDict[skillID] = skill;
        }
    }
    public void SetSkillIsPurchased(int skillID, bool value){
        if(skillsDict.ContainsKey(skillID)){
            Skill skill = skillsDict[skillID];
            skill.IsPurchased = value;
            skillsDict[skillID] = skill;
        }
    }
    //================Initialization============
    public DojoRawData(){}

    public void Init(){
        skillsDict = new Dictionary<int, Skill>();
    }
}
