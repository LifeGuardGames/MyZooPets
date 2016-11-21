/// <summary>
/// Items that are stored in the inventory. 
/// Mutable data but contains reference to immutable data
/// </summary>
public class InventoryItem{
    public string ItemID {get; set;}

	/// <summary>
	/// DEPRECATED .Gets or sets the name of the item texture. 
	/// This data shouldn't be serialize since they are immutable.
	/// Will be removed later.
	/// </summary>
	/// <value>The name of the item texture.</value>
    public string ItemTextureName {get; set;}

	/// <summary>
	/// DEPRECATED .Gets or sets the type of the item. 
	/// This data shouldn't be serialized since they are immutable.
	/// Will be removed at later time.
	/// </summary>
	/// <value>The type of the item.</value>
    public ItemType ItemType {get; set;}

    public int Amount {get; set;}
	
	/// <summary>
	/// Shortcut to return the immutable datda of item with ItemID
	/// </summary>
	/// <value>The item data.</value>
	public Item ItemData{
		get{return DataLoaderItems.GetItem(ItemID);}
	}

    public InventoryItem(string itemID, ItemType itemType, string textureName){
        ItemID = itemID;
        ItemType = itemType;
        ItemTextureName = textureName;
        Amount = 1;
    }

    public InventoryItem(){}
}
