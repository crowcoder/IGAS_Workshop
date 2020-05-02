# Exercise 4 - Configuration in the Azure Portal (30 min.)

At this point you have seen a variety of ways to configure settings in code and in Azure DevOps. It is time to turn our attention to Application settings in the Azure Portal.

Let's dig right in and set a configuration value.

> OBJECTIVE: Learn to add configuration settings to an App Service in the Azure portal.

1. From Home in the Azure portal select App Services.
1. Click the App Service you have been working with.
1. Open the Configuration blade.
1. Click "+ New application setting".
1. Enter a name and value. Here I'm setting a fictitions maximum time to live with a value of 30.
1. Be sure to Save.
1. Click "Continue" to save changes and restart your App Service.
1. Wait for it to complete.
1. Make a GET request to `/Configuration/all`. You should see the new setting under the Environment Variables Configuration Provider. 


 | Step 1 | Step 2 | Step 3 | 
 | --- | --- | --- |
| ![Step 1](./img/portal_settings_1.png) | ![Step 2](./img/portal_settings_2.png) | ![Step 3](./img/portal_settings_3.png) |
 | **Step 4** | **Step 5** | **Step 6** | 
| ![Step 4](./img/portal_settings_4.png) | ![Step 5](./img/portal_settings_5.png) | ![Step 6](./img/portal_settings_6.png) |
 | **Step 7** | **Step 8** | **Step 9** | 
 | ![Step 7](./img/portal_settings_7.png) | ![Step 8](./img/portal_settings_8.png) | ![Step 9](./img/portal_settings_9.png) |

 That was very quick and easy. Let's step back and consider what we did.
 We added configuration to our application without any code changes and without even performing a deployment. The new setting appeared under the Environment Variables, which as we know, is configured for use in our application's program.cs file. This is a very powerful capability. This behavior is what allows us to use appsettings.Development.json or User Secrets, etc., during development but still provide the necessary configuration to production through environment variables. All of the settings you see in the Configuration blade of the App Service become environment variables.

 > **Environment variable caveat**: While environment vars are convenient and easy to use, you must be sure to trust any libraries you use or software you run, especially if you have sensitive values in environment variables. This is because they are accessible to anything. Azure App Services are generally safer for environment variable use than using your own web server, but you should still be sure to trust any third party libraries your code is using.

 You can see many details of your App Service's operating system at the Service Control Management URL, which is the normal URL, except with "scm" appended to the host. For instance, if your application is at:

    https://aqixjv2y-igas-01.azurewebsites.net
Then the SCM URL is here:

    https://aqixjv2y-igas-01.scm.azurewebsites.net

1. Navigate your browser to your App Service's SCM endpoint and click "Environment"
1. Click "Environment variables".
1. Locate the "IGAS_MaxTTL" configuration setting.


  | Step 1 | Step 2 | Step 3 | 
 | --- | --- | --- |
| ![Step 1](./img/kudu_1.png) | ![Step 2](./img/kudu_2.png) | ![Step 3](./img/kudu_3.png) |

#### More fun with KeyVault

> OBJECTIVE: Learn to use a KeyVault secret reference in Azure App Service configuration.

One of my favorite features of App Services is how easy and yet secure it is to pull KeyVault secrets into configuration. Lets do this now. In the Exercise #4 section of [workshop.ps1](../scripts/workshop.ps1) run the following lines to create another KeyVault secret. You may find you need to re-connect or re-set some variables:

```Powershell
# Create another KeyVault secret
    $bankVaultCombination = ConvertTo-SecureString -String "10-31-15-3" -AsPlainText -Force
    $secret = Set-AzKeyVaultSecret -Name "BankVaultCombination" -VaultName $kv.VaultName -SecretValue $bankVaultCombination
```
Next, your App Service must be granted access to the KeyVault. You will recall creating an access policy for the Azure DevOps service connection. We will do basically the same thing now, except instead of a DevOps service connection, we are granting access to the App Service itself. To make this happen we will enable a very powerful feature called Managed Identity. Managed Identities allow azure resources to have an identity, much like a user, and makes accessing secure resources easier and prevents storing secrets in code. 

Imagine what would be required with a Managed Identity. Our application would have to store some credentials to access a KeyVault, or pass along an end user's credentials. With Managed Identity we can simply assign an access policy and implicitly allow access to KeyVault secrets. All of this interaction occurs securely within the Azure cloud.

```Powershell
# Enable Managed Identity for Azure Resources    
# Don't forget to change $appname to match yours
    $appname = "aqixjv2y-igas-01"
    Set-AzWebApp -AssignIdentity $true -Name $appname -ResourceGroupName $groupName 
    
# Create Access Policy for Managed Identity
    $svcPrincipal = Get-AzADServicePrincipal -DisplayName $appname
    Set-AzKeyVaultAccessPolicy -VaultName $kv.VaultName -ObjectId $svcPrincipal.Id -PermissionsToSecrets get, list 
```
Now we have configured our App Service to have read access to our KeyVault's secrets. We can pull a secret into Configuration with a special syntax call a KeyVault reference. This syntax is a special instruction to the App Service that tells it go look up a secret value and pull that into Configuration, and thus Environment Variables.

