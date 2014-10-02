using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using fastJSON;
using System.Threading.Tasks;

/// <summary>
/// Save data script for the accessory system.
/// </summary>
public class MutableDataAccessories : MutableData{

	/// <summary>
	/// Gets or sets the placed accessories.
	/// NOTE: Don't use this property. Use the methods provided by this class
	/// otherwise data will not be synced to the backend properly
	/// </summary>
	/// <value>The placed accessories.</value>
	public Dictionary<string, string> PlacedAccessories {get; set;} // dictionary of placed decorations; Key: node ID, Value: item ID

	/// <summary>
	/// Gets the placed accessory.
	/// </summary>
	/// <returns>The placed accessory itemID.</returns>
	/// <param name="nodeID">Node ID.</param>
	public string GetPlacedAccessory(string nodeID){
		string retVal = "";

		if(PlacedAccessories.ContainsKey(nodeID))
			retVal = PlacedAccessories[nodeID];

		return retVal;
	}

	/// <summary>
	/// Sets the accessory at node.
	/// </summary>
	/// <param name="nodeID">Node ID.</param>
	/// <param name="itemID">Item ID.</param>
	public void SetAccessoryAtNode(string nodeID, string itemID){
		if(!PlacedAccessories.ContainsKey(nodeID)){
			PlacedAccessories.Add(nodeID, itemID);
			IsDirty = true;
		}
	}

	/// <summary>
	/// Removes the accessory at node.
	/// </summary>
	/// <param name="nodeID">Node ID.</param>
	public void RemoveAccessoryAtNode(string nodeID){
		if(PlacedAccessories.ContainsKey(nodeID)){
			PlacedAccessories.Remove(nodeID);
			IsDirty = true;
		}
	}

	#region Initialization and override functions
	public MutableDataAccessories() : base(){
		PlacedAccessories = new Dictionary<string, string>();
	}

	public override void VersionCheck(System.Version currentDataVersion){
		throw new System.NotImplementedException();
	}
	
	public override void SaveAsyncToParseServer(string kidAccountID){
//		string placedAccessoriesJSON = JSON.Instance.ToJSON(PlacedAccessories);
//
//		//make the query that will get the kid account and eager load the pet accessory
//		ParseQuery<ParseObjectKidAccount> query = new ParseQuery<ParseObjectKidAccount>()
//			.Include("petAccessory");
//
//		query.GetAsync(kidAccountID).ContinueWith(t => {
//			//here we have the kid account the the loaded PetAccessory
//			ParseObjectKidAccount fetchedAccount = t.Result;
//			ParseObject petAccessory = fetchedAccount.PetAccessory;
//			List<ParseObject> objectsToSave = new List<ParseObject>();
//
//			//if petAccessory is null that means it hasn't been synced up to the
//			//server yet. create one here and save both the fetchedAccount and petAccessory
//			//because they are both modified.
//			if(petAccessory == null){
//				petAccessory = new ParseObject("PetAccessory");
//				ParseACL acl = new ParseACL();
//				acl.PublicReadAccess = true;
//				acl.PublicWriteAccess = false;
//				acl.SetWriteAccess(ParseUser.CurrentUser, true);
//				petAccessory.ACL = acl;
//
//				fetchedAccount.PetAccessory = petAccessory;
//				objectsToSave.Add(fetchedAccount);
//			}
//
//			petAccessory["placedAccessoriesJSON"] = placedAccessoriesJSON;
//			objectsToSave.Add(petAccessory);
//
//			return ParseObject.SaveAllAsync(objectsToSave);
//		}).Unwrap().ContinueWith(t => {
//			if(t.IsFaulted || t.IsCanceled){
//				Debug.Log("Fail to save async: " + this.ToString());
//			}
//			else{
//				IsDirty = false;
//				Debug.Log("save async successful: " + this.ToString());
//				Debug.Log("is data dirty: " + IsDirty);
//			}
//		});
	}
	#endregion
}
