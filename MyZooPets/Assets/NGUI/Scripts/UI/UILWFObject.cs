using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Use this script to create create LWF animations in NGUI
public class UILWFObject : UIWidget
{
    public GameObject showCasePrefab;
    private GameObject showCaseAnimator;
	public string strClip;

    override protected void OnStart()
    {
        InitLWF();
    }

    //Display LWF animation in NGUI
    private void InitLWF()
    {
        //Instantiate LWF animator
        showCaseAnimator = (GameObject)Instantiate(showCasePrefab);
        showCaseAnimator.name = "ShowCaseAnimator";
        showCaseAnimator.layer = gameObject.layer;

        //Get pet species and color information from DataManager
        Dictionary<string, MutableData_PetMenuInfo> petMenuInfoDict = DataManager.Instance.MenuSceneData;
        string petID = this.transform.parent.name;
        if(petMenuInfoDict.ContainsKey(petID)){
            string speciesColor = petMenuInfoDict[petID].PetSpecies + petMenuInfoDict[petID].PetColor;
            showCaseAnimator.GetComponent<LWFAnimator>().animName = speciesColor; 
        }

        Transform showCaseAnimTransform = showCaseAnimator.transform;
        showCaseAnimTransform.parent = gameObject.transform;
        showCaseAnimTransform.localPosition = showCasePrefab.transform.localPosition;
        showCaseAnimTransform.localScale = showCasePrefab.transform.localScale; 

        StartCoroutine(StartAnimation());
    }

    //Play animation clip after waiting for a frame
    private IEnumerator StartAnimation(){
        yield return 0;

        showCaseAnimator.GetComponent<LWFAnimator>().PlayClip(strClip);
    }

}
