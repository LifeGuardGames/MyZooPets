using System.Collections.Generic;

public class MutableDataInventory{
    public Dictionary<string, InventoryItem> InventoryItems {get; set;}
    public Dictionary<string, InventoryItem> DecorationItems {get; set;}
	public Dictionary<string, InventoryItem> AccessoryItems {get; set;}
    public List<string> OneTimePurchasedItems {get; set;}

    public MutableDataInventory(){
        Init(); 
    }

    public void Init(){
        InventoryItems = new Dictionary<string, InventoryItem>();
		DecorationItems = new Dictionary<string, InventoryItem>();
		AccessoryItems = new Dictionary<string, InventoryItem>();
        OneTimePurchasedItems = new List<string>();
    }
}
