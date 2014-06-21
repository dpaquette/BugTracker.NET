REM
REM Change the http://127.0.0.1/btnet part to be your host, your virtual dir, etc.
REM cd to the root of your check out and run this.
REM
REM See the follwoing for an explanation of this .bat file
REM http://tortoisesvn.net/issuetracker_integration
REM http://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-dug-bugtracker.html
REM
REM If you have only installed TortoiseSVN but not the svn command line, this script won't work.
REM
svn propset -R bugtraq:label "BugTracker.NET ID:" .
svn propset -R bugtraq:url "http://127.0.0.1/btnet/edit_bug.aspx?id=%%BUGID%%" .
svn propset -R bugtraq:message "bugid: %%BUGID%%" .
svn propset -R bugtraq:number "true" .
svn propset -R bugtraq:warnifnoissue "true" .
svn commit -q -m "Added BugTracker.NET properties to the repository"
