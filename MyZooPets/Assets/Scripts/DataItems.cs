using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
    This class loads constant item data from a xml file. Only ItemLogic should
    be interfacing with this class
*/
public class DataItems{
    //Key: itemtype, Value: dictionary of items
    //Key: itemID, Value: instance of Item.cs
    private static Dictionary<ItemType, Dictionary<string, Item>> allItems = 
        new Dictionary<ItemType, Dictionary<string, Item>>();
    private static bool dataLoaded = false; //Prohibit double loading data

    //Look for item with itemID in the dictionary
    public static Item GetItem(string itemID){
        Item item = null;

        if(allItems[ItemType.Foods].ContainsKey(itemID)){
            item = allItems[ItemType.Foods][itemID];
        }else if(allItems[ItemType.Usables].ContainsKey(itemID)){
            item = allItems[ItemType.Usables][itemID];
        }else if(allItems[ItemType.Decorations].ContainsKey(itemID)){
            item = allItems[ItemType.Decorations][itemID];
        }

        return item;
    }

    //Return the ItemType of the item with itemID
    public static ItemType GetItemType(string itemID){
       Item item = GetItem(itemID);
       return item.Type;
    }

    //Returns the texture name of item with itemID
    public static string GetItemTextureName(string itemID){
        Item item = GetItem(itemID);
        return item.TextureName;
    }

    //Returns all the data for a specific item type
    public static Dictionary<string, Item> GetAllItemsOfType(ItemType type){
        return allItems[type];
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

