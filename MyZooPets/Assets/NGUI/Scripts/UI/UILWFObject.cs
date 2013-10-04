using UnityEngine;

public class UILWFObject : UIWidget
{
    public GameObject showCasePrefab;
    private GameObject showCaseAnimator;

    override protected void OnStart()
    {
        InitLWF();
    }

    // override protected void OnEnable(){
    //     InitLWF();
    // }

    private void InitLWF()
    {
        showCaseAnimator = (GameObject)Instantiate(showCasePrefab);
        showCaseAnimator.name = "ShowCaseAnimator";
        showCaseAnimator.layer = gameObject.layer;

        Transform transform = showCaseAnimator.transform;
        transform.parent = gameObject.transform;
        transform.localPosition = showCasePrefab.transform.localPosition;
        transform.localScale = showCasePrefab.transform.localScale; 

        //TO DO: add finish loading call back in LWFAnimator
        Invoke("Testing", 1.0f);
    }

    private void Testing(){
        showCaseAnimator.GetComponent<LWFAnimator>().PlayClip("happyIdle");
    }
}
