using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Hash -- Key: scene name, Value: (Dictionary<string, ImmutableDataTrigger> -- Key: TriggerID, Value: ImmutableDataTrigger)
/// </summary>
public class DataLoaderTriggers : XMLLoaderGeneric<DataLoaderTriggers>{
    public static ImmutableDataTrigger GetRandomSceneTrigger(string scene){
		instance.InitXMLLoader();
        ImmutableDataTrigger randomTrigger = null;

        Dictionary<string, ImmutableDataTrigger> sceneTriggers = 
			instance.GetData<Dictionary<string, ImmutableDataTrigger>>(scene);

        //Get random element from the dictionary
        randomTrigger = sceneTriggers.ElementAt(UnityEngine.Random.Range(0, sceneTriggers.Count)).Value;
        
        return randomTrigger;
    }

    public static ImmutableDataTrigger GetTriggerData(string triggerID){
 		instance.InitXMLLoader();
		ImmutableDataTrigger triggerData = null;

		List<Dictionary<string, ImmutableDataTrigger>> scenes = 
			instance.GetDataList<Dictionary<string, ImmutableDataTrigger>>();

        foreach(Dictionary<string, ImmutableDataTrigger> sceneTriggers in scenes){
            if(sceneTriggers.ContainsKey(triggerID)){
                triggerData = sceneTriggers[triggerID];
                break;
            }
        }

        return triggerData;
    }

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataTrigger data = new ImmutableDataTrigger(id, xmlNode, errorMessage);
		string scene = data.Scene;
		if(!hashData.ContainsKey(scene)) {
			hashData.Add(scene, new Dictionary<string, ImmutableDataTrigger>());
		}

        Dictionary<string, ImmutableDataTrigger> sceneTriggers = (Dictionary<string, ImmutableDataTrigger>)hashData[scene];

		if(sceneTriggers.ContainsKey(id)) {
			Debug.LogError("Duplicate trigger id: " + id + " for " + scene);
		}
		else {
			sceneTriggers.Add(id, data);
		}
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "Triggers";
	}
}
