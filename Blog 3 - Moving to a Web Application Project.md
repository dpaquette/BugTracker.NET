Our first big changes to the BugTracker.NET code base will be to convert the existing Web Site project to a Web Application Project. Web application projects have a number of advantages, including better management of references and a support for a more structured separation of code. Also, moving to a Web Application Project will give us the option to use MVC, WebAPI and other newer ASP.NET technologies.

For the most part, we will be following the walkthrough outlined [on MSDN](http://msdn.microsoft.com/en-us/library/aa983476(v=vs.100).aspx) Each section below has a corresponding commit to the [MoveToWebApplication branch](https://github.com/dpaquette/BugTracker.NET/commits/ConvertToWebApplication) so you can see the exact changes that were made.

#Creating a Solution
The current web site project is not contained in a Visual Studio solution. It is simply a folder that contains the entire web site. Our first step will be to create an empty ASP.NET Web Application by selecting File -> New -> Project in Visual Studio. Select the ASP.NET Web Application project template, name the project BugTracker.Web and click OK. In the New ASP.NET Project dialog, select the Empty template.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/6ffc0f951c117ba96f4305e661164d976ec45067)


#Setting Up References
BugTracker.NET relies on a few third party libraries.: Lucene.NET, log4net and SharpMimeTools. In a Web Site project, these references are managed by adding the required assemblies to the web site's bin folder. Ultimately, we would like to manage these references using NuGet, but BugTracker references older versions of these assemblies that are not available on NuGet. We will handle the library upgrades and transition to NuGet in a future post.

For now, we will copy these assemblies to a separate folder named references and add references to these assemblies in the new Web Application Project.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/4a0bb85d9b9670bdd791a454a3e438eb1c32e515)

#Move the Files to the Web Application
The next step is to move all the files from the Web site folder to the new Web application project. Open Windows Explorer and browse to the folder containing the Web site project. In our case, we browse to src\www. Select all the files in this folder with the exception of the \bin folder. Copy the files and paste them into the src\BugTracker.Web folder. Overwrite any files that already exist in the BugTracker.Web folder. Delete the original Web site project folder (src\www).

Back in Visual Studio, select the BugTracker.Web project in solution explorer and select Show All Files. Select all the files in all folders with the exception of the App_Data, bin and obj folders. Right click and select Include In Project.

Build the project and ensure that there are no compile errors. You probably won't see any errors at this point, because files in the App_Code folder are dynamically compile at runtime. We can force the C# files in the App_Code folder to compile by renaming the folder to btnet, then selecting all the files in this folder and changing the Build Action from Content to Compile. Now you might receive some build errors. In Bug Tracker, we receive the following build error:

    The type or namespace name 'DirectoryServices' does not exist in the namespace 'System' (are you missing an assembly reference?)	~\BugTracker.NET\src\BugTracker.Web\App_Code\authenticate.cs	9	14	BugTracker.Web

We are seeing this error because the project is missing a reference to the System.DirectoryServices.Protocols assembly. After adding the required references, the project seems to compiles cleanly.

Now that we have verified the class files compile cleanly, we should also confirm that all the .aspx pages and .ascx controls compile cleanly. These files contain a lot of code that by default will only be dynamically compiled at runtime when the pages and controls are accessed in the application. You can confirm these pages compile by running the application and accessing all the pages individually, or you can precompile everything using aspnet_compiler.exe as follows:

    c:\Windows\Microsoft.NET\Framework\v4.0.30319>aspnet_compiler.exe -p %USERPROFILE%\documents\github\BugTracker.NET\src\BugTracker.Web -v / c:\staging

You might find more missing references at this point, but in our case there are no compile errors. We are safe to commit changes and move to the next step.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/3c64d84de9af96763713eae862d2b2eeeb1cf665)

#Generating code-behind files
One of the big advantages of a Web application project is that we can have a cleaner separation between the page or user control (aspx / ascx) and the C# code that is associated with that page or user control. In a web application, each aspx and ascx file should have a corresponding aspx.designer.cs. This designer class contains strongly typed fields that can be used in C# code to reference the elements in the aspx file. For example, if you have a text input element with id="user" then the designer.cs file will contain a field of type HtmlInputText called user:


    protected global::System.Web.UI.HtmlControls.HtmlInputText user;


The advantage of these strongly-typed fields is that the C# compiler can detect compilation errors at compile time in Visual Studio. In a Website Project, there is no aspx.designer.cs file and errors like this are detected at run-time instead of at compile time in Visual Studio.

