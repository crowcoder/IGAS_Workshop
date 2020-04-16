
############
## IN VS CODE, TO RUN ONLY A SECTION OF CODE, SELECT THE LINES YOU WANT TO RUN AND PRESS F8
###########

# You can use this to test the application or you can use any web client like
# a browser, Postman, Fiddler, CUrl, etc.
$api = "https://localhost:5001/Configuration"
Invoke-WebRequest -URI $api

#####################
## EXERCISE 2
#####################

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
# This generates a random name.
$prefix = [System.IO.Path]::GetFileNameWithoutExtension([System.IO.Path]::GetRandomFileName())
Write-Debug $prefix
$app = New-AzWebApp -ResourceGroupName $groupName -Name "$prefix-igas-01" -Location $location -AppServicePlan $plan.Id

# Browse to the URL of the new application to make sure the app service is up.
$newurl = "https://$prefix-igas-01.azurewebsites.net"
[System.Diagnostics.Process]::Start($newurl)
