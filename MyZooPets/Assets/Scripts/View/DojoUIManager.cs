using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DojoUIManager : Singleton<DojoUIManager> {
    public GameObject skillPrefab;
    public GameObject skillGrid;

    void Start(){
        InitSkills();
    } 

    void OnDestroy(){

    }
	
	// list of flame UI objects
	private List<GameObject> listSkills = new List<GameObject>();

    //When the scene starts initializes all the skills once
    private void InitSkills(){
        List<Skill> skills = DojoLogic.Instance.AllSkills;

        foreach(Skill skill in skills){
            GameObject skillGO = NGUITools.AddChild(skillGrid, skillPrefab);
            skillGO.name = skill.ID;
			
			listSkills.Add( skillGO );

            skillGO.transform.Find("Sprite_ActionImage").GetComponent<UISprite>().spriteName = skill.TextureName;
            skillGO.transform.Find("Label_Damage").GetComponent<UILabel>().text = skill.GetDamagePointString();
            skillGO.transform.Find("Label_Name").GetComponent<UILabel>().text = skill.Name;
            skillGO.transform.Find("Label_UnlockLevel").GetComponent<UILabel>().text = skill.GetUnlockLevelString();

            GameObject buttonGO = skillGO.transform.Find("Button_Buy").gameObject;
            buttonGO.GetComponent<UIButtonMessage>().target = this.gameObject;
            buttonGO.GetComponent<UIButtonMessage>().functionName = "BuySkill";
            buttonGO.transform.Find("Label_Cost").GetComponent<UILabel>().text = skill.Cost.ToString();
        }
		
		UpdateSkillItems();
    }
	
	//---------------------------------------------------
	// UpdateSkillItems()
	// This function loops through all the skill items on
	// display and properly sets them to reflect how
	// the skill is used/available.
	//---------------------------------------------------		
	private void UpdateSkillItems() {
		foreach ( GameObject goSkill in listSkills ) {
			GameObject goButton = goSkill.transform.Find("Button_Buy").gameObject;
			Skill skill = DojoLogic.Instance.GetSkill( goSkill.name );
			
            // if the skill is locked, just disable it
            if(!skill.IsUnlocked)
                goButton.GetComponent<UIImageButton>().isEnabled = false;
			
			if ( skill.IsPurchased ) {
				// if the skill is already purchased, disable it and change the message
				goButton.GetComponent<UIImageButton>().isEnabled = false;
				
				// if the skill is equipped or owned, a different message is displayed
				string strLabel;
				if ( skill.IsEquipped )
					strLabel = "EQUIPPED";
				else
					strLabel = "OWNED";
				
				goButton.transform.Find("Label_Cost").GetComponent<UILabel>().text = Localization.Localize( strLabel );
			}
		}
	}

    private void UnlockSkill(object senders, DojoLogic.SkillEventArgs arg){
        //Get Skill from arg and find that specific skill in skillgrid. then unlock the skill
    }

    private void BuySkill(GameObject go){
        string skillID = go.transform.parent.name;
        Skill skill = DojoLogic.Instance.GetSkill(skillID);

        if(skill != null && DataManager.Instance.GameData.Stats.Stars >= skill.Cost){
			// mark the skill as purchased
            DojoLogic.Instance.BuySkill(skillID);
			
			// subtract cold hard cash from the player
			StatsController.Instance.ChangeStats(0, Vector3.zero, skill.Cost * -1, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);
			
			// update UI
			UpdateSkillItems();
        }
    }
}
