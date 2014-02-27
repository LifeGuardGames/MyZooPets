using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Jemast.LocalCache {
	
	[InitializeOnLoad]
	public class Platform {
		
		public delegate void CallbackFunction();
		public static CallbackFunction PlatformChange;
		
		private static Jemast.LocalCache.Shared.CacheTarget? currentCacheTarget {
			get {
				return 
					EditorPrefs.HasKey("Jemast.LocalCache.Platform.currentCacheTarget") ?
						(Jemast.LocalCache.Shared.CacheTarget?)EditorPrefs.GetInt("Jemast.LocalCache.Platform.currentCacheTarget") :
						null;
			}
			set {
				if (value == null)
					EditorPrefs.DeleteKey("Jemast.LocalCache.Platform.currentCacheTarget");
				else
					EditorPrefs.SetInt("Jemast.LocalCache.Platform.currentCacheTarget", (int)value);
			}
		}
		
		private static Jemast.LocalCache.Shared.CacheSubtarget? currentCacheSubtarget {
			get {
				return 
					EditorPrefs.HasKey("Jemast.LocalCache.Platform.currentCacheSubtarget") ?
						(Jemast.LocalCache.Shared.CacheSubtarget?)EditorPrefs.GetInt("Jemast.LocalCache.Platform.currentCacheSubtarget") :
						null;
			}
			set {
				if (value == null)
					EditorPrefs.DeleteKey("Jemast.LocalCache.Platform.currentCacheSubtarget");
				else
					EditorPrefs.SetInt("Jemast.LocalCache.Platform.currentCacheSubtarget", (int)value);
			}
		}
		
		private static Jemast.LocalCache.Shared.CacheTarget? wantedCacheTarget {
			get {
				return 
					EditorPrefs.HasKey("Jemast.LocalCache.Platform.wantedCacheTarget") ?
						(Jemast.LocalCache.Shared.CacheTarget?)EditorPrefs.GetInt("Jemast.LocalCache.Platform.wantedCacheTarget") :
						null;
			}
			set {
				if (value == null)
					EditorPrefs.DeleteKey("Jemast.LocalCache.Platform.wantedCacheTarget");
				else
					EditorPrefs.SetInt("Jemast.LocalCache.Platform.wantedCacheTarget", (int)value);
			}
		}
		
		private static Jemast.LocalCache.Shared.CacheSubtarget? wantedCacheSubtarget {
			get {
				return 
					EditorPrefs.HasKey("Jemast.LocalCache.Platform.wantedCacheSubtarget") ?
						(Jemast.LocalCache.Shared.CacheSubtarget?)EditorPrefs.GetInt("Jemast.LocalCache.Platform.wantedCacheSubtarget") :
						null;
			}
			set {
				if (value == null)
					EditorPrefs.DeleteKey("Jemast.LocalCache.Platform.wantedCacheSubtarget");
				else
					EditorPrefs.SetInt("Jemast.LocalCache.Platform.wantedCacheSubtarget", (int)value);
			}
		}
		
		private static bool? shouldSwitch {
			get {
				return 
					EditorPrefs.HasKey("Jemast.LocalCache.Platform.shouldSwitch") ?
						(bool?)EditorPrefs.GetBool("Jemast.LocalCache.Platform.shouldSwitch") :
						null;
			}
			set {
				if (value == null)
					EditorPrefs.DeleteKey("Jemast.LocalCache.Platform.shouldSwitch");
				else
					EditorPrefs.SetBool("Jemast.LocalCache.Platform.shouldSwitch", value.Value);
			}
		}
		
		private static bool? switching {
			get {
				return 
					EditorPrefs.HasKey("Jemast.LocalCache.Platform.switching") ?
						(bool?)EditorPrefs.GetBool("Jemast.LocalCache.Platform.switching") :
						null;
			}
			set {
				if (value == null)
					EditorPrefs.DeleteKey("Jemast.LocalCache.Platform.switching");
				else
					EditorPrefs.SetBool("Jemast.LocalCache.Platform.switching", value.Value);
			}
		}
		
		static Platform ()
	    {
	        EditorApplication.update += Update;
	    }
		
		public static bool SwitchPlatform(Jemast.LocalCache.Shared.CacheTarget target, Jemast.LocalCache.Shared.CacheSubtarget? subtarget = null) {
			if (shouldSwitch == true || switching == true) {
				Debug.LogWarning ("You can only perform one switch operation at a time.");
				Cleanup();
				return false;
			}
			
			// Check we're in editor and not running
			if (Application.isEditor == false || Application.isPlaying) {
				Debug.LogWarning ("Platform switch can only happen in editor and not during play mode.");
				Cleanup();
				return false;
			}
			
			// Check external version control for Unity < 4.3
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			if (EditorSettings.externalVersionControl == ExternalVersionControl.Disabled) {
				Debug.LogWarning ("External version control (.meta files) needs to be enabled in your editor settings.");
				Cleanup();
				return false;
			}
#endif
			
			// Check that we're trying to switch to a new target
			currentCacheTarget = Jemast.LocalCache.Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
			if (currentCacheTarget == Jemast.LocalCache.Shared.CacheTarget.Android)
				currentCacheSubtarget = Jemast.LocalCache.Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			else if (wantedCacheTarget == Jemast.LocalCache.Shared.CacheTarget.BB10)
				currentCacheSubtarget = Jemast.LocalCache.Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
#endif
			if (target == currentCacheTarget && subtarget == currentCacheSubtarget) {
				Debug.LogWarning ("Switch operation not triggered because target platform is already active.");
				Cleanup();
				return false;
			}
			
			// Refresh Asset Database
			AssetDatabase.Refresh();
			
			wantedCacheTarget = target;
			wantedCacheSubtarget = subtarget;
			shouldSwitch = true;
			
			return true;
		}
		
		private static void Update() {
			if (shouldSwitch == true && switching != true && EditorApplication.isCompiling == false) {
				switching = true;
				Jemast.LocalCache.CacheManager.SwitchPlatform(currentCacheTarget, currentCacheSubtarget, wantedCacheTarget, wantedCacheSubtarget, true);
			}
		
			if (switching == true && LocalCache.CacheManager.ShouldSwitchPlatform) {
				LocalCache.CacheManager.SwitchPlatformOperation(currentCacheTarget, currentCacheSubtarget, wantedCacheTarget, wantedCacheSubtarget);
			}
			
			if (switching == true && LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isCompiling && !LocalCache.CacheManager.SwitchOperationInProgress) {
				ActiveBuildTargetChanged();
			}
		}
		
		private static void Cleanup() {
			// Cleanup
			shouldSwitch = null;
			switching = null;
			currentCacheTarget = null;
			currentCacheSubtarget = null;
			wantedCacheTarget = null;
			wantedCacheSubtarget = null;
		}
		
		private static void ActiveBuildTargetChanged() {
			// Cleanup
			Cleanup();
			
			LocalCache.LogUtility.LogImmediate("Applying trick to refresh subtargets");
			
			// This will trigger refresh for subtargets
			// Unfortunately this code is not functional anymore since Unity 4.3.x
//			UnityEngine.Object playerSettings = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
//			SerializedObject ser = new SerializedObject(playerSettings);
//			SerializedProperty prop = ser.FindProperty("AndroidBundleVersionCode");
//			prop.intValue = prop.intValue + 1;
//			ser.ApplyModifiedProperties();
//			prop.intValue = prop.intValue - 1;
//			ser.ApplyModifiedProperties();

			// New method requires building an asset bundle from an empty scene
			// Slower but hopefully will last
			// Only required with subtarget-only switches
			if (LocalCache.CacheManager.PlatformRefreshShouldBustCache == true) {
				EditorUtility.DisplayProgressBar("Hold on", "Forcing synchronization with empty asset bundle build...", 0.5f);
				BuildPipeline.BuildStreamedSceneAssetBundle(new string[] { LocalCache.Shared.EditorAssetsPath + "SubtargetCacheBust.unity" }, "JLocalCachePluginCacheBuster.unity3d", EditorUserBuildSettings.activeBuildTarget);
				System.IO.File.Delete(LocalCache.Shared.ProjectPath + "/JLocalCachePluginCacheBuster.unity3d");
				Jemast.LocalCache.CacheManager.PlatformRefreshShouldBustCache = false;
			}

			LocalCache.LogUtility.LogImmediate("Platform refresh is all done");
			
			LocalCache.CacheManager.PlatformRefreshInProgress = false;
			
			AssetDatabase.Refresh();
			
			LocalCache.LogUtility.LogImmediate("Reopening previous scene if any");

			if (!string.IsNullOrEmpty(LocalCache.CacheManager.PlatformRefreshCurrentScene)) {
				EditorUtility.DisplayProgressBar("Hold on", "Reopening previous scene...", 0.5f);
				EditorApplication.OpenScene(LocalCache.CacheManager.PlatformRefreshCurrentScene);
				LocalCache.CacheManager.PlatformRefreshCurrentScene = "";
			}

			EditorUtility.ClearProgressBar();

			if (PlatformChange != null)
				PlatformChange();
		}
		
	}
	
}