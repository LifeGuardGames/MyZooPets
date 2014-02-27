//
//  JLocalCacheWindow.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using LocalCache = Jemast.LocalCache;
using System.IO;

public class JLocalCacheWindow : EditorWindow {
	
	static string[] buildTargetOptions = new string[] {
		"Web Player",
		"PC, Mac and Linux Standalone",
		"iOS",
		"Android",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		"Blackberry",
		"Windows Store Apps",
		"Windows Phone 8",
#endif
#if !UNITY_3_4 && !UNITY_3_5
		"Google Native Client",
#endif
		"Flash Player",
		"PS3",
		"Xbox 360",
		"Wii"
	};
	
	static string[][] buildTargetWithSubtargetsOptions = new string[][] {
		new string[] { "Web Player" },
		new string[] { "PC, Mac and Linux Standalone" },
		new string[] { "iOS" },
		new string[] { 
			"Android (DXT)",
			"Android (PVRTC)",
			"Android (ATC)",
			"Android (ETC1)",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			"Android (ETC2)",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			"Android (ASTC)"
#endif
		},
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		new string[] {
			"Blackberry (PVRTC)",
			"Blackberry (ATC)",
			"Blackberry (ETC1)",
		},
		new string[] { "Windows Store Apps" },
		new string[] { "Windows Phone 8" },
#endif
#if !UNITY_3_4 && !UNITY_3_5
		new string[] { "Google Native Client" },
#endif
		new string[] { "Flash Player" },
		new string[] { "PS3" },
		new string[] { "Xbox 360" },
		new string[] { "Wii" }
	};
	
	static string[] androidTextureCompressionOptions = new string[] {
		"DXT (Tegra)",
		"PVRTC (PowerVR)",
		"ATC (Adreno)",
		"ETC1 or RGBA16 (GLES 2.0)",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		"ETC2 (GLES 3.0)",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
		"ASTC"
#endif
	};
	
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
	static string[] blackberryTextureCompressionOptions = new string[] {
		"PVRTC (PowerVR)",
		"ATC (Adreno)",
		"ETC1 or RGBA16 (GLES 2.0)"
	};
#endif
	
	Texture2D[] buildTargetTextures = null;
	
	Texture2D[] androidSubtargetTextures = null;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
	Texture2D[] blackberrySubtargetTextures = null;
#endif
	
	Texture2D selectedBuildTargetTexture = null;
	Texture2D trashBuildTargetTexture = null;
	Texture2D zipBuildTargetTexture = null;
	Texture2D unzipBuildTargetTexture = null;
	
	LocalCache.Shared.CacheTarget? currentCacheTarget;
	LocalCache.Shared.CacheTarget? wantedCacheTarget;
	LocalCache.Shared.CacheSubtarget? currentCacheSubtarget;
	LocalCache.Shared.CacheSubtarget? wantedCacheSubtarget;
	
	int toolbarIndex = 0;
	string[] toolbarOptions = new string[] { "Platform", "Status", "Settings" };
	Vector2 scrollPos1 = Vector2.zero;
	Vector2 scrollPos2 = Vector2.zero;
	Vector2 scrollPos3 = Vector2.zero;
	Rect scrollViewRect;
	Rect[] listViewRects = null;
	
	struct CacheData {
		public bool CacheStatus;
		public bool CompressionStatus;
		public System.DateTime Date;
		public string Size;
	}
	
	CacheData[][] cacheData = null;
	long totalCacheSize = 0;
	
	// Styles
	GUIStyle listFontStyle = new GUIStyle();
	
	GUIStyle imageStyle = new GUIStyle();
	GUIStyle areaStyle = new GUIStyle();
	GUIStyle areaStyleAlt = new GUIStyle();
	GUIStyle areaStyleActive = new GUIStyle();
	
	GUIStyle statusFontStyle = new GUIStyle();
	GUIStyle statusFontCachedStyle = new GUIStyle();
	GUIStyle statusFontCompressedStyle = new GUIStyle();

	[MenuItem("Window/Fast Platform Switch")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(JLocalCacheWindow), false, "Platform");
	}
	
	void OnEnable() {
		// Listen for platform changes & cache manager refreshes
		//EditorUserBuildSettings.activeBuildTargetChanged += ActiveBuildTargetChanged;
		
		// Local Cache Version
		if (LocalCache.Preferences.LocalCacheVersion == -1) {
			LocalCache.Preferences.LocalCacheVersion = 1;
			LocalCache.CacheManager.ClearAllCache();
		}
		
		// Warmup preferences
		LocalCache.Preferences.Warmup();
		
		// Auto compress?
		if (LocalCache.Preferences.AutoCompress)
			LocalCache.CacheManager.CompressAllCache(true);
		
		// Force repaint
		LocalCache.CacheManager.ShouldRefreshUI = true;
		
		EditorApplication.update += EditorUpdate;
	}
	
	void OnGUI()
	{
		if (currentCacheTarget == null || areaStyle.normal.background == null)
			SetupWindow();

		bool isDirty = false;
		
		toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarOptions);
		
		GUILayout.Space(10);
		
		if (toolbarIndex == 0) {
			scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(40f*(float)buildTargetOptions.Length));
			for (int i = 0; i < buildTargetOptions.Length; i++) {
				if (wantedCacheTarget.HasValue && i == (int)wantedCacheTarget.Value)
					EditorGUILayout.BeginHorizontal(areaStyleActive, GUILayout.Height(40), GUILayout.ExpandWidth(true));
				else
					EditorGUILayout.BeginHorizontal((i % 2 == 0) ? areaStyle : areaStyleAlt, GUILayout.Height(40), GUILayout.ExpandWidth(true));
				
				GUILayout.Space(5);
				
				// Image
				EditorGUILayout.BeginVertical(GUILayout.Height(40));
				GUILayout.FlexibleSpace();
				GUILayout.Box(buildTargetTextures[i], imageStyle);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndVertical();
				
				GUILayout.Space(5);
				
				// Text
				EditorGUILayout.BeginVertical(GUILayout.Height(40));
				GUILayout.FlexibleSpace();
				GUILayout.Label(buildTargetOptions[i], listFontStyle);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndVertical();
				
				GUILayout.FlexibleSpace();
				
				// Selected platform
				if (i == (int)currentCacheTarget) {
					EditorGUILayout.BeginVertical(GUILayout.Height(40));
					GUILayout.FlexibleSpace();
					GUILayout.Box(selectedBuildTargetTexture, imageStyle);
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndVertical();
					
					GUILayout.Space(5);
				}
				
				EditorGUILayout.EndHorizontal();
				
				if (Event.current.type == EventType.Repaint) {
					listViewRects[i] = GUILayoutUtility.GetLastRect();
				}
			}
			EditorGUILayout.EndScrollView();
			
			if(Event.current.type == EventType.Repaint) {
				scrollViewRect = GUILayoutUtility.GetLastRect();
				for (int i = 0; i < listViewRects.Length; i++)
					listViewRects[i].y += scrollViewRect.y;
			}
			
			if (wantedCacheTarget == LocalCache.Shared.CacheTarget.Android) {
				GUILayout.Space(10);
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Texture Compression");
				GUILayout.FlexibleSpace();
		
				int androidCompressionOption = CacheSubtargetToAndroidCompressionOption(wantedCacheSubtarget ?? currentCacheSubtarget);
				androidCompressionOption = EditorGUILayout.Popup(androidCompressionOption, androidTextureCompressionOptions);
				wantedCacheSubtarget = AndroidCompressionOptionToCacheSubtarget(androidCompressionOption);
				GUILayout.EndHorizontal();
			}
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			else if (wantedCacheTarget == LocalCache.Shared.CacheTarget.BB10) {
				GUILayout.Space(10);
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Texture Compression");
				GUILayout.FlexibleSpace();
		
				int blackberryCompressionOption = CacheSubtargetToBlackBerryCompressionOption(wantedCacheSubtarget ?? currentCacheSubtarget);
				blackberryCompressionOption = EditorGUILayout.Popup(blackberryCompressionOption, blackberryTextureCompressionOptions);
				wantedCacheSubtarget = BlackBerryCompressionOptionToCacheSubtarget(blackberryCompressionOption);
				GUILayout.EndHorizontal();
			}
#endif
			else {
				wantedCacheSubtarget = null;
			}
			
			GUILayout.Space(10);
			
			GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlaying && !EditorApplication.isCompiling;
			if (GUILayout.Button("Switch Platform"))
			{
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				if (EditorSettings.externalVersionControl == ExternalVersionControl.Disabled) {
					if (EditorUtility.DisplayDialog("External Version Control Required", "You need to enable external version control for Fast Platform Switch proper operation.", "Enable Meta Files", "Cancel")) {
						EditorSettings.externalVersionControl = ExternalVersionControl.Generic;
					}
				}
#endif
				
				AssetDatabase.Refresh();
				
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				if (EditorSettings.externalVersionControl != ExternalVersionControl.Disabled) {
#endif
					if (!EditorApplication.isCompiling)
						LocalCache.CacheManager.SwitchPlatform(currentCacheTarget, currentCacheSubtarget, wantedCacheTarget, wantedCacheSubtarget, false);
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				}
#endif
			}
			GUI.enabled = true;
			
			GUILayout.Space(10);
			
			if (LocalCache.CacheManager.BackgroundCacheCompressionInProgress) {
				GUILayout.Label("Please wait for background compression to end...");
				GUILayout.Space(10);
			}
			
			if (LocalCache.CacheManager.PlatformRefreshInProgress) {
				GUILayout.Label("Please wait for platform refresh to end...");
				GUILayout.Space(10);
			} else if (EditorApplication.isCompiling) {
				GUILayout.Label("Please wait for script compilation to end...");
				GUILayout.Space(10);
			}
				
			
			// Handle list view events at the end
			Vector2 mousePosition = Event.current.mousePosition;
			if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && scrollViewRect.Contains(mousePosition))
	        {
				mousePosition.y += scrollPos1.y;
				for (int i = 0; i < listViewRects.Length; i++) {
					if (listViewRects[i].Contains(mousePosition)) {
						wantedCacheTarget = (LocalCache.Shared.CacheTarget)i;
						isDirty = true;
						break;
					}
				}
	        }
		} else if (toolbarIndex == 1) {
			int totalOptionsLength = 0;
			for (int i = 0; i < buildTargetWithSubtargetsOptions.Length; i++)
				totalOptionsLength += buildTargetWithSubtargetsOptions[i].Length;
			
			bool hasCacheData = cacheData != null;
			bool cacheDataIsInvalid = false;
			if (!hasCacheData) {
				cacheData = new CacheData[buildTargetWithSubtargetsOptions.Length][];
				totalCacheSize = 0;
			}
			
			scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(40f*(float)totalOptionsLength));
			var altStyle = false;
			for (int i = 0; i < buildTargetWithSubtargetsOptions.Length; i++) {
				if (!hasCacheData)
					cacheData[i] = new CacheData[buildTargetWithSubtargetsOptions[i].Length];
				
				for (int j = 0; j < buildTargetWithSubtargetsOptions[i].Length; j++) {
					LocalCache.Shared.CacheTarget cacheTarget = (LocalCache.Shared.CacheTarget)i;
					LocalCache.Shared.CacheSubtarget? cacheSubtarget = null;
					if (cacheTarget == LocalCache.Shared.CacheTarget.Android)
						cacheSubtarget = (LocalCache.Shared.CacheSubtarget)j + (int)LocalCache.Shared.CacheSubtarget.Android_First + 1;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
					else if (cacheTarget == LocalCache.Shared.CacheTarget.BB10)
						cacheSubtarget = (LocalCache.Shared.CacheSubtarget)j + (int)LocalCache.Shared.CacheSubtarget.BB10_First + 1;
#endif
					
					// Cacheception
					if (!hasCacheData) {
						cacheData[i][j].CacheStatus = LocalCache.CacheManager.GetCacheStatus (cacheTarget, cacheSubtarget);
						cacheData[i][j].CompressionStatus = LocalCache.CacheManager.GetCacheCompression(cacheTarget, cacheSubtarget);
					}
					
					EditorGUILayout.BeginHorizontal(altStyle == false ? areaStyle : areaStyleAlt, GUILayout.Height(40), GUILayout.ExpandWidth(true));
					altStyle = !altStyle;
					
					GUILayout.Space(5);
					
					// Image
					EditorGUILayout.BeginVertical(GUILayout.Height(40));
					GUILayout.FlexibleSpace();
					
					if (cacheTarget == LocalCache.Shared.CacheTarget.Android)
						GUILayout.Box(androidSubtargetTextures[j], imageStyle);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
					else if (cacheTarget == LocalCache.Shared.CacheTarget.BB10)
						GUILayout.Box(blackberrySubtargetTextures[j], imageStyle);
#endif
					else
						GUILayout.Box(buildTargetTextures[i], imageStyle);
					
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndVertical();
					
					GUILayout.Space(5);
					
					// Text
					EditorGUILayout.BeginVertical(GUILayout.Height(40));
					GUILayout.FlexibleSpace();
					GUILayout.Label(cacheData[i][j].CacheStatus ? "Cached" : "Not cached", cacheData[i][j].CacheStatus ? statusFontCachedStyle : statusFontStyle);
					GUILayout.Label(cacheData[i][j].CacheStatus ? (cacheData[i][j].CompressionStatus ? "Compressed" : "Uncompressed") : "-", cacheData[i][j].CompressionStatus ? statusFontCompressedStyle : statusFontStyle);
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndVertical();
					
					GUILayout.FlexibleSpace();
					
					if (cacheData[i][j].CacheStatus) {
						if (!hasCacheData) {
							cacheData[i][j].Date = LocalCache.CacheManager.GetCacheDate(cacheTarget, cacheSubtarget);
							long cacheSize = LocalCache.CacheManager.GetCacheSize (cacheTarget, cacheSubtarget);
							totalCacheSize += cacheSize;
							
							if (cacheSize >= 1073741824)
								cacheData[i][j].Size = (cacheSize * 9.31322575 * 1E-10).ToString("F2") + " GB";
							if (cacheSize >= 1048576)
								cacheData[i][j].Size = (cacheSize * 9.53674316 * 1E-7).ToString("F2") + " MB";
							else
								cacheData[i][j].Size = (cacheSize * 0.0009765625).ToString("F2") + " kB";
						}
						
						EditorGUILayout.BeginVertical(GUILayout.Height(40));
						GUILayout.FlexibleSpace();
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(cacheData[i][j].Date.ToString ("d"), statusFontStyle);
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(cacheData[i][j].Date.ToString ("t"), statusFontStyle);
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndVertical();
						
						GUILayout.Space(5);
						
						EditorGUILayout.BeginVertical(GUILayout.Height(40));
						GUILayout.FlexibleSpace();
						GUILayout.Label(cacheData[i][j].Size, statusFontStyle);
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndVertical();
						
						GUILayout.Space(5);
						
						EditorGUILayout.BeginVertical(GUILayout.Height(40));
						GUILayout.FlexibleSpace();
						GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlaying;
						if (cacheData[i][j].CompressionStatus) {
							if (GUILayout.Button(unzipBuildTargetTexture)) {
								LocalCache.CacheManager.DecompressCache(cacheTarget, cacheSubtarget);
								cacheDataIsInvalid = true;
								isDirty = true;
							}
						} else {
							if (GUILayout.Button(zipBuildTargetTexture)) {
								LocalCache.CacheManager.CompressCache(cacheTarget, cacheSubtarget);
								cacheDataIsInvalid = true;
								isDirty = true;
							}
						}
						GUI.enabled = true;
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndVertical();
						
						GUILayout.Space(5);
						
						EditorGUILayout.BeginVertical(GUILayout.Height(40));
						GUILayout.FlexibleSpace();
						GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlaying;
						if (GUILayout.Button(trashBuildTargetTexture)) {
							LocalCache.CacheManager.ClearCache(cacheTarget, cacheSubtarget);
							cacheDataIsInvalid = true;
							isDirty = true;
						}
						GUI.enabled = true;
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndVertical();
						
						GUILayout.Space(5);
					}
					
					EditorGUILayout.EndHorizontal();
					
					if (Event.current.type == EventType.Repaint) {
						listViewRects[i] = GUILayoutUtility.GetLastRect();
					}
				}
			}
			EditorGUILayout.EndScrollView();
			
			GUILayout.Space(10);
			if (totalCacheSize >= 1073741824)
				GUILayout.Label("Total cache size: " + (totalCacheSize * 9.31322575 * 1E-10).ToString("F2") + " GB");
			if (totalCacheSize >= 1048576)
				GUILayout.Label("Total cache size: " + (totalCacheSize * 9.53674316 * 1E-7).ToString("F2") + " MB");
			else
				GUILayout.Label("Total cache size: " + (totalCacheSize * 0.0009765625).ToString("F2") + " kB");
			GUILayout.Space(10);
			
			if (cacheDataIsInvalid)
				cacheData = null;
		} else if (toolbarIndex == 2) {
			scrollPos3 = EditorGUILayout.BeginScrollView(scrollPos3, GUILayout.ExpandHeight(true));
			GUILayout.Label("COMPRESSION", EditorStyles.boldLabel);
			
			GUILayout.Space(10);
			
			GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlaying;
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Automatic Compression");
			GUILayout.FlexibleSpace();
			LocalCache.Preferences.AutoCompress = EditorGUILayout.Toggle("",LocalCache.Preferences.AutoCompress, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Compression Algorithm");
			GUILayout.FlexibleSpace();
			LocalCache.Preferences.CompressionAlgorithm = EditorGUILayout.Popup(LocalCache.Preferences.CompressionAlgorithm, LocalCache.Shared.CompressionAlgorithmOptions);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Compression Quality");
			GUILayout.FlexibleSpace();
			if (LocalCache.Preferences.CompressionAlgorithm == 0)
				LocalCache.Preferences.CompressionQualityLZ4 = EditorGUILayout.Popup(LocalCache.Preferences.CompressionQualityLZ4, LocalCache.Shared.CompressionQualityLZ4Options);
			EditorGUILayout.EndHorizontal();
			
			GUILayout.Space(5);
			if (GUILayout.Button("Compress all cached data immediately")) {
				LocalCache.CacheManager.CompressAllCache(false);
				cacheData = null;
				isDirty = true;
			}
			GUILayout.Space(5);
			if (GUILayout.Button("Decompress all cached data immediately")) {
				LocalCache.CacheManager.DecompressAllCache();
				cacheData = null;
				isDirty = true;
			}
			
			GUI.enabled = true;
			
			GUILayout.Space(10);
			
			GUILayout.Label("CLEAN UP", EditorStyles.boldLabel);
			
			GUILayout.Space(10);
			
			GUI.enabled = !LocalCache.CacheManager.BackgroundCacheCompressionInProgress && !LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isPlaying;
			if (GUILayout.Button("Delete all cached data immediately")) {
				if (EditorUtility.DisplayDialog("Delete all cached data?", "All cached data will be destroyed and switching platform will result in cache reinitialization and full asset reimport for that platform.", "Ok, I understand!", "Wait... Cancel!")) {
					LocalCache.CacheManager.ClearAllCache();
					cacheData = null;
					isDirty = true;
				}
			}
			GUI.enabled = true;
			
			if (GUILayout.Button("Unlock user interface")) {
				if (EditorUtility.DisplayDialog("Unlock user interface?", "If the plugin user interface is stuck in either 'Waiting for platform refresh to end' or 'Waiting for background compression to end' and you are sure that the switch/compression operation halted or crashed and is over, you can use this functionnality to force unlocking the user interface.", "Ok, I understand!", "Wait... Cancel!")) {
					LocalCache.CacheManager.PlatformRefreshInProgress = false;
					LocalCache.CacheManager.SwitchOperationInProgress = false;
					LocalCache.CacheManager.PlatformRefreshCurrentScene = "";
					File.Delete(LocalCache.Shared.CachePath + "Background.txt");
				}
			}
			
			if (GUILayout.Button("Clear all cache & Reimport everything")) {
				if (EditorUtility.DisplayDialog("Perform full clean up?", "You'll need to accept to Reimport all assets when prompted. You will also be prompted to save your current scene if you haven't done it. Good luck...", "Ok, I understand!", "Wait... Cancel!")) {
					LocalCache.CacheManager.FixIssues();
					cacheData = null;
					isDirty = true;
				}
			}
			
			GUILayout.Space(10);
			
			GUILayout.Label("VERSION CONTROL", EditorStyles.boldLabel);
			
			GUILayout.Space(10);
			
			if (GUILayout.Button("Add Git ignore rule")) {
				string vcFilePath = LocalCache.Shared.ProjectPath + "/.gitignore";
				if (!File.Exists(vcFilePath)) {
					var stream = File.Create(vcFilePath);
					stream.Dispose();
				}
				string vcFileContent = File.ReadAllText (vcFilePath);
				if (!vcFileContent.Contains("/LocalCache/")) {
					vcFileContent += "\n\n#Fast Platform Switch\n/LocalCache/";
					File.WriteAllText(vcFilePath, vcFileContent);
				}
			}
			GUILayout.Space(5);
			if (GUILayout.Button("Add Mercurial ignore rule")) {
				string vcFilePath = LocalCache.Shared.ProjectPath + "/.hgignore";
				if (!File.Exists(vcFilePath)) {
					var stream = File.Create(vcFilePath);
					stream.Dispose();
				}
				string vcFileContent = File.ReadAllText (vcFilePath);
				if (!vcFileContent.Contains("/LocalCache/")) {
					vcFileContent += "\n\n#Fast Platform Switch\nsyntax: regexp\n/^LocalCache/";
					File.WriteAllText(vcFilePath, vcFileContent);
				}
			}
			GUILayout.Space(5);
			GUI.skin.label.wordWrap = true;
			GUILayout.Label ("For other version control systems, make sure you ignore the \"LocalCache\" directory located in your project folder at the same level as the \"Assets\" directory.", GUILayout.Width(position.width - 25), GUILayout.Height(40));
			GUI.skin.label.wordWrap = false;
			
			GUILayout.Space(10);
			
			if (LocalCache.CacheManager.BackgroundCacheCompressionInProgress) {
				GUILayout.Label("Please wait for background compression to end...");
				GUILayout.Space(10);
			}
			
			if (LocalCache.CacheManager.PlatformRefreshInProgress) {
				GUILayout.Label("Please wait for platform refresh to end...");
				GUILayout.Space(10);
			}
			
			if (EditorApplication.isPlaying) {
				GUILayout.Label("Cannot switch platform while in Play mode...");
				GUILayout.Space(10);
			}
			
			GUILayout.Space(10);
			
			GUILayout.Label("DEBUG", EditorStyles.boldLabel);
			
			GUILayout.Space(10);
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Enable Log File");
			GUILayout.FlexibleSpace();
			LocalCache.Preferences.EnableLogFile = EditorGUILayout.Toggle("",LocalCache.Preferences.EnableLogFile, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.EndScrollView();
		}
		
		GUILayout.Space(10);
		
		if (isDirty) {
			cacheData = null;
			EditorUtility.SetDirty(this);
			Repaint();
		}
	}
	
	void SetupWindow() {
		if (LocalCache.CacheManager.PlatformRefreshInProgress)
			return;

		var script = MonoScript.FromScriptableObject(this);
		var scriptPath = AssetDatabase.GetAssetPath(script);
		var assetsPath = scriptPath.Substring(0, scriptPath.Length - 20);

		// Load textures
		buildTargetTextures = new Texture2D[] {
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_WEBPLAYER.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_STANDALONE.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_IOS.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID.png", typeof(Texture2D)) as Texture2D,
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_METRO.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_WP8.png", typeof(Texture2D)) as Texture2D,
			#endif
			#if !UNITY_3_4 && !UNITY_3_5
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_NACL.png", typeof(Texture2D)) as Texture2D,
			#endif
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_FLASH.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_PS3.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_X360.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_WII.png", typeof(Texture2D)) as Texture2D
		};
		
		androidSubtargetTextures = new Texture2D[] {
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_DXT.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_PVRTC.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ATC.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ETC1.png", typeof(Texture2D)) as Texture2D,
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ETC2.png", typeof(Texture2D)) as Texture2D,
			#endif
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ASTC.png", typeof(Texture2D)) as Texture2D,
			#endif
		};
		
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		blackberrySubtargetTextures = new Texture2D[] {
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10_PVRTC.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10_ATC.png", typeof(Texture2D)) as Texture2D,
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10_ETC1.png", typeof(Texture2D)) as Texture2D
		};
#endif
		
		selectedBuildTargetTexture = AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Selected.png", typeof(Texture2D)) as Texture2D;
		trashBuildTargetTexture = AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Trash.png", typeof(Texture2D)) as Texture2D;
		zipBuildTargetTexture = AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Zip.png", typeof(Texture2D)) as Texture2D;
		unzipBuildTargetTexture = AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Unzip.png", typeof(Texture2D)) as Texture2D;
		
		// Setup current target
		wantedCacheTarget = currentCacheTarget = LocalCache.Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
		if (wantedCacheTarget == LocalCache.Shared.CacheTarget.Android)
			wantedCacheSubtarget = currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		else if (wantedCacheTarget == LocalCache.Shared.CacheTarget.BB10)
			wantedCacheSubtarget = currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
#endif
		else
			wantedCacheSubtarget = currentCacheSubtarget = null;
		
		// Setup rects for list view handling
		listViewRects = new Rect[buildTargetOptions.Length];
		
		// Setup font styles
    	listFontStyle.fontSize = 12;
		statusFontStyle.fontSize = 10;
		statusFontCachedStyle.fontSize = 10;
		statusFontCompressedStyle.fontSize = 10;
		if (EditorGUIUtility.isProSkin) {
			listFontStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
			statusFontStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
			statusFontCachedStyle.normal.textColor = new Color(0.0f, 0.6f, 0.0f, 1.0f);
			statusFontCompressedStyle.normal.textColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);
		} else {
			listFontStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
			statusFontStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
			statusFontCachedStyle.normal.textColor = new Color(0.0f, 0.6f, 0.0f, 1.0f);
			statusFontCompressedStyle.normal.textColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);
		}
		
		// Set up GUI styles
		areaStyle.normal.background = MakeTex(1, 1, new Color(1.0f, 1.0f, 1.0f, 0.025f));
		areaStyleAlt.normal.background = MakeTex(1, 1, new Color(1.0f, 1.0f, 1.0f, 0.05f));
		areaStyleActive.normal.background = MakeTex(1, 1, new Color(0.0f, 0.5f, 1.0f, 0.5f));
		
		// Set up textures
		string[] allTextures = new string[] {
			assetsPath + "Images/Platform_WEBPLAYER.png",
			assetsPath + "Images/Platform_STANDALONE.png",
			assetsPath + "Images/Platform_IOS.png",
			assetsPath + "Images/Platform_ANDROID.png",
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			assetsPath + "Images/Platform_BB10.png",
			assetsPath + "Images/Platform_BB10_PVRTC.png",
			assetsPath + "Images/Platform_BB10_ATC.png",
			assetsPath + "Images/Platform_BB10_ETC1.png",
			assetsPath + "Images/Platform_METRO.png",
			assetsPath + "Images/Platform_WP8.png",
			#endif
			#if !UNITY_3_4 && !UNITY_3_5
			assetsPath + "Images/Platform_NACL.png",
			#endif
			assetsPath + "Images/Platform_FLASH.png",
			assetsPath + "Images/Platform_PS3.png",
			assetsPath + "Images/Platform_X360.png",
			assetsPath + "Images/Platform_WII.png",
			assetsPath + "Images/Platform_ANDROID_DXT.png",
			assetsPath + "Images/Platform_ANDROID_PVRTC.png",
			assetsPath + "Images/Platform_ANDROID_ATC.png",
			assetsPath + "Images/Platform_ANDROID_ETC1.png",
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
			assetsPath + "Images/Platform_ANDROID_ETC2.png",
			#endif
			#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
			assetsPath + "Images/Platform_ANDROID_ASTC.png",
			#endif
			assetsPath + "Images/Platform_Selected.png",
			assetsPath + "Images/Platform_Trash.png",
			assetsPath + "Images/Platform_Zip.png",
			assetsPath + "Images/Platform_Unzip.png"
		};
		
		foreach (var path in allTextures) {
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter == null)
				continue;
			
			bool hasChange = false;
			if (textureImporter.textureType != TextureImporterType.GUI) {
				textureImporter.textureType = TextureImporterType.GUI;
				hasChange = true;
			}
			if (textureImporter.filterMode != FilterMode.Bilinear) {
				textureImporter.filterMode = FilterMode.Bilinear;
				hasChange = true;
			}
			if (textureImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor) {
				textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				hasChange = true;
			}
			
			if (hasChange)
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}
	}
	
	void CacheManagerRefresh() {
		if (this != null) {
			cacheData = null;
			EditorUtility.SetDirty(this);
			this.Repaint();
		}
	}
	
	
	void EditorUpdate () {
		if (LocalCache.CacheManager.ShouldRefreshUI) {
			LocalCache.CacheManager.ShouldRefreshUI = false;
			
			if (this != null) {
				cacheData = null;
				EditorUtility.SetDirty(this);
				this.Repaint();
			}
		}
		
		if (LocalCache.CacheManager.ShouldSwitchPlatform) {
			LocalCache.CacheManager.SwitchPlatformOperation(currentCacheTarget, currentCacheSubtarget, wantedCacheTarget, wantedCacheSubtarget);
		}
		
		if (LocalCache.CacheManager.PlatformRefreshInProgress && !EditorApplication.isCompiling && !LocalCache.CacheManager.SwitchOperationInProgress) {
			ActiveBuildTargetChanged();
		}
	}
	
	void ActiveBuildTargetChanged() {
		wantedCacheTarget = currentCacheTarget = LocalCache.Shared.CacheTargetForBuildTarget(EditorUserBuildSettings.activeBuildTarget);
		if (currentCacheTarget == LocalCache.Shared.CacheTarget.Android)
			wantedCacheSubtarget = currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForAndroidBuildSubtarget(EditorUserBuildSettings.androidBuildSubtarget);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		else if (wantedCacheTarget == LocalCache.Shared.CacheTarget.BB10)
			wantedCacheSubtarget = currentCacheSubtarget = LocalCache.Shared.CacheSubtargetForBlackBerryBuildSubtarget(EditorUserBuildSettings.blackberryBuildSubtarget);
#endif
		else
			wantedCacheSubtarget = currentCacheSubtarget = null;
		
		LocalCache.LogUtility.LogImmediate("Applying trick to refresh subtargets");
		
		// This will trigger refresh for subtargets
		// Unfortunately this code is not functional anymore since Unity 4.3.x
//		Object playerSettings = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
//		SerializedObject ser = new SerializedObject(playerSettings);
//		SerializedProperty prop = ser.FindProperty("AndroidBundleVersionCode");
//		prop.intValue = prop.intValue + 1;
//		ser.ApplyModifiedProperties();
//		prop.intValue = prop.intValue - 1;
//		ser.ApplyModifiedProperties();

		// New method requires building an asset bundle from an empty scene
		// Slower but hopefully will last
		// Only required with subtarget-only switches
		if (LocalCache.CacheManager.PlatformRefreshShouldBustCache == true) {
			var script = MonoScript.FromScriptableObject(this);
			var scriptPath = AssetDatabase.GetAssetPath(script);
			var assetsPath = scriptPath.Substring(0, scriptPath.Length - 20);

			EditorUtility.DisplayProgressBar("Hold on", "Forcing synchronization with empty asset bundle build...", 0.5f);
			BuildPipeline.BuildStreamedSceneAssetBundle(new string[] { assetsPath + "SubtargetCacheBust.unity" }, "JLocalCachePluginCacheBuster.unity3d", EditorUserBuildSettings.activeBuildTarget);
			File.Delete(LocalCache.Shared.ProjectPath + "/JLocalCachePluginCacheBuster.unity3d");
			Jemast.LocalCache.CacheManager.PlatformRefreshShouldBustCache = false;
		}

		LocalCache.LogUtility.LogImmediate("Platform refresh is all done");
		
		LocalCache.CacheManager.PlatformRefreshInProgress = false;
		
		if (this != null) {
			cacheData = null;
			EditorUtility.SetDirty(this);
			this.Repaint();
		}
		
		AssetDatabase.Refresh();
		
		LocalCache.LogUtility.LogImmediate("Reopening previous scene if any");
		
		if (!string.IsNullOrEmpty(LocalCache.CacheManager.PlatformRefreshCurrentScene)) {
			EditorUtility.DisplayProgressBar("Hold on", "Reopening previous scene...", 0.5f);
			EditorApplication.OpenScene(LocalCache.CacheManager.PlatformRefreshCurrentScene);
			LocalCache.CacheManager.PlatformRefreshCurrentScene = "";
		}
		
		EditorUtility.ClearProgressBar();
	}
	
	// Handy method by andeeeee...
	private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width*height];

        for(int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
	
	private static int CacheSubtargetToAndroidCompressionOption(LocalCache.Shared.CacheSubtarget? target) {
		switch (target) {
		case LocalCache.Shared.CacheSubtarget.Android_ETC:
			return 3;
		case LocalCache.Shared.CacheSubtarget.Android_DXT:
			return 0;
		case LocalCache.Shared.CacheSubtarget.Android_PVRTC:
			return 1;
		case LocalCache.Shared.CacheSubtarget.Android_ATC:
			return 2;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		case LocalCache.Shared.CacheSubtarget.Android_ETC2:
			return 4;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
		case LocalCache.Shared.CacheSubtarget.Android_ASTC:
			return 5;
#endif
		default:
			return 3;
		}
	}
	
	private static LocalCache.Shared.CacheSubtarget? AndroidCompressionOptionToCacheSubtarget(int option) {
		switch (option) {
		case 0:
			return LocalCache.Shared.CacheSubtarget.Android_DXT;
		case 1:
			return LocalCache.Shared.CacheSubtarget.Android_PVRTC;
		case 2:
			return LocalCache.Shared.CacheSubtarget.Android_ATC;
		case 3:
			return LocalCache.Shared.CacheSubtarget.Android_ETC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
		case 4:
			return LocalCache.Shared.CacheSubtarget.Android_ETC2;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
		case 5:
			return LocalCache.Shared.CacheSubtarget.Android_ASTC;
#endif
		default:
			return null;
		}
	}
	
	
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
	private static int CacheSubtargetToBlackBerryCompressionOption(LocalCache.Shared.CacheSubtarget? target) {
		switch (target) {
		case LocalCache.Shared.CacheSubtarget.BB10_PVRTC:
			return 0;
		case LocalCache.Shared.CacheSubtarget.BB10_ATC:
			return 1;
		case LocalCache.Shared.CacheSubtarget.BB10_ETC:
			return 2;
		default:
			return 2;
		}
	}
	
	
	private static LocalCache.Shared.CacheSubtarget? BlackBerryCompressionOptionToCacheSubtarget(int option) {
		switch (option) {
		case 0:
			return LocalCache.Shared.CacheSubtarget.BB10_PVRTC;
		case 1:
			return LocalCache.Shared.CacheSubtarget.BB10_ATC;
		case 2:
			return LocalCache.Shared.CacheSubtarget.BB10_ETC;
		default:
			return null;
		}
	}
#endif
	
}
