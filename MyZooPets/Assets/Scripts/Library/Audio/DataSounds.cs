using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataSounds
// Loads sound data from xml.
//---------------------------------------------------
public class DataSounds{
	private static Dictionary<string, DataSound> dictSounds;

	//Look for sound with id in the dictionary
	public static DataSound GetSoundData(string clipName, int variations = 1){
		if(dictSounds == null){
			SetupData();
		}
		
		DataSound sound = null;

		// If there is more than one variation, append a number to it randomly
		if(variations > 1){
			int randomVariation = UnityEngine.Random.Range(1, variations + 1);
			clipName = clipName + randomVariation.ToString();
		}

		if(dictSounds.ContainsKey(clipName))
			sound = dictSounds[clipName];
		else{
			Debug.LogError("No such sound with id " + clipName + " -- creating one with default values");
			sound = new DataSound(clipName);
		}
		return sound;
	}

	public static void SetupData(){
		dictSounds = new Dictionary<string, DataSound>();
		
		//Load all item xml files
		UnityEngine.Object[] files = Resources.LoadAll("Sounds", typeof(TextAsset));
		foreach(TextAsset file in files){
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
				string id = (string)hashAttr["ID"];
				
				DataSound sound = new DataSound(id, hashAttr);
				
				// store the sound
				dictSounds.Add(id, sound);		
			}
		}
	}
}

