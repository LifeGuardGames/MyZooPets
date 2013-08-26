using UnityEngine;
using System.Collections;

public class TutorialHighlighting : MonoBehaviour {

    public GameObject arrowSprite;

	// Use this for initialization
	void Start () {
        HideArrow();
	}

    public void ShowArrow(){
        arrowSprite.SetActive(true);
    }
	
    public void HideArrow(){
        arrowSprite.SetActive(false);
    }
}
