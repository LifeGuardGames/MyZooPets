using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataItems{
    //Key: itemtype, value: dictionary of items
    //Key: itemID, value: instance of Item.cs
    private static Dictionary<ItemType, Dictionary<string, Item>> allItems = 
        new Dictionary<ItemType, Dictionary<string, Item>>();
    private static bool dataLoaded = false;

    // void Start(){
    //    SetupData(); 
    //    Item hi = GetItem("Usable0", ItemType.Usables);
    //    Item test = GetItem("Food0", ItemType.Foods);
    //    Item test2 = GetItem("Food1", ItemType.Foods);
    // }

    //Look for item with itemID in the dictionary
    public static Item GetItem(string itemID, ItemType category){
        Item item = null;
        Dictionary<string, Item> items = allItems[category];
        if(items.ContainsKey(itemID)){
            item = items[itemID];
        }
        return item;
    }

    public static void SetupData(){
        if(dataLoaded) return; //Don't load from xml if data already loaded

        //Load all item xml files
         UnityEngine.Object[] files = Resources.LoadAll("Items", typeof(TextAsset));
         foreach(TextAsset file in files){
            string xmlString = file.text;

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

                Item item = null;
                switch(itemType){
                    case ItemType.Foods:
                        item = new FoodItem(itemID, itemType, hashItemData);
                    break;
                    case ItemType.Usables:
                        item = new UsableItem(itemID, itemType, hashItemData);
                    break;
                    case ItemType.Decorations:
                    break;
                }

                categoryItem.Add(itemID, item);
            }

            //Store dictionary into allItems
            allItems.Add(itemType, categoryItem);
         }
         dataLoaded = true;
    }
}

