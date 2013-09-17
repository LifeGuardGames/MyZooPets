using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataBadges{
    private static Dictionary<string, Badge> allBadges =
        new Dictionary<string, Badge>(); //Key: badgeID, Value: instance of Badge.cs
    private static bool dataLoaded = false; //Prohibit double loading data

    //Return all the data for all badges
    public static Dictionary<string, Badge> GetAllBadges(){
        return allBadges;
    }

    //Return the Badge with badgeID
    public static Badge GetBadge(string badgeID){
        Badge badge = null;
        if(allBadges.ContainsKey(badgeID)){
            badge = allBadges[badgeID];
        }
        return badge;
    }

    public static void SetupData(){
        if(dataLoaded) return;

        //Load all item xml files
         TextAsset file = (TextAsset) Resources.Load("Badges", typeof(TextAsset));
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
            string badgeID = (string)hashAttr["ID"]; 

            //Get badge properties from xml node
            Hashtable hashItemData = XMLUtils.GetChildren(childNode);

            Badge badge = null;
            badge = new Badge(badgeID, hashItemData);

            allBadges.Add(badgeID, badge);
        }
        dataLoaded = true;
    } 
}