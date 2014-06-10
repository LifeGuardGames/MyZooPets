using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// XML loader generic.
/// Loads xml in the following format:
/// <Names>
///    <Name ID=""></Name>
/// </Names>
/// 
/// Tag name can be defined however you want, but ID needs to be in the attribute
/// You can also define the elements however you want since you will also need to
/// provide a xmlNodeHandler to deinfe how you want to bind the xml element to a object property
/// </summary>
public class XMLLoaderGeneric{
	public delegate void XmlNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage);
	public XmlNodeHandler xmlNodeHandler; // the handler that binds xml element to a class property and add it to the hashData

	private Hashtable hashData;

	public string XmlFileFolderPath{ get; set;} // the loader will load all the xml from this folder

	/// <summary>
	/// Gets the data.
	/// </summary>
	public T GetData<T>(string id){
		if(hashData == null)
			SetupData();
		
		T data = default(T);
		if(hashData.ContainsKey(id))
			data = (T)hashData[id];
		else
			Debug.LogError("No such data for ID: " + id);
		
		return data;
	}
	
	private void SetupData(){
		hashData = new Hashtable();
		
		//Load all files from the folder
		UnityEngine.Object[] files = Resources.LoadAll(XmlFileFolderPath, typeof(TextAsset));
		foreach(TextAsset file in files){
			string xmlString = file.text;

			string errorMessage = "Error in file " + file.name + ": ";
			
			//Create XMLParser instance
			XMLParser xmlParser = new XMLParser(xmlString);
			
			//Call the parser to build the IXMLNode objects
			XMLElement xmlElement = xmlParser.Parse();
			
			//Go through all child node of xmlElement (the parent of the file)
			for(int i=0; i<xmlElement.Children.Count; i++){
				IXMLNode childNode = xmlElement.Children[i];

				//Get id
				Hashtable hashAttr = XMLUtils.GetAttributes(childNode);

				string id = "";
				if(hashAttr.ContainsKey("ID")){
					id = (string)hashAttr["ID"];
					errorMessage = errorMessage + "(" + id + "): ";
				}else{
					Debug.LogError("XML format error. Node needs to have ID. Read class summary for more explanation");
				}

				if(xmlNodeHandler != null)
					xmlNodeHandler(id, childNode, hashData, errorMessage);
				else
					Debug.LogError("xmlNodeHandler is not defined. This method implementation is required" +
						"to match the correct xml element to correct c# object");	
			}
		}				
	}
}
