using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DoNotSerializePublic]
public class DojoRawData{
    private class Skill{
        public bool IsUnlocked {get; set;}
        public bool IsPurchased {get; set;}

        public Skill(bool isUnlocked, bool isPurchased){
            IsUnlocked = isUnlocked;
            IsPurchased = isPurchased;
        }
    } 
    [SerializeThis]
    private Dictionary<int, Skill> skills; //Key: ID of the Dojo Skill, Value: skill data 
    
    //===============Getters & Setters=================
    //Get IsUnlocked field for entity with skillID
    //if the entity with skillID can't be found that means it hasn't been initialized
    //add the entity into the dictionary
    public bool GetSkillIsUnlocked(int skillID){
        bool retVal = false;
        Skill skill; 
        if(skills.TryGetValue(skillID, out skill)){
            retVal = skill.IsUnlocked;
        }else{
            skill = new Skill(false, false);
            skills.Add(skillID, skill);
            retVal = skill.IsUnlocked;
        }
        return retVal;
    }
    public bool GetSkillIsPurchased(int skillID){
        bool retVal = false;
        Skill skill;
        if(skills.TryGetValue(skillID, out skill)){
            retVal = skill.IsPurchased; 
        }
        return retVal;
    }
    //Set IsUnlocked field for entity with skillID
    public void SetSkillIsUnlocked(int skillID, bool value){
        Skill skill;
        if(skills.TryGetValue(skillID, out skill)){
            skill.IsUnlocked = value;
        }
    }
    public void SetSkillIsPurchased(int skillID, bool value){
        Skill skill;
        if(skills.TryGetValue(skillID, out skill)){
            skill.IsPurchased = value;
        }
    }
    //================Initialization============
    public DojoRawData(){}

    public void Init(){
        skills = new Dictionary<int, Skill>();
    }
}
