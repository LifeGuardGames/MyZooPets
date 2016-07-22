using UnityEngine;
using System;
using System.Linq;
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
public abstract class XMLLoaderGeneric<T> where T : new() {
	//Handles what kind of data structure to store the xml node in
	//Will be called for every node.
	protected abstract void XMLNodeHandler(string id, IXMLNode xmlNode, 
	                                       Hashtable hashData, string errorMessage);
	//Initialize the loader. Usually just the xmlFileFolderPath
	protected abstract void InitXMLLoader();

	protected static T mInstance; // caching an instance of T
		
	protected string xmlFileFolderPath; // file path

	private Hashtable hashData; // storing all the xml data

	/// <summary>
	/// instance of T class. Will create one if doesn't exist
	/// </summary>
	/// <value>The instance.</value>
	protected static T instance{
		get{
			if(mInstance == null)
				mInstance = new T();
			
			return mInstance;
		}
	}

	/// <summary>
	/// Gets the data of type T.
	/// </summary>
	protected U GetData<U>(string id){
		if(hashData == null)
			SetupData();
		
		U data = default(U);

		try{
			if(hashData.ContainsKey(id))
				data = (U)hashData[id];
			else
				Debug.LogError("No such data for ID: " + id);
		}
		catch(ArgumentNullException e){
			Debug.LogException(e);
		}
		
		return data;
	}

	protected bool IsDataExist(string id) {
		if(hashData == null)
			SetupData();

		return hashData.ContainsKey(id);
    }

	/// <summary>
	/// Gets the data list.
	/// </summary>
	protected List<U> GetDataList<U>(){
		if(hashData == null)
			SetupData();

		List<U> dataList = new List<U>();

		try{
			dataList = hashData.Values.OfType<U>().ToList();
		}
		catch(Exception e){
			Debug.LogException(e);
		}

		return dataList;
	}

	/// <summary>
	/// Forces the setup.
	/// </summary>
	protected void ForceSetup(){
		if(hashData == null)
			SetupData();
	}
	
	private void SetupData(){
		hashData = new Hashtable();
		
		//Load all files from the folder
		UnityEngine.Object[] files = Resources.LoadAll(xmlFileFolderPath, typeof(TextAsset));
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

				XMLNodeHandler(id, childNode, hashData, errorMessage);
			}
		}				
	}
}