If you are lucky, your application will already have code-behind files for all pages and user controls. For example, each .aspx file will have a corresponding aspx.cs file with the associated C# code. If this is the case, all you need to do now is generate the .aspx.Designer.cs by selecting the project, then selecting Convert to Web Application from the Project menu.

In the case of BugTracker, there are no .aspx.cs or .ascx.cs files. All the C# code is contained within the aspx and ascx files in a script tag. Moving the code to a code-behind file is a multi-step process.

Before we get started, this step will go much easier if we match the namespaces of the new code behind files with the original root namespace of BugTracker (btnet). Right click the BugTracker.Web project and select Properties. Change the Default Namespace from BugTracker.Web to btnet.

**Warning**: This step can be very time consuming and will very significantly from file to file. Patterns will emerge as you progress through your application and it will become easier as you go.

The basic steps for each file are as follows:

1. Add a new class named _PageName_.aspx.cs where _PageName_.aspx is the name of the page you are converting
2. Make the new class partial and make it inherit from Page
3. Add the Inherits and CodeBehind directives to the Page element in the aspx file
4. Run the Convert to Web Application process (Select BugTracker.Web in Solution Explorer, then select Convert to Web Application from the Project menu)
5. Move any code contained in a `<script language="C#" runat="server">` tag into the newly created class
6. Fix the visibility of class variables and methods. As a general rule, Page_Load and other page events should be public

###Simple Example
Let's run through a simple example first. The login page for BugTracker is default.aspx.
- First, we add a new class to the BugTracker.Web project. In the Add New Class dialog, enter the name as default.aspx.cs
- Next, we make the new class partial and make it extend from the Page class


     namespace btnet
     {
         public partial class @default : Page
         {
         }
     }


- Open the default.aspx file to add the CodeBehind and Inherits properties to the Page element as follows:


    <%@ Page language="C#" validateRequest="false" CodeBehind="default.aspx.cs" Inherits="btnet.default" %>



- Run the Convert to Web Application process
- Move the C# code from default.aspx to the code behind file
- Make the Page_Load method public
- Finally, we compile and run BugTracker.NET to make sure the login page still works as expected

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/29f9e86759a5be7e455c74ddf10eb40dd7b8306e)

If your application is small enough, you might be able to complete all the pages at once. If your application is much larger and your team is working on a number of other tasks, you will want to complete this process slowly over an extended period of time. You might even choose to leave some of your lesser used / modified aspx pages as is and not bother moving the C# code to a code behind file.

BugTracker.NET has over 120 pages, which will take a significant amount of time to complete. Let's take a look at one of the more complicated examples before we move on to the next phase.

###More Complicated Example

In BugTracker.NET, the main page for the application is bugs.aspx. This page is a little more complicated than most because it is fairly large and also because it is sharing code with another large page: search.aspx. These pages are both including code from inc_bugs.inc and inc_bugs2.inc using the [Classic ASP method including code](http://msdn.microsoft.com/en-us/library/ms524876.aspx) from another file.

    <!-- #include file = "inc_bugs.inc" -->
    <!-- #include file = "inc_bugs2.inc" -->

My first instinct here was to introduce a base class that contained the shared code located in inc_bugs.inc. After attempting this twice, I realized that a base class was probably not the best approach to achieve code reuse. A more suitable solution here would be to introduce a user control. Unfortunately, this is a much bigger task than simply moving the C# code from the aspx files to code-behind files. In this case, we decided it would be best to start by duplicating a small amount of code, then refactoring to introduce a better design at a later date. In refactoring, this approach is referred to as [inline module](http://refactoring.com/catalog/inlineModule.html).

- We start by inlining the code from inc_bugs.inc and inc_bugs2.inc into both bugs.aspx and search.aspx by replacing the include directives with the code from the included files.
- Delete inc_bugs.inc and inc_bugs2.inc since they are no longer used.
- Add a new class to the BugTracker.Web project. In the Add New Class dialog, enter the name as bugs.aspx.cs.
- Make the new bugs class partial and make it extend from Page
- Open the bugs.aspx file. Add the CodeBehind and Inherits properties to the Page directive.
- Add a new class to the BugTracker.Web project. In the Add New Class dialog, enter the name as search.aspx.cs.
- Make the new search class partial and make it extend from Page
- Open the search.aspx file. Add the CodeBehind and Inherits properties to the Page directive.
- Run the Convert to Web Application process
- Move the C# code from bugs.aspx to the code behind file.
- Make the Page_Load method public
- Move the C# code from search.aspx to the code behind file.
- Make the Page_Load method public
- Compile the application. We receive some compile errors due missing using statements. Add the following using statements to bugs.aspx.cs and search.aspx.cs to fix those errors.

```
     using System.Web.UI.WebControls;
     using System.Data;
```


- Run aspnet_compiler.exe to ensure the aspx files compile correctly. We receive a number of errors indicating fields and methods that cannot be accessed from bugs.aspx and search.aspx. This is because the code was moved to the code-behind files. In order for these fields and methods to be visible to the aspx code, we need to  change the visibility of those fields and methods to protected:
    - In bugs.aspx.cs, change the the visibility of the sql, dv, ds_custom_cols, security and sql_error fields to protected
    - In bugs.aspx.cs, change the visibility of the display_bugs method to proctected
    - In search.aspx.cs, change the visibility of the security, show_udf, use_full_names, dt_users, project_dropdown_select_cols, sql, dv and ds_custom_cols fields to protected
    - In search.aspx.cs, change the visibility of the display_bugs and write_custom_date_controls methods to protected
- Compile the application in Visual Studio, then re-run aspnet_compiler.exe. The application now compiles without errors.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/f6a8c364a14cb553c0ef80da84ef639fe540032e)

