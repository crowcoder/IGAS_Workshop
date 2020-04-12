# Exercise #2, Get it into DevOps (30 min.)
Now that you have completed development of the API, it is time to #RubDevOpsOnIt.
Make sure you have completed the prerequisites so you have an Azure DevOps organization ready to go, log into your DevOps account, then:

1. Create project
    * Name the project whatever you want.
    * Choose between public and private. It may be easier to receive assistance if you make it public. 
    * Click "Create project" to generate a project backed by a Git repository.
1. Once the project has been created, choose "Repos" to get your source code into Azure Git.
1. Choose "Import a repository option".
1. For the clone URL, enter https://github.com/crowcoder/IGAS_Workshop.git
1. When the import is complete you will see your new repository. This repo is a copy of the GitHub repo, the two are not linked. Changes to one will not affect changes to the other.
1. Change to the Prod branch.

 | Step 1 | Step 2 | Step 3 | 
 | --- | --- | --- |
 | ![Step 1](./img/project_setup_1.png) | ![Step 2](./img/project_setup_2.png) | ![Step 3](./img/project_setup_3.png) |
 | **Step 4** | **Step 5** | **Step 6**| 
 | ![Step 4](./img/project_setup_4.png) | ![Step 5](./img/project_setup_5.png) | ![Step 6](./img/project_setup_6.png) |

## Prod branch differences
 In just a moment you will create a Build of the project in Azure DevOps. But first, let's go over what I have changed between what you have done so far on the master branch and Prod branch which we will be working with from here on out. You can view the files in the DevOps user interface or in your IDE by changing to the Prod branch.

 #### Changes to Program.cs
In the setup of the Environment Variable configuration provider I have included a prefix argument of "IGAS_". This means that only environment variables that begin with "IGAS_" will be pulled into configuration. Note that the prefix itself will be removed. If you have an environment variable named "IGAS_Password" then your C# code will need to look for just "Password".
```
 config.AddEnvironmentVariables(prefix: "IGAS_");
 ```
 #### Changes to ConfigurationController.cs
The `Get()` method has been modified to return all configuration settings instead of writing a line of code for each one (e.g. `_config.GetValue<string>("..setting..")`). This will allow us to focus on how and where to set configuration without needing to make a bunch of code edits. It also facilitates experimentation because any changes you make, regardless of the technique, will echo back from your API call.
 ```
var AllConfigSettings = _config.AsEnumerable()
    .Select(c => new { ConfigKey = c.Key, ConfigValue = c.Value});

return AllConfigSettings;
```
#### Changes to appsettings.json
The appsettings file has been modified to include a few fictitious settings. The values are formatted as "\_\_*value*\_\_". The double underscores are meant to make the value a "token" that can be replaced during deployment.
```
{
    "ConnectionString" : "__ConnectionString__",
    "StockQuoteAPIURL" : "__StockQuoteAPIURL__",
    "Password" : "__Password__"
}
```
 ## Setup a Build
 We don't have a lot of work to do in the Build pipeline. There are no configuration steps included here (though you could), we just need to have a published project to deploy in the Release pipeline. It is in the Release Pipeline that we will manage configuration.
 > [More information on Build pipelines](https://docs.microsoft.com/en-us/azure/devops/pipelines/ecosystems/dotnet-core?view=azure-devops#package-and-deliver-your-code)

1. Click on the Rocket Ship icon to open Pipelines and then click "Create Pipeline"
1. Choose Azure Repos Git YAML
1. Select the IGAS Git code repository
1. Select Existing Azure Pipelines YAML file. 
1. Select the "prod" branch and "azure-pipelines.yml" file.
1. Click run to build the project and produce an artifact that will be deployed later in a Release pipeline.
1. Review the successful deployment.

  | Step 1 | Step 2 | Step 3 | 
 | --- | --- | --- |
 | ![Step 1](./img/build_1.png) | ![Step 2](./img/build_2.png) | ![Step 3](./img/build_3.png) |
 | **Step 4** | **Step 5** | **Step 6**| 
 | ![Step 4](./img/build_4.png) | ![Step 5](./img/build_5.png) | ![Step 6](./img/build_6.png) |
  | **Step 7** |  | | 
 | ![Step 4](./img/build_7.png) | | |

  ## Create a Release
  A "Release" is the process that deploys your application. It will pick up the artifact created by the Build pipeline and push it to an Azure App Service. Here we also have an opportunity to further configure the application by replacing setting values based on the environment or "stage". For instance, a Release pipeline may have, for example, 3 stages. Development, QA and Production. At each stage we can swap out values that are specific to that stage.

  First things first though, we need an Azure App Service.

  #### Create the App Service
  An Azure App Service is a resource that hosts your web application. The App Service runs within an App Service Plan that provides the cpu, memory and other features. Later, we will experiment with Slots, which requires a paid version of the App Service Plan. So be sure to delete the resources you create when you are done with the workshop.

> [The Powershell for Azure command reference](https://docs.microsoft.com/en-us/powershell/module/?view=azps-3.7.0)

> [workshop.ps1 in the scripts folder has all the commands you will see here, in an easier to execute format](../scripts/workshop.ps1)

You will execute the following Powershell commands to create a Resource Group, an Azure App Service Plan, and a Web App, **but edits are required.**
1. [Optional] If your default subscription is not the one you want to use for this workshop, uncomment **line 10** and enter the Azure Subscription ID you want to use.
1. [Optional] On **line 15** change the location to a region closest to you.
1. On **line 24** enter a value to be used as a prefix to the web app name. This must be unique across Azure. Your site will be accessed at "https://~your prefix~-igas-01.azurewebsites.net". Feel free to completely change this name as desired.

```
# Authenticate to Azure
Connect-AzAccount

# List the subscriptions your account has access to
Get-AzSubscription

# You may have multiple subscriptions. If the one you want to use for this tutorial
# is not your default subscription, you can do this to change what subscription
# this script will operate against.
# Set-AzContext -SubscriptionId "<enter your subscription id>"

# Create resource group to hold all resources for the tutorial.
# Change the region as appropriate for your location
$groupName = "rg-igas-01"
$location = "eastus"
New-AzResourceGroup -Name $groupName -Location $location

# Create an app service plan
$plan = New-AzAppServicePlan -Location $location  -Name "asp-igas-01" -ResourceGroupName $groupName -Tier "S1"

# Create the App Service to host the application
# The name has to be unique across all of Azure because it is part of the public URL.
# Set the prefix variable to something unique, either completely random or meaningful, doesn't matter.
$prefix = "???"
$app = New-AzWebApp -ResourceGroupName $groupName -Name "$prefix-igas-01" -Location $location -AppServicePlan $plan.Id

# Browse to the URL of the new application to make sure the app service is up.
$newurl = "https://$prefix-igas-01.azurewebsites.net"
[System.Diagnostics.Process]::Start($newurl)
```
#### Create a Release Pipeline
Now that you have a place to deploy your application, it is time to create a Release Pipeline.

