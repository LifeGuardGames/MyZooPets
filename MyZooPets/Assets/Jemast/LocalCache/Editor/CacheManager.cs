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
		public static bool ShouldSwitchPlatform = false;
		
		public static void SwitchPlatform(LocalCache.Shared.CacheTarget? previousCacheTarget, LocalCache.Shared.CacheSubtarget? previousCacheSubtarget, LocalCache.Shared.CacheTarget? newCacheTarget, LocalCache.Shared.CacheSubtarget? newCacheSubtarget, bool silent) {
			BuildTarget? newBuildTarget = LocalCache.Shared.BuildTargetForCacheTarget(newCacheTarget);
			
			if (previousCacheTarget == null || newBuildTarget == null || newCacheTarget == null || (newBuildTarget == EditorUserBuildSettings.activeBuildTarget && previousCacheSubtarget == newCacheSubtarget))
				return;
			
			if (silent == false) {
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
			LocalCache.LogUtility.LogImmediate("Switching from {0} ({1}) to {2} ({3})", previousCacheTarget.ToString(), previousCacheSubtarget.HasValue ? previousCacheSubtarget.Value.ToString() : "Base", newCacheTarget, newCacheSubtarget.HasValue ? newCacheSubtarget.Value.ToString() : "Base");
				
			// Save all import manager timestamps
			string assetsPath = Application.dataPath;
			int assetsPathLength = assetsPath.Length - 6;
			
			string cachePath = assetsPath.Remove(assetsPath.Length - 6, 6) + "LocalCache";
			Directory.CreateDirectory(cachePath);
			
			string[] assetFiles = Directory.GetFiles(assetsPath, "*.*", SearchOption.AllDirectories);
			//string[] assetDirectories = Directory.GetDirectories(assetsPath, "*.*", SearchOption.AllDirectories);
			
			LocalCache.LogUtility.LogImmediate("Writing import settings to disk and fetching assets timestamps");
			
			Dictionary<string,ulong> timestamps = new Dictionary<string, ulong>();
			
			foreach (var file in assetFiles) {
				if (file.EndsWith(".meta"))
					continue;
				if (file.EndsWith(".DS_Store"))
					continue;
				
				string assetPath = file.Remove(0, assetsPathLength);
				
				AssetDatabase.WriteImportSettingsIfDirty(assetPath);
				
				AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
				if (assetImporter != null) {
					timestamps.Add(assetPath, assetImporter.assetTimeStamp);
				}
			}
			
			LocalCache.LogUtility.LogImmediate("Serializing asset timestamps to disk");
			
			string currentTargetMetadataTimestampsPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)previousCacheTarget] + (previousCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)previousCacheSubtarget] : "") + "timestamps";
			using (FileStream destFileStream = File.Create(currentTargetMetadataTimestampsPath)) {
				SerializeTimestamps(timestamps, destFileStream);
			}
			
			SwitchOperationInProgress = true;
			PlatformRefreshInProgress = true;
			ShouldSwitchPlatform = true;
		}
		
		public static void SwitchPlatformOperation(LocalCache.Shared.CacheTarget? previousCacheTarget, LocalCache.Shared.CacheSubtarget? previousCacheSubtarget, LocalCache.Shared.CacheTarget? newCacheTarget, LocalCache.Shared.CacheSubtarget? newCacheSubtarget) {
			ShouldSwitchPlatform = false;
			ShouldRefreshUI = true;
			
			BuildTarget? newBuildTarget = LocalCache.Shared.BuildTargetForCacheTarget(newCacheTarget);
			PlatformRefreshCurrentScene = EditorApplication.currentScene;
			EditorApplication.NewScene();

			if (newCacheSubtarget != null && previousCacheTarget == newCacheTarget && previousCacheSubtarget != newCacheSubtarget) {
				PlatformRefreshShouldBustCache = true;
			}

			string assetsPath = Application.dataPath;
			int assetsPathLength = assetsPath.Length - 6;
			
			string libraryPath = assetsPath.Remove(assetsPath.Length - 6, 6) + "Library";
			string cachePath = assetsPath.Remove(assetsPath.Length - 6, 6) + "LocalCache";
			
			Directory.CreateDirectory(cachePath);
			
			string metadataPath = libraryPath + "/metadata";
			
			string currentTargetMetadataDirectoryPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)previousCacheTarget] + (previousCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)previousCacheSubtarget] : "") + "metadata";
			string newTargetMetadataDirectoryPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)newCacheTarget] + (newCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)newCacheSubtarget] : "") + "metadata";

			string currentTargetMetadataTimestampsPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)previousCacheTarget] + (previousCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)previousCacheSubtarget] : "") + "timestamps";
			string newTargetMetadataTimestampsPath = cachePath + "/" + LocalCache.Shared.CacheTargetPrefixes[(int)newCacheTarget] + (newCacheSubtarget.HasValue ? LocalCache.Shared.CacheSubtargetPrefixes[(int)newCacheSubtarget] : "") + "timestamps";
			
			LocalCache.LogUtility.LogImmediate("Attempting to perform switch operation");
			
			try
			{
				// Preemptively get a list of assets
				string[] assetFiles = Directory.GetFiles(assetsPath, "*.*", SearchOption.AllDirectories);
				string[] assetDirectories = Directory.GetDirectories(assetsPath, "*.*", SearchOption.AllDirectories);
				
				LocalCache.LogUtility.LogImmediate("Performing decompression of current build target if required");
				
				// Save current platform
				LocalCache.CompressionManager.PerformDecompression(currentTargetMetadataDirectoryPath);
				
				LocalCache.LogUtility.LogImmediate("Merging current build target to cache");
				
				// Perform save
				progressTitle = "Hold on";
				progressDescription = "Saving current platform to cache...";
				progress = 0f;
				DirectoryMerge(metadataPath, currentTargetMetadataDirectoryPath);
				
				LocalCache.LogUtility.LogImmediate("Performing decompression of new build target if required");
				
				// Override with new platform
				LocalCache.CompressionManager.PerformDecompression(newTargetMetadataDirectoryPath);
				
				LocalCache.LogUtility.LogImmediate("Merging new build target to cache");
				
				// Perform override
				progressTitle = "Hold on";
				progressDescription = "Attempting to load previous platform from cache...";
				progress = 0f;
				DirectoryMerge(newTargetMetadataDirectoryPath, metadataPath);
				
				// Chmod/Chown metadata
				if (Application.platform == RuntimePlatform.OSXEditor) {
					LocalCache.LogUtility.LogImmediate("Setting correct permissions on metadata");
						
					System.Diagnostics.Process process;
					
					process = new System.Diagnostics.Process();
					process.StartInfo.FileName = "find";
					process.StartInfo.Arguments = "\"" + metadataPath + "\" -type d -exec chmod 755 {} +";
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.CreateNoWindow = true;
					process.Start();
					
					process.WaitForExit();
					
					process = new System.Diagnostics.Process();
					process.StartInfo.FileName = "find";
					process.StartInfo.Arguments = "\"" + metadataPath + "\" -type f -exec chmod 644 {} +";
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.CreateNoWindow = true;
					process.Start();
					
					process.WaitForExit();
					
					process = new System.Diagnostics.Process();
					process.StartInfo.FileName = "whoami";
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.CreateNoWindow = true;
					process.Start();
				
					process.WaitForExit();
					
					string whoami = process.StandardOutput.ReadLine();
					
					process = new System.Diagnostics.Process();
					process.StartInfo.FileName = "chown";
					process.StartInfo.Arguments = "-RH \""+whoami+":staff\" \"" + metadataPath + "\"";
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.CreateNoWindow = true;
					process.Start();
					
					process.WaitForExit();
				}
				
				LocalCache.LogUtility.LogImmediate("Deleting excess metadata files");
				
				// Look for excess metadata files
				string[] metadataFiles = Directory.GetFiles(metadataPath, "*.*", SearchOption.AllDirectories);
				foreach (var file in metadataFiles) {
					string assetPath = AssetDatabase.GUIDToAssetPath(System.IO.Path.GetFileName(file));
					if (string.IsNullOrEmpty (assetPath) || (!File.Exists(LocalCache.Shared.ProjectPath + assetPath) && !Directory.Exists(LocalCache.Shared.ProjectPath + assetPath))) {
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
				else if (newCacheTarget == LocalCache.Shared.CacheTarget.BB10) {
					BlackBerryBuildSubtarget? blackberrySubtarget = LocalCache.Shared.BlackBerryBuildSubtargetForCacheSubtarget(newCacheSubtarget);
					if (blackberrySubtarget.HasValue)
						EditorUserBuildSettings.blackberryBuildSubtarget = blackberrySubtarget.Value;
				}
#endif
				
				EditorUserBuildSettings.SwitchActiveBuildTarget(newBuildTarget.Value);
				
				if (EditorUserBuildSettings.activeBuildTarget != newBuildTarget.Value)
					throw new System.Exception();
				
				// Make as selected build target group
				BuildTargetGroup? newBuildTargetGroup = LocalCache.Shared.BuildTargetGroupForCacheTarget(newCacheTarget);
				if (newBuildTargetGroup.HasValue)
					EditorUserBuildSettings.selectedBuildTargetGroup = newBuildTargetGroup.Value;
				
				// Platform refresh
				SwitchOperationInProgress = true;
				PlatformRefreshInProgress = true;
				
				LocalCache.LogUtility.LogImmediate("Looking for missing metadata files");
				
				// Look for missing metadata files
				int currentAsset = 0;
				int assetCount = assetFiles.Length + assetDirectories.Length;
				foreach (var file in assetFiles) {
					if (file.EndsWith(".meta"))
						continue;
					if (file.EndsWith(".DS_Store"))
						continue;
					
					EditorUtility.DisplayProgressBar("Hold on", "Reimporting changed assets...", (float)currentAsset/(float)assetCount);
					string assetPath = file.Remove(0, assetsPathLength);
					string guid = AssetDatabase.AssetPathToGUID(assetPath);
					if (!string.IsNullOrEmpty(guid) && !File.Exists(metadataPath + "/" + guid.Substring(0,2) + "/" + guid))
						AssetDatabase.ImportAsset (assetPath, ImportAssetOptions.ForceUpdate);
					currentAsset++;
				}
				
				foreach (var directory in assetDirectories) {
					EditorUtility.DisplayProgressBar("Hold on", "Reimporting changed assets...", (float)currentAsset/(float)assetCount);
					string assetPath = directory.Remove(0, assetsPathLength);
					string guid = AssetDatabase.AssetPathToGUID(assetPath);
					if (!string.IsNullOrEmpty(guid) && !File.Exists(metadataPath + "/" + guid.Substring(0,2) + "/" + guid))
						AssetDatabase.ImportAsset (assetPath, ImportAssetOptions.ForceUpdate);
					currentAsset++;
				}
				
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
				LocalCache.LogUtility.LogImmediate("Switch operation error: " + e.Message);
				
				// Cleanup
				if (Directory.Exists(currentTargetMetadataDirectoryPath))
					Directory.Delete(currentTargetMetadataDirectoryPath, true);
				
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
				Directory.Delete(metadataCacheFolder, true);
			if (File.Exists(metadataCacheLZ4File))
				File.Delete(metadataCacheLZ4File);
			if (File.Exists(metadataCacheTimestampsFile))
				File.Delete(metadataCacheTimestampsFile);
			
			return;
		}
		
		public static void ClearAllCache() {
			// Delete local cache folder
			if (Directory.Exists(LocalCache.Shared.CachePath))
				Directory.Delete(LocalCache.Shared.CachePath, true);
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
			if (background) {
				Directory.CreateDirectory(LocalCache.Shared.CachePath);
				
				var backgroundLockFilePath = LocalCache.Shared.CachePath + "Background.txt";
				var stream = File.Create(backgroundLockFilePath);
				stream.Dispose();
				
				System.Threading.ThreadPool.QueueUserWorkItem(delegate {
					CompressAllCacheOperation(true);
					
					File.Delete(backgroundLockFilePath);
					
					ShouldRefreshUI = true;
				});
			} else {
				CompressAllCacheOperation(false);
			}
		}
		
		private static void CompressAllCacheOperation(bool background) {
			// Process all targets & subtargets
			for (int i = 0; i < (int)LocalCache.Shared.CacheTarget.Count; i++) {
				var target = (LocalCache.Shared.CacheTarget)i;
				
				if (target == LocalCache.Shared.CacheTarget.Android) {
					for (int j = (int)LocalCache.Shared.CacheSubtarget.Android_First + 1; j < (int)LocalCache.Shared.CacheSubtarget.Android_Last; j++) {
						var subtarget = (LocalCache.Shared.CacheSubtarget)j;
						if (GetCacheStatus(target, subtarget) && !GetCacheCompression(target, subtarget))
							CompressCache(target, subtarget, background);
					}
				}
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
				else if (target == LocalCache.Shared.CacheTarget.BB10) {
					for (int j = (int)LocalCache.Shared.CacheSubtarget.BB10_First + 1; j < (int)LocalCache.Shared.CacheSubtarget.BB10_Last; j++) {
						var subtarget = (LocalCache.Shared.CacheSubtarget)j;
						if (GetCacheStatus(target, subtarget) && !GetCacheCompression(target, subtarget))
							CompressCache(target, subtarget, background);
					}
				}
#endif
				else {
					if (GetCacheStatus(target, null) && !GetCacheCompression(target, null))
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
				else if (target == LocalCache.Shared.CacheTarget.BB10) {
					for (int j = (int)LocalCache.Shared.CacheSubtarget.BB10_First + 1; j < (int)LocalCache.Shared.CacheSubtarget.BB10_Last; j++) {
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
			//ClearAllCache();
			
			// Delete local cache folder
			if (Directory.Exists(LocalCache.Shared.CachePath))
				Directory.Delete(LocalCache.Shared.CachePath, true);
			
			// Delete metadata folder
			string assetsPath = Application.dataPath;
			string libraryPath = assetsPath.Remove(assetsPath.Length - 6, 6) + "Library";
			if (Directory.Exists(libraryPath + "/metadata"))
				Directory.Delete(libraryPath + "/metadata", true);
			
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
				Directory.CreateDirectory(Path.GetDirectoryName(destFile));
				
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
	}
}