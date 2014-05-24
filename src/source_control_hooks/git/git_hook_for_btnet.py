#!C:/apps/Python26/python.exe

# Copyright 2009 Corey Trager
# Distributed under the terms of the GNU General Public License

import sys
import subprocess
import re
import urllib
import os


#######################################################################
#
# This script is new as of October, 2009.  Please let me know if it's
# working, not working, good, bad, whatever.  Thanks! 
#     Corey
#     ctrager@yahoo.com.
#
# This script is intended to be invoked by the git "post-commit", 
# "post-merge" hooks, and maybe others?
#
# For me, for example, I put this line in the "post-commit" file:
# c:/cit/btnet/git/git_hook_for_btnet.py
#
# If your commit message starts with an integer, then BugTracker.NET
# will consider that to be a bug id, and will associate the git commit
# with the bug.
#
# The git repository needs to be on the same machine as the web server.
# 
# You can put multiple repositories on the web server.
#
# Put a copy of this file, appropriately configured, in each repository
#
#
#######################################################################


#######################################################################
#
# !!!   Start of stuff you need to change
#
########################################################################

# !! Check the #! "shebang" on the very frist line.  It needs to point 
# to your Python interpreter.


# BugTracker.NET username and password.  The username needs to match
# a setting in Web.config.

btnet_username = "admin"
btnet_password = "admin"

# The path to the git executable
git_path = "\"c:\\program files (x86)\\git\\bin\\git\""


# The URL needs to be reachable from the machine where this script is
# running.  (I have a virtual directory named "btnet").  Don't
# use IIS windows security on git_hook.aspx.
btnet_url = "http://127.0.0.1/btnet/git_hook.aspx"

# Path to repository.  BugTracker.NET web pages will use this.
this_repository_url = "c:\\cit\\mygit"

# 

bDebug = True

#######################################################################
#
# !!!   End of stuff you need to change
#
#######################################################################


#######################################################################
# for debugging
#######################################################################
def debug_out(s):
	if (bDebug):
		f = open((os.environ["TEMP"]) + '\\btnet_git_hook_log.txt', 'a')
		f.write(s)
		f.write('\n')
		f.close	
		print s


#######################################################################
#
# This is where this script keeps track of previously processed
# commits. If you delete this file, this script will send log info
# for ALL commits to BugTracker.NET.  You might want to do that to
# reload the git_revisions and git_affected_paths tables.
#
#######################################################################
prev_commit_file = this_repository_url + "\\.git\\btnet_prev_commit.txt"
debug_out(prev_commit_file)

#######################################################################
# Get the log info from git.
# If we've already fetched info in the past, just fetch the info
# since the last commit
#######################################################################

# get the previous commit
try:
	most_recent_commit_file = open(prev_commit_file,"r")
	prev_commit = most_recent_commit_file.read(40)
	most_recent_commit_file.close() 
except:
	prev_commit = ""

debug_out("prev_commit")
debug_out(prev_commit)

cmd = git_path + " log --name-status --date=iso"

# just since previously processed commit
if prev_commit != "":
	cmd += " " + prev_commit + ".."

debug_out(cmd)

process = subprocess.Popen(cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
debug_out("log stdout:")
log_string = process.stdout.read()
debug_out(log_string)
debug_out("stderr:")
debug_out(process.stderr.read())

# remember that we already processed this commit, for next time
if len(log_string) > 50:
	prev_commit = log_string[7:47]
	debug_out("prev_commit2")
	debug_out(prev_commit)
	most_recent_commit_file = open(prev_commit_file,"w")
	most_recent_commit_file.write(prev_commit)
	most_recent_commit_file.close()


# get the first commit in the log string and save it to the file.

#######################################################################
# send an http request to BugTracker.NET
#######################################################################

params = urllib.urlencode({
	'git_log': log_string,
	'repo': this_repository_url,
	'username': btnet_username,
	'password': btnet_password})

response = urllib.urlopen(btnet_url, params)
data = response.read()
debug_out(data)