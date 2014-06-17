//
//  JCF.cs
//  Jemast Concatenation Format
//
//  Copyright (c) 2013 jemast software.
//


using System;
using System.IO;
using UnityEngine;

namespace Jemast.Utils
{
    public sealed class JCF
    {
		public static void Concatenate(string source, string destination) {
			if (File.Exists(destination))
				File.Delete(destination);
			
			FileStream destFileStream = File.Create(destination);
			BinaryWriter writer = new BinaryWriter(destFileStream);
			
			int sourceLength = source.Length;
			
			// Write serialization version
			writer.Write(0);
			
			string[] directories = Directory.GetDirectories(source, "*.*", SearchOption.AllDirectories);
			
			writer.Write(directories.Length);
			for (int i = directories.Length - 1; i >= 0; i--) {
				string directory = directories[i].Remove(0, sourceLength);
				writer.Write(directory);
			}
			
			string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
			
			writer.Write(files.Length);
			
			int count;
			byte[] buffer = new byte[4096];
			for (int i = files.Length - 1; i >= 0; i--) {
				FileInfo fileInfo = new FileInfo(files[i]);
				
				string file = files[i].Remove(0, sourceLength);
				writer.Write(file);
				writer.Write((int)fileInfo.Length);
				writer.Write(fileInfo.CreationTimeUtc.Ticks);
				writer.Write(fileInfo.LastAccessTimeUtc.Ticks);
				writer.Write(fileInfo.LastWriteTimeUtc.Ticks);
				
				using (Stream srcFileStream = File.OpenRead(files[i]))
			    {
					while( (count = srcFileStream.Read(buffer, 0, buffer.Length)) > 0)
					    writer.Write(buffer, 0, count);
			    }
			}
			
			writer.Close();
		}
		
		public static void Unconcatenate(string source, string destination) {
			BinaryReader reader = new BinaryReader(File.OpenRead(source));
			
			// Read serialization version
			reader.ReadInt32();
			
			int directoryCount = reader.ReadInt32();
			for (int i = directoryCount - 1; i >= 0; i--) {
				Directory.CreateDirectory(destination + reader.ReadString());
			}
			
			int fileCount = reader.ReadInt32();
			
			int count;
			byte[] buffer = new byte[4096];
			for (int i = fileCount - 1; i >= 0; i--) {
				string fileName = destination + reader.ReadString();
				int fileLength = reader.ReadInt32();
				long fileCreationTime = reader.ReadInt64();
				long fileAccessTime = reader.ReadInt64();
				long fileWriteTime = reader.ReadInt64();
				
				using (FileStream fileStream = File.Create(fileName)) {
					while (fileLength > 0) {
						if ((count = reader.Read(buffer, 0, Math.Min(fileLength, buffer.Length))) <= 0)
							throw new Exception();
						
						fileLength -= count;
						fileStream.Write(buffer, 0, count);
					}
				}
				
				FileInfo fileInfo = new FileInfo(fileName);
				fileInfo.CreationTimeUtc = new DateTime(fileCreationTime);
				fileInfo.LastAccessTimeUtc = new DateTime(fileAccessTime);
				fileInfo.LastWriteTimeUtc = new DateTime(fileWriteTime);
				fileInfo.Refresh();
			}
			
			reader.Close();
			
			// Chmod/Chown
			if (Application.platform == RuntimePlatform.OSXEditor) {
				System.Diagnostics.Process process;
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "find";
				process.StartInfo.Arguments = "\"" + destination + "\" -type d -exec chmod 755 {} +";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				
				process.WaitForExit();
				
				process = new System.Diagnostics.Process();
				process.StartInfo.FileName = "find";
				process.StartInfo.Arguments = "\"" + destination + "\" -type f -exec chmod 644 {} +";
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
				process.StartInfo.Arguments = "-RH \""+whoami+":staff\" \"" + destination + "\"";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				
				process.WaitForExit();
			}
		}
	}
}