using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataPetLevels{
    private static Dictionary<Level, PetLevel> allLevels =
        new Dictionary<Level, PetLevel>(); //Key: LevelID, Value: instance of PetLevel.cs
    private static bool dataLoaded = false;

    public static PetLevel GetLevel(Level id){
        PetLevel petLv = null;

        if(allLevels.ContainsKey(id)){
            petLv = allLevels[id];
        }

        return petLv;
    }

    public static void SetupData(){
        if(dataLoaded) return;

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

        dataLoaded = true;
    }
}
