using UnityEngine;
using System.Collections;

/*
    LifeGuard Games customization of NGUITools functions
*/
public static class LgNGUITools{

    /// <summary>
    /// Instantiate an object and add it to the specified parent. use the position of the prefab
    /// </summary>
    static public GameObject AddChildWithPosition(GameObject parent, GameObject prefab)
    {
       GameObject go = NGUITools.AddChild(parent, prefab);
       if (go != null )
       {
               Transform t = go.transform;
               t.localPosition = prefab.transform.localPosition;
       }
       
       return go;
    }
}
