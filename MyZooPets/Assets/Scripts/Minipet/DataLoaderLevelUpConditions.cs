using UnityEngine;
using System.Collections;

public class DataLoaderLevelUpConditions{
	private static XMLLoaderGeneric xmlLoader;
	
	public static ImmutableDataMiniPetLevelUpConditions GetData(string id){
		if(xmlLoader == null){
			xmlLoader = new XMLLoaderGeneric();
			xmlLoader.XmlFileFolderPath = "MiniPetLevels";
			xmlLoader.xmlNodeHandler = NodeHandler;
		}
		
		return xmlLoader.GetData<ImmutableDataMiniPetLevelUpConditions>(id);
	}
	
	public static void NodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataMiniPetLevelUpConditions data = new ImmutableDataMiniPetLevelUpConditions(id, xmlNode, errorMessage); 
		
		// store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data); 
	}
}
