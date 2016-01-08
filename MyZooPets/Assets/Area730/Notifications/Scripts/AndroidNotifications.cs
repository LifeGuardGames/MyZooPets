using UnityEngine;

namespace Area730.Notifications
{
    /// <summary>
    /// Class that handles requests to android java lib
    /// </summary>
    public class AndroidNotifications
    {

        /// <summary>
        /// Instance of java notification handler class
        /// </summary>
#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass notifHandlerClass;
#endif

        /// <summary>
        /// Default constructor
        /// </summary>
        static AndroidNotifications()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // Find the java class that handles notifications
            notifHandlerClass = new AndroidJavaClass("com.area730.localnotif.NotificationHandler");
            if (notifHandlerClass == null)
            {
                Debug.LogError("Class com.area730.localnotif.NotificationHandler not found");
                return;
            }

            Debug.Log("Android notifications plugin loaded. Version: " + getVersion());
#else
            Debug.LogWarning("Android notifications can work only on android devices");
#endif
        }

        /// <summary>
        /// Get the version of the plugin
        /// </summary>
        /// <returns>Vertion of the plugin</returns>
        public static float getVersion()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            float version = notifHandlerClass.CallStatic<float>("getVersion");
            return version;
#else
            return -1;            
#endif
        }

        /// <summary>
        /// Schedule the notification
        /// </summary>
        /// <param name="notif">Notification to be scheduled</param>
        public static void scheduleNotification(Notification notif)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            notifHandlerClass.CallStatic("scheduleNotification",
                    notif.Delay,
                    notif.ID,
                    notif.Title,
                    notif.Body,
                    notif.Ticker,
                    notif.SmallIcon,
                    notif.LargeIcon,
                    notif.Defaults,
                    notif.AutoCancel,
                    notif.Sound,
                    notif.VibratePattern,
                    notif.When,
                    notif.IsRepeating,
                    notif.Interval,
                    notif.Number,
                    notif.AlertOnce,
                    notif.Color);
#endif

        }

       
        /// <summary>
        /// Cancels the notification. Both repeating and non-repeating.
        /// </summary>
        /// <param name="notif">Notification to be cancelled</param>
        public static void cancelNotification(Notification notif)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            cancelNotification(notif.ID);
#endif
        }

        /// <summary>
        /// Cancels the notification. Both repeating and non-repeating.
        /// </summary>
        /// <param name="id">Id of the notification to be scheduled</param>
        public static void cancelNotification(int id)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            notifHandlerClass.CallStatic("cancelNotifications", id);
#endif
        }

        /// <summary>
        /// Clear shown notification with specified if
        /// </summary>
        /// <param name="id">Id of the notification to be cleated</param>
        public static void clear(int id)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            notifHandlerClass.CallStatic("clear", id);
#endif
        }

        /// <summary>
        /// Cleared all shown notifications
        /// </summary>
        public static void clearAll()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            notifHandlerClass.CallStatic("clearAll");
#endif
        }

        /// <summary>
        /// Shows native android toast notification
        /// </summary>
        /// <param name="text">Text of the toast</param>
        public static void showToast(string text)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            notifHandlerClass.CallStatic("showToast", text);
#endif
        }
        

    }
}
