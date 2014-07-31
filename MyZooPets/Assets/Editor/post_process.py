import os
from sys import argv
from mod_pbxproj import XcodeProject

path = argv[1]

# print('----------------------------------prepare for excuting our magic scripts to tweak our xcode ----------------------------------')

# print('Step 1: start add libraries ')
project = XcodeProject.Load(path +'/Unity-iPhone.xcodeproj/project.pbxproj')

#project.add_file('System/Library/Frameworks/Accounts.framework', tree='SDKROOT', weak=True)
#project.add_file('System/Library/Frameworks/CoreTelephony.framework', tree='SDKROOT')
#project.add_file('System/Library/Frameworks/GameKit.framework', tree='SDKROOT', weak=True)
#project.add_file('System/Library/Frameworks/MessageUI.framework', tree='SDKROOT')
#project.add_file('System/Library/Frameworks/MobileCoreServices.framework', tree='SDKROOT')
# project.add_file('System/Library/Frameworks/StoreKit.framework', tree='SDKROOT')
# project.add_file('System/Library/Frameworks/Social.framework', tree='SDKROOT', weak=True)
# project.add_file('usr/lib/libsqlite3.dylib', tree='SDKROOT')

# print('Step 2: add files to xcode, exclude any .meta file')
# files_in_dir = os.listdir(fileToAddPath)
# for f in files_in_dir:    
#     if not f.startswith('.'): #ignore .DS_STORE
#         print f        
#         pathname = os.path.join(fileToAddPath, f)
#         fileName, fileExtension = os.path.splitext(pathname)
#         if not fileExtension == '.meta': #ignore .meta as it is under asset server
#             if os.path.isfile(pathname):
#                 print "is file : " + pathname
#                 project.add_file(pathname)
#             if os.path.isdir(pathname):
#                 print "is dir : " + pathname
#                 project.add_folder(pathname, excludes=["^.*\.meta$"])

if project.modified:
  project.backup()
  project.saveFormat3_2()

# print('----------------------------------end for excuting our magic scripts to tweak our xcode ----------------------------------')