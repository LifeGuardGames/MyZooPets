using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// NinjaData
// A single piece of data for the ninja gameplay; a
// list of entries pointing to spawn groups.
//---------------------------------------------------

public class NinjaData {
	// id of this group
	private string strID;
	public string GetID() {
		return strID;	
	}
	
	// list of scoring categories for this group
	private List<NinjaScoring> listScoring = new List<NinjaScoring>();
	public List<NinjaScoring> GetScoringCategories() {
		return listScoring;	
	}
	
	// weight for this group
	private int nWeight;
	public int GetWeight() {
		return nWeight;
	}
	
	// list of entries in this group
	private List<NinjaDataEntry> listEntries = new List<NinjaDataEntry>();
	public List<NinjaDataEntry> GetEntries() {
		return listEntries;	
	}

	public NinjaData( string id, Hashtable hashAttr, IXMLNode nodeEntries, string strError ) {
		// set id
		strID = id;
		
		// get the weight for this group
		string strWeight = HashUtils.GetHashValue<string>( hashAttr, "Weight", "1" );
		nWeight = int.Parse( strWeight );
		
		// get the scoring categories
		string strScoring = HashUtils.GetHashValue<string>( hashAttr, "Scoring", "Med", strError );
		string[] arrayScoring = strScoring.Split(","[0]);
		for ( int i = 0; i < arrayScoring.Length; ++i ) {
			NinjaScoring eScoring = (NinjaScoring) System.Enum.Parse( typeof( NinjaScoring ), arrayScoring[i] );
			listScoring.Add( eScoring );
		}
		
		// go through the list of entries and add them to our list
		List<IXMLNode> listEntries = XMLUtils.GetChildrenList( nodeEntries );
		for ( int i = 0; i < listEntries.Count; ++i ) {
			Hashtable hashEntryAttr = XMLUtils.GetAttributes( listEntries[i] );
			NinjaDataEntry entry = 	new NinjaDataEntry( hashEntryAttr, strError );
			this.listEntries.Add( entry );
		}
	}
}
