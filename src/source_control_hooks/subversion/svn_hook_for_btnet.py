# Copyright 2009 Corey Trager
# Distributed under the terms of the GNU General Public License

import sys
import subprocess
import re
import urllib
import os

#######################################################################
#
# See svn documentation for how to install a hook script.
#
#######################################################################


#######################################################################
#
# !!!   Start of stuff you need to change
#
########################################################################

# BugTracker.NET username and password.  The username needs to match
# a setting in Web.config.
btnet_username = "admin"
btnet_password = "admin"

# The path to the svn executable
#svn_path =  "\"C:\\Program Files (x86)\\Subversion\\bin\\svn\""
svn_path = "C:\\apps\\svn-win32-1.6.17\\bin\svn"

# The URL needs to be reachable from the machine where this script is
# running.  (I have a virtual directory named "btnet").  Don't
# use IIS windows security on svn_hook.aspx.
btnet_url = "http://127.0.0.1/btnet/svn_hook.aspx"

# The repository URL. BugTracker.NET web pages will use this URL to 
# interact with this repository.  If you need a username and password
# to access this repo, you configure that in Web.config.  See
# Web.config for more info.
this_repository_url = "svn://127.0.0.1/mysvn_repo"

bDebug = True

#######################################################################
#
# !!!   End of stuff you need to change
#
#######################################################################


#######################################################################
# for debugging, display info and keep a log
#######################################################################
def debug_out(s):
	if (bDebug):
		f = open((os.environ["TEMP"]) + '\\btnet_svn_hook_log.txt', 'a')
		f.write(s)
		f.write('\n')
		f.close	
		print s

repo = sys.argv[1]
rev = sys.argv[2]
debug_out(repo)
debug_out(rev)

#######################################################################
#
# This is where this script keeps track of previously processed
# revisions. If you delete this file, this script will send log info
# for ALL revisions to BugTracker.NET.  You might want to do that to
# reload the svn_revisions and svn_affected_paths tables.
#
#######################################################################

prev_revision_file = repo + "\\hooks\\btnet_prev_revision.txt"
debug_out(prev_revision_file)


#######################################################################
# Get the log info from svn
# If we've already fetched info in the past, just fetch the info
# since the last revision
#######################################################################

# get the previous revision
try:
	most_recent_revision_file = open(prev_revision_file,"r")
	prev_revision = most_recent_revision_file.read(40)
	most_recent_revision_file.close() 
except:
	prev_revision = ""

debug_out("prev_revision")
debug_out(prev_revision)

# just since previously processed revision

file_url = " file:///" + repo.replace("\\","/").replace(" ","%20")
subcommand = " log --verbose --xml "

if prev_revision != "":
	cmd = svn_path + subcommand + " -r " + prev_revision + ":" + rev + file_url
else:
	cmd = svn_path + subcommand + file_url

debug_out(cmd)

process = subprocess.Popen(cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
debug_out("log stdout:")
log_string = process.stdout.read()
debug_out(log_string)
debug_out("stderr:")
debug_out(process.stderr.read())

# remember that we already processed this revision
most_recent_revision_file = open(prev_revision_file,"w")
most_recent_revision_file.write(rev)
most_recent_revision_file.close()


# get the first revision in the log string and save it to the file.

#######################################################################
# send an http request to BugTracker.NET, the svn_hook.aspx page.
#######################################################################

params = urllib.urlencode({
	'svn_log': log_string,
	'repo': this_repository_url,
	'username': btnet_username,
	'password': btnet_password})

response = urllib.urlopen(btnet_url, params)
data = response.read()
debug_out(data)