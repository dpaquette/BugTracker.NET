@echo off
echo You must run this as Administrator.
echo You'll need to know a SQL Server username and password that works with SQL Server security (not a Windows user)
echo The script assumes a SQL Server instance of local\SQLEXPRESS that works with both Windows and SQL Server security.
echo
set BTNET=
set SQLUSER=
set SQLPSWD=
set /p BTNET=Enter name of btnet instance.  Hit enter for default "btnet":
set /p SQLUSER=Enter SQL Server username.  Hit enter for default "sa":
set /p SQLPSWD=Enter SQL Server password:
if "%BTNET%"=="" set BTNET=btnet
if "%SQLUSER%"=="" set SQLUSER=sa
@echo on

REM create the iis7 application
C:\Windows\System32\inetsrv\appcmd.exe add app /site.name:"Default Web Site" /path:/%BTNET% /physicalPath:"%CD%\www"
REM create the database
sqlcmd -S localhost\SQLEXPRESS -Q "create database %BTNET%"
REM create the tables
sqlcmd -S localhost\SQLEXPRESS -d %BTNET% -i "%CD%\www\setup.sql"
REM modify Web.config
powershell "$hostname = hostname; Get-Content %CD%\www\Web.config | ForEach-Object {$_ -replace '127.0.0.1', $hostname } | ForEach-Object {$_ -replace 'database=btnet', 'database=%BTNET%' } | ForEach-Object {$_ -replace 'user id=sa;password=x', 'user id=%SQLUSER%;password=%SQLPSWD%' }  | Set-Content %CD%\www\Web.config2"
del %CD%\www\Web.config
rename %CD%\www\Web.config2 Web.config
REM if all goes well, show the web page.
start http://localhost/%BTNET%