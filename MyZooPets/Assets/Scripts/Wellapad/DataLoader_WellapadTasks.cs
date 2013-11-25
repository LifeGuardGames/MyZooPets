using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DataLoader_WellapadTasks
// Loads wellapad task data from xml.  A group of 1
// or more tasks make up a mission.
//---------------------------------------------------

public class DataLoader_WellapadTasks {
	// hashtable that contains all task data
	private static Hashtable hashData = new Hashtable();
	
    private static bool dataLoaded = false; //Prohibit double loading data
	
	//---------------------------------------------------
	// GetTasks()
	// Returns all available tasks for a given mission
	// type.
	//---------------------------------------------------	
	public static List<Data_WellapadTask> GetTasks( string strMissionType ) {
		List<Data_WellapadTask> listTasks = new List<Data_WellapadTask>();
		
		if ( hashData.ContainsKey( strMissionType ) )
			listTasks = (List<Data_WellapadTask>) hashData[strMissionType];
		else
			Debug.Log("Error...no missions for " + strMissionType);
		
		// loop through the list of all data and prune the ones that are not currently available to the player
		List<Data_WellapadTask> listFinal = new List<Data_WellapadTask>();
		for ( int i = 0; i < listTasks.Count; ++i ) {
			Data_WellapadTask task = listTasks[i];
			string strInclude = task.GetInclusionKey();
			
			// if the list of unlocked tasks includes our key (or doesn't have an inclusion key), it is good to go
			if ( string.IsNullOrEmpty( strInclude ) || DataManager.Instance.GameData.Wellapad.TasksUnlocked.Contains( strInclude ) )
				listFinal.Add( task );
		}
		
		return listFinal;
	}
	
	//---------------------------------------------------
	// GetTask()
	// Returns a specific task based on a mission type
	// and id.
	//---------------------------------------------------		
	public static Data_WellapadTask GetTask( string strMissionType, string strID ) {
		List<Data_WellapadTask> listTasks = GetTasks( strMissionType );
		
		for ( int i = 0; i < listTasks.Count; ++i ) {
			Data_WellapadTask task = listTasks[i];
			string strTaskID = task.GetID();
			if ( strTaskID == strID )
				return task;
		}
		
		// if we get here, we did not find the task...
		Debug.Log("Tried to find a specific task: " + strID + " of " + strMissionType + " -- but could not");
		return null;
	}

    public static void SetupData(){
        if(dataLoaded) return; //Don't load from xml if data already loaded

        //Load all item xml files
         UnityEngine.Object[] files = Resources.LoadAll("Wellapad/Tasks", typeof(TextAsset));
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
                Hashtable hashData = XMLUtils.GetChildren(childNode);				
				
				Data_WellapadTask data = new Data_WellapadTask( id, hashAttr, hashData, strError );
				
				StoreData( data );
            }
         }
         dataLoaded = true;
    }
	
	//---------------------------------------------------
	// StoreData()
	// Store the task based on its mission type.
	//---------------------------------------------------		
	private static void StoreData( Data_WellapadTask data ) {
		string strType = data.GetTaskType();
		if ( !hashData.ContainsKey( strType ) )
			hashData[strType] = new List<Data_WellapadTask>();
		
		List<Data_WellapadTask> tasks = (List<Data_WellapadTask>) hashData[strType];
		tasks.Add( data );
	}
}

