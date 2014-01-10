using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataSkills{
    private static Dictionary<string, Skill> allSkills; //Key: skillID, Value: instance of Skill.cs
   
    //Return all the data for all Skills 
    public static Dictionary<string, Skill> GetAllSkills(){
		if ( allSkills == null )
			SetupData();
		
        return allSkills;
    } 

    public static Skill GetSkill(string skillID){
		Dictionary<string, Skill> allSkills = GetAllSkills();
			
        Skill skill = null;
        
        if(allSkills.ContainsKey(skillID)){
            skill = allSkills[skillID];
        }
		else
			Debug.LogError("No skill data for " + skillID);

        return skill;
    }

    public static void SetupData(){
		allSkills = new Dictionary<string, Skill>();
		
        //Load from xml
        TextAsset file = (TextAsset) Resources.Load("Skills", typeof(TextAsset));
        string xmlString = file.text;

        //Create XMLParser instance
        XMLParser xmlParser = new XMLParser(xmlString);

        //Call the parser to build the IXMLNode objects
        XMLElement xmlElement = xmlParser.Parse();

        //Go through all child node of xmlElement (the parent of the file)
        for(int i=0; i<xmlElement.Children.Count; i++){
            IXMLNode childNode = xmlElement.Children[i];

            //Get id
            Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
            string skillID = (string)hashAttr["ID"]; 

            //Get badge properties from xml node
            Hashtable hashItemData = XMLUtils.GetChildren(childNode);

            Skill skill = null;
            skill = new Skill(skillID, hashItemData);

            allSkills.Add(skillID, skill);
        }
    }
}