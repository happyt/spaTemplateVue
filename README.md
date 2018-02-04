Basic use of Microsoft templates for Asp.Net Core

Using the vue version, with typescript

dotnet new --install Microsoft.AspNetCore.SpaTemplates::*
dotnet new   (to list them)

dotnet new vue

add C# extension to VS Code, and Debugger for Chrome extension
Choose Debug option .Net Core launch (web), F5,  -> Configure task runner
Creates launch.json
Change "program": "${workspaceRoot}/bin/Debug/netcoreapp1.1/AspCoreVue.dll",

npm install

Can then launch in debug

Add new debug option  Chrome Launch
with combined launch in launch.json
"compounds": [
    {
        "name": ".Net+Browser",
        "configurations": [ ".NET Core Launch (console)", "Launch Chrome" ]
    }
],

Will then have debug option Net + Browser

===

Add todo list section


