Our first big changes we make to the BugTracker.NET code base will be to convert the existing Web Site project to a Web Application Project. Web application projects have a number of advantages, including better management of references and a support for a more structured separation of code. Also, moving to a Web Application Project will give us the option to use MVC, WebAPI and other newer ASP.NET technologies.

For the most part, we will be following the walkthrough outlined [on MSDN](http://msdn.microsoft.com/en-us/library/aa983476(v=vs.100).aspx) Each section below has a corresponding commit to the MoveToWebApplicationProject branch so you can see the exact changes that were made.

#Creating a Solution
The current web site project is not contained in a Visual Studio solution. It is simply a folder that contains the entire web site. Our first step will be to create an empty ASP.NET Web Application by selecting File -> New -> Project in Visual Studio. Select the ASP.NET Web Application project template, name the project BugTracker.Web and click OK. In the New ASP.NET Project dialog, select the Empty template. 

#Setting Up References
BugTracker.NET relies on a few third party libraries.: Lucene.NET, log4net and SharpMimeTools. In a Web Site project, these references are managed by simply adding the required assemblies to the web site's bin folder. Ultimately, we would like to manage these references using NuGet, but BugTracker references older versions of these assemblies that are not available on NuGet. We will handle the library upgrades and transition to NuGet in a future post. 

For now, we will need to copy these assemblies to a separate folder and add references to these assemblies in the new Web Application Project.

#Move the Files to the Web Application
The next step is to move all the files from the Web site folder to the new Web application project. Open Windows Explorer and browse to the folder containing the Web site project. In our case, we browse to src\www. Select all the files in this folder with the exception of the \bin folder. Copy the files and paste them into the src\BugTracker.NET folder. Overwrite any files that already exist in the BugTracker.NET folder. Delete the original Web site project folder (src\www).

Back in Visual Studio, select the BugTracker.NET project and select Show All Files. Select all the files in all folders with the exception of the App_Data, bin and obj folders. 

Build the project and ensure that there are no compile errors. You probably won't see any errors at this point, because files in the App_Code folder are dynamically compile at runtime. We can force the C# files in the App_Code folder to compile by renaming the folder to btnet, then selecting all the files in this folder and changing the Build Action from Content to Compile. Now you might receive some build errors. In Bug Tracker, we receive the following build error:

    The type or namespace name 'DirectoryServices' does not exist in the namespace 'System' (are you missing an assembly reference?)	~\BugTracker.NET\src\BugTracker.Web\App_Code\authenticate.cs	9	14	BugTracker.Web

We are seeing this error is because the project is missing a reference to the System.DirectoryServices.Protocols assembly. After adding the required references, the project seems to compiles cleanly.

Now that we have verified the class files compile cleanly, we should also confirm that all the .aspx pages and .ascx controles compile cleanly. These files contain a lot of code that by default will only be dynamically compiled at runtime when the pages and controls are accessed in the application. You can confirm confirm these pages compile by running the application and accessing all the pages individually, or you can precompile everything using aspnet_compiler.exe as follows:
    
    c:\Windows\Microsoft.NET\Framework\v4.0.30319>aspnet_compiler.exe -p c:\users\david\documents\github\BugTracker.NET\src\BugTracker.Web -v / c:\staging

You might find more missing references at this point, but in our case there are no compile errors. We are safe to commit changes and move to the next step.


#Generating code-behind files
One of the big advantages of a Web application project is that we can have a cleaner separation between the page or user control (aspx / ascx) and the C# code that is associated with that page or user control. In the case of BugTracker, there all the C# code is contained within the aspx and ascx files in a script tag. The first step will be to move all this code into an associated .cs file.

--outline steps for doing the code extract

Select the BugTracker.NET project, then select Convert to Web Application from Project menu.

#Modifying namespaces
Now that all the code has been extracted to .cs files, we can start to restructure the codebase a little. Since the code is no longer in a script tag, our refactoring tools will work a little better. 
We will start by adding namespaces to the classes in the btnet folder. Currently, none of these classes have any namespaces... 