using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// FlameLevelUpLogic 
// Manager layer for the fire levelup
//---------------------------------------------------
public class FlameLevelLogic : Singleton<FlameLevelLogic> {
    public static EventHandler<FlameLevelEventArgs> OnFlameLevelUp;
    public class FlameLevelEventArgs : EventArgs{
        private Skill unlockedSkill;

        public Skill UnlockedSkill{
            get{return unlockedSkill;}
        }

        public FlameLevelEventArgs(Skill skill){
            unlockedSkill = skill;
        }
    }

    private List<Skill> allSkills;

    void Awake(){
        DataSkills.SetupData();
    }	

    void Start(){
        HUDAnimator.OnLevelUp += CheckFlameLevelUp;
    }

    void OnDestroy(){
        HUDAnimator.OnLevelUp -= CheckFlameLevelUp;
    }

	//---------------------------------------------------
	// GetSkillData()
	// Returns the skill data for the incoming skill id.
	//---------------------------------------------------
	public Skill GetSkillData( string strID ) {
		Skill data = DataSkills.GetSkill( strID );
		return data;
	}

    public Skill GetCurrentSkill(){
        Skill currentSkill = DataSkills.GetSkill(DataManager.Instance.GameData.Flame.CurrentSkillID);
        return currentSkill;
    }

    private void CheckFlameLevelUp(object sender, EventArgs args){
        if(allSkills == null){
            allSkills = SelectListFromDictionaryAndSort(DataSkills.GetAllSkills());
        }

        int currentLevel = (int) LevelLogic.Instance.CurrentLevel;
        foreach(Skill skill in allSkills){
            if(skill.UnlockLevel == currentLevel){
                if(!skill.IsUnlocked){
                    DataManager.Instance.GameData.Flame.UpdateSkillStatus(skill.ID, true);
                    DataManager.Instance.GameData.Flame.CurrentSkillID = skill.ID;

                    if(OnFlameLevelUp != null){
                        FlameLevelEventArgs flameArgs = new FlameLevelEventArgs(skill);
                        OnFlameLevelUp(this, flameArgs);
                    }
                }
            }
        }
    }

    private List<Skill> SelectListFromDictionaryAndSort(Dictionary<string, Skill> skillDict){
        var skills = from keyValuePair in skillDict
                        select keyValuePair.Value;
        List<Skill> skillList = (from skill in skills
                                orderby skill.UnlockLevel ascending
                                select skill).ToList();
        return skillList;
    }


}

        // replace this incoming skill as our current skill if it is better (or current skill is null)
        // if ( string.IsNullOrEmpty( CurrentSkillID ) )
        //  CurrentSkillID = skillID;
        // else {
        //  Skill newSkill = DataSkills.GetSkill( skillID );
        //  Skill curSkill = DataSkills.GetSkill( CurrentSkillID );
            
        //  if ( newSkill.DamagePoint > curSkill.DamagePoint )
        //      CurrentSkillID = skillID;
        // }
