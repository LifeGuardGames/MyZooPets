using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataSounds
// Loads sound data from xml.
//---------------------------------------------------

public class DataSounds {

    private static Dictionary<string, DataSound> dictSounds = new Dictionary<string, DataSound>();
    private static bool dataLoaded = false; //Prohibit double loading data

    //Look for sound with id in the dictionary
    public static DataSound GetSoundData(string id){
        DataSound sound = null;

        if(dictSounds.ContainsKey(id))
            sound = dictSounds[id];
		else {
			Debug.Log("No such sound with id " + id + " -- creating one with default values");
			sound = new DataSound(id);
		}

        return sound;
	}

    public static void SetupData(){
        if(dataLoaded) return; //Don't load from xml if data already loaded

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
				
				DataSound sound = new DataSound( id, hashAttr );
				
	           	// store the sound
	            dictSounds.Add(id, sound);		
            }
         }
         dataLoaded = true;
    }
}

