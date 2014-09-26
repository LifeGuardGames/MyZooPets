using UnityEngine;
using System;
using System.Collections;


public abstract class MutableData{
	/// <summary>
	/// Gets or sets a value indicating whether this instance is dirty.
	/// If the class is modified then it becomes dirty. dirty class will be synced
	/// up to parse backend
	/// </summary>
	/// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
	public bool IsDirty {get; set;}

	/// <summary>
	/// check the version of the current data. This method will handle any backward
	/// compatability issues.
	/// </summary>
	/// <param name="currentDataVersion">Current data version.</param>
	public abstract void VersionCheck(Version currentDataVersion);

	/// <summary>
	/// Syncs to parse server. This is where the code to save data to parse backend
	/// will go
	/// </summary>
	public abstract void SaveAsyncToParseServer(string kidAccountID);

	public MutableData(){
		IsDirty = true;
	}
}
