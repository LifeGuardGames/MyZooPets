using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataLoaderPetAnimations
// Loads pet animation data from xml.
//---------------------------------------------------

public class DataLoaderPetAnimations {
	// hashtable that contains animations for the pet.
	// this is a bit crazy.
	// this is a hash of a hash of a hash of lists
	// first hash: pet health
	// second hash: pet mood
	// third hash: animation categories
	// and the final list is of animations that fit the bill
	private static Hashtable hashData;
	
	// this is another hash of just categories to lists, in case we ever want to just get a random animation in a category
	// without all the restrictions
	private static Hashtable hashDataUnrestricted;
	
	// this is just a hash of IDs to data
	private static Hashtable hashDataList;

	//---------------------------------------------------
	// GetUnrestrictedData()
	// Get a random unrestricted animation for a type of category.
	// unrestricted meaning the animation does not depend on mood or health
	// The boolean is used for determining if weighting
	// should still be used.
	//---------------------------------------------------
    public static DataPetAnimation GetUnrestrictedData( string strCat, bool bUseWeights = false ) {
		if ( hashDataUnrestricted == null )
			SetupData();
		
        DataPetAnimation data = null;
		
		if ( hashDataUnrestricted.ContainsKey(strCat) ) {
			List<DataPetAnimation> listAnims = (List<DataPetAnimation>) hashDataUnrestricted[strCat];
			
			data = GetRandomData( listAnims, bUseWeights );
		}
		
		if ( data == null )
			Debug.Log("A query for an unrestricted animation in category " + strCat + " came up with nothing!");
		
        return data;
	}
	
	//---------------------------------------------------
	// GetAllData()
	// Returns the hash of all anim data.
	//---------------------------------------------------	
	public static Hashtable GetAllData() {
		if ( hashDataList == null )
			SetupData();
		
		return hashDataList;
	}
	
	//---------------------------------------------------
	// GetRestrictedData()
	// Get a random animation for a category that is
	// restricted; i.e. based on the pet's current mood,
	// health, etc.
	//---------------------------------------------------	
	public static DataPetAnimation GetRestrictedData( string strCat, bool bUseWeights = true ) {
		if ( hashData == null )
			SetupData();
		
		// get the pet's mood and health states
		PetMoods eMood = DataManager.Instance.GameData.Stats.GetMoodState();
		PetHealthStates eHealth = DataManager.Instance.GameData.Stats.GetHealthState();
		
		// if the pet is NOT healthy, their mood does not matter
		//if ( eHealth != PetHealthStates.Healthy )
		//	eMood = PetMoods.Any;
		
		DataPetAnimation data = null;
		
		// this is a little complex...there are a bunch of checks we need to do so that we don't access any null objects
		if ( hashData.ContainsKey(eHealth) ) {
			Hashtable hashHealth = (Hashtable) hashData[eHealth];
			
			if ( hashHealth.ContainsKey(eMood) ) {
				Hashtable hashMood = (Hashtable) hashHealth[eMood];
				
				if ( hashMood.ContainsKey(strCat) ) {
					List<DataPetAnimation> listAnims = (List<DataPetAnimation>) hashMood[strCat];
					
					// if we get here, all the data existed just right...so return a random, weighted animation
					data = GetRandomData( listAnims, true );
				}
			}
		}
		
		if ( data == null )
			Debug.Log("A query for a restricted animation(" + eMood + ", " + eHealth + ", " + strCat + ") came up with nothing!");
		
		return data;
	}
	
	//---------------------------------------------------
	// GetRandomData()
	// Returns a random data from the incoming list.
	//---------------------------------------------------	
	private static DataPetAnimation GetRandomData( List<DataPetAnimation> listAnims, bool bUseWeights ) {
		DataPetAnimation data = null;
		
		// if we are using weights, create a new list and use that
		if ( bUseWeights ) {
			List<DataPetAnimation> listWeighted = new List<DataPetAnimation>();
			
			for ( int i = 0; i < listAnims.Count; ++i ) {
				DataPetAnimation dataWeight = listAnims[i];	
				int nWeight = dataWeight.GetWeight();
				for ( int j = 0; j < nWeight; ++j )
					listWeighted.Add(dataWeight);
			}
			
			// a little recursion never hurt any body!
			return GetRandomData( listWeighted, false );
		}
		
		if ( listAnims.Count > 0 ) {
			int nIndex = UnityEngine.Random.Range(0, listAnims.Count);
			data = listAnims[nIndex];
		}
		
		return data;
	}

