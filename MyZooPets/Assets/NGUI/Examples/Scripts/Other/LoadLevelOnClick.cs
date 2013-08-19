using UnityEngine;

public class LoadLevelOnClick : MonoBehaviour
{
	public string levelName;
	public GameObject optionalLoadingScreen;

	void Init(){
		if(optionalLoadingScreen != null){
			optionalLoadingScreen.SetActive(false);
		}
	}

	void Start(){

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