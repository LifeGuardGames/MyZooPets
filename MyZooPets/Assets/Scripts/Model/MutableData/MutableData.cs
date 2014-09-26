using UnityEngine;
using System;
using System.Collections;

public abstract class MutableData{
	public bool IsDirty {get; set;}
	public abstract void VersionCheck(Version currentDataVersion);
	public abstract void SyncToParseServer();

	public MutableData(){
		IsDirty = false;
	}
}
