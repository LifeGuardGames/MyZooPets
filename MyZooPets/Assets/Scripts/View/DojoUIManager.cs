using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DojoUIManager : Singleton<DojoUIManager> {
    public GameObject grid; //Parent of the unlockGroups
    public GameObject unlockGroupPrefab; //Template for the unlockGroups
    private int[] groupLabels = {3, 6, 9};
    private int groupCounter = 0;
    private List<GameObject> skillUIReferences; //list of UI components that represent all the skills 

    void Awake(){
        skillUIReferences = new List<GameObject>();
        InitUIReference();
    }

    void Start(){
        DojoLogic.OnNewDojoSkillUnlocked += UpdateDojoSkill;
        UpdateDojoSkill(this, EventArgs.Empty);
    }

    void OnDestroy(){
        DojoLogic.OnNewDojoSkillUnlocked -= UpdateDojoSkill;
    }

    //Called when Button_Buy from UI has been clicked
    public void BuySkill(GameObject go){
       DojoLogic.Instance.BuySkill(Convert.ToInt32(go.transform.parent.name));
    }

    //Called when Button_Preview from UI has been clicked
    public void PreviewSkill(GameObject go){
        print("testin");
    }

    //Called when Back Button Clicked
    public void CloseDojo(){
		// Handled by button
//        Application.LoadLevel("NewBedRoom");
    }

    //Event Listener. Update dojo skill UI when a new level is unlocked
    private void UpdateDojoSkill(object sender, EventArgs args){
        foreach(GameObject go in skillUIReferences){
            DojoUIData skill = DojoLogic.Instance.GetSkillWithID(Convert.ToInt32(go.name));
            if(!skill.IsUnlocked || skill.IsPurchased){
                go.transform.Find("Button_Buy").GetComponent<UIImageButton>().isEnabled = false;
            }
        }
    }

    //Spawn the unlockgroups and store them in skillUIReference
    private void InitUIReference(){
        List<DojoUIData> dojoSkills = DojoLogic.Instance.DojoSkills;
        GameObject unlockGroup = null;
        for(int i=0; i<dojoSkills.Count; i++){
            if(i % 2 == 0){ //Spawn new unlockGroup
                unlockGroup = NGUITools.AddChild(grid, unlockGroupPrefab);
				string strLevel = Localization.Localize( "LEVEL" );
                unlockGroup.transform.Find("Label_Level").GetComponent<UILabel>().text = strLevel + " " + groupLabels[groupCounter];
                groupCounter++;
            }
            Transform trans = unlockGroup.transform.Find("Skill");
            //Change the gameObject name to skillID
            trans.name = dojoSkills[i].SkillID.ToString();
            //Name and desc of the skill
			string strSkillKey = "SKILL_" + dojoSkills[i].SkillName;
            trans.Find("Label_Name").GetComponent<UILabel>().text = Localization.Localize( strSkillKey );
            //Fill in the cost
            trans.Find("Label_Cost").GetComponent<UILabel>().text = dojoSkills[i].CostStars.ToString();
            //Set the OnClick Target and functionName
            trans.Find("Button_Buy").GetComponent<UIButtonMessage>().target = gameObject;
            trans.Find("Button_Buy").GetComponent<UIButtonMessage>().functionName = "BuySkill";
            trans.Find("Button_Preview").GetComponent<UIButtonMessage>().target = gameObject;
            trans.Find("Button_Preview").GetComponent<UIButtonMessage>().functionName = "PreviewSkill";
            skillUIReferences.Add(trans.gameObject);
        }
    }
}
