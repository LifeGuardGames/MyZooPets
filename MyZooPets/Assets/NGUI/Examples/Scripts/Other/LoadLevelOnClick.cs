using UnityEngine;

[AddComponentMenu("NGUI/Examples/Load Level On Click")]
public class LoadLevelOnClick : MonoBehaviour
{
	public string levelName;
	public GameObject optionalLoadingScreen;

	void Init(){
		if(optionalLoadingScreen != null){
			optionalLoadingScreen.SetActive(false);
		}
	}

	void OnClick ()
	{
		if (!string.IsNullOrEmpty(levelName))
		{
			if(optionalLoadingScreen != null){
				optionalLoadingScreen.SetActive(true);
			}
			Application.LoadLevel(levelName);
		}
	}
}