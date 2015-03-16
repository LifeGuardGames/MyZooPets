using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DataLoaderMerchantItem {

	/*private static Dictionary <string ,ImmutableDataMerchantItem> blackMerch;

	public static ImmutableDataMerchantItem GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataMerchantItem>(id);
	}

	public static List<string> getMerchantList(){
		List<string> merchTags = new List<string>(blackMerch.Keys); 
		return merchTags;
	}

	public ImmutableDataMerchantItem getItem(string itemId){
		return blackMerch[itemId];
	}

	public static void SetupData(){
		blackMerch = new Dictionary<string, ImmutableDataMerchantItem>();
		
		//Load all item xml files
		TextAsset files = Resources.Load("SecretMerchantItems")as TextAsset;
			string xmlString = files.text;
			
			//Create XMLParser instance
			XMLParser xmlParser = new XMLParser(xmlString);
			
			//Call the parser to build the IXMLNode objects
			XMLElement xmlElement = xmlParser.Parse();
			
			//Get item type                
			ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), xmlElement.value);
			
			//Create dictionary to store data
			Dictionary<string, Item> categoryItem = new Dictionary<string, Item>();
			
			//Go through all child node of xmlElement (the parent of the file)
			for(int i=0; i<xmlElement.Children.Count; i++){
				IXMLNode childNode = xmlElement.Children[i];
				
				//Get id
				Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
				string itemID = (string)hashAttr["ID"]; 
				
				//Get Item properties from xml node
				Hashtable hashItemData = XMLUtils.GetChildren(childNode);
				ImmutableDataMerchantItem item;
				item = GetData(itemID);

				
				if ( !blackMerch.ContainsKey(itemID) )
					blackMerch.Add(itemID,item);
				else
					Debug.LogError( itemID + " already in items dict" );
			}

	}
	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataMerchantItem data = new ImmutableDataMerchantItem(id, xmlNode, errorMessage);
		// Store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data);
	}
	protected override void InitXMLLoader(){
		xmlFileFolderPath = "SecretMerchantItems";
	}*/
}