###Automating Repetitive Tasks

Many of these steps above are extremely tedious and do not vary at all from file to file. This is a good candidate for automation. By automating some of these steps, we can reduce the chances of human error and hopefully complete the bulk of the work in a lot less time that would otherwise be required. This is a technique sometimes referred to as Automated Software Engineering.

We created a PowerShell script to automate these tasks. A better choice might have been [T4](http://msdn.microsoft.com/en-us/library/bb126445.aspx). T4 (Text Template Transformation Toolkit) is a set of tools from Microsoft that is suited specifically to [code generation tasks](http://msdn.microsoft.com/en-us/magazine/hh882448.aspx).

We created a script that will take care of the following steps for each aspx file:

1. Add a new class named _PageName_.aspx.cs where _PageName_.aspx is the name of the page you are converting
2. Make the new class partial and set it to inherit from Page
3. Add the Inherits and CodeBehind directives to the Page element in the aspx file

This is a one-time use script, so we don't need to worry about making it perfect.

[View the script](https://github.com/dpaquette/BugTracker.NET/commit/1bda330a0f87658b73953940cf746fb250773972)

Using this script we were able to save countless hours and avoid almost certain repetitive strain injury.

After running the script, we run the Convert to Web Application project. At this point, each aspx file in our project now has a matching aspx.cs and aspx.designer.cs files.

Next, we compile the application to see if any errors were introduced. In this case, a single error was found in query.aspx.designer.cs due ot a naming conflict that was introduced:

    'query': member names cannot be the same as their enclosing type	 .\BugTracker.NET\src\BugTracker.Web\query.aspx.designer.cs	40	67	BugTracker.Web

The best way to resolve this issue is to rename the class in the code-behind file from query to queryPage.

Now that the application compiles in Visual Studio, run aspnet_compiler.exe to ensure all the pages still compile without errors.

Finally, run the application to ensure the application behaves as expected.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/b928ab9acef266d7f1f97b107338fa5a9ac249e8)

After completing a code-review of these changes, Simon suggested that it might be a good idea to introduce a common base Page class that all our pages would inherit from. This will give us a place to add common logic in future refactorings. We modified the script to have all pages inherit BasePage and added an empty BasePage class that inherits from Page.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/727972b6d92d011041adba58068dd746dc5e69e8)

Note that the C# code is still inlined in to most of the aspx pages in BugTracker.NET and most of the code-behind files are simply empty classes. We have decided to tackle moving the C# code in to the code-behind files as we go. Basically, anytime we need to make a change to a page, we will move the C# code to the code-behind file at that time. At least now, we have the structure needed to move forward.

#Final Notes
By moving from a Web Site project to a Web Application project, we have laid the initial groundwork required to start making use of some of the newer ASP.NET technologies. Through this process, we have relied heavily on the C# compiler and the aspnet compiler to ensure that we have not introduced any errors. We have also used some automated software engineering techniques to automate some of the more repetitive and relatively mindless tasks required in this step.
