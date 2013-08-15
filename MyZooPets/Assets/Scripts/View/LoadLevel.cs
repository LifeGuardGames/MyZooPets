using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {
    public string levelName;
    public GameObject optionalLoadingScreen;
    
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
