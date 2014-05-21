using UnityEngine;
using System;
using System.Collections;

//----------------------------------------------------------
// ImmutableDadta_Trigger
// Data for each individual trigger.
// Immutable data
//----------------------------------------------------------
public class ImmutableDataTrigger{
    private string id; //unique id of the trigger
    private string name; //name of the trigger. could repeat 
    private string prefabName; //name of the prefab to be loaded into scene. 
    private string floatyDesc; //the text to appear when trigger is removed by user
    private string scene; //the scene that the trigger is associated with

    public string ID{
        get{return id;}
    }
    public string Name{
        get{return Localization.Localize(name);}
    }
    public string PrefabName{
        get{return prefabName;}
    }
    public string FloatyDesc{
        get{
            return String.Format(Localization.Localize(floatyDesc), Name);
        }
    }
    public string Scene{
        get{return scene;}
    }

    public ImmutableDataTrigger(string id, Hashtable hashTriggerData){
        this.id = id;

        name = XMLUtils.GetString(hashTriggerData["Name"] as IXMLNode);
        prefabName = XMLUtils.GetString(hashTriggerData["PrefabName"] as IXMLNode);
        floatyDesc = XMLUtils.GetString(hashTriggerData["FloatyDesc"] as IXMLNode);
        scene = XMLUtils.GetString(hashTriggerData["Scene"] as IXMLNode);
    }

}
