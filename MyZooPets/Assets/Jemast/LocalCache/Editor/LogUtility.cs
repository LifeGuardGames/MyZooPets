using System;
using System.IO;

namespace Jemast.LocalCache {
	public class LogUtility {
		private static object logLock = new object();
		
		public static void LogImmediate(string message) {
			if (LocalCache.Preferences.EnableLogFile == false)
				return;
			
			lock (logLock) {
				if (!Directory.Exists(LocalCache.Shared.CachePath))
					Directory.CreateDirectory(LocalCache.Shared.CachePath);
				
				using (StreamWriter w = File.AppendText(LocalCache.Shared.CachePath + "log.txt"))
	        	{
	            	w.WriteLine(string.Format("[{0}] {1}", DateTime.Now, message), w);
				}
			}
		}
		
		public static void LogImmediate(string message, params object[] args) {
			if (LocalCache.Preferences.EnableLogFile == false)
				return;
			
			lock (logLock) {
				if (!Directory.Exists(LocalCache.Shared.CachePath))
					Directory.CreateDirectory(LocalCache.Shared.CachePath);
				
				using (StreamWriter w = File.AppendText(LocalCache.Shared.CachePath + "log.txt"))
	        	{
	            	w.WriteLine(string.Format("[{0}] {1}", DateTime.Now, string.Format(message, args)), w);
				}
			}
		}
	}
}
