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

    private void InitSkills(){
        List<Skill> skills = DojoLogic.Instance.AllSkills;

        foreach(Skill skill in skills){
            GameObject skillGO = NGUITools.AddChild(skillGrid, skillPrefab);
            skillGO.name = skill.ID;

            skillGO.transform.Find("Label_Damage").GetComponent<UILabel>().text = skill.GetDamagePointString();
            skillGO.transform.Find("Label_Name").GetComponent<UILabel>().text = skill.Name;
            skillGO.transform.Find("Label_UnlockLevel").GetComponent<UILabel>().text = skill.GetUnlockLevelString();

            GameObject buttonGO = skillGO.transform.Find("Button_Buy").gameObject;
            buttonGO.GetComponent<UIButtonMessage>().target = this.gameObject;
            buttonGO.GetComponent<UIButtonMessage>().functionName = "BuySkill";
            buttonGO.transform.Find("Label_Cost").GetComponent<UILabel>().text = skill.Cost.ToString();

            //TO DO: Need to figure how unlock and purchased item will be displayed
            if(!skill.IsUnlocked){
                buttonGO.GetComponent<UIImageButton>().isEnabled = false;
            }

        }
    }

    private void UnlockSkill(object senders, DojoLogic.SkillEventArgs arg){

    }

    private void BuySkill(GameObject go){

    }
}
