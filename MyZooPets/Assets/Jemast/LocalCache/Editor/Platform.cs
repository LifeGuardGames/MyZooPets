using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Jemast.LocalCache {
	
	[InitializeOnLoad]
	public class Platform {

		public delegate void CallbackFunction();
		public static CallbackFunction PlatformChange;
		
		private static bool? _wantsSwitchOperation;
		private static bool wantsSwitchOperation {
			get {
				if (_wantsSwitchOperation == null) {
					_wantsSwitchOperation = System.IO.File.Exists(LocalCache.Shared.CachePath + "WantsSwitchOperation");
				}
				
				return _wantsSwitchOperation.Value;
			}
			set {
				if (value == true) {
					System.IO.FileStream s = System.IO.File.Create(LocalCache.Shared.CachePath + "WantsSwitchOperation");
					s.Dispose();
				} else {
					System.IO.File.Delete(LocalCache.Shared.CachePath + "WantsSwitchOperation");
				}
				
				_wantsSwitchOperation = value;
			}
		}
		
		private static LocalCache.Shared.CacheTarget? wantedCacheTarget {
			get {
				string filePath = LocalCache.Shared.CachePath + "WantedCacheTarget";
				if (System.IO.File.Exists(filePath)) {
					return (LocalCache.Shared.CacheTarget?)int.Parse(System.IO.File.ReadAllText(filePath));
				}

				return null;
			}
			set {
				string filePath = LocalCache.Shared.CachePath + "WantedCacheTarget";

				if (value == null) {
					System.IO.File.Delete(filePath);
				} else {
					System.IO.File.WriteAllText(filePath, ((int)value.Value).ToString());
				}
			}
		}
		
		private static LocalCache.Shared.CacheSubtarget? wantedCacheSubtarget {
			get {
				string filePath = LocalCache.Shared.CachePath + "WantedCacheSubtarget";
				if (System.IO.File.Exists(filePath)) {
					return (LocalCache.Shared.CacheSubtarget?)int.Parse(System.IO.File.ReadAllText(filePath));
				}
				
				return null;
			}
			set {
				string filePath = LocalCache.Shared.CachePath + "WantedCacheSubtarget";
				
				if (value == null) {
					System.IO.File.Delete(filePath);
				} else {
					System.IO.File.WriteAllText(filePath, ((int)value.Value).ToString());
				}
			}
		}

		private static bool shouldPerformCleanup = false;
		
		static Platform ()
		{
	        EditorApplication.update += Update;
			
			// Warmup preferences
			LocalCache.Preferences.Warmup();
			
			// Local Cache Version
			if (LocalCache.Preferences.LocalCacheVersion < 2) {
				LocalCache.Preferences.LocalCacheVersion = 2;
				EditorUtility.DisplayDialog("Fast Platform Switch - IMPORTANT", "This update features an updated and improved cache manager. Due to breaking changes, we need you to CLEAR ALL YOUR CACHE and REIMPORT ALL once on all your projects using Fast Platform Switch.\n\nTo do so, go to Fast Platform Switch settings tab and hit the \"Clear all cache and Reimport everything button\" on each of your projects (note: this will disable hard links). We also recommend you backup your project and/or commit/push to version control for better security.\n\nSorry for the inconvience caused and thanks for your understanding!", "OK, I understand");
			}
			
			// Perform pending cleanup
			LocalCache.CacheManager.Cleanup();
	    }
		
		public static bool SwitchPlatform(Jemast.LocalCache.Shared.CacheTarget target, Jemast.LocalCache.Shared.CacheSubtarget? subtarget = null) {
			if (LocalCache.CacheManager.SwitchOperationInProgress == true) {
				Debug.LogWarning ("You can only perform one switch operation at a time.");
				return false;
			}
			
			// Check we're in editor and not running
			if (Application.isEditor == false || EditorApplication.isPlayingOrWillChangePlaymode) {
				Debug.LogWarning ("Platform switch can only happen in editor and not during play mode.");
				return false;
			}
			
			// Check external version control for Unity < 4.3
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			if (EditorSettings.externalVersionControl == ExternalVersionControl.Disabled) {
				Debug.LogWarning ("External version control (.meta files) needs to be enabled in your editor settings.");
				return false;
			}
#endif
			
			// Check that we're trying to switch to a new target
			if (target == LocalCache.CacheManager.CurrentCacheTarget && subtarget == LocalCache.CacheManager.CurrentCacheSubtarget) {
				Debug.LogWarning ("Switch operation not triggered because target platform is already active.");
				return false;
			}
			
			// Refresh Asset Database
			AssetDatabase.Refresh();

			wantsSwitchOperation = true;
			wantedCacheTarget = target;
			wantedCacheSubtarget = subtarget;
			
			return true;
		}
		
		private static void Update() {
			if (LocalCache.CacheManager.HasCheckedHardLinkStatus == false) {
				if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating) {
					try {
						LocalCache.CacheManager.CheckHardLinkStatus();
						LocalCache.CacheManager.HasCheckedHardLinkStatus = true;
						
						// Auto compress?
						if (LocalCache.Preferences.AutoCompress)
							LocalCache.CacheManager.CompressAllCache(true);
					}
					catch {
					}
				}
			} else {
				if (shouldPerformCleanup) {
					try {
						LocalCache.CacheManager.Cleanup();
						shouldPerformCleanup = false;
					}
					catch {
					}
				} else if (wantsSwitchOperation && wantedCacheTarget != null && LocalCache.CacheManager.SwitchOperationInProgress != true && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating) {
					Jemast.LocalCache.CacheManager.SwitchPlatform(wantedCacheTarget, wantedCacheSubtarget, true);
					wantsSwitchOperation = false;
					wantedCacheTarget = null;
					wantedCacheSubtarget = null;
				} else if (LocalCache.CacheManager.ShouldSerializeAssets && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating) {
					if (LocalCache.CacheManager.ShouldSerializeAssetsFrameSkip)
						LocalCache.CacheManager.SerializeAssetsOperation();
					else
						LocalCache.CacheManager.ShouldSerializeAssetsFrameSkip = true;
				} else if (LocalCache.CacheManager.ShouldSwitchPlatform && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating) {
					LocalCache.CacheManager.SwitchPlatformOperation();
				} else if (LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating && !LocalCache.CacheManager.SwitchOperationInProgress) {
					ActiveBuildTargetChanged();
				}
			}
		}

		private static void ActiveBuildTargetChanged() {	
			LocalCache.LogUtility.LogImmediate("Applying trick to refresh subtargets");
			
			// Only required with subtarget-only switches
			if (LocalCache.CacheManager.PlatformRefreshShouldBustCache == true) {
				EditorUtility.DisplayProgressBar("Hold on", "Forcing synchronization with empty asset bundle build...", 0.5f);
				
				// We'd rather avoid that but since the BuildStreamedSceneAssetBundle method says it requires pro while docs says no...
				if (UnityEditorInternal.InternalEditorUtility.HasPro()) {
					BuildPipeline.BuildStreamedSceneAssetBundle(new string[] { LocalCache.Shared.EditorAssetsPath + "SubtargetCacheBust.unity" }, "JLocalCachePluginCacheBuster.unity3d", EditorUserBuildSettings.activeBuildTarget);
				} else {
					try {
						System.Reflection.MethodInfo minfo = typeof(BuildPipeline).GetMethod("BuildPlayerInternalNoCheck", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
						if (minfo.GetParameters().Length == 5) {
							minfo.Invoke(null, new object[] {
								new string[] { LocalCache.Shared.EditorAssetsPath + "SubtargetCacheBust.unity" }, "JLocalCachePluginCacheBuster.unity3d", EditorUserBuildSettings.activeBuildTarget, BuildOptions.BuildAdditionalStreamedScenes, EditorApplication.isCompiling
							});
						} else {
							minfo.Invoke(null, new object[] {
								new string[] { LocalCache.Shared.EditorAssetsPath + "SubtargetCacheBust.unity" }, "JLocalCachePluginCacheBuster.unity3d", EditorUserBuildSettings.activeBuildTarget, BuildOptions.BuildAdditionalStreamedScenes, EditorApplication.isCompiling, null
							});
						}
					}
					catch {
					}
				}

				System.IO.File.Delete(LocalCache.Shared.ProjectPath + "/JLocalCachePluginCacheBuster.unity3d");
				Jemast.LocalCache.CacheManager.PlatformRefreshShouldBustCache = false;
			}

			LocalCache.LogUtility.LogImmediate("Platform refresh is all done");
			
			LocalCache.CacheManager.PlatformRefreshInProgress = false;
			
			// Cleanup
			shouldPerformCleanup = true;
			wantsSwitchOperation = false;
			wantedCacheTarget = null;
			wantedCacheSubtarget = null;
			AssetDatabase.Refresh();
			
			LocalCache.LogUtility.LogImmediate("Reopening previous scene if any");

			if (!string.IsNullOrEmpty(LocalCache.CacheManager.PlatformRefreshCurrentScene)) {
				EditorUtility.DisplayProgressBar("Hold on", "Reopening previous scene...", 0.5f);
				EditorApplication.OpenScene(LocalCache.CacheManager.PlatformRefreshCurrentScene);
				LocalCache.CacheManager.PlatformRefreshCurrentScene = "";
			}

			EditorUtility.ClearProgressBar();

			if (LocalCache.CacheManager.SwitchOperationIsAPI == true && PlatformChange != null) {
				LocalCache.CacheManager.SwitchOperationIsAPI = false;
				PlatformChange();
			} else {
				LocalCache.CacheManager.SwitchOperationIsAPI = false;
			}
		}
		
	}
	
}