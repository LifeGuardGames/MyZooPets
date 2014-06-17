//
//  CompressionManager.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;

namespace Jemast.LocalCache {
	public class CompressionManager {
		public static void PerformCompression(string path, bool silent = false) {
			if (!silent)
				EditorUtility.DisplayProgressBar("Hold on", "Compressing cache...", 0.5f);
			
			// Delete old compressed cache
			File.Delete(path + ".jcf");
			File.Delete(path + ".jcf.lz4");
			
			string compressedFilePath = path + ".jcf.lz4";
		
			Jemast.Utils.JCF.Concatenate(path, path + ".jcf");
			
			System.Diagnostics.Process process;
			
			if (Application.platform == RuntimePlatform.OSXEditor) {
				// Make lz4 process executable
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "chmod";
				process.StartInfo.Arguments = "+x \"" + LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "lz4\"";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				
				// Wait for process to end
				process.WaitForExit();
				process.Dispose();
			}
			
			// Start lz4 process
			process = new System.Diagnostics.Process();
			process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "lz4" + (Application.platform == RuntimePlatform.WindowsEditor ? ".exe" : "");
			process.StartInfo.Arguments = string.Format("-{0} \"{1}\" \"{2}\"", LocalCache.Preferences.CompressionQualityLZ4 == 0 ? "1" : "9", path + ".jcf", compressedFilePath);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = false;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			
			// Wait for process to end
			process.WaitForExit();
			process.Dispose();
			
			// Fix times
			File.SetCreationTime(compressedFilePath, Directory.GetCreationTime(path));
			File.SetLastAccessTime(compressedFilePath, Directory.GetLastAccessTime(path));
			File.SetLastWriteTime(compressedFilePath, Directory.GetLastWriteTime(path));
			
			// Cleanup
			LocalCache.Shared.DeleteDirectory(path);
			File.Delete(path + ".jcf");
			
			if (!silent)
				EditorUtility.ClearProgressBar();
		}
		
		public static void PerformDecompression(string path, bool silent = false) {
			string compressedPath = path + ".jcf.lz4";
			
			if (!File.Exists(compressedPath))
				return;
			
			if (Directory.Exists(path))
				LocalCache.Shared.DeleteDirectory(path);
			
			if (!silent)
				EditorUtility.DisplayProgressBar("Hold on", "Decompressing cache...", 0.5f);
			
			System.Diagnostics.Process process;
			
			if (Application.platform == RuntimePlatform.OSXEditor) {
				// Make lz4 process executable
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "chmod";
				process.StartInfo.Arguments = "+x \"" + LocalCache.Shared.ProjectPath +  LocalCache.Shared.UtilsPaths + "lz4\"";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				
				// Wait for process to end
				process.WaitForExit();
				process.Dispose();
			}
			
			// Start lz4 process
			process = new System.Diagnostics.Process();
			process.StartInfo.FileName = LocalCache.Shared.ProjectPath + LocalCache.Shared.UtilsPaths + "lz4" + (Application.platform == RuntimePlatform.WindowsEditor ? ".exe" : "");
			process.StartInfo.Arguments = string.Format("-d \"{0}\" \"{1}\"", compressedPath, path + ".jcf");
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = false;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			
			// Wait for process to end
			process.WaitForExit();
			process.Dispose();
			
			// Unconcatenate
			Jemast.Utils.JCF.Unconcatenate(path + ".jcf", path);
			
			// Fix times
			Directory.SetCreationTime(path, File.GetCreationTime(path));
			Directory.SetLastAccessTime(path, File.GetLastAccessTime(path));
			Directory.SetLastWriteTime(path, File.GetLastWriteTime(path));
			
			// Cleanup
			File.Delete(path + ".jcf");
			File.Delete(path + ".jcf.lz4");
			
			if (!silent)
				EditorUtility.ClearProgressBar();
		}
	}
}