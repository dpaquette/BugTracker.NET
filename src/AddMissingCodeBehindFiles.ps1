$CSProjFileChanges = ""

#1) Get all the ASPX files in the current directory

Get-ChildItem ".\" -Filter *.aspx | `
Foreach-Object{

    #Check if a code behind file already exists
    $CodeBehindPath = $_.FullName + ".cs"
    $CodeBehindExists = Test-Path $CodeBehindPath
    If  (-Not $CodeBehindExists){              
          
        $PageName = $_.FullName.Replace(".aspx", "")
        $PageName = $PageName.Substring($PageName.LastIndexOf("\") + 1)
        
        Write-Host "Creating file: " $CodeBehindPath
        #2) Create a code behind file with a partial class
        $CodeBehindContents = "using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace btnet
{
    public partial class " + $PageName + " : Page
    {
    }
}"
        $CodeBehindContents | Set-Content $CodeBehindPath


        #3) Add the CodeBehind and Inherits attributes to the Page directive in the ASPX file
        $PageDirectiveWithCodeBehind = '<%@ Page language="C#" CodeBehind="' + $PageName + '.aspx.cs" Inherits="btnet.' + $PageName +'"'
        (Get-Content $_.FullName) | 
            Foreach-Object {$_ -replace '<%@ Page language="C#"',$PageDirectiveWithCodeBehind}  | 
            Set-Content $_.FullName

        #Add to the changes that will be made to the CSProj file
        $CSProjFileChanges = $CSProjFileChanges  + '
            <Compile Include="' + $PageName + '.aspx.cs">
                <DependentUpon>' + $PageName + '.aspx</DependentUpon>
                <SubType>ASPXCodeBehind</SubType>
            </Compile>'

    }
}

#4) Add the new code behind files to the CSproj file
$FoundCompileItemGroup = $false
(Get-Content ".\BugTracker.Web.csproj") |
        Foreach-Object {
            If ($_.ToString().Trim().StartsWith("<Compile") -and $FoundCompileItemGroup -eq $false){
                
                $CSProjFileChanges
                 $_
                $FoundCompileItemGroup = $true
                 
            }                    
            Else{
                $_
            }
         }  |  Set-Content ".\BugTracker.Web.csproj"
