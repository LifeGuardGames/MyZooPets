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
	// type.  For multiple tasks within a category, only
	// one such task is added.
	//---------------------------------------------------	
	public static List<Data_WellapadTask> GetTasks( string strMissionType ) {
		List<Data_WellapadTask> listTasksFinal = new List<Data_WellapadTask>();
		
		if ( hashData.ContainsKey( strMissionType ) ) {
			// get the hashtable of categories->tasks for this mission
			Hashtable hashCategories = (Hashtable) hashData[strMissionType];
			
			// now go through each category in this hash and pick one task at random and add it to our list of tasks
			// (but also check to make sure the category is unlocked)
			foreach ( DictionaryEntry pair in hashCategories ) {
				string strCategory = (string)pair.Key;
				
				if ( DataManager.Instance.GameData.Wellapad.TasksUnlocked.Contains( strCategory ) ) {
					List<Data_WellapadTask> listTasks = (List<Data_WellapadTask>) pair.Value;
					
					// get a random number of tasks to add to the list -- if the category is "Always" we want all the tasks,
					// otherwise we just want to pick 1 at random
					// int nTasks = strCategory == WellapadData.ALWAYS_UNLOCKED ? listTasks.Count : 1;
                    int nTasks = strCategory == "Always" ? listTasks.Count : 1;
					List<Data_WellapadTask> tasks = ListUtils.GetRandomElements<Data_WellapadTask>( listTasks, nTasks );
					
					foreach ( Data_WellapadTask task in tasks )
						listTasksFinal.Add( task );
				}
			}
		}
		else
			Debug.Log("Error...no missions for " + strMissionType);		

		return listTasksFinal;
	}
	
	//---------------------------------------------------
	// GetTask()
	// Returns a specific task based on a mission type
	// and id.
	//---------------------------------------------------		
	/*public static Data_WellapadTask GetTask( string strMissionType, string strID ) {
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
	*/

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
		string strMission = data.GetTaskType();
		if ( !hashData.ContainsKey( strMission ) )
			hashData[strMission] = new Hashtable();
		
		// this is the hashtable of tasks in a mission
		Hashtable hashTasks = (Hashtable) hashData[strMission];
		
		// now drill deeper -- we want to sort by categories
		string strCategory = data.GetCategory();
		if ( !hashTasks.ContainsKey( strCategory ) )
			hashTasks[strCategory] = new List<Data_WellapadTask>();
		
		List<Data_WellapadTask> tasks = (List<Data_WellapadTask>) hashTasks[strCategory];
		tasks.Add( data );
	}
}

