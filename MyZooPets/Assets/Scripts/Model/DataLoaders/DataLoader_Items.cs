using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// This class loads constant item data from a xml file. 

//  An immutable class, meaning that no other classes should be modifying the data
//  from this class
//---------------------------------------------------
public class DataLoader_Items{
    //Key: itemtype, Value: dictionary of items
    //Key: itemID, Value: instance of Item.cs
    private static Dictionary<ItemType, Dictionary<string, Item>> allItems;

    //Look for item with itemID in the dictionary
    public static Item GetItem(string itemID){
		Dictionary<ItemType, Dictionary<string, Item>> dictItems = GetAllItems();
		
        Item item = null;

        if(dictItems[ItemType.Foods].ContainsKey(itemID)){
            item = dictItems[ItemType.Foods][itemID];
        }else if(dictItems[ItemType.Usables].ContainsKey(itemID)){
            item = dictItems[ItemType.Usables][itemID];
        }else if(dictItems[ItemType.Decorations].ContainsKey(itemID)){
            item = dictItems[ItemType.Decorations][itemID];
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

    //Returns the prefab name of item with itemID
    public static string GetDecoItemPrefabName(string itemID){
        DecorationItem item = (DecorationItem) GetItem(itemID);
        return item.PrefabName;
    }

    //Returns the material name of item with itemID
    public static string GetDecoItemMaterialName(string itemID){
        DecorationItem item = (DecorationItem) GetItem(itemID);
        return item.MaterialName;
    }

    //Returns all the data for a specific item type
    public static Dictionary<string, Item> GetAllItemsOfType(ItemType type){
		Dictionary<ItemType, Dictionary<string, Item>> dictItems = GetAllItems();
        return dictItems[type];
    }
	
	private static Dictionary<ItemType, Dictionary<string, Item>> GetAllItems() {
		if ( allItems == null )
			SetupData();
		
		return allItems;
	}

    public static void SetupData(){
		allItems = new Dictionary<ItemType, Dictionary<string, Item>>();
		
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
						item = new DecorationItem(itemID, itemType, hashItemData);
                    break;
                }
				
				if ( !categoryItem.ContainsKey( itemID ) )
                	categoryItem.Add(itemID, item);
				else
					Debug.LogError( itemID + " already in items dict" );
            }

            //Store dictionary into allItems
            allItems.Add(itemType, categoryItem);
         }
    }
}

