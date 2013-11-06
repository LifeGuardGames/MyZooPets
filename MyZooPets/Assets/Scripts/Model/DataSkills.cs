using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataSkills{
    private static Dictionary<string, Skill> allSkills = 
        new Dictionary<string, Skill>(); //Key: skillID, Value: instance of Skill.cs
    private static bool dataLoaded = false;
   
    //Return all the data for all Skills 
    public static Dictionary<string, Skill> GetAllSkills(){
        return allSkills;
    } 

    public static Skill GetSkill(string skillID){
		if ( allSkills.Count == 0 )
			SetupData();
		
        Skill skill = null;
        if(allSkills.ContainsKey(skillID)){
            skill = allSkills[skillID];
        }
        return skill;
    }

    public static void SetupData(){
        if(dataLoaded) return;

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
        dataLoaded = true;
    }
}