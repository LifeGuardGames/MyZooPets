using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataPetLevels{
    private static Dictionary<Level, PetLevel> allLevels; //Key: LevelID, Value: instance of PetLevel.cs

    public static PetLevel GetLevel(Level id){
		Dictionary<Level, PetLevel> allLevels = GetData();
		
        PetLevel petLv = null;

        if(allLevels.ContainsKey(id)){
            petLv = allLevels[id];
        }

        return petLv;
    }
	
	private static Dictionary<Level, PetLevel> GetData() {
		if ( allLevels == null )
			SetupData();
		
		return allLevels;
	}

    public static void SetupData(){
		allLevels = new Dictionary<Level, PetLevel>();
		
         //Load from xml
        TextAsset file = (TextAsset) Resources.Load("PetLevels", typeof(TextAsset));
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
            Level petLevelID = (Level)Enum.Parse(typeof(Level), (string) hashAttr["ID"]); 

            //Get badge properties from xml node
            Hashtable hashItemData = XMLUtils.GetChildren(childNode);

            PetLevel petLevel = null;
            petLevel = new PetLevel(petLevelID, hashItemData);

            allLevels.Add(petLevelID, petLevel);
        }
    }
}
