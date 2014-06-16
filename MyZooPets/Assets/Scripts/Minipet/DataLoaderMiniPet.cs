using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderMiniPet{
	private static XMLLoaderGeneric xmlLoader;

    public static ImmutableDataMiniPet GetData(string id){
		if(xmlLoader == null){
			xmlLoader = new XMLLoaderGeneric();
			xmlLoader.XmlFileFolderPath = "MiniPets";
			xmlLoader.xmlNodeHandler = NodeHandler;
		}

		return xmlLoader.GetData<ImmutableDataMiniPet>(id);
    }

	public static void NodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataMiniPet data = new ImmutableDataMiniPet(id, xmlNode, errorMessage); 
		            
        // store the data
        if(hashData.ContainsKey(id))
            Debug.LogError(errorMessage + "Duplicate keys!");
        else
            hashData.Add(id, data); 
	}
}

