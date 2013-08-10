using UnityEngine;
using System.Collections;

/*
    Inherit from this class if you need to spawn a giant semi-trainsparent collider
    to prevent user from interacting with the game while a popup message is displayed

    Ex.
    public class message:BackDrop{
        protected override void Awake(){
            base.Awake();
        }
    }
*/
public class BackDrop : MonoBehaviour {
    private GameObject tempBackDrop;
    private static GameObject prefabReference; //Use this giant black background to prevent clicking
    public static GameObject PrefabReference{ //Cache the gameobject after it's loaded for the first time
        get{
            if(prefabReference == null){
                prefabReference = Resources.Load("NotificationBackDrop") as GameObject;
                D.Assert(prefabReference != null, "NotificationBackDrop prefab cannot be found in the Resources folder");
            }
            return prefabReference;
        }
    }
    protected GameObject backDropParent; //the parent that you want the back drop to be spawn under

    protected virtual void Awake(){}
    protected virtual void Start(){}
    protected virtual void Update(){}

    //Display the back drop
    protected void DisplayBackDrop(){
       //spawn  
       float zVal = BackDrop.PrefabReference.transform.localPosition.z;
       Vector3 prefabScale = BackDrop.PrefabReference.transform.localScale;
       tempBackDrop = NGUITools.AddChild(backDropParent, BackDrop.PrefabReference);
       tempBackDrop.name = "NotificationBackDrop";
       tempBackDrop.transform.localPosition = new Vector3(tempBackDrop.transform.localPosition.x,
            tempBackDrop.transform.localPosition.y, zVal);
       tempBackDrop.transform.localScale = prefabScale; 
    }

    //Destroy the spawned back drop
    protected void HideBackDrop(){
        if(tempBackDrop != null) Destroy(tempBackDrop, 0.5f);
    }

}
