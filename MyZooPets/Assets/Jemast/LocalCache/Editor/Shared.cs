﻿//
//  Shared.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace Jemast.LocalCache {
	public class Shared {
		
		public static readonly string ProjectPath = Application.dataPath.Remove(Application.dataPath.Length - 6, 6);
		public static readonly string CachePath = ProjectPath + "LocalCache/";

		private static string utilsPaths = null;
		public static string UtilsPaths {
			get {
				if (utilsPaths != null)
					return utilsPaths;
				
				// Recursively parse assets to look for JLocalCacheWindow.cs
				var fileList = new DirectoryInfo(ProjectPath + "Assets").GetFiles("lz4.exe", SearchOption.AllDirectories); 
				if (fileList.Length == 1)
					utilsPaths = fileList[0].DirectoryName.Substring(ProjectPath.Length, fileList[0].DirectoryName.Length - ProjectPath.Length).Replace('\\', '/') + '/';
				else
					utilsPaths = "Assets/Jemast/Shared/Editor/Utils/";
				
				return utilsPaths;
			}
		}

		private static string editorAssetsPath = null;
		public static string EditorAssetsPath {
			get {
				if (editorAssetsPath != null)
					return editorAssetsPath;

				// Recursively parse assets to look for JLocalCacheWindow.cs
				var fileList = new DirectoryInfo(ProjectPath + "Assets").GetFiles("JLocalCacheWindow.cs", SearchOption.AllDirectories); 
				if (fileList.Length == 1)
					editorAssetsPath = fileList[0].DirectoryName.Substring(ProjectPath.Length, fileList[0].DirectoryName.Length - ProjectPath.Length).Replace('\\', '/') + '/';
				else
					editorAssetsPath = "Assets/Jemast/LocalCache/Editor/";

				return editorAssetsPath;
			}
		}
		
		public enum CacheTarget {
			WebPlayer,
			Standalone,
			iOS,
			Android,
	#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			BB10,
			Metro,
			WP8,
	#endif
	#if !UNITY_3_4 && !UNITY_3_5
			NaCl,
	#endif
			Flash,
			PS3,
			X360,
			Wii,
			Count
		};
		
		public enum CacheSubtarget {
			Android_First,
			Android_DXT,
			Android_PVRTC,
			Android_ATC,
			Android_ETC,
	#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			Android_ETC2,
	#endif
	#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			Android_ASTC,
	#endif
			Android_Last,
	#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			BB10_First,
			BB10_PVRTC,
			BB10_ATC,
			BB10_ETC,
			BB10_Last,
	#endif
			Count
		}
			
		static public string[] CacheTargetPrefixes = new string[] {
			"WEBPLAYER_",
			"STANDALONE_",
			"IOS_",
			"ANDROID_",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			"BB10_",
			"METRO_",
			"WP8_",
#endif
#if !UNITY_3_4 && !UNITY_3_5
			"NACL_",
#endif
			"FLASH_",
			"PS3_",
			"X360_",
			"WII_"
		};
		
		static public string[] CacheSubtargetPrefixes = new string[] {
			"Android_First_",
			"DXT_",
			"PVRTC_",
			"ATC_",
			"ETC1_",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			"ETC2_",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			"ASTC_",
#endif
			"Android_Last_",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			"BlackBerry_First_",
			"PVRTC_",
			"ATC_",
			"ETC1_",
			"BlackBerry_Last_"
#endif
		};
		
		public static LocalCache.Shared.CacheTarget? CacheTargetForBuildTarget(BuildTarget? target) {
			switch (target) {
			case BuildTarget.Android:
				return LocalCache.Shared.CacheTarget.Android;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case BuildTarget.BB10:
				return LocalCache.Shared.CacheTarget.BB10;
#endif
			case BuildTarget.FlashPlayer:
				return LocalCache.Shared.CacheTarget.Flash;
			case BuildTarget.iPhone:
				return LocalCache.Shared.CacheTarget.iOS;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case BuildTarget.MetroPlayer:
				return LocalCache.Shared.CacheTarget.Metro;
#endif
#if !UNITY_3_4 && !UNITY_3_5
			case BuildTarget.NaCl:
				return LocalCache.Shared.CacheTarget.NaCl;
#endif
			case BuildTarget.PS3:
				return LocalCache.Shared.CacheTarget.PS3;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
#endif
			case BuildTarget.StandaloneOSXIntel:
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
#endif
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return LocalCache.Shared.CacheTarget.Standalone;
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
				return LocalCache.Shared.CacheTarget.WebPlayer;
			case BuildTarget.Wii:
				return LocalCache.Shared.CacheTarget.Wii;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case BuildTarget.WP8Player:
				return LocalCache.Shared.CacheTarget.WP8;
#endif
			case BuildTarget.XBOX360:
				return LocalCache.Shared.CacheTarget.X360;
			default:
				return null;
			}
		}
		
		public static BuildTarget? BuildTargetForCacheTarget(LocalCache.Shared.CacheTarget? option) {
			switch (option) {
			case LocalCache.Shared.CacheTarget.Android:
				return BuildTarget.Android;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case LocalCache.Shared.CacheTarget.BB10:
				return BuildTarget.BB10;
#endif
			case LocalCache.Shared.CacheTarget.Flash:
				return BuildTarget.FlashPlayer;
			case LocalCache.Shared.CacheTarget.iOS:
				return BuildTarget.iPhone;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case LocalCache.Shared.CacheTarget.Metro:
				return BuildTarget.MetroPlayer;
#endif
#if !UNITY_3_4 && !UNITY_3_5
			case LocalCache.Shared.CacheTarget.NaCl:
				return BuildTarget.NaCl;
#endif
			case LocalCache.Shared.CacheTarget.PS3:
				return BuildTarget.PS3;
			case LocalCache.Shared.CacheTarget.Standalone:
				return BuildTarget.StandaloneWindows;
			case LocalCache.Shared.CacheTarget.WebPlayer:
				return BuildTarget.WebPlayer;
			case LocalCache.Shared.CacheTarget.Wii:
				return BuildTarget.Wii;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case LocalCache.Shared.CacheTarget.WP8:
				return BuildTarget.WP8Player;
#endif
			case LocalCache.Shared.CacheTarget.X360:
				return BuildTarget.XBOX360;
			default:
				return null;
			}
		}
		
		public static LocalCache.Shared.CacheSubtarget? CacheSubtargetForAndroidBuildSubtarget(AndroidBuildSubtarget? target) {
			switch (target) {
			case AndroidBuildSubtarget.ATC:
				return LocalCache.Shared.CacheSubtarget.Android_ATC;
			case AndroidBuildSubtarget.DXT:
				return LocalCache.Shared.CacheSubtarget.Android_DXT;
			case AndroidBuildSubtarget.Generic:
			case AndroidBuildSubtarget.ETC:
				return LocalCache.Shared.CacheSubtarget.Android_ETC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case AndroidBuildSubtarget.ETC2:
				return LocalCache.Shared.CacheSubtarget.Android_ETC2;
#endif
			case AndroidBuildSubtarget.PVRTC:
				return LocalCache.Shared.CacheSubtarget.Android_PVRTC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			case AndroidBuildSubtarget.ASTC:
				return LocalCache.Shared.CacheSubtarget.Android_ASTC;
#endif
			default:
				return null;
			}
		}
		
		public static AndroidBuildSubtarget? AndroidBuildSubtargetForCacheSubtarget(LocalCache.Shared.CacheSubtarget? target) {
			switch (target) {
			case LocalCache.Shared.CacheSubtarget.Android_ATC:
				return AndroidBuildSubtarget.ATC;
			case LocalCache.Shared.CacheSubtarget.Android_DXT:
				return AndroidBuildSubtarget.DXT;
			case LocalCache.Shared.CacheSubtarget.Android_ETC:
				return AndroidBuildSubtarget.ETC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case LocalCache.Shared.CacheSubtarget.Android_ETC2:
				return AndroidBuildSubtarget.ETC2;
#endif
			case LocalCache.Shared.CacheSubtarget.Android_PVRTC:
				return AndroidBuildSubtarget.PVRTC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			case LocalCache.Shared.CacheSubtarget.Android_ASTC:
				return AndroidBuildSubtarget.ASTC;
#endif
			default:
				return null;
			}
		}
		
		
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		public static LocalCache.Shared.CacheSubtarget? CacheSubtargetForBlackBerryBuildSubtarget(BlackBerryBuildSubtarget? target) {
			switch (target) {
			case BlackBerryBuildSubtarget.PVRTC:
				return LocalCache.Shared.CacheSubtarget.BB10_PVRTC;
			case BlackBerryBuildSubtarget.ATC:
				return LocalCache.Shared.CacheSubtarget.BB10_ATC;
			case BlackBerryBuildSubtarget.ETC:
				return LocalCache.Shared.CacheSubtarget.BB10_ETC;
			default:
				return null;
			}
		}
#endif
		
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		public static BlackBerryBuildSubtarget? BlackBerryBuildSubtargetForCacheSubtarget(LocalCache.Shared.CacheSubtarget? target) {
			switch (target) {
			case LocalCache.Shared.CacheSubtarget.BB10_PVRTC:
				return BlackBerryBuildSubtarget.PVRTC;
			case LocalCache.Shared.CacheSubtarget.BB10_ATC:
				return BlackBerryBuildSubtarget.ATC;
			case LocalCache.Shared.CacheSubtarget.BB10_ETC:
				return BlackBerryBuildSubtarget.ETC;
			default:
				return null;
			}
		}
#endif
		
		public static BuildTargetGroup? BuildTargetGroupForCacheTarget(LocalCache.Shared.CacheTarget? option) {
			switch (option) {
			case LocalCache.Shared.CacheTarget.Android:
				return BuildTargetGroup.Android;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case LocalCache.Shared.CacheTarget.BB10:
				return BuildTargetGroup.BB10;
#endif
			case LocalCache.Shared.CacheTarget.Flash:
				return BuildTargetGroup.FlashPlayer;
			case LocalCache.Shared.CacheTarget.iOS:
				return BuildTargetGroup.iPhone;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case LocalCache.Shared.CacheTarget.Metro:
				return BuildTargetGroup.Metro;
#endif
#if !UNITY_3_4 && !UNITY_3_5
			case LocalCache.Shared.CacheTarget.NaCl:
				return BuildTargetGroup.NaCl;
#endif
			case LocalCache.Shared.CacheTarget.PS3:
				return BuildTargetGroup.PS3;
			case LocalCache.Shared.CacheTarget.Standalone:
				return BuildTargetGroup.Standalone;
			case LocalCache.Shared.CacheTarget.WebPlayer:
				return BuildTargetGroup.WebPlayer;
			case LocalCache.Shared.CacheTarget.Wii:
				return BuildTargetGroup.Wii;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			case LocalCache.Shared.CacheTarget.WP8:
				return BuildTargetGroup.WP8;
#endif
			case LocalCache.Shared.CacheTarget.X360:
				return BuildTargetGroup.XBOX360;
			default:
				return null;
			}
		}
		
		public static bool IsWin64() { 
			string path = System.Environment.GetEnvironmentVariable("ProgramFiles");
			return path.Contains("x86");
		}
		
		public static string[] CompressionAlgorithmOptions = new string[] {
			"LZ4"
		};
		
		public static string[] CompressionQualityLZ4Options = new string[] {
			"Fast Compression",
			"High Compression (Slow)"
		};
	}
}
