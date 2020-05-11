# Exercise 5 - Secure Files (30 min)

Exercise 3 introduced you to Variable Groups which are variables stored globally within the DevOps Library. Another feature of the Library is Secure Files, which allow you to securely store arbitrary files. They cannot be downloaded directly nor viewed, they are only available to pipelines and they are deleted from the pipeline temporary folder immediately after use. The primary purpose is to store [signing certificates, Apple Provisioning Profiles, Android Keystore files, and SSH keys](https://docs.microsoft.com/en-us/azure/devops/pipelines/library/secure-files?view=azure-devops), but they can also be extremely handy for storing configuration secrets needed by your applications.

The basic idea is to store a json file containing configuration settings in the DevOps Library and copy it into your publish folder during a Build. If, in the start up of your .NET Core application you do an Add() of a json file by the same name as your secure file, your application will pull the settings into Configuration the same as if you put your settings into appsettings.json or any other provider.

You may have noticed that in [program.cs](/Program.cs) there is a line of code that adds a json file named "prod_appsettings.json" to Configuration. This was designated optional but now we are going to make use of it.

#### Extend the Build to Bring in prod_appsettings.json

> OBJECTIVE: Learn how to copy a Secure File into your build for consumption by your application at runtime.

We need to add two steps to the Build pipeline. First it will download the secure file, then a Command Line task will copy it into the staging directory so that it gets pulled into the artifact that is deployed by the Release.


![Step 1](./img/steps/step1.png)

With any text editor, create a json file named `prod_appsettings.json` with a few settings. Paste the following if you like:
    
````json
{
    "ProdSetting1" : "sample value 1",
    "ProdSetting2" : "sample value 2",
    "ProdSetting3" : "sample value 3"
}
````

![Step 2](./img/steps/step2.png)

Now go to DevOps, access the Library by clicking the Rocket Ship, select Library, then click Secure Files.

![Step 2](./img/secure_file_1.png)

![Step 3](./img/steps/step3.png)

Upload a Secure File.

![Step 3](./img/secure_file_2.png)

![Step 4](./img/steps/step4.png)

Confirm

![Step 4](./img/secure_file_3.png)

![Step 5](./img/steps/step5.png)

Click on your secure file to access its properties.

![Step 5](./img/secure_file_4.png)

![Step 6](./img/steps/step6.png)

Slide the toggle to Authorize for use in all Pipelines. The mores secure option is to leave this disabled but if you do, you will need to perform an "allow" step before the build will continue.

![Step 6](./img/secure_file_5.png)

![Step 7](./img/steps/step7.png)

Begin an edit on the IGAS **Build** pipeline (not the release pipeline).

![Step 7](./img/secure_file_7.png)

![Step 8](./img/steps/step8.png)

Edit the azure-pipelines.yml file by un-commenting the following lines. Yaml is whitespace sensitive, so be careful to delete only the "#" characters. Save your changes.

````yaml
#- task: DownloadSecureFile@1
#  inputs:
#    secureFile: 'prod_appsettings.json'
#
#- task: CmdLine@2
#  inputs:
#    script: 'cp "$(Agent.TempDirectory)/prod_appsettings.json" "$(Build.ArtifactStagingDirectory)"'
````
![Step 8](./img/secure_file_8.png)

![Step 9](./img/steps/step9.png)

Choose to commit the change directly to the prod branch and Save. 

![Step 9](./img/secure_file_9.png)

![Step 10](./img/steps/step10.png)

Run the Build pipeline.

![Step 10](./img/secure_file_10.png)

![Step 11](./img/steps/step11.png)

Click on the executing build and wait for it to complete.

![Step 11](./img/secure_file_11.png)

![Step 12](./img/steps/step12.png)

Click on the artifact.

![Step 12](./img/secure_file_12.png)

![Step 13](./img/steps/step13.png)

Drill into the artifact to locate the .zip file and download it by clicking on it.

![Step 13](./img/secure_file_13.png)

![Step 14](./img/steps/step14.png)

Open the zip file and notice the secure file is part of the package.

![Step 14](./img/secure_file_14.png)

![Step 15](./img/steps/step15.png)

Now that there is an updated artifact, create a Release and what for it to succeed (steps omitted). Finally, make a request to your API **staging** slot and observe the settings from the secure file are displaying.

![Step 15](./img/secure_file_15.png)




