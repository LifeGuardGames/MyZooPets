using UnityEngine;
using System.Collections;

public class ListTween : MonoBehaviour {
	
	/// <summary>
	/// Tweens shitloads of GameObjects
	/// </summary>
	
	public GameObject obj1;
	public GameObject obj2;
	public GameObject obj3;
	public GameObject obj4;
	
	public float interval;	// Intervals between different objects falling
	
	void Start () {
		
		Hashtable optional = new Hashtable();
    	optional.Add("ease", LeanTweenType.easeOutBounce);
		optional.Add("delay", 0.0f);
    	LeanTween.move(obj1, new Vector2(obj1.transform.position.x, obj1.transform.position.y - 1), 1f, optional);
		optional["delay"] = 0.15f;
		LeanTween.move(obj2, new Vector2(obj2.transform.position.x, obj2.transform.position.y - 1), 1f, optional);
		optional["delay"] = 0.3f;
		LeanTween.move(obj3, new Vector2(obj3.transform.position.x, obj3.transform.position.y - 1), 1f, optional);
		optional["delay"] = 0.45f;
		LeanTween.move(obj4, new Vector2(obj4.transform.position.x, obj4.transform.position.y - 1), 1f, optional);
	}
	
	void Update () {
	
	}
}
