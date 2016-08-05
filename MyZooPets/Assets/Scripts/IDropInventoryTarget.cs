/// <summary>
/// Interface for an object that is a InventoryItem drop target
/// </summary>
interface IDropInventoryTarget {
	/// <summary>
	/// Logic of item dropping, also tells the inventory if it needs to decrease item count
	/// </summary>
	/// <returns>True if item valid, false if item is invalid</returns>
	void OnItemDropped(InventoryItem itemData);
}
