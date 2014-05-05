using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderTriggers{
    //Key: scene name, value: dictionary of ImmutableData_Trigger
    //Key: triggerID, value: instance of ImmutableData_Trigger.cs
    private static Dictionary<string, Dictionary<string, ImmutableData_Trigger>> allTriggers;

    //------------------------------------------------------------------
    // GetRandomSceneTrigger()
    // Return a random trigger from a specific scene
    //------------------------------------------------------------------
    public static ImmutableData_Trigger GetRandomSceneTrigger(string scene){
        ImmutableData_Trigger randomTrigger = null;

        if(allTriggers == null)
            SetupData();

        if(allTriggers.ContainsKey(scene)){
            Dictionary<string, ImmutableData_Trigger> sceneTriggers = allTriggers[scene];

            //Get random element from the dictionary
            randomTrigger = sceneTriggers.ElementAt(UnityEngine.Random.Range(0, sceneTriggers.Count)).Value;
        }
        return randomTrigger;
    }


    //------------------------------------------------------------------
    // GetTrigger()
    // Return the trigger data of with id = triggerID
    //------------------------------------------------------------------
    public static ImmutableData_Trigger GetTrigger(string triggerID){
        ImmutableData_Trigger triggerData = null;

        if(allTriggers == null)
            SetupData();

        //loop through all the values of allTriggers Dict
        Dictionary<string, Dictionary<string, ImmutableData_Trigger>>.ValueCollection valueColl = allTriggers.Values;
        foreach(Dictionary<string, ImmutableData_Trigger> sceneTriggers in valueColl){
            if(sceneTriggers.ContainsKey(triggerID)){
                triggerData = sceneTriggers[triggerID];
                break;
            }
        }

        // if(allTriggers.ContainsKey(scene)){
        //     Dictionary<string, ImmutableData_Trigger> sceneTriggers = allTriggers[scene];

        //     if(sceneTriggers.ContainsKey(triggerID))
        //         triggerData = sceneTriggers[triggerID];

        // }

        return triggerData;
    }

    private static void SetupData(){
        allTriggers = new Dictionary<string, Dictionary<string, ImmutableData_Trigger>>();

        //Load all item xml files
         UnityEngine.Object[] files = Resources.LoadAll("Triggers", typeof(TextAsset));
         foreach(TextAsset file in files){
            string xmlString = file.text;

            //Create XMLParser instance
            XMLParser xmlParser = new XMLParser(xmlString);

            //Call the parser to build the IXMLNode objects
            XMLElement xmlElement = xmlParser.Parse();

            foreach(IXMLNode childNode in xmlElement.Children){

                //Get id
                Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
                string id = (string) hashAttr["ID"];

                //Get trigger properties from xml node
                Hashtable hashTriggerData = XMLUtils.GetChildren(childNode);

                ImmutableData_Trigger triggerData = new ImmutableData_Trigger(id, hashTriggerData);

                string scene = triggerData.Scene;
                if(!allTriggers.ContainsKey(scene))
                    allTriggers.Add(scene, new Dictionary<string, ImmutableData_Trigger>());

                Dictionary<string, ImmutableData_Trigger> sceneTriggers = allTriggers[scene];
                if(sceneTriggers.ContainsKey(id))
                    Debug.LogError("Duplicate trigger id: " + id + " for " + scene);
                else
                    sceneTriggers.Add(id, triggerData);
            }
        }
    }
}
