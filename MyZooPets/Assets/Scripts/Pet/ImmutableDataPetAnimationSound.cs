using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImmutableDataPetAnimationSound{
#pragma warning disable 0414
	private string id;
#pragma warning restore 0414
	private List<string> soundClipNames;

	/// <summary>
	/// Gets the random name of a sound clip.
	/// </summary>
	/// <returns>The random clip name.</returns>
	public string GetRandomClipName(){
		string clipName = "";

		if(soundClipNames.Count != 0){
			int randomIndex = UnityEngine.Random.Range(0, soundClipNames.Count);
			clipName = soundClipNames[randomIndex];
		}

		return clipName;
	}

	public ImmutableDataPetAnimationSound(string id, IXMLNode xmlNode, string errorMsg){
		soundClipNames = new List<string>();
		List<IXMLNode> elements = XMLUtils.GetChildrenList(xmlNode);
		
		this.id = id;
		
		foreach(IXMLNode node in elements){
			string clipName = XMLUtils.GetString(node, "", errorMsg);
			soundClipNames.Add(clipName);
		}
	}
}
