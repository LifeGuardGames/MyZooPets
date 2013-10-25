using UnityEngine;
using System.Collections;
using System.IO;

#pragma warning disable 0219

public class TakeScreenshot : MonoBehaviour {
	
	private int num = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown ("m")) 
			StartCoroutine( ScreenshotEncode() );
	}
	
	IEnumerator ScreenshotEncode()
    {
		yield return new WaitForEndOfFrame();
		
		Debug.Log("Beginning screenshot");
		
        // wait for graphics to render
        //yield return new WaitForEndOfFrame();

        // create a texture with portrait orientation
        //Texture2D texture = new Texture2D(Screen.height, Screen.width, TextureFormat.RGB24, false);
		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		
		//Debug.Log("What is screen height and width? " + Screen.height + " and " + Screen.width);
		
        // put buffer into texture
        //texture.ReadPixels(new Rect(0, 0, Screen.height, Screen.width), 0, 0);
		texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        
		texture.Apply();
		
		/*
		// create a texture with landscape orientation
		Texture2D texture_ok = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			
		// read pixels from portrait and write to landscape
		for(int i=0;i<Screen.height;i++)
		{
			for(int j=0;j<Screen.width;j++)
			{
				Color c = texture.GetPixel(i,j);
				texture_ok.SetPixel(Screen.width-j,i,c);
			}
		}
		 */
		
        // split the process up
        yield return 0;

        // save our image
		//byte[] bytes = texture_ok.EncodeToPNG();
		//string path = Application.dataPath + "/Screenshot.png";
		
		byte[] bytes2 = texture.EncodeToPNG();
		string path2 = Application.dataPath + "/../Screenshot_" + num + ".png";
		Debug.Log("Saving screenshot " + num );
		num++;
		//File.WriteAllBytes(path, bytes);

		File.WriteAllBytes(path2, bytes2);

    }

	
}
