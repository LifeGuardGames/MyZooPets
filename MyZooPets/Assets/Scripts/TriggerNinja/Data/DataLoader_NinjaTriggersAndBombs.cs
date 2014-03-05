using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataLoader_NinjaTriggersAndBombs{

	// Hashtable that contains all the NinjaTriggersAndBombs.xml data
	private static ArrayList triggerList = null;
	private static ArrayList bombList = null;

	public static int numTriggers = 0;
	public static int numBombs = 0;

	// Gets a random trigger given a combination number, priority head first
	public static string GetRandomTrigger(int combinations){
		return GetTrigger(UnityEngine.Random.Range(0, combinations));	// Exclusive max
	}

	// Gets a trigger from the list given index
	private static string GetTrigger(int index){
		if(triggerList == null){
			SetupData();
		}
		return triggerList[index] as string;
	}

	// Gets a random bomb given a combination number, priority head first
	public static string GetRandomBomb(int combinations){
		return GetBomb(UnityEngine.Random.Range(0, combinations));	// Exclusive max
	}
	
	// Gets a bomb from the list given index
	private static string GetBomb(int index){
		if(bombList == null){
			SetupData();
		}
		return bombList[index] as string;
	}

	// Initializing the data from XML
	private static void SetupData(){
		if(bombList != null || triggerList != null){
			Debug.LogError("Ninja object data already initialized!");
		}
		else{
			triggerList = new ArrayList();
			bombList = new ArrayList();

			TextAsset file = Resources.Load("Ninja/NinjaTriggersAndBombs", typeof(TextAsset)) as TextAsset;
			string xmlString = file.text;

			Debug.Log(xmlString);
			//Create XMLParser instance
			XMLParser xmlParser = new XMLParser(xmlString);

			//Call the parser to build the IXMLNode objects
			XMLElement xmlElement = xmlParser.Parse();

			foreach(IXMLNode childNode in xmlElement.Children){
				Debug.Log("SDFSDFDS");

				// Dont need ID for anything...
				Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
				string id = (string) hashAttr["ID"];
				Debug.Log("SDFSDFDS");
				Debug.Log(id);

				//Get trigger properties from xml node
				Hashtable hashTriggerData = XMLUtils.GetChildren(childNode);

				// Add to trigger list if type is trigger
				if(XMLUtils.GetString(hashTriggerData["Type"] as IXMLNode).Equals("Trigger")){
					triggerList.Add(XMLUtils.GetString(hashTriggerData["PrefabString"] as IXMLNode));
					numTriggers++;
				}
				// Add to bomb list if type is bomb
				else if(XMLUtils.GetString(hashTriggerData["Type"] as IXMLNode).Equals("Bomb")){
					bombList.Add(XMLUtils.GetString(hashTriggerData["PrefabString"] as IXMLNode));
					numBombs++;
				}
				else{
					Debug.LogError("Unknown ID inside NinjaTriggersAndBombs");
				}
			}
		}
	}
}