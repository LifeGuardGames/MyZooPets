using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataLoaderPetAnimations
// Loads pet animation data from xml.
//---------------------------------------------------

public class DataLoaderPetAnimations {
	// hashtable that contains animations for the pet.
	// this is a bit crazy.
	// this is a hash of a hash of a hash of lists
	// first hash: pet health
	// second hash: pet mood
	// third hash: animation categories
	// and the final list is of animations that fit the bill
	private static Hashtable hashData;
    private static bool dataLoaded = false; //Prohibit double loading data

    //Look for sound with id in the dictionary
    public static DataPetAnimation GetData(string id){
        DataPetAnimation data = null;
		/*
        if(dictSounds.ContainsKey(id))
            sound = dictSounds[id];
		else {
			Debug.Log("No such sound with id " + id + " -- creating one with default values");
			sound = new DataSound(id);
		}
		 */
        return data;
	}

    public static void SetupData(){
        if(dataLoaded) return; //Don't load from xml if data already loaded

        //Load all item xml files
         UnityEngine.Object[] files = Resources.LoadAll("PetAnimations", typeof(TextAsset));
         foreach(TextAsset file in files){
            string xmlString = file.text;

            //Create XMLParser instance
            XMLParser xmlParser = new XMLParser(xmlString);

            //Call the parser to build the IXMLNode objects
            XMLElement xmlElement = xmlParser.Parse();

            //Go through all child node of xmlElement (the parent of the file)
            for(int i=0; i<xmlElement.Children.Count; i++){
                IXMLNode childNode = xmlElement.Children[i];

                // Get id
                Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
                string id = (string)hashAttr["ID"];
				
                // Get  properties from xml node
                Hashtable hashData = XMLUtils.GetChildren(childNode);				
				
				DataPetAnimation data = new DataPetAnimation( id, hashAttr, hashData );
				
	           	// store the sound
	           // dictSounds.Add(id, sound);		
            }
         }
         dataLoaded = true;
    }
}

