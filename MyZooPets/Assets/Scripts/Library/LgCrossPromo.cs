using UnityEngine;
using System;
using System.Collections;
// using Chartboost;

public class LgCrossPromo : MonoBehaviour {
    public const string LAST_GATE = "Gate 2";
    public const string WELLAPAD = "Wellapad";

	void Awake(){
        // Init();	
	}
    
    void OnApplicationPause(bool isPaused){
        // if(!isPaused){
        //     Init();
        // }
    }

#if UNITY_ANDROID
    void Update(){
        //handles the back button on android
        // if (Application.platform == RuntimePlatform.Android) {
        //     if (Input.GetKeyUp(KeyCode.Escape)) {
        //         if (CBBinding.onBackPressed())
        //             return;
        //         else
        //             Application.Quit();
        //     }
        // }
    }
#endif

    // void OnGUI(){
    //     if(GUI.Button(new Rect(0, 0, 100, 100), "Promo")){
    //         ShowInterstitial(null);
    //     }
    // }	

    public static void ShowInterstitial(string location){
        // if(!String.IsNullOrEmpty(location)){
        //     //make sure CB is initialized before spawning ads
        //     CBBinding.showInterstitial(location);
        //     if(!CBBinding.hasCachedInterstitial(location)){
        //         CBBinding.cacheInterstitial(location);
        //     }
        // }
    }

    private void Init(){
// #if UNITY_ANDROID
//         CBBinding.init();
// #elif UNITY_IPHONE
//         CBBinding.init("5328e4c2f8975c5d8e3e8b14", "7c7ddd62d438ef00feec0e21be2ccd00456891b0");
// #endif
    }
}
