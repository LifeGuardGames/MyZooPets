﻿using System;

namespace Area730.Notifications
{
    public class NotificationBuilder
    {
        /// <summary>
        /// Use all default values (where applicable).
        /// </summary>
        public const int DEFAULT_ALL       = -1;

        /// <summary>
        /// Use the default notification sound. This will ignore any given sound.
        /// </summary>
        public const int DEFAULT_SOUND     = 1;

        /// <summary>
        /// Use the default notification vibrate. This will ignore any given vibrate. Using phone vibration requires the VIBRATE permission.
        /// </summary>
        public const int DEFAULT_VIBRATE   = 2;


        /// <summary>
        /// Name of the default icon included with plugin
        /// </summary>
        public const string DEFAULT_ICON = "default_icon";



        private int     _id                 = 1;
        private string  _smallIcon          = DEFAULT_ICON;
        /// <summary>
        /// Default icon name. Comes with package.
        /// </summary>
        private string  _largeIcon          = "";
        private int     _defaults           = 0;
        private bool    _autoCancel         = false;
        private string  _sound              = "";
        private string  _ticker             = "Notification ticker";
        private string  _title              = "Notification title";
        private string  _body               = "Notification body";
        private long[]  _vibratePattern;
        private long    _when               = 0;
        private long    _delay              = 0;
        private bool    _isRepeating        = false;
        private long    _interval;
        private int     _number;
        private bool    _alertOnce          = false;
        private string  _color              = "";


        /// <summary>
        /// Coustructor. Takes 3 arguments that are required to make basic notification
        /// </summary>
        /// <param name="id">ID of the notification</param>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Notification message</param>
        public NotificationBuilder(int id, string title, string body)
        {
            _id     = id;
            _title  = title;
            _body   = body;
        }

        /// <summary>
        /// Set title of the notification
        /// </summary>
        /// <param name="title">Notification title</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setTitle(string title)
        {
            _title = title;
            return this;
        }

        /// <summary>
        /// Set body of the notification
        /// </summary>
        /// <param name="body">Notification body</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setBody(string body)
        {
            _body = body;
            return this;
        }

        /// <summary>
        /// Set the color of notification small icon background. The color must be in format #RRGGBB or #AARRGGBB
        /// </summary>
        /// <param name="color">Color you want to set in correct format</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setColor(string color)
        {
            _color = color;
            return this;
        }

        /// <summary>
        /// Set the text that is displayed in the status bar when the notification first arrives.
        /// </summary>
        /// <param name="ticker">Notification ticker</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setTicker(string ticker)
        {
            _ticker = ticker;
            return this;
        }

        /// <summary>
        /// Set whether it is a repeating notification. 
        /// If this is set to true - interval of repetition must be also specified
        /// <seealso cref="NotificationBuilder.setInterval(long)"/>
        /// <seealso cref="NotificationBuilder.setInterval(TimeSpan)"/>
        /// </summary>
        /// <param name="state">True if repeating, false if one-time</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setRepeating(bool state)
        {
            _isRepeating = state;
            return this;
        }

        /// <summary>
        /// Sets the interval of repetition of the notification in milliseconds
        /// </summary>
        /// <param name="interval">Interval of repetition in milliseconds</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setInterval(long interval)
        {
            _interval = interval;
            return this;
        }

        /// <summary>
        /// Sets the interval of repetition of the notification with TimeSpan
        /// <seealso cref="System.TimeSpan"/>
        /// </summary>
        /// <param name="intervalTimeSpan">Interval of repetition of type TimeSpan</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setInterval(TimeSpan intervalTimeSpan)
        {
            _interval = (long)intervalTimeSpan.TotalMilliseconds;
            return this;
        }

        /// <summary>
        /// Set this flag if you would only like the sound, vibrate and ticker to be played if the notification is not already showing.
        /// </summary>
        /// <param name="state">State</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setAlertOnlyOnce(bool state)
        {
            _alertOnce = state;
            return this;
        }

