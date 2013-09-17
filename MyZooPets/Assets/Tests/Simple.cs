using UnityEngine;
using System.Collections;

public class Simple : LWFObject {

	void Start()
	{
		setLoader();
		// #1 Show popup lwf/textures
		Load("test2.swf/test2", "test2.swf/");
	}


	void setLoader()
	{
		LWFObject.SetLoader(
			lwfDataLoader:(name) => {
				TextAsset asset = Resources.Load(name) as TextAsset;
				if (asset == null) {
					return null;
				}
				return asset.bytes;
			},
			textureLoader:(name) => {
				Texture2D texture = Resources.Load(name) as Texture2D;
				if (texture == null) {
					return null;
				}
				return texture;
			}
		);
	}
}