using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DojoLogic : Singleton<DojoLogic> {
    public List<DojoUIData> dojoSkills = new List<DojoUIData>(); //list of dojo skills. for use in the inspector only

    //=======================Events========================
    public static EventHandler<EventArgs> OnNewDojoSkillUnlocked; //when there are changes to the dojo skills
    public static EventHandler<EventArgs> OnNotEnoughStars; //try to buy dojo skill, but doesn't have enough stars
    //======================================================

    //=====================API============================
    public int MAX_SKILLS_COUNT{
        get {return dojoSkills.Count;}
    }

    public List<DojoUIData> DojoSkills{
        get {return dojoSkills;}
    }

    public DojoUIData GetSkillWithID(int skillID){
        return dojoSkills.Find(skill => skill.SkillID == skillID);
    }

    //Buy the dojo skill with skillID
    public void BuySkill(int skillID){
        DojoUIData dojoSkill = dojoSkills.Find(entity => entity.SkillID == skillID);
        if(D.Assert(dojoSkill != null)){
            if(DataManager.Instance.Stats.Stars >= dojoSkill.CostStars){
                StatsController.Instance.ChangeStats(0, Vector3.zero, dojoSkill.CostStars * -1, 
                    Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);
                dojoSkill.IsPurchased = true;                   
            }else{
                if(D.Assert(OnNotEnoughStars != null, "OnNotEnoughStars has no listeners")) 
                    OnNotEnoughStars(this, EventArgs.Empty);
            }
        }
    }
    //======================================================

    void Start(){
        //listen to on level up event
        HUDAnimator.OnLevelUp += CheckForNewDojoSkills;
    }

    void OnDestroy(){
        HUDAnimator.OnLevelUp -= CheckForNewDojoSkills;
    }

    //Event listener. checks if new dojo skills can be unlocked when level up event is fired
    private void CheckForNewDojoSkills(object sender, EventArgs args){
        bool fireEvent = false;
        switch(DataManager.Instance.Level.CurrentLevel){
            case Level.Level3:
                UnlockSkills(1);
                UnlockSkills(2);
                fireEvent = true;
            break;
            case Level.Level6:
                UnlockSkills(3);
                UnlockSkills(4);
                fireEvent = true;
            break;
            case Level.Level9:
                UnlockSkills(5);
                UnlockSkills(6);
                fireEvent = true;
            break;
        }
        if(fireEvent){
           if(D.Assert(OnNewDojoSkillUnlocked != null, "OnNewDojoSkillUnlocked has no listeners"))
                OnNewDojoSkillUnlocked(this, EventArgs.Empty);
        }
    }

    private void UnlockSkills(int skillID){
        DojoUIData skill = dojoSkills.Find(entity => entity.SkillID == skillID);
        skill.IsUnlocked = true;    
    }
}
