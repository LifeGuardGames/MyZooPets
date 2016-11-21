using System.Collections;

public class ImmutableDataTrigger {
	private string id;          //unique id of the trigger
	public string ID {
		get { return id; }
	}

	private string name;        //name of the trigger. could repeat 
	public string Name {
		get { return Localization.Localize(name); }
	}

	private string prefabName;  //name of the prefab to be loaded into scene. 
	public string PrefabName {
		get { return prefabName; }
	}

	private string floatyDesc;  //the text to appear when trigger is removed by user
	public string FloatyDesc {
		get {
			return string.Format(Localization.Localize(floatyDesc), Name);
		}
	}

	private string scene;       //the scene that the trigger is associated with
	public string Scene {
		get { return scene; }
	}

	public ImmutableDataTrigger(string id, IXMLNode xmlNode, string errorMsg) {
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		this.id = id;
		name = XMLUtils.GetString(hashElements["Name"] as IXMLNode);
		prefabName = XMLUtils.GetString(hashElements["PrefabName"] as IXMLNode);
		floatyDesc = XMLUtils.GetString(hashElements["FloatyDesc"] as IXMLNode);
		scene = XMLUtils.GetString(hashElements["Scene"] as IXMLNode);
	}
}
