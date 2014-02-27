------------------------------------------
Fast Platform Switch
v1.3.1
Codename: JLocalCachePlugin
Product Page: http://jemast.com/unity/fast-platform-switch/
Copyright (c) 2013-2014 jemast software.
------------------------------------------


------------------------------------------
Quick Start
------------------------------------------

After importing Fast Platform Switch from the Asset Store, open the Window menu and click Fast Platform Switch.

You must now exclusively switch platforms using the Fast Platform Switch interface and not rely on the Build Settings platform list.

Pick a platform and hit the Switch Platform button. That's it. The plugin will do the rest.

------------------------------------------
About Fast Platform Switch
------------------------------------------

Fast Platform Switch is not meant to replace Unity's Cache Server. It's meant as a personal caching utility for individuals and small teams. If you're already using Unity's Cache Server, you should not use Fast Platform Switch as this means you'll cache your data twice and will probably loose time and disk space.

Because Fast Platform Switch caches data for each platform at switch time, this means it takes up valuable disk space which can become quite large depending on your project size. We've included a Status panel to check the status of your cache and the size it takes up.

We've also included a compression mechanism to save space. While this mechanism will slow down a bit the switch time due to decompression time, the savings in terms of disk space are generally around 50% which is also significant. Compression happens in the background after you perform a platform switch. While this allows you to continue working as soon as the switch happened, it may incur slowdowns as it eats up CPU resources.

Finally, because it creates and operates the cache folder in your project directory, we've also included convenient methods to ignore this folder for popular version control mechanisms (Git and Mercurial). We've also added instructions for other version control mechanisms.


------------------------------------------
Help & Support
------------------------------------------

Check out our documentation at http://jemast.com/unity/fast-platform-switch/documentation/

If you need any additional help, please review our product page at http://jemast.com/unity/fast-platform-switch/

Feel free to contact us at contact@jemast.com for personal support


-------------------------------------------
Release Notes
-------------------------------------------

-------------------
v1.3.1 (01/19/2014)
-------------------

- FIX: Fixed texture refresh not being triggered when changing texture compression in Android or BB10 on Unity 4.3+
- FIX: Textures from Fast Platform Switch window do not get reimported after each platform switch
- FIX: Paths to required assets and tools are now relative and you should be able to move the Jemast folder around

-------------------
v1.3 (01/13/2014)
-------------------

- NEW: API to integrate Fast Platform Switch into your custom pipeline

-------------------
v1.2.5 (11/13/2013)
-------------------

- NEW: Full Unity 4.3 support
- NEW: Added support for Unity 4.3+ hidden meta files
- NEW: Added support for ASTC texture compression for Android (Unity 4.3+)
- FIX: Fixed incorrect automatic Mercurial rule for version control
- FIX: Attempted to fix UI lockup where switch is not happening

-------------------
v1.2 (11/04/2013)
-------------------

- NEW: Support for Blackberry texture compression options (ETC1, ATC, PVRTC)
- NEW: Verbose mode to help debugging issues (log file)
- NEW: Option to unlock the plugin UI should switch or compression operation go wrong and UI is stuck
- FIX: New method for forcing texture refresh on subtarget change (texture compression for Android & BB10)
- FIX: Fixed plugin remaining locked if there are script compilation errors after switching platform
- FIX: Timestamps now get correctly deleted when deleting a specific platform cache
- FIX: Fixed some minor UI quirks

-------------------
v1.1 (10/13/2013)
-------------------

- CRITICAL FIX: You are now prompted to enable external version control in your project as this is required
- FIX: Asset importers are correctly saved to disk before switching
- FIX: Asset timestamps are now written to cache in order to fix issues
- FIX: Fixed an innocuous error in Unity 3 branch after switch
- As a result of those fixes, cache is deleted once upon update
- Please restart Unity after applying this update

-------------------
v1.0.2 (10/12/2013)
-------------------

- FIX: Cancel now working when asking to save scene before switching
- UI: Clearer dialog box for saving scene before switching

-------------------
v1.0.1 (10/09/2013)
-------------------

- FIX: New platform is now correctly selected for build after switch
- FIX: Removed Native Client as an independent platform for Unity 3 branch
- Please restart Unity after applying this update

-------------------
v1.0 (09/15/2013)
-------------------

- Initial release.



jemast software



-------------------------------------------
Third-party acknowledgments
-------------------------------------------

lz4 Command line utility
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Fast compression algorithm
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Copyright (c) 2013 Yann Collet.

-------------------------------------------

Fast Platform Switch logo uses various icons from The Noun Project

Computer by The Noun Project (Designed by Edward Boatman)
http://thenounproject.com/noun/computer/#icon-No115

iPhone by The Noun Project (Designed by Edward Boatman)
http://thenounproject.com/noun/iphone/#icon-No414