using UnityEngine;
using System.Collections;

/// <summary>
/// Text mesh localize.
/// Copied from UILocalize
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class TextMeshLocalize : MonoBehaviour{
	/// <summary>
	/// Localization key.
	/// </summary>
	
	public string key;
	string mLanguage;
	bool mStarted = false;
	
	/// <summary>
	/// This function is called by the Localization manager via a broadcast SendMessage.
	/// </summary>
	
	void OnLocalize(Localization loc){
		if(mLanguage != loc.currentLanguage)
			Localize();
	}
	
	/// <summary>
	/// Localize the widget on enable, but only if it has been started already.
	/// </summary>
	
	void OnEnable(){
		if(mStarted && Localization.instance != null)
			Localize();
	}
	
	/// <summary>
	/// Localize the widget on start.
	/// </summary>
	
	void Start(){
		mStarted = true;
		if(Localization.instance != null)
			Localize();
	}
	
	/// <summary>
	/// Force-localize the widget.
	/// </summary>
	
	public void Localize(){
		Localization loc = Localization.instance;
		TextMesh mesh = GetComponent<TextMesh>();
		
		// If no localization key has been specified, use the label's text as the key
		if(string.IsNullOrEmpty(mLanguage) && string.IsNullOrEmpty(key) && mesh != null)
			key = mesh.text;
		
		// If we still don't have a key, leave the value as blank
		string val = string.IsNullOrEmpty(key) ? "" : loc.Get(key);
		
		if(mesh != null){
			mesh.text = val;
		}
		mLanguage = loc.currentLanguage;
	}
}