        /// <summary>
        /// Set the large number at the right-hand side of the notification.
        /// </summary>
        /// <param name="num">Number to set</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setNumber(int num)
        {
            _number = num;
            return this;
        }



        /// <summary>
        /// Set the small icon resource name. 
        /// <para>The icon must be in Plugins/Android/Notifications/res/drawable folder (or one of those)</para>
        /// <para>This icon is used as MASK</para>
        /// </summary>
        /// <param name="iconResourceName">Name of the icon resource</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setSmallIcon(string iconResourceName)
        {
            _smallIcon = iconResourceName;
            return this;
        }

        /// <summary>
        /// Set the large icon that is shown in the ticker and notification.
        /// <para>The icon must be in Plugins/Android/Notifications/res/drawable folder (or one of those)</para>
        /// </summary>
        /// <param name="iconResourceName">Name of the icon resource</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setLargeIcon(string iconResourceName)
        {
            _largeIcon = iconResourceName;
            return this;
        }

        /// <summary>
        /// Set the default notification options that will be used.
        /// <para>Can be NotificationBuilder.DEFAULT_SOUND, NotificationBuilder.DEFAULT_VIBRATE, NotificationBuilder.DEFAULT_ALL</para>
        /// </summary>
        /// <param name="defaultFlags">Flags to set</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setDefaults(int defaultFlags)
        {
            _defaults = defaultFlags;
            return this;
        }

        /// <summary>
        /// Setting this flag will make it so the notification is automatically canceled when the user clicks it in the panel
        /// </summary>
        /// <param name="state">Autocancel state</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setAutoCancel(bool state)
        {
            _autoCancel = state;
            return this;
        }

        /// <summary>
        /// Set id of the notification
        /// </summary>
        /// <param name="id">Notification id</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setId(int id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Set the vibration pattern to use.
        /// <para>Details on the pattern can be found here http://developer.android.com/reference/android/os/Vibrator.html </para>
        /// </summary>
        /// <param name="pattern">Vibration pattern</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setVibrate(long [] pattern)
        {
            _vibratePattern = pattern;
            return this;
        }

        /// <summary>
        /// Time in milliseconds that the notification should go off
        /// </summary>
        /// <param name="delayMilliseconds"></param>
        /// <returns></returns>
        public NotificationBuilder setDelay(long delayMilliseconds)
        {
            _delay = delayMilliseconds;
            return this;
        }

        /// <summary>
        /// Time in TimeSpan that the notification should go off
        /// <seealso cref="System.TimeSpan"/>
        /// </summary>
        /// <param name="delayTimeSpan">Time interval</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setDelay(TimeSpan delayTimeSpan)
        {
            _delay = (long)delayTimeSpan.TotalMilliseconds;
            return this;
        }

        /// <summary>
        /// Set the sound resource name to play
        /// <para>The sound must be in Assets/Plugins/Android/Notifications/res/raw folder</para>
        /// </summary>
        /// <param name="soundResourceName">The sound resource name</param>
        /// <returns>Reference to builder</returns>
        public NotificationBuilder setSound(string soundResourceName)
        {
            _sound = soundResourceName;
            return this;
        }

        /// <summary>
        /// Converts DateTime to milliseconds
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <returns>Time in milliseconds</returns>
        private long ConvertToMillis(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000;
            return epoch;
        }

        /// <summary>
        /// Builds the norification from specified data
        /// </summary>
        /// <returns>Built notification</returns>
        public Notification build()
        {
            Notification n = new Notification(
                id:             _id,
                smallIcon:      _smallIcon,
                largeIcon:      _largeIcon,
                defaults:       _defaults,
                autoCancel:     _autoCancel,
                ticker:         _ticker,
                title:          _title,
                body:           _body,
                sound:          _sound,
                vibroPattern:   _vibratePattern, 
                when:           _when,
                delay:          _delay,
                interval:       _interval,
                repeating:      _isRepeating,
                number:         _number,
                alertOnce:      _alertOnce,
                color:          _color
                );


            return n;
        }

    }
}