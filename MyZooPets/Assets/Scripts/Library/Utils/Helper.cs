using System.Linq;
using UnityEngine;
 
public static class Helper
{
    public static GameObject FindInChildren(this GameObject go, string name)
    {
        return (from x in go.GetComponentsInChildren<Transform>()
                where x.gameObject.name == name
                select x.gameObject).First();
    }
	
	public static GameObject GetParent( this GameObject go ) {
		GameObject goParent = null;
		
		if ( go.transform.parent )
			goParent = go.transform.parent.gameObject;
		
		if ( goParent == null )
			Debug.LogError("Something trying to get a game object's parent that doesn't have one...");
		
		return goParent;
	}
}