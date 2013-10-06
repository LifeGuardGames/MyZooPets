using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//----------------------
//Provides data for DojoUIManager
//Checks and manages when skill can be unlocked and purchased
public class DojoLogic : Singleton<DojoLogic> {
    public static EventHandler<EventArgs> OnNewDojoSkillUnlocked; //when there are changes to the dojo skills
    public class SkillEventArgs : EventArgs{
        private Skill unlockedSkill;

        public Skill UnlockedSkill{
            get{return unlockedSkill;}
        }

        public SkillEventArgs(Skill skill){
            unlockedSkill = skill;
        }
    }

    private List<Skill> allSkills;

    public List<Skill> AllSkills{
        get{return allSkills;}
    }

    void Awake(){
        DataSkills.SetupData();

        Dictionary<string, Skill> skillsDict = DataSkills.GetAllSkills();
        allSkills = SelectListFromDictionaryAndSort(skillsDict);
    }

    //Return skill with skillID
    public Skill GetSkill(string skillID){
        return DataSkills.GetSkill(skillID);
    }

    //Return the current skill that the pet is equipped with
    public Skill GetCurrentSkill(){
        string currentSkillID = DataManager.Instance.Dojo.CurrentSkillID;
        return DataSkills.GetSkill(currentSkillID);
    }

    //Buy skill with skillID. Update SkillMutableData
    public void BuySkill(string skillID){
        DataManager.Instance.Dojo.UpdateSkillStatus(skillID, true, true);
    }

    private void CheckForUnlockSkills(){
        
    }

    //Select list from dictionary and sort by unlocklevel
    private List<Skill> SelectListFromDictionaryAndSort(Dictionary<string, Skill> skillDict){
        var skills = from keyValuePair in skillDict
                        select keyValuePair.Value;
        List<Skill> skillList = (from skill in skills
                                orderby skill.UnlockLevel ascending
                                select skill).ToList();
        return skillList;
    }
}
