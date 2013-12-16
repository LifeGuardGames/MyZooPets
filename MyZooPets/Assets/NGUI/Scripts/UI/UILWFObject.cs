using UnityEngine;
using System.Collections;

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
        showCaseAnimator = (GameObject)Instantiate(showCasePrefab);
        showCaseAnimator.name = "ShowCaseAnimator";
        showCaseAnimator.layer = gameObject.layer;
        showCaseAnimator.GetComponent<LWFAnimator>().animName = 
            DataManager.Instance.GetPetSpeciesColor(this.transform.parent.name);

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
