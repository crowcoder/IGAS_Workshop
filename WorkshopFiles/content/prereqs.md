# Prerequisites

#### This workshop has been designed to be as flexible as possible. Whether on Windows, Mac or Linux, you will be able to complete this workshop with the following tools:

* [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1). If you are using full Visual Studio 2019 just make sure it has the latest update. If you are not already setup for .NET Core development, download and install the SDK, not just the runtime.
* An Azure account. [The free trial](https://azure.microsoft.com/en-us/free/) is more than sufficient for this course. Even if you already have an Azure subscription you might consider starting a free trial under another email address because part of this workshop requires resources that cost a small amount of money. However, the cost is very negligible assuming you remove the resource after finishing the workshop.
* Setup an Azure DevOps project. Use [the quickstart guide](https://docs.microsoft.com/en-us/azure/devops/user-guide/sign-up-invite-teammates?view=azure-devops) to create your account and setup a "Project". You will not need to invite team members or configure invitations. If you already have a DevOps account then you can easily add a new Project for this workshop.
* Azure CLI access. You can [install the Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) locally, or if you prefer, you can perform all Azure CLI operations from the [Azure Cloud Shell](https://shell.azure.com) which runs entirely in the browser. 
* [Powershell or Powershell Core](https://docs.microsoft.com/en-us/powershell/azure/install-az-ps?view=azps-3.0.0) (which has its own requirements). Once you have this installed, you can execute Powershell commands right from your choice of environments, including VS Code.
* Get the application we will be working with and make sure it runs. [Exercise #1](.\exercise_1.md) provides an overview of the sample application. For now, just acquire it and test it:
     * Clone the repository (https://github.com/crowcoder/IGAS_Workshop.git)
     * Run with debugging in VS Code. Tip, use the debug charm: ![debug charm](.\img\debug.png) (ctrl+shift+d on windows)
     * Use your favorite HTTP client (Browser, Postman, Fiddler, Powershell) and navigate to https://localhost:5001/Configuration. You should receive the folowing  JSON:
     * `["Test -  Hello World!"]`

  [A powershell script is provided in this project](../scripts/workshop.ps1) for performing various tasks.
 
***Optional***
* The new **Windows Terminal**. Avaiable in the Windows Store (for free). With this terminal you can utilize multiple tabs, each with it's own shell - Powershell, cmd or Cloud Shell.
* [Visual Studio Code](https://code.visualstudio.com/). While any text editor will work, VS Code is a great, free editor that integrates nicely with .NET and Powershell. If you do use VS Code, you will also need the C# extension: [C# for Visual Studio Code (powered by OmniSharp).](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp). Of course you can also use full Visual Studio, Rider or even notepad. 