using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data loader items.
/// Doesn't inherit from XMLLoaderGeneric like other DataLoader class because
/// this class require a specific way of parsing xml. It needs to node the parent
/// tag as well not just the child tags
/// </summary>
public class DataLoaderItems{
    //Key: itemtype, Value: dictionary of items
    //Key: itemID, Value: instance of Item.cs
    private static Dictionary<ItemType, Dictionary<string, Item>> allItems;
	
	/// <summary>
	/// Gets the item.
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="itemID">Item ID.</param>
    public static Item GetItem(string itemID){
		Dictionary<ItemType, Dictionary<string, Item>> dictItems = GetAllItems();
		
        Item item = null;

        if(dictItems[ItemType.Foods].ContainsKey(itemID)){
            item = dictItems[ItemType.Foods][itemID];
        }else if(dictItems[ItemType.Usables].ContainsKey(itemID)){
            item = dictItems[ItemType.Usables][itemID];
        }else if(dictItems[ItemType.Decorations].ContainsKey(itemID)){
            item = dictItems[ItemType.Decorations][itemID];
		}else if(dictItems[ItemType.Accessories].ContainsKey(itemID)){
			item = dictItems[ItemType.Accessories][itemID];
		}

        return item;
    }
	
	/// <summary>
	/// Gets the type of the item.
	/// </summary>
	/// <returns>The item type.</returns>
	/// <param name="itemID">Item ID.</param>
    public static ItemType GetItemType(string itemID){
       Item item = GetItem(itemID);
       return item.Type;
    }
	
	/// <summary>
	/// Gets the name of the item texture.
	/// </summary>
	/// <returns>The item texture name.</returns>
	/// <param name="itemID">Item ID.</param>
    public static string GetItemTextureName(string itemID){
        Item item = GetItem(itemID);
        return item.TextureName;
    }

	public static int GetCost(string itemID){
		Item item = GetItem(itemID);
		return item.Cost;
	}
	
	/// <summary>
	/// Gets the name of the deco item prefab.
	/// </summary>
	/// <returns>The deco item prefab name.</returns>
	/// <param name="itemID">Item ID.</param>
    public static string GetDecoItemPrefabName(string itemID){
        DecorationItem item = (DecorationItem) GetItem(itemID);
        return item.PrefabName;
    }
	
	/// <summary>
	/// Gets the name of the deco item material.
	/// </summary>
	/// <returns>The deco item material name.</returns>
	/// <param name="itemID">Item ID.</param>
    public static string GetDecoItemMaterialName(string itemID){
        DecorationItem item = (DecorationItem) GetItem(itemID);
        return item.MaterialName;
    }

	/// <summary>
	/// Gets the name of the accessory item prefab.
	/// </summary>
	/// <returns>The accessory item prefab name.</returns>
	/// <param name="itemID">Item ID.</param>
	public static string GetAccessoryItemPrefabName(string itemID){
		AccessoryItem item = (AccessoryItem) GetItem(itemID);
		return item.PrefabName;
	}
	
	/// <summary>
	/// Gets Items of type
	/// </summary>
	/// <returns>The all items of type.</returns>
	/// <param name="type">Type.</param>
    public static Dictionary<string, Item> GetAllItemsOfType(ItemType type){
		Dictionary<ItemType, Dictionary<string, Item>> dictItems = GetAllItems();
        return dictItems[type];
    }
	
	private static Dictionary<ItemType, Dictionary<string, Item>> GetAllItems(){
		if(allItems == null)
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
					case ItemType.Accessories:
						item = new AccessoryItem(itemID, itemType, hashItemData);
						break;
                }
				
				if ( !categoryItem.ContainsKey( itemID ) )
                	categoryItem.Add(itemID, item);
				else
					Debug.LogError( itemID + " already in items dict" );
            }

            //Store dictionary into all Items
            allItems.Add(itemType, categoryItem);
         }
    }
}

