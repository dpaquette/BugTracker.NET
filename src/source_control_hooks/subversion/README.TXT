*******************************************************************************
*
*  Files for integrating with subversion.
*
*******************************************************************************


1) svn_hook_for_btnet.py - A Subversion pre or post commit hook script 
written in Python.  You need Python 2.5 or 2.6 to run it.  Open the file with
a text editor (notepad) and follow the instructions in it.

Then you'll need to change a subversion hook to execute it.  For example, 
I have a post-commit.bat file in my svn repository's hooks folder that
has this line in it:

c:\btnet\subversion\svn_hook_for_btnet.py %1 %2

Here is the svn documentation about hook scripts:
http://svnbook.red-bean.com/en/1.0/svn-book.html#svn-ch-5-sect-2.1



2) TortoiseSVN_integration.bat -  A bat file to set up integration between 
TortoiseSVN and bug trackers like BugTracker.NET. 

Here is the TortoiseSVN documentation about bugtracker integration:
http://tortoisesvn.net/issuetracker_integration.


For more on BugTracker.NET's integration with subversion, see:
http://www.ifdefined.com/doc_bug_tracker_subversion.html

LINUX, OS Users:

If you are running your subversion server on Linux (thanks to Shaun Verrall for this info):

Your post_commit file should look something like this:

#! /bin/sh
#
/your/path/to/svn_hook_for_btnet.py "$1" "$2"

Change permission on the python script:
chmod +x svn_hook_for_btnet.py 

In svn_hook_for_btnet.py itself, change the paths, of course, but note also that in the line below the "TEMP" environment variable won't necessarily be defined, so hardcode your \tmp\.

f = open((os.environ["TEMP"]) + '\\btnet_svn_hook_log.txt', 'a')

If you are running your subversion server on OSX, this is a sample script from Philipp Brendel

OSX_svn_hook_for_btnet.
