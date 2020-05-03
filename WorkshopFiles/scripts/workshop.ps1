
############
## IN VS CODE, YOU CAN EXECUTE JUST THE SELECTED LINES BY SELECTING THE 
## LINES YOU WANT TO RUN AND PRESSING F8 OR CLICKING THE "RUN SELECTION"
## BUTTON IN THE TOP RIGHT CORNER.
###########

# You can use this to test the application or you can use any web client like
# a browser, Postman, Fiddler, CUrl, etc.
$api = "https://localhost:5001/Configuration"
Invoke-WebRequest -URI $api

## ███████╗██╗  ██╗███████╗██████╗  ██████╗██╗███████╗███████╗    ██████╗ 
## ██╔════╝╚██╗██╔╝██╔════╝██╔══██╗██╔════╝██║██╔════╝██╔════╝    ╚════██╗
## █████╗   ╚███╔╝ █████╗  ██████╔╝██║     ██║███████╗█████╗       █████╔╝
## ██╔══╝   ██╔██╗ ██╔══╝  ██╔══██╗██║     ██║╚════██║██╔══╝      ██╔═══╝ 
## ███████╗██╔╝ ██╗███████╗██║  ██║╚██████╗██║███████║███████╗    ███████╗
## ╚══════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝ ╚═════╝╚═╝╚══════╝╚══════╝    ╚══════╝

# Authenticate to Azure
Connect-AzAccount

# List the subscriptions your account has access to
Get-AzSubscription

# You may have multiple subscriptions. If the one you want to use for this tutorial
# is not your default subscription, you can do this to change what subscription
# this script will operate against.
Set-AzContext -SubscriptionId "<enter your subscription id>"

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


##  ███████╗██╗  ██╗███████╗██████╗  ██████╗██╗███████╗███████╗    ██████╗ 
##  ██╔════╝╚██╗██╔╝██╔════╝██╔══██╗██╔════╝██║██╔════╝██╔════╝    ╚════██╗
##  █████╗   ╚███╔╝ █████╗  ██████╔╝██║     ██║███████╗█████╗       █████╔╝
##  ██╔══╝   ██╔██╗ ██╔══╝  ██╔══██╗██║     ██║╚════██║██╔══╝       ╚═══██╗
##  ███████╗██╔╝ ██╗███████╗██║  ██║╚██████╗██║███████║███████╗    ██████╔╝
##  ╚══════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝ ╚═════╝╚═╝╚══════╝╚══════╝    ╚═════╝                                                              

# Be sure to connect to your Azure subscription first

# You will need the objectId of your Azure AD account. Run this command and copy the Id
# from the output for your account. Note this command can return multiple accounts if
# more than one matches the search string.
    Get-AzADUser -SearchString 'your name here'

# Create a KeyVault

    # Once again we need to make an Azure-unique name so we will randomize it.
    $rndName = [System.IO.Path]::GetFileNameWithoutExtension([System.IO.Path]::GetRandomFileName())

    $kv = New-AzKeyVault -Name "kv-$rndName-igas-01" -ResourceGroupName "rg-igas-01" -Location "East US" 

    # Give yourself full access to the keyvault
    Set-AzKeyVaultAccessPolicy `
        -VaultName $kv.VaultName `
        -ObjectId "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" `
        -PermissionsToSecrets get, list, set, delete, backup, restore, recover, purge
    
# Add a secret
    $passwordAsSecureString = ConvertTo-SecureString -String "P@ssw0rd1" -AsPlainText -Force
    Set-AzKeyVaultSecret -Name "PasswordSecret" -VaultName $kv.VaultName -SecretValue $passwordAsSecureString

# Define an Access Policy for the DevOps Service Principal
    Set-AzKeyVaultAccessPolicy `
        -VaultName $kv.VaultName `
        -ObjectId "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" `
        -PermissionsToSecrets get, list


## ███████╗██╗  ██╗███████╗██████╗  ██████╗██╗███████╗███████╗    ██╗  ██╗
## ██╔════╝╚██╗██╔╝██╔════╝██╔══██╗██╔════╝██║██╔════╝██╔════╝    ██║  ██║
## █████╗   ╚███╔╝ █████╗  ██████╔╝██║     ██║███████╗█████╗      ███████║
## ██╔══╝   ██╔██╗ ██╔══╝  ██╔══██╗██║     ██║╚════██║██╔══╝      ╚════██║
## ███████╗██╔╝ ██╗███████╗██║  ██║╚██████╗██║███████║███████╗         ██║
## ╚══════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝ ╚═════╝╚═╝╚══════╝╚══════╝         ╚═╝

# Create another KeyVault secret
    $bankVaultCombination = ConvertTo-SecureString -String "10-31-15-3" -AsPlainText -Force
    $secret = Set-AzKeyVaultSecret -Name "BankVaultCombination" -VaultName $kv.VaultName -SecretValue $bankVaultCombination

# Enable Managed Identity for Azure Resources    
# Don't forget to change $appname to match yours
    $appname = "aqixjv2y-igas-01"
    Set-AzWebApp -AssignIdentity $true -Name $appname -ResourceGroupName $groupName 
    
# Create Access Policy for Managed Identity
    $svcPrincipal = Get-AzADServicePrincipal -DisplayName $appname
    Set-AzKeyVaultAccessPolicy -VaultName $kv.VaultName -ObjectId $svcPrincipal.Id -PermissionsToSecrets get, list