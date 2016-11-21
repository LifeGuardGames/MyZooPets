//
//  Preferences.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using UnityEngine;
using UnityEditor;

namespace Jemast.LocalCache {
	public class Preferences {
		public static void Warmup() {
			// Dumb but effective
			autoCompress = AutoCompress;
			compressionAlgorithm = CompressionAlgorithm;
			compressionQualityLZ4 = CompressionQualityLZ4;
			localCacheVersion = LocalCacheVersion;
			enableLogFile = EnableLogFile;
			enableHardLinks = EnableHardLinks;
		}
		
		private static int? localCacheVersion;
		public static int LocalCacheVersion {
			get {
				if (!localCacheVersion.HasValue)
					localCacheVersion = EditorPrefs.GetInt("Jemast_LocalCache_Version", 2);
				
				return localCacheVersion.Value;
			}
			set {
				if (localCacheVersion != value) {
					localCacheVersion = value;
					EditorPrefs.SetInt("Jemast_LocalCache_Version", value);
				}
			}
		}
		
		private static bool? enableLogFile;
		public static bool EnableLogFile { 
			get {
				if (!enableLogFile.HasValue)
					enableLogFile = EditorPrefs.GetBool("Jemast_LocalCache_Enable_Log_File", false);
				
				return enableLogFile.Value;
			}
			set {
				if (enableLogFile != value) {
					enableLogFile = value;
					EditorPrefs.SetBool("Jemast_LocalCache_Enable_Log_File", value);
				}
			}
		}
		
		private static bool? enableHardLinks;
		public static bool EnableHardLinks { 
			get {
				if (!enableHardLinks.HasValue)
					enableHardLinks = EditorPrefs.GetBool("Jemast_LocalCache_Enable_Hard_Links", false);
				
				return enableHardLinks.Value;
			}
			set {
				if (enableHardLinks != value) {
					enableHardLinks = value;
					EditorPrefs.SetBool("Jemast_LocalCache_Enable_Hard_Links", value);
				}
			}
		}
		
		private static bool? autoCompress;
		public static bool AutoCompress { 
			get {
				if (!autoCompress.HasValue)
					autoCompress = EditorPrefs.GetBool("Jemast_LocalCache_Automatic_Compression", false);
				
				return autoCompress.Value;
			}
			set {
				if (autoCompress != value) {
					autoCompress = value;
					EditorPrefs.SetBool("Jemast_LocalCache_Automatic_Compression", value);
				}
			}
		}
		
		private static int? compressionAlgorithm;
		public static int CompressionAlgorithm {
			get {
				if (!compressionAlgorithm.HasValue)
					compressionAlgorithm = EditorPrefs.GetInt("Jemast_LocalCache_Compression_Algorithm", 0);
				
				return compressionAlgorithm.Value;
			}
			set {
				if (compressionAlgorithm != value) {
					compressionAlgorithm = value;
					EditorPrefs.SetInt("Jemast_LocalCache_Compression_Algorithm", value);
				}
			}
		}
		
		private static int? compressionQualityLZ4;
		public static int CompressionQualityLZ4 {
			get {
				if (!compressionQualityLZ4.HasValue)
					compressionQualityLZ4 = EditorPrefs.GetInt("Jemast_LocalCache_Compression_Quality_LZ4", 0);
				
				return compressionQualityLZ4.Value;
			}
			set {
				if (compressionQualityLZ4 != value) {
					compressionQualityLZ4 = value;
					EditorPrefs.SetInt("Jemast_LocalCache_Compression_Quality_LZ4", value);
				}
			}
		}
	}
}
