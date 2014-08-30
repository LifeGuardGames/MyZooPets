using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class PushNotificationsIOS : MonoBehaviour {

	//
	//private methods
	//

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static private void internalSendStringTags (string tagName, string[] tags);

	//
	//public
	//

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void registerForRemoteNotifications();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void unregisterForRemoteNotifications();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void setListenerName(string listenerName);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public System.IntPtr _getPushToken();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public System.IntPtr _getPushwooshHWID();
	
	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void setIntTag(string tagName, int tagValue);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void setStringTag(string tagName, string tagValue);
	
	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void startLocationTracking();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void stopLocationTracking();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void clearNotificationCenter();

	static public void setListTag(string tagName, List<object> tagValues)
	{
		List <string> stringTags = new List<string>();

		foreach (object tagValue in tagValues) {
			string stringTag = tagValue.ToString();

			if (stringTag != null)
				stringTags.Add(stringTag);
		}

		string[] array = stringTags.ToArray();

		internalSendStringTags (tagName, array);
	}

	private static bool isCreated;
	void Awake(){
		//Make Object persistent
		if(isCreated){
			//If There is a duplicate in the scene. delete the object and jump Awake
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		isCreated = true;
	}

	// Use this for initialization
	void Start () {
		#if !UNITY_EDITOR && UNITY_IPHONE
			registerForRemoteNotifications();
			setListenerName(this.gameObject.name);
			Debug.Log(getPushToken());	
		#endif
	}

	
	static public string getPushToken()
	{
		return Marshal.PtrToStringAnsi(_getPushToken());
	}

	static public string getPushwooshHWID()
	{
		return Marshal.PtrToStringAnsi(_getPushwooshHWID());
	}

	void onRegisteredForPushNotifications(string token)
	{
		//do handling here
		Debug.Log(token);
	}

	void onFailedToRegisteredForPushNotifications(string error)
	{
		//do handling here
		Debug.Log(error);
	}

	void onPushNotificationsReceived(string payload)
	{
		//do handling here
		Debug.Log(payload);
	}
}