1. You will need to know the secret id, so navigate to your KeyVault and select the Secrets blade.
1. Open the BankVaultCombination secret.
1. Continue drilling into the secret details by clicking the CURRENT VERSION.
1. Copy the Secret Identifier.
1. Go back to your App Service, Configuration blade and begin adding a new application setting.
1. For the name, enter "IGAS_BankVaultCombo". For the Value, enter: "@Microsoft.KeyVault(SecretUri= \<the Secret Identifier>)". Mine looks like this: *@Microsoft.KeyVault(SecretUri=https://kv-vi53gndb-igas-01.vault.azure.net/secrets/BankVaultCombination/d18bc738075e4c8ca24a346dc1fe8150)*
1. Be sure to Save.
1. If you did everything correctly, you will see a green checkmark indicating the KeyVault reference has been successfully resolved.
1. Make a GET request to `/Configuration/all` and observe the value coming from a KeyVault secret.

 | Step 1 | Step 2 | Step 3 | 
 | --- | --- | --- |
| ![Step 1](./img/kv_ref_1.png) | ![Step 2](./img/kv_ref_2.png) | ![Step 3](./img/kv_ref_3.png) |
 | **Step 4** | **Step 5** | **Step 6** | 
| ![Step 4](./img/kv_ref_4.png) | ![Step 5](./img/kv_ref_5.png) | ![Step 6](./img/kv_ref_6.png) |
 | **Step 7** | **Step 8** | **Step 9** | 
| ![Step 7](./img/kv_ref_7.png) | ![Step 8](./img/kv_ref_8.png) | ![Step 9](./img/kv_ref_9.png) |

#### Slot Settings

> OBJECTIVE: Learn what slot deployments are and how to use them. Configure settings such that they either stick to a slot, or apply to all slots.

The last bit of magic we will do in the Azure Portal is to enable slots for our application and provide slot-specific configuration. 
> [**Slots Documentation**](https://docs.microsoft.com/en-us/azure/app-service/deploy-staging-slots)

Slots used to be called "testing in production". While I prefer the term "slots", the old name is admittedly descriptive. It allows you to deploy your application to the production App Service, *but on a differt URL*. Testing can be done at this staging URL, and when everything looks good, you perform a simple "swap" and what you just deployed to the test/staging slot is now full production. This is a great feature. If you currently do production deployments directly to production you know that as soon as you pull that trigger it either works or it doesn't. This can lead to chaos if it doesn't. With slots, if it doesn't work in the staged slot then you just do not perform the swap. You fix the issue, re-deploy to the staging slot, and test again. If a bug is found after a swap, you can swap again to roll back.

Let's enable slots for our application and tweak some configuration.

1. Open the Deployment Slots blade of your Azure App Service in the Azure portal and click "Add Slot".
1. Give it a name, "Staging" being a good choice. Choose to copy settings from the existing App Service. Click "Add".
1. Click your new slot to see its Overview.
1. Note its URL is different than your "production" app service. However, there is no application there yet. Creating a slot does not carry over any deployed application. 
1. Now navigate over to DevOps and open the Release pipeline. Select the "Deploy Azure App Service" task. Edit this task to deploy to the slot we just created. Your Resource Group and Slot selections should be available in the drop down lists. Be sure to Save your changes.
1. Now create a Release and Deploy both the DEV and PROD stages.
1. Make a GET request to the staging URL and observe that the deployment succeeded.
1. 

 | Step 1 | Step 2 | Step 3 | 
 | --- | --- | --- |
| ![Step 1](./img/slot_1.png) | ![Step 2](./img/slot_2.png) | ![Step 3](./img/slot_3.png) |
 | **Step 4** | **Step 5** | **Step 6** | 
| ![Step 4](./img/slot_4.png) | ![Step 5](./img/slot_5.png) | ![Step 6](./img/slot_6.png) |
 | **Step 7** | **Step 8** | **Step 9** | 
| ![Step 7](./img/slot_7.png) | ![Step 8](./img/slot_8.png) | ![Step 9](./img/slot_9.png) |
 | **Step 10** | **Step 11** | **Step 12** | 
| ![Step 10](./img/slot_10.png) | ![Step 11](./img/slot_11.png) | ![Step 12](./img/slot_12.png) |
 | **Step 13** | **Step 14** | **Step 15** | 
| ![Step 13](./img/slot_13.png) | ![Step 14](./img/slot_14.png) | ![Step 15](./img/slot_15.png) |
 | **Step 16** | **Step 17** | | 
| ![Step 16](./img/slot_16.png) | ![Step 17](./img/slot_17.png) |  |
