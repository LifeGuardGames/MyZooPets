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
    public static EventHandler<EventArgs> OnNotEnoughStars; //try to buy dojo skill, but doesn't have enough stars
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

    void Start(){
        //listen to on level up event
        // HUDAnimator.OnLevelUp += CheckForNewDojoSkills;
    }

    void OnDestroy(){
        // HUDAnimator.OnLevelUp -= CheckForNewDojoSkills;
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
