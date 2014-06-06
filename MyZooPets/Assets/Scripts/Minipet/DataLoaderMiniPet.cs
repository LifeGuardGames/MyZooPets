using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderMiniPet{
	private static Dictionary<string, ImmutableDataMiniPet> allMiniPets;

    public static ImmutableDataMiniPet GetData(string id){
        ImmutableDataMiniPet data = null;
        if(allMiniPets == null)
            SetupData();

        if(allMiniPets.ContainsKey(id))
            data = allMiniPets[id];
        else
            Debug.LogError("No such mini pet with id " + id);

        return data;
    }

    public static void SetupData(){
        allMiniPets = new Dictionary<string, ImmutableDataMiniPet>();

        UnityEngine.Object[] files = Resources.LoadAll("MiniPets", typeof(TextAsset));
        foreach(TextAsset file in files){
            string xmlString = file.text;
            
            // error message
            string fileErrorMessage = "Error in file" + file.name;

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
                string errorMessage = fileErrorMessage + "(" + id + "): ";

                // Get  properties from xml node
                Hashtable hashElements = XMLUtils.GetChildren(childNode);
               
                ImmutableDataMiniPet data = new ImmutableDataMiniPet(id, hashElements, errorMessage); 
                
                // store the data
                if(allMiniPets.ContainsKey(id))
                    Debug.LogError(errorMessage + "Duplicate keys!");
                else
                    allMiniPets.Add(id, data);     
            }
        }
    }
}
