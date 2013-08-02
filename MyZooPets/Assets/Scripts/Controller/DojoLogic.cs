using UnityEngine;
using System.Collections;

/*
    Make sure that max_skills_count accurately reflects how many skills there are in total (both locked and unlocked).
*/
public class DojoLogic : Singleton<DojoLogic> {

    private static int max_skills_count = 3;
    private static int numSkillsUnlocked;
    private Level lastRecordedLevel;
    private DojoSkill[] skills;

    public static int MAX_SKILLS_COUNT{
        get {return max_skills_count;}
    }

    public static int NumSkillsUnlocked{
        get {return numSkillsUnlocked;}
    }

    public void Buy(DojoSkillType skillType){
        if (DataManager.Instance.Dojo.NumOfPurchasedSkills < numSkillsUnlocked){ // Still enough skill slots
            int index = (int)skillType;
            DojoSkill skill = skills[index];
            if (DataManager.Instance.Stats.Stars >= skill.CostStars){ // can afford it
                DataManager.Instance.Stats.SubtractStars(skill.CostStars);
                DataManager.Instance.Dojo.PurchasedSkills[index] = true;
                DataManager.Instance.Dojo.NumOfPurchasedSkills ++;
            }
        }
    }

    public DojoSkill GetSkill (DojoSkillType skillType){
        return skills[(int)skillType];
    }

    void Start(){
        UpdateNumSkillsShown();
        InitSkillsList();
        Buy(DojoSkillType.Backflip); // unlock starter skill
    }

    // todo: fill in rest of descriptions
    void InitSkillsList(){
        skills = new DojoSkill[max_skills_count];
        DojoSkill skill;

        skill = new DojoSkill("Sit", "Just sitting. Nothing spectacular.", "NoIconName", "NoPetAnimationName", "NoGestureAnimationName", 0); // starter skill, so costs nothing
        skills[(int)DojoSkillType.Sit] = skill;

        skill = new DojoSkill("Wave", "Say hello!", "NoIconName", "NoPetAnimationName", "NoGestureAnimationName", 200);
        skills[(int)DojoSkillType.Wave] = skill;

        skill = new DojoSkill("Backflip", "A superb backflip.", "NoIconName", "NoPetAnimationName", "NoGestureAnimationName", 0);
        skills[(int)DojoSkillType.Backflip] = skill;

    }

    void Update(){
        //To Do. define a listener for OnLevelUp
        // check if can unlock additional skills
        if (DataManager.Instance.Level.CurrentLevel != lastRecordedLevel){
            UpdateNumSkillsShown();
            lastRecordedLevel = DataManager.Instance.Level.CurrentLevel;
        }
    }

    void UpdateNumSkillsShown(){
        numSkillsUnlocked = (int)DataManager.Instance.Level.CurrentLevel / 3 * 2; // 2 additional skill slots unlocked every 3 levels
        numSkillsUnlocked += 1; // to account for the first skill given by default
    }

    // //todo: change return type
    // void GetPetAnimationForSkill(DojoSkillType skillType){

    // }
    // //todo: change return type
    // void GetGestureAnimationForSkill(DojoSkillType skillType){

    // }
    // // string GetDescForSkill(DojoSkillType skillType){
    // //     return skills[(int)skillType].Desc;
    // // }
    // //todo: change return type
    // void GetIconForSkill(DojoSkillType skillType){
    // }
}