    public static void SetupData(){
		hashDataList = new Hashtable();
		hashDataUnrestricted = new Hashtable();
		hashData = new Hashtable();

        //Load all item xml files
         UnityEngine.Object[] files = Resources.LoadAll("PetAnimations", typeof(TextAsset));
         foreach(TextAsset file in files){
            string xmlString = file.text;
			
			// error message
			string strErrorFile = "Error in file " + file.name;				
			
            //Create XMLParser instance
            XMLParser xmlParser = new XMLParser(xmlString);

            //Call the parser to build the IXMLNode objects
            XMLElement xmlElement = xmlParser.Parse();

            //Go through all child node of xmlElement (the parent of the file)
            for(int i=0; i<xmlElement.Children.Count; i++){
                IXMLNode childNode = xmlElement.Children[i];

                // Get id
                Hashtable hashAttr = XMLUtils.GetAttributes(childNode);
                string id = (string)hashAttr["ID"];
				string strError = strErrorFile + "(" + id + "): ";
				
                // Get  properties from xml node
                Hashtable hashNode = XMLUtils.GetChildren(childNode);				
				
				DataPetAnimation data = new DataPetAnimation( id, hashAttr, hashNode, strError );
				
				// store the data for later access
				StoreData( data );
            }
         }
    }
	
	//---------------------------------------------------
	// StoreData()
	// Storing data for pet animations is a little complex,
	// so I created this function for it.
	//---------------------------------------------------	
	private static void StoreData( DataPetAnimation data ) {
       	// first let's just put it in the unrestricted hash based on the categories of the data	
		StoreCategories( hashDataUnrestricted, data );
		
		// second store the data in a plain hash table for a list of all animations
		string strID = data.GetID();
		if ( hashDataList.ContainsKey(strID ) )
			Debug.Log("Warning!  Multiple pet animations with the same ID!");
		else
			hashDataList[strID] = data;
		
		// now let's do the real complex stuff...
		// the first hash for data is based on health
		PetHealthStates eHealth = data.GetHealth();
		
		// create hashtable for the healt if it doesn't exist
		if ( !hashData.ContainsKey(eHealth) )
			hashData[eHealth] = new Hashtable();
		
		Hashtable hashHealth = (Hashtable) hashData[eHealth];
		
		// next is mood
		PetMoods eMood = data.GetMood();
		
		// slightly special case-y here...if the mood is "any", we want to duplicate the data across all hashes
		if ( eMood == PetMoods.Any ) {
			// iterate through all the moods, -1 (for any)
			for ( int i = 0; i < System.Enum.GetValues(typeof(PetMoods)).Length-1; ++i ) {
				Hashtable hashMood = GetMoodHash( hashHealth, (PetMoods) i );
				
				// store the categories for this mood
				StoreCategories( hashMood, data );				
			}
		}
		else {
			Hashtable hashMood = GetMoodHash( hashHealth, eMood );	
			
			// store the categories for this mood
			StoreCategories( hashMood, data );			
		}
	}
	
	//---------------------------------------------------
	// GetMoodHash()
	// Returns a valid hashtable for the incoming mood,
	// from the incoming health hashtable.
	//---------------------------------------------------	
	private static Hashtable GetMoodHash( Hashtable hashHealth, PetMoods eMood ) {
		// create the hashtable if it doesn't exist
		if ( !hashHealth.ContainsKey(eMood) )
			hashHealth[eMood] = new Hashtable();
		
		Hashtable hashMood = (Hashtable) hashHealth[eMood];
		
		return hashMood;
	}
	
	//---------------------------------------------------
	// StoreCategories()
	// Stores anim data in a hashtable based on categories
	// of the data.
	//---------------------------------------------------	
	private static void StoreCategories( Hashtable hash, DataPetAnimation data ) {
		List<string> listCats = data.GetCategories();
		for ( int i = 0; i < listCats.Count; ++i ) {
			string strCat = listCats[i];
			
			// if the hash doesn't contain this category yet, initialize it
			if ( !hash.ContainsKey( strCat ) )
				hash[strCat] = new List<DataPetAnimation>();
			
			// stick the data in the list for this category
			List<DataPetAnimation> listData = (List<DataPetAnimation>) hash[strCat];
			listData.Add( data );
		}		
	}
}

