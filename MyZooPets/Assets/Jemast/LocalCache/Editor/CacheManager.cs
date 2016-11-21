//
//  CacheManager.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Jemast.LocalCache {
	public class CacheManager {
		
		static public LocalCache.Shared.CacheTarget? CurrentCacheTarget {
			get {
				return LocalCache.Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
			}
		}
		
		static public Jemast.LocalCache.Shared.CacheSubtarget? CurrentCacheSubtarget {
			get {
				if (CurrentCacheTarget == Jemast.LocalCache.Shared.CacheTarget.Android)
					return Jemast.LocalCache.Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
				else if (CurrentCacheTarget == Jemast.LocalCache.Shared.CacheTarget.BlackBerry)
					 return Jemast.LocalCache.Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
#endif
				else
					return null;
			}
		}
		
		
		static string progressTitle = null;
		static string progressDescription = null;
		static float progress = 0f;
		
		private static bool? platformRefreshInProgress = null;
		public static bool PlatformRefreshInProgress {
			get {
				if (platformRefreshInProgress == null) {
					platformRefreshInProgress = File.Exists(LocalCache.Shared.CachePath + "PlatformRefreshInProgress");
				}
				
				return platformRefreshInProgress.Value;
			}
			set {
				if (value == true) {
					FileStream s = File.Create(LocalCache.Shared.CachePath + "PlatformRefreshInProgress");
					s.Dispose();
				} else {
					File.Delete(LocalCache.Shared.CachePath + "PlatformRefreshInProgress");
				}
				
				platformRefreshInProgress = value;
			}
		}
		
		private static bool? switchOperationInProgress = null;
		public static bool SwitchOperationInProgress {
			get {
				if (switchOperationInProgress == null) {
					switchOperationInProgress = File.Exists(LocalCache.Shared.CachePath + "SwitchOperationInProgress");
				}
				
				return switchOperationInProgress.Value;
			}
			set {
				if (value == true) {
					FileStream s = File.Create(LocalCache.Shared.CachePath + "SwitchOperationInProgress");
					s.Dispose();
				} else {
					File.Delete(LocalCache.Shared.CachePath + "SwitchOperationInProgress");
				}
				
				switchOperationInProgress = value;
			}
		}
		
		private static bool? switchOperationIsAPI = null;
		public static bool SwitchOperationIsAPI {
			get {
				if (switchOperationIsAPI == null) {
					switchOperationIsAPI = File.Exists(LocalCache.Shared.CachePath + "SwitchOperationIsAPI");
				}
				
				return switchOperationIsAPI.Value;
			}
			set {
				if (value == true) {
					FileStream s = File.Create(LocalCache.Shared.CachePath + "SwitchOperationIsAPI");
					s.Dispose();
				} else {
					File.Delete(LocalCache.Shared.CachePath + "SwitchOperationIsAPI");
				}
				
				switchOperationIsAPI = value;
			}
		}
		
		
		private static string platformRefreshCurrentScene = null;
		public static string PlatformRefreshCurrentScene {
			get {
				if (platformRefreshCurrentScene != null)
					return platformRefreshCurrentScene;
				
				string filePath = LocalCache.Shared.CachePath + "PlatformRefreshCurrentScene";
				if (File.Exists(filePath)) {
					platformRefreshCurrentScene = File.ReadAllText(filePath);
					return platformRefreshCurrentScene;
				}
				
				platformRefreshCurrentScene = "";
				return platformRefreshCurrentScene;
			}
			set {
				string filePath = LocalCache.Shared.CachePath + "PlatformRefreshCurrentScene";
				
				if (string.IsNullOrEmpty(value)) {
					File.Delete(filePath);
				} else {
					File.WriteAllText(filePath, value);
				}
			}
		}

		private static bool? platformRefreshShouldBustCache = null;
		public static bool? PlatformRefreshShouldBustCache {
			get {
				if (platformRefreshShouldBustCache != null)
					return platformRefreshShouldBustCache;
				
				string filePath = LocalCache.Shared.CachePath + "PlatformRefreshShouldBustCache";
				if (File.Exists(filePath)) {
					platformRefreshShouldBustCache = true;
					return platformRefreshShouldBustCache;
				}
				
				platformRefreshShouldBustCache = false;
				return platformRefreshShouldBustCache;
			}
			set {
				if (value == true) {
					FileStream s = File.Create(LocalCache.Shared.CachePath + "PlatformRefreshShouldBustCache");
					s.Dispose();
				} else {
					File.Delete(LocalCache.Shared.CachePath + "PlatformRefreshShouldBustCache");
				}
				
				platformRefreshShouldBustCache = value;
			}
		}
		
		public static bool ShouldRefreshUI = false;
		public static bool ShouldSerializeAssets = false;
		public static bool ShouldSerializeAssetsFrameSkip = false;
		public static bool ShouldSwitchPlatform = false;
		
		public static bool HasCheckedHardLinkStatus = false;
		
		private static LocalCache.Shared.CacheTarget? newCacheTarget;
		private static LocalCache.Shared.CacheSubtarget? newCacheSubtarget;
		
		public static void SwitchPlatform(LocalCache.Shared.CacheTarget? cacheTarget, LocalCache.Shared.CacheSubtarget? cacheSubtarget, bool apiSwitch) {
			BuildTarget? newBuildTarget = LocalCache.Shared.BuildTargetForCacheTarget(cacheTarget);
			
			if (CurrentCacheTarget == null || newBuildTarget == null || cacheTarget == null || (newBuildTarget == EditorUserBuildSettings.activeBuildTarget && CurrentCacheSubtarget == cacheSubtarget))
				return;
			
			if (apiSwitch == false) {
				int option = EditorUtility.DisplayDialogComplex("Do you want to save the changes you made in the scene " + (string.IsNullOrEmpty(EditorApplication.currentScene) ? "Untitled" : EditorApplication.currentScene) + "?", "Platform switching requires closing the current scene to process cache. Your changes will be lost if you don't save them.", "Save", "Don't Save", "Cancel");
				switch (option) {
				case 0:
					EditorApplication.SaveScene();
					break;
				case 1:
					break;
				case 2:
					return;
				default:
					return;
				}
			}
			
			LocalCache.LogUtility.LogImmediate("-------------------------------------------------------");
			LocalCache.LogUtility.LogImmediate("Switching from {0} ({1}) to {2} ({3})", CurrentCacheTarget.ToString(), CurrentCacheSubtarget.HasValue ? CurrentCacheSubtarget.Value.ToString() : "Base", cacheTarget, cacheSubtarget.HasValue ? cacheSubtarget.Value.ToString() : "Base");
			
			EditorUtility.DisplayProgressBar("Hold on", "Persisting assets and import settings to disk...", 0.5f);
			
			PlatformRefreshCurrentScene = EditorApplication.currentScene;
			EditorApplication.NewScene();
			
			newCacheTarget = cacheTarget;
			newCacheSubtarget = cacheSubtarget;
			
			SwitchOperationInProgress = true;
			SwitchOperationIsAPI = apiSwitch;
			PlatformRefreshInProgress = true;
			ShouldSerializeAssets = true;
			ShouldSerializeAssetsFrameSkip = false;
		}

		public static void SerializeAssetsOperation() {
			ShouldSerializeAssets = false;
			ShouldSerializeAssetsFrameSkip = false;

			// Save assets
			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
			
			// Save all import manager timestamps
			string assetsPath = Application.dataPath;
			
			string cachePath = assetsPath.Remove(assetsPath.Length - 6, 6) + "LocalCache";
			Directory.CreateDirectory(cachePath);
			
			string[] assetPaths = AssetDatabase.GetAllAssetPaths();
			
			LocalCache.LogUtility.LogImmediate("Writing import settings to disk and fetching assets timestamps");
			
			Dictionary<string,ulong> timestamps = new Dictionary<string, ulong>();
			
			foreach (var path in assetPaths) {
				AssetDatabase.WriteImportSettingsIfDirty(path);
				
				AssetImporter assetImporter = AssetImporter.GetAtPath(path);
				if (assetImporter != null) {
					timestamps.Add(path, assetImporter.assetTimeStamp);
				}
			}
			
			LocalCache.LogUtility.LogImmediate("Serializing asset timestamps to disk");
			
			string currentTargetMetadataTimestampsPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)CurrentCacheTarget] + (CurrentCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)CurrentCacheSubtarget] : "") + "timestamps";
			using (FileStream destFileStream = File.Create(currentTargetMetadataTimestampsPath)) {
				SerializeTimestamps(timestamps, destFileStream);
			}

			ShouldSwitchPlatform = true;
		}
		
		public static void SwitchPlatformOperation() {
			ShouldSwitchPlatform = false;
			ShouldRefreshUI = true;
			
			BuildTarget? newBuildTarget = LocalCache.Shared.BuildTargetForCacheTarget(newCacheTarget);
			BuildTargetGroup? newBuildTargetGroup = LocalCache.Shared.BuildTargetGroupForCacheTarget(newCacheTarget);
			
			EditorApplication.NewScene();

			if (newCacheSubtarget != null && CurrentCacheTarget == newCacheTarget && CurrentCacheSubtarget != newCacheSubtarget) {
				PlatformRefreshShouldBustCache = true;
			}

			string assetsPath = Application.dataPath;
			
			string libraryPath = assetsPath.Remove(assetsPath.Length - 6, 6) + "Library";
			string cachePath = assetsPath.Remove(assetsPath.Length - 6, 6) + "LocalCache";
			
			Directory.CreateDirectory(cachePath);
			
			string metadataPath = libraryPath + "/metadata";
			
			string currentTargetMetadataDirectoryPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)CurrentCacheTarget] + (CurrentCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)CurrentCacheSubtarget] : "") + "metadata";
			string newTargetMetadataDirectoryPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)newCacheTarget] + (newCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)newCacheSubtarget] : "") + "metadata";

			string currentTargetMetadataTimestampsPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)CurrentCacheTarget] + (CurrentCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)CurrentCacheSubtarget] : "") + "timestamps";
			string newTargetMetadataTimestampsPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)newCacheTarget] + (newCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)newCacheSubtarget] : "") + "timestamps";
			
			LocalCache.LogUtility.LogImmediate("Attempting to perform switch operation");
			
			try
			{
				// Preemptively get a list of assets
				string[] assetPaths = AssetDatabase.GetAllAssetPaths();
				
				LocalCache.LogUtility.LogImmediate("Performing decompression of current build target if required");
				LocalCache.CompressionManager.PerformDecompression(currentTargetMetadataDirectoryPath);
				
				LocalCache.LogUtility.LogImmediate("Performing decompression of new build target if required");
				LocalCache.CompressionManager.PerformDecompression(newTargetMetadataDirectoryPath);

				if (LocalCache.Preferences.EnableHardLinks) {
					EditorUtility.DisplayProgressBar("Hold on", "Swapping hard links...", 0.5f);
					LocalCache.LogUtility.LogImmediate("Swapping hard links");

					// If platform is not cached copy data from current platform
					if (System.IO.Directory.Exists(newTargetMetadataDirectoryPath) == false) {
						LocalCache.Shared.DirectoryCopy(metadataPath, newTargetMetadataDirectoryPath, true);
					}

					// Swap hard link
					if (Application.platform == RuntimePlatform.OSXEditor) {
						// Make hardlink process executable
						System.Diagnostics.Process process = null;
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = "chmod";
						process.StartInfo.Arguments = "+x \"" + LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();
						
						// Delete current hard link
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink";
						process.StartInfo.Arguments = "-u \"" + metadataPath + "\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();

						// Make new hard link
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink";
						process.StartInfo.Arguments = "\"" + newTargetMetadataDirectoryPath + "\" \"" + metadataPath + "\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();
					} else {
						// Delete current hard link
						System.Diagnostics.Process process = null;
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "junction.exe";
						process.StartInfo.Arguments = "/accepteula -d \"" + metadataPath + "\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();

						// Make new hard link
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "junction.exe";
						process.StartInfo.Arguments = "/accepteula \"" + metadataPath + "\" \"" + newTargetMetadataDirectoryPath + "\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();
					}
				} else {
					// Save current platform
					LocalCache.LogUtility.LogImmediate("Merging current build target to cache");

					progressTitle = "Hold on";
					progressDescription = "Saving current platform to cache...";
					progress = 0f;
					DirectoryMerge(metadataPath, currentTargetMetadataDirectoryPath);
					
					LocalCache.LogUtility.LogImmediate("Performing decompression of new build target if required");
					
					// Override with new platform
					LocalCache.LogUtility.LogImmediate("Merging new build target to cache");

					progressTitle = "Hold on";
					progressDescription = "Attempting to load previous platform from cache...";
					progress = 0f;
					DirectoryMerge(newTargetMetadataDirectoryPath, metadataPath);
				}
				
				// Chmod/Chown metadata
				LocalCache.LogUtility.LogImmediate("Setting correct permissions on metadata");
				FixPermissions();
				
				LocalCache.LogUtility.LogImmediate("Deleting excess metadata files");
				
				// Look for excess metadata files
				string[] metadataFiles = Directory.GetFiles(metadataPath, "*.*", SearchOption.AllDirectories);
				foreach (var file in metadataFiles) {
					string assetPath = AssetDatabase.GUIDToAssetPath(System.IO.Path.GetFileName(file));
					if (assetPath.StartsWith("Assets") && (string.IsNullOrEmpty (assetPath) || (!File.Exists(LocalCache.Shared.ProjectPath + assetPath) && !Directory.Exists(LocalCache.Shared.ProjectPath + assetPath)))) {
						File.Delete(file);
					}
				}
				
				LocalCache.LogUtility.LogImmediate("Looking for invalid/expired timestamps");
				
				// Look for invalid timestamps
				if (File.Exists(currentTargetMetadataTimestampsPath) && File.Exists(newTargetMetadataTimestampsPath)) {
					Dictionary<string,ulong> currentTimestamps;
					Dictionary<string,ulong> newTimestamps;
					
					using (FileStream stream = File.OpenRead(currentTargetMetadataTimestampsPath)) {
						currentTimestamps = DeserializeTimestamps(stream);
					}
					
					using (FileStream stream = File.OpenRead(newTargetMetadataTimestampsPath)) {
						newTimestamps = DeserializeTimestamps(stream);
					}
					
					foreach (var kv in newTimestamps) {
						if (currentTimestamps.ContainsKey(kv.Key) && (currentTimestamps[kv.Key] != kv.Value)) {
							string guid = AssetDatabase.AssetPathToGUID(kv.Key);
							File.Delete(metadataPath + "/" + guid.Substring(0,2) + "/" + guid);
						}
					}
				}
				
				LocalCache.LogUtility.LogImmediate("Perform actual switch");
				
				// Perform switch
				if (newCacheTarget == LocalCache.Shared.CacheTarget.Android) {
					AndroidBuildSubtarget? androidSubtarget = LocalCache.Shared.AndroidBuildSubtargetForCacheSubtarget(newCacheSubtarget);
					if (androidSubtarget.HasValue)
						EditorUserBuildSettings.androidBuildSubtarget = androidSubtarget.Value;
				}
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
				else if (newCacheTarget == LocalCache.Shared.CacheTarget.BlackBerry) {
					BlackBerryBuildSubtarget? blackberrySubtarget = LocalCache.Shared.BlackBerryBuildSubtargetForCacheSubtarget(newCacheSubtarget);
					if (blackberrySubtarget.HasValue)
						EditorUserBuildSettings.blackberryBuildSubtarget = blackberrySubtarget.Value;
				}
#endif
				
				EditorUserBuildSettings.SwitchActiveBuildTarget(newBuildTarget.Value);
				
				if (EditorUserBuildSettings.activeBuildTarget != newBuildTarget.Value)
					throw new System.Exception();
				
				// Make as selected build target group
				if (newBuildTargetGroup.HasValue)
					EditorUserBuildSettings.selectedBuildTargetGroup = newBuildTargetGroup.Value;
				
				// Platform refresh
				SwitchOperationInProgress = true;
				PlatformRefreshInProgress = true;
				
				LocalCache.LogUtility.LogImmediate("Looking for missing metadata files");
				
				// Look for missing metadata files
				int currentAsset = 0;
				int assetCount = assetPaths.Length;
				
				AssetDatabase.StartAssetEditing();
				
				foreach (var path in assetPaths) {
					EditorUtility.DisplayProgressBar("Hold on", "Reimporting changed assets...", (float)currentAsset/(float)assetCount);
					
					string filename = System.IO.Path.GetFileName(path);
					string guid = AssetDatabase.AssetPathToGUID(path);
					if (path.StartsWith("Assets") && !string.IsNullOrEmpty(guid) && !File.Exists(metadataPath + "/" + guid.Substring(0,2) + "/" + guid)) {
						EditorUtility.DisplayProgressBar("Hold on", "Reimporting changed assets... (" + filename + ")", (float)currentAsset/(float)assetCount);
						AssetDatabase.ImportAsset (path, ImportAssetOptions.ForceUpdate);
					}
					currentAsset++;
				}
				
				EditorUtility.DisplayProgressBar("Hold on", "Reimporting changed assets... (processing batch)", 1.0f);
				AssetDatabase.StopAssetEditing();
				
				EditorUtility.ClearProgressBar();
				
				// Refresh asset database
				AssetDatabase.Refresh();
				
				EditorUtility.DisplayProgressBar("Hold on", "Waiting for platform change to become active...", 0.5f);
			}
			catch (System.Exception e) {
				LocalCache.LogUtility.LogImmediate("Switch operation exception: ", e.Message);
				
				Debug.LogError(e.Message);
				
				EditorUtility.ClearProgressBar();
				
				SwitchOperationInProgress = false;

				if (LocalCache.Preferences.EnableHardLinks) {
					// Check if platform switch happened -- Get current target
					LocalCache.Shared.CacheTarget? currentCacheTarget = LocalCache.Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
					LocalCache.Shared.CacheSubtarget? currentCacheSubtarget = null;
					
					if (currentCacheTarget == LocalCache.Shared.CacheTarget.Android)
						currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
					#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
					else if (currentCacheTarget == LocalCache.Shared.CacheTarget.BlackBerry)
						currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
					#endif

					// Revert hard links
					if (currentCacheTarget != newCacheTarget || currentCacheSubtarget != newCacheSubtarget) {
						if (Application.platform == RuntimePlatform.OSXEditor) {
							// Make hardlink process executable
							System.Diagnostics.Process process = null;
							process = new System.Diagnostics.Process();
							process.StartInfo.FileName = "chmod";
							process.StartInfo.Arguments = "+x \"" + LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink\"";
							process.StartInfo.UseShellExecute = false;
							process.StartInfo.RedirectStandardOutput = true;
							process.StartInfo.CreateNoWindow = true;
							process.Start();
							process.WaitForExit();
							process.Dispose();
							
							// Delete current hard link
							process = new System.Diagnostics.Process();
							process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink";
							process.StartInfo.Arguments = "-u \"" + metadataPath + "\"";
							process.StartInfo.UseShellExecute = false;
							process.StartInfo.RedirectStandardOutput = true;
							process.StartInfo.CreateNoWindow = true;
							process.Start();
							
							// Wait for process to end
							process.WaitForExit();
							process.Dispose();
	
							// Make new hard link
							process = new System.Diagnostics.Process();
							process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink";
							process.StartInfo.Arguments = "\"" + currentTargetMetadataDirectoryPath + "\" \"" + metadataPath + "\"";
							process.StartInfo.UseShellExecute = false;
							process.StartInfo.RedirectStandardOutput = true;
							process.StartInfo.CreateNoWindow = true;
							process.Start();
							
							// Wait for process to end
							process.WaitForExit();
							process.Dispose();
						} else {
							// Delete current hard link
							System.Diagnostics.Process process = null;
							process = new System.Diagnostics.Process();
							process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "junction.exe";
							process.StartInfo.Arguments = "/accepteula -d \"" + metadataPath + "\"";
							process.StartInfo.UseShellExecute = false;
							process.StartInfo.RedirectStandardOutput = true;
							process.StartInfo.CreateNoWindow = true;
							process.Start();
							
							// Wait for process to end
							process.WaitForExit();
							process.Dispose();
	
							// Make new hard link
							process = new System.Diagnostics.Process();
							process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "junction.exe";
							process.StartInfo.Arguments = "/accepteula \"" + metadataPath + "\" \"" + currentTargetMetadataDirectoryPath + "\"";
							process.StartInfo.UseShellExecute = false;
							process.StartInfo.RedirectStandardOutput = true;
							process.StartInfo.CreateNoWindow = true;
							process.Start();
							
							// Wait for process to end
							process.WaitForExit();
							process.Dispose();
						}
	
						// Mark for cleanup
						System.IO.File.Create(newTargetMetadataDirectoryPath + ".cleanup");
					}
				}
				
				return;
			}
			
			SwitchOperationInProgress = false;
			LocalCache.LogUtility.LogImmediate("Switch operation successful");
		}
		
		public static bool GetCacheStatus(LocalCache.Shared.CacheTarget? target, LocalCache.Shared.CacheSubtarget? subtarget) {
			if (!target.HasValue)
				return false;
			
			string metadataCacheFolder = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata";
			string metadataCacheLZ4File = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata.jcf.lz4";
			
			return Directory.Exists(metadataCacheFolder) || File.Exists(metadataCacheLZ4File);
		}
		
		public static bool GetCacheCompression(LocalCache.Shared.CacheTarget? target, LocalCache.Shared.CacheSubtarget? subtarget) {
			if (!target.HasValue)
				return false;
			
			string metadataCacheLZ4File = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata.jcf.lz4";

			return File.Exists(metadataCacheLZ4File);
		}
		
		public static System.DateTime GetCacheDate(LocalCache.Shared.CacheTarget? target, LocalCache.Shared.CacheSubtarget? subtarget) {
			if (!target.HasValue)
				return System.DateTime.Now;
			
			string metadataCacheFolder = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata";
			string metadataCacheLZ4File = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata.jcf.lz4";

			return File.Exists(metadataCacheLZ4File) ? File.GetLastWriteTime(metadataCacheLZ4File) : Directory.GetLastWriteTime(metadataCacheFolder);
		}
		
		public static long GetCacheSize(LocalCache.Shared.CacheTarget? target, LocalCache.Shared.CacheSubtarget? subtarget) {
			if (!target.HasValue)
				return -1;
			
			string metadataCacheFolder = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata";
			string metadataCacheLZ4File = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata.jcf.lz4";
			
			return File.Exists(metadataCacheLZ4File) ? (new FileInfo(metadataCacheLZ4File)).Length : CalculateFolderSize(metadataCacheFolder);
		}
		
		public static void ClearCache(LocalCache.Shared.CacheTarget? target, LocalCache.Shared.CacheSubtarget? subtarget) {
			if (!target.HasValue)
				return;
			
			string metadataCacheFolder = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata";
			string metadataCacheLZ4File = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata.jcf.lz4";
			string metadataCacheTimestampsFile = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "timestamps";
			
			if (Directory.Exists(metadataCacheFolder))
				LocalCache.Shared.DeleteDirectory(metadataCacheFolder);
			if (File.Exists(metadataCacheLZ4File))
				File.Delete(metadataCacheLZ4File);
			if (File.Exists(metadataCacheTimestampsFile))
				File.Delete(metadataCacheTimestampsFile);
			
			return;
		}
		
		public static void ClearAllCache() {
			string assetsPath = Application.dataPath;
			string cachePath = assetsPath.Remove(assetsPath.Length - 6, 6) + "LocalCache";
			Directory.CreateDirectory(cachePath);

			// Get current target
			LocalCache.Shared.CacheTarget? currentCacheTarget = LocalCache.Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
			LocalCache.Shared.CacheSubtarget? currentCacheSubtarget = null;
			
			if (currentCacheTarget == LocalCache.Shared.CacheTarget.Android)
				currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			else if (currentCacheTarget == LocalCache.Shared.CacheTarget.BlackBerry)
				currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
			#endif
			
			string currentTargetMetadataDirectoryPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)currentCacheTarget] + (currentCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)currentCacheSubtarget] : "") + "metadata";

			foreach (var directory in System.IO.Directory.GetDirectories(cachePath)) {
				if (LocalCache.Preferences.EnableHardLinks && directory.Replace("\\","/").Equals(currentTargetMetadataDirectoryPath))
					continue;

				LocalCache.Shared.DeleteDirectory(directory);
			}

			foreach (var file in System.IO.Directory.GetFiles(cachePath)) {
				System.IO.File.Delete(file);
			}
		}
		
		public static void CompressCache(LocalCache.Shared.CacheTarget? target, LocalCache.Shared.CacheSubtarget? subtarget, bool silent = false) {
			if (!target.HasValue)
				return;
			
			string metadataCacheFolder = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata";
			
			if (Directory.Exists(metadataCacheFolder))
				LocalCache.CompressionManager.PerformCompression(metadataCacheFolder, silent);
			
			return;
		}
		
		public static void DecompressCache(LocalCache.Shared.CacheTarget? target, LocalCache.Shared.CacheSubtarget? subtarget, bool silent = false) {
			if (!target.HasValue)
				return;
			
			string metadataCacheFolder = LocalCache.Shared.CachePath + LocalCache.Shared.CacheTargetPrefixes[(int)target.Value] + (subtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)subtarget.Value] : "") + "metadata";
	
			LocalCache.CompressionManager.PerformDecompression(metadataCacheFolder, silent);
			
			return;
		}
		
		public static void CompressAllCache(bool background) {
			// Get current target
			LocalCache.Shared.CacheTarget? currentCacheTarget = LocalCache.Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
			LocalCache.Shared.CacheSubtarget? currentCacheSubtarget = null;
			
			if (currentCacheTarget == LocalCache.Shared.CacheTarget.Android)
				currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			else if (currentCacheTarget == LocalCache.Shared.CacheTarget.BlackBerry)
				currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
			#endif

			if (background) {
				Directory.CreateDirectory(LocalCache.Shared.CachePath);
				
				var backgroundLockFilePath = LocalCache.Shared.CachePath + "Background.txt";
				var stream = File.Create(backgroundLockFilePath);
				stream.Dispose();
				
				System.Threading.ThreadPool.QueueUserWorkItem(delegate {
					try {
						CompressAllCacheOperation(true, currentCacheTarget, currentCacheSubtarget);
					}
					catch (System.Exception e) {
						Debug.LogError(e.Message);

						// Cleanup
						foreach (var file in System.IO.Directory.GetFiles(LocalCache.Shared.CachePath)) {
							if (file.EndsWith(".jcf"))
								System.IO.File.Delete(file);
						}
					}
					finally {
						File.Delete(backgroundLockFilePath);
						ShouldRefreshUI = true;
					}
				});
			} else {
				CompressAllCacheOperation(false, currentCacheTarget, currentCacheSubtarget);
			}
		}
		
		private static void CompressAllCacheOperation(bool background, LocalCache.Shared.CacheTarget? currentCacheTarget, LocalCache.Shared.CacheSubtarget? currentCacheSubtarget) {
			// Process all targets & subtargets
			for (int i = 0; i < (int)LocalCache.Shared.CacheTarget.Count; i++) {
				var target = (LocalCache.Shared.CacheTarget)i;
				
				if (target == LocalCache.Shared.CacheTarget.Android) {
					for (int j = (int)LocalCache.Shared.CacheSubtarget.Android_First + 1; j < (int)LocalCache.Shared.CacheSubtarget.Android_Last; j++) {
						var subtarget = (LocalCache.Shared.CacheSubtarget)j;
						if (GetCacheStatus(target, subtarget) && !GetCacheCompression(target, subtarget) && !(LocalCache.Preferences.EnableHardLinks && target == currentCacheTarget && subtarget == currentCacheSubtarget))
							CompressCache(target, subtarget, background);
					}
				}
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
				else if (target == LocalCache.Shared.CacheTarget.BlackBerry) {
					for (int j = (int)LocalCache.Shared.CacheSubtarget.BlackBerry_First + 1; j < (int)LocalCache.Shared.CacheSubtarget.BlackBerry_Last; j++) {
						var subtarget = (LocalCache.Shared.CacheSubtarget)j;
						if (GetCacheStatus(target, subtarget) && !GetCacheCompression(target, subtarget) && !(LocalCache.Preferences.EnableHardLinks && target == currentCacheTarget && subtarget == currentCacheSubtarget))
							CompressCache(target, subtarget, background);
					}
				}
#endif
				else {
					if (GetCacheStatus(target, null) && !GetCacheCompression(target, null) && !(LocalCache.Preferences.EnableHardLinks && target == currentCacheTarget && null == currentCacheSubtarget))
						CompressCache(target, null, background);
				}
			}
		}
		
		public static void DecompressAllCache() {
			// Process all targets & subtargets
			for (int i = 0; i < (int)LocalCache.Shared.CacheTarget.Count; i++) {
				var target = (LocalCache.Shared.CacheTarget)i;
				
				if (target == LocalCache.Shared.CacheTarget.Android) {
					for (int j = (int)LocalCache.Shared.CacheSubtarget.Android_First + 1; j < (int)LocalCache.Shared.CacheSubtarget.Android_Last; j++) {
						var subtarget = (LocalCache.Shared.CacheSubtarget)j;
						if (GetCacheStatus(target, subtarget) && GetCacheCompression(target, subtarget))
							DecompressCache(target, subtarget);
					}
				}
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
				else if (target == LocalCache.Shared.CacheTarget.BlackBerry) {
					for (int j = (int)LocalCache.Shared.CacheSubtarget.BlackBerry_First + 1; j < (int)LocalCache.Shared.CacheSubtarget.BlackBerry_Last; j++) {
						var subtarget = (LocalCache.Shared.CacheSubtarget)j;
						if (GetCacheStatus(target, subtarget) && !GetCacheCompression(target, subtarget))
							CompressCache(target, subtarget);
					}
				}
#endif
				else {
					if (GetCacheStatus(target, null) && GetCacheCompression(target, null))
						DecompressCache(target, null);
				}
			}
		}
		
		public static bool BackgroundCacheCompressionInProgress {
			get {
				return File.Exists(LocalCache.Shared.CachePath + "Background.txt");
			}
		}
		
		public static void FixIssues() {
			// Disable hard links
			LocalCache.Preferences.EnableHardLinks = false;
			CheckHardLinkStatus();

			// Delete local cache folder
			if (Directory.Exists(LocalCache.Shared.CachePath))
				LocalCache.Shared.DeleteDirectory(LocalCache.Shared.CachePath);
			
			// Delete metadata folder
			string assetsPath = Application.dataPath;
			string libraryPath = assetsPath.Remove(assetsPath.Length - 6, 6) + "Library";
			if (Directory.Exists(libraryPath + "/metadata"))
				LocalCache.Shared.DeleteDirectory(libraryPath + "/metadata");
			
			// Reimport all
			EditorApplication.ExecuteMenuItem("Assets/Reimport All");
		}
		
		protected static long CalculateFolderSize(string folder)
		{
		    long folderSize = 0;
		    try
		    {
		        if (!Directory.Exists(folder))
		            return -1;
		        else
		        {
		            try
		            {
		                foreach (string file in Directory.GetFiles(folder))
		                {
		                    if (File.Exists(file))
		                    {
		                        FileInfo finfo = new FileInfo(file);
		                        folderSize += finfo.Length;
		                    }
		                }
		
		                foreach (string dir in Directory.GetDirectories(folder))
		                    folderSize += CalculateFolderSize(dir);
		            }
		            catch (System.Exception e)
		            {
		                Debug.LogError(string.Format("Unable to calculate folder size: {0}", e.Message));
		            }
		        }
		    }
		    catch (System.Exception e)
		    {
		        Debug.LogError(string.Format("Unable to calculate folder size: {0}", e.Message));
		    }
			
		    return folderSize;
		}
		
		private static void DirectoryMerge(string sourceDirName, string destDirName)
	    {
			if (!Directory.Exists(sourceDirName))
				return;
			
			EditorUtility.DisplayProgressBar(progressTitle, progressDescription, progress);
			
			// Check for dest directory
			bool destDirectoryExists = Directory.Exists(destDirName);
			if (destDirectoryExists) {
				// First delete files from dest not in source
				string[] destFiles = Directory.GetFiles(destDirName, "*.*", SearchOption.AllDirectories);
				foreach (string file in destFiles) {
					if (!File.Exists(sourceDirName + file.Remove(0, destDirName.Length)))
						File.Delete(file);
					
					progress += 1.0f / ((float)destFiles.Length * 3.0f);
					EditorUtility.DisplayProgressBar(progressTitle, progressDescription, progress);
				}
			} else {
				Directory.CreateDirectory(destDirName);
			}
					
			// Then copy changed and new files from source to dest
			string[] sourceFiles = Directory.GetFiles(sourceDirName, "*.*", SearchOption.AllDirectories);
			foreach (string file in sourceFiles) {
				var destFile = destDirName + file.Remove(0, sourceDirName.Length);
				
				// Check if exists, same write time, same size
				if (File.Exists(destFile) && File.GetLastWriteTimeUtc(file) == File.GetLastWriteTimeUtc(destFile) && new FileInfo(file).Length == new FileInfo(destFile).Length)
					continue;
				
				// Create subdirs if needed
				Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destFile));
				
				// Copy file
				//File.Delete(destFile);
				File.Copy (file, destFile, true);
				File.SetCreationTime(destFile, File.GetCreationTime(file));
				File.SetLastAccessTime(destFile, File.GetLastAccessTime(file));
				File.SetLastWriteTime(destFile, File.GetLastWriteTime(file));
				
				progress += 1.0f / ((float)sourceFiles.Length * (destDirectoryExists ? 3.0f : 1.0f));
				EditorUtility.DisplayProgressBar(progressTitle, progressDescription, progress);
			}
			
			EditorUtility.ClearProgressBar();
		}
		
		private static void SerializeTimestamps(Dictionary<string, ulong> dictionary, Stream stream)
		{
		    BinaryWriter writer = new BinaryWriter(stream);
		    writer.Write(dictionary.Count);
		    foreach (var kvp in dictionary)
		    {
		        writer.Write(kvp.Key);
		        writer.Write(kvp.Value);
		    }
		    writer.Flush();
		}
		
		private static Dictionary<string, ulong> DeserializeTimestamps(Stream stream)
		{
		    BinaryReader reader = new BinaryReader(stream);
		    int count = reader.ReadInt32();
		    var dictionary = new Dictionary<string,ulong>(count);
		    for (int n = 0; n < count; n++)
		    {
		        var key = reader.ReadString();
		        var value = reader.ReadUInt64();
		        dictionary.Add(key, value);
		    }
		    return dictionary;                
		}
		
		public static void CheckHardLinkStatus() {
			bool metadataIsHardLink = false;
			bool hardLinkIsValid = false;
			string hardLinkDirectory = null;
			
			string assetsPath = Application.dataPath;
			
			string libraryPath = assetsPath.Remove(assetsPath.Length - 6, 6) + "Library";
			string cachePath = assetsPath.Remove(assetsPath.Length - 6, 6) + "LocalCache";
			Directory.CreateDirectory(cachePath);
			
			string metadataPath = libraryPath + "/metadata";

			
			// Get current target
			LocalCache.Shared.CacheTarget? currentCacheTarget = LocalCache.Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
			LocalCache.Shared.CacheSubtarget? currentCacheSubtarget = null;

			if (currentCacheTarget == LocalCache.Shared.CacheTarget.Android)
				currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			else if (currentCacheTarget == LocalCache.Shared.CacheTarget.BlackBerry)
				currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
			#endif
			
			string currentTargetMetadataDirectoryPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)currentCacheTarget] + (currentCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)currentCacheSubtarget] : "") + "metadata";
			
			
			if (Application.platform == RuntimePlatform.OSXEditor) {
				// Get inum value
				string inum = null;
				System.Diagnostics.Process process;
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "ls";
				process.StartInfo.Arguments = "-id \"" + metadataPath + "\"";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				
				using (StreamReader reader = process.StandardOutput)
				{
					string result = reader.ReadToEnd();
					inum = result.Split(' ')[0];
				}
				
				process.Dispose();
				
				// Find by inum in cache
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "find";
				process.StartInfo.Arguments = "\"" + cachePath + "\" -inum " + inum;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				
				using (StreamReader reader = process.StandardOutput)
				{
					string result = reader.ReadToEnd().Trim();
					if (!string.IsNullOrEmpty(result)) {
						metadataIsHardLink = true;
						hardLinkDirectory = result;
						hardLinkIsValid = result.Equals(currentTargetMetadataDirectoryPath);
					}
				}
				
				process.Dispose();
			} else {
				System.Diagnostics.Process process;
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "junction.exe";
				process.StartInfo.Arguments = "/accepteula \"" + metadataPath + "\"";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
        		process.StartInfo.StandardOutputEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
				process.Start();
				
				// Wait for process to end
				process.WaitForExit();
				
				using (StreamReader reader = process.StandardOutput)
				{
					string result = reader.ReadToEnd();
					int indexOfSubstituteName = result.IndexOf("Substitute Name: ");
					if (indexOfSubstituteName != -1) {
						metadataIsHardLink = true;
						hardLinkDirectory = result.Substring(indexOfSubstituteName + 17).Split('\n')[0].Trim ().Replace("\\", "/");
						hardLinkIsValid = hardLinkDirectory.Equals(currentTargetMetadataDirectoryPath);
					}
				}
				
				process.Dispose();
			}
			
			if (LocalCache.Preferences.EnableHardLinks) {
				if (metadataIsHardLink == false) {
					EditorUtility.DisplayProgressBar("Hold on", "Enabling hard links...", 0.5f);
					
					// Cleanup
					if (System.IO.File.Exists(currentTargetMetadataDirectoryPath + ".jcf"))
						File.Delete(currentTargetMetadataDirectoryPath + ".jcf");

					if (System.IO.File.Exists(currentTargetMetadataDirectoryPath + ".jcf.lz4"))
						File.Delete(currentTargetMetadataDirectoryPath + ".jcf.lz4");
					
					if (System.IO.Directory.Exists(currentTargetMetadataDirectoryPath))
						LocalCache.Shared.DeleteDirectory(currentTargetMetadataDirectoryPath);
					
					FixPermissions();
					
					System.IO.Directory.Move(metadataPath, currentTargetMetadataDirectoryPath);
				
					System.Diagnostics.Process process = new System.Diagnostics.Process();
					if (Application.platform == RuntimePlatform.OSXEditor) {
						// Make hardlink process executable
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = "chmod";
						process.StartInfo.Arguments = "+x \"" + LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();
						
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink";
						process.StartInfo.Arguments = "\"" + currentTargetMetadataDirectoryPath + "\" \"" + metadataPath + "\"";
					} else {
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "junction.exe";
						process.StartInfo.Arguments = "/accepteula \"" + metadataPath + "\" \"" + currentTargetMetadataDirectoryPath + "\"";
					}

					// Wait for process to end
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.CreateNoWindow = true;
					process.Start();
					process.WaitForExit();
					process.Dispose();
					
					EditorUtility.ClearProgressBar();
				} else if (hardLinkIsValid == false) {
					// Cleanup current target directories & files
					if (System.IO.Directory.Exists(currentTargetMetadataDirectoryPath))
						LocalCache.Shared.DeleteDirectory(currentTargetMetadataDirectoryPath);
					if (System.IO.File.Exists(currentTargetMetadataDirectoryPath + ".jcf"))
						System.IO.File.Delete(currentTargetMetadataDirectoryPath + ".jcf");
					if (System.IO.File.Exists(currentTargetMetadataDirectoryPath + ".jcf.lz4"))
						System.IO.File.Delete(currentTargetMetadataDirectoryPath + ".jcf.lz4");
					if (System.IO.File.Exists(currentTargetMetadataDirectoryPath.Substring(0, currentTargetMetadataDirectoryPath.Length - 9) + "_timestamps"))
						System.IO.File.Delete(currentTargetMetadataDirectoryPath.Substring(0, currentTargetMetadataDirectoryPath.Length - 9) + "_timestamps");
					
					// Copy current data to new metadata path
					LocalCache.Shared.DirectoryCopy(hardLinkDirectory, currentTargetMetadataDirectoryPath, true);
					
					System.Diagnostics.Process process = new System.Diagnostics.Process();
					if (Application.platform == RuntimePlatform.OSXEditor) {
						// Make hardlink process executable
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = "chmod";
						process.StartInfo.Arguments = "+x \"" + LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();
						
						// Delete current hard link
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink";
						process.StartInfo.Arguments = "-u \"" + metadataPath + "\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();

						// Make new hard link
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink";
						process.StartInfo.Arguments = "\"" + currentTargetMetadataDirectoryPath + "\" \"" + metadataPath + "\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();
					} else {
						// Delete current hard link
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "junction.exe";
						process.StartInfo.Arguments = "/accepteula -d \"" + metadataPath + "\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();
						
						// Make new hard link
						process = new System.Diagnostics.Process();
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "junction.exe";
						process.StartInfo.Arguments = "/accepteula \"" + metadataPath + "\" \"" + currentTargetMetadataDirectoryPath + "\"";
						process.StartInfo.UseShellExecute = false;
						process.StartInfo.RedirectStandardOutput = true;
						process.StartInfo.CreateNoWindow = true;
						process.Start();
						process.WaitForExit();
						process.Dispose();
					}
					
					// Cleanup previous target directories & files
					if (System.IO.Directory.Exists(hardLinkDirectory))
						LocalCache.Shared.DeleteDirectory(hardLinkDirectory);
					if (System.IO.File.Exists(hardLinkDirectory + ".jcf"))
						System.IO.File.Delete(hardLinkDirectory + ".jcf");
					if (System.IO.File.Exists(hardLinkDirectory + ".jcf.lz4"))
						System.IO.File.Delete(hardLinkDirectory + ".jcf.lz4");
					if (System.IO.File.Exists(hardLinkDirectory.Substring(0, hardLinkDirectory.Length - 9) + "_timestamps"))
						System.IO.File.Delete(hardLinkDirectory.Substring(0, hardLinkDirectory.Length - 9) + "_timestamps");
				}
			} else {
				if (metadataIsHardLink == true) {
					EditorUtility.DisplayProgressBar("Hold on", "Disabling hard links...", 0.5f);
					
					System.Diagnostics.Process process = new System.Diagnostics.Process();
					if (Application.platform == RuntimePlatform.OSXEditor) {
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "hardlink";
						process.StartInfo.Arguments = "-u \"" + metadataPath + "\"";
					} else {
						process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "junction.exe";
						process.StartInfo.Arguments = "/accepteula -d \"" + metadataPath + "\"";
					}
					
					// Wait for process to end
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.CreateNoWindow = true;
					process.Start();
					process.WaitForExit();
					process.Dispose();
					
					FixPermissions();
					
					if (System.IO.Directory.Exists(metadataPath))
						LocalCache.Shared.DeleteDirectory(metadataPath);
					
					LocalCache.Shared.DirectoryCopy(hardLinkDirectory, metadataPath, true);
					
					// Cleanup previous target directories & files if hard link was invalid
					if (hardLinkIsValid == false) {
						if (System.IO.Directory.Exists(hardLinkDirectory))
							LocalCache.Shared.DeleteDirectory(hardLinkDirectory);
						if (System.IO.File.Exists(hardLinkDirectory + ".jcf"))
							System.IO.File.Delete(hardLinkDirectory + ".jcf");
						if (System.IO.File.Exists(hardLinkDirectory + ".jcf.lz4"))
							System.IO.File.Delete(hardLinkDirectory + ".jcf.lz4");
						if (System.IO.File.Exists(hardLinkDirectory.Substring(0, hardLinkDirectory.Length - 9) + "_timestamps"))
							System.IO.File.Delete(hardLinkDirectory.Substring(0, hardLinkDirectory.Length - 9) + "_timestamps");
					}
					
					EditorUtility.ClearProgressBar();
				}
			}
		}
		
		public static void FixPermissions() {
			string assetsPath = Application.dataPath;
			string libraryPath = assetsPath.Remove(assetsPath.Length - 6, 6) + "Library";
			string cachePath = assetsPath.Remove(assetsPath.Length - 6, 6) + "LocalCache";
			string metadataPath = libraryPath + "/metadata";
			Directory.CreateDirectory(cachePath);
			
			if (Application.platform == RuntimePlatform.OSXEditor) {
				// Chmod/Chown metadata
				System.Diagnostics.Process process;
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "whoami";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				
				string whoami = process.StandardOutput.ReadLine();

				process.Dispose();
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "find";
				process.StartInfo.Arguments = "\"" + metadataPath + "\" -type d -exec chmod 755 {} +";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				process.Dispose();
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "find";
				process.StartInfo.Arguments = "\"" + metadataPath + "\" -type f -exec chmod 644 {} +";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				process.Dispose();
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "chown";
				process.StartInfo.Arguments = "-RH \""+whoami+":staff\" \"" + metadataPath + "\"";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				process.Dispose();
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "find";
				process.StartInfo.Arguments = "\"" + cachePath + "\" -type d -exec chmod 755 {} +";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				process.Dispose();
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "find";
				process.StartInfo.Arguments = "\"" + cachePath + "\" -type f -exec chmod 644 {} +";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				process.Dispose();
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "chown";
				process.StartInfo.Arguments = "-RH \""+whoami+":staff\" \"" + cachePath + "\"";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				process.Dispose();
			}
		}
		
		public static void Cleanup() {
			System.IO.Directory.CreateDirectory(LocalCache.Shared.CachePath);
			
			string[] cleanupFiles = Directory.GetFiles(LocalCache.Shared.CachePath, "*.cleanup", SearchOption.TopDirectoryOnly);
			
			foreach (var file in cleanupFiles) {
				var directory = file.Remove(file.Length - 8, 8);
				if (System.IO.Directory.Exists(directory))
					LocalCache.Shared.DeleteDirectory(directory);
				if (System.IO.File.Exists(directory + ".jcf"))
					System.IO.File.Delete(directory + ".jcf");
				if (System.IO.File.Exists(directory + ".jcf.lz4"))
					System.IO.File.Delete(directory + ".jcf.lz4");
				if (System.IO.File.Exists(directory.Substring(0, directory.Length - 9) + "_timestamps"))
					System.IO.File.Delete(directory.Substring(0, directory.Length - 9) + "_timestamps");
				
				System.IO.File.Delete(file);
			}
		}
	}
}