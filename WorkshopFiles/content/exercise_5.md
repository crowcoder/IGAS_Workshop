# Exercise 5 - Secure Files (30 min)

Exercise 3 introduced you to Variable Groups which are variables stored globally within the DevOps Library. Another feature of the Library is Secure Files, which allow you to securely store arbitrary files. They cannot be downloaded nor viewed, they are only available to pipelines and they are deleted from the pipeline temporary folder immediately after use. The primary purpose is to store [signing certificates, Apple Provisioning Profiles, Android Keystore files, and SSH keys](https://docs.microsoft.com/en-us/azure/devops/pipelines/library/secure-files?view=azure-devops), but they can also be extremely handy for storing configuration secrets needed by your applications.

The basic idea is to store a json file containing configuration settings in the DevOps Library and copy it into your publish folder during a Build. If, in the start up of your .NET Core application you do an Add() of a json file by the same name as your secure file, your application will pull the settings into Configuration the same as if you put your settings into appsettings.json or any other provider.

You may have noticed that in [program.cs](/Program.cs) there is a line of code that adds a json file named "prod_appsettings.json" to Configuration. This was designated optional but now we are going to make use of it.

#### Extend the Build to Bring in prod_appsettings.json
We need to add two steps to the Build pipeline. First it will download the secure file, then a Command Line task will copy it into the staging directory so that it gets pulled into the artifact that is deployed by the Release.

Edit the azure-pipelines.yml file by un-commenting the following lines. Yaml is whitespace sensitive, so be careful to delete only the "#" characters. 

````yaml
#- task: DownloadSecureFile@1
#  inputs:
#    secureFile: 'prod_appsettings.json'
#
#- task: CmdLine@2
#  inputs:
#    script: 'cp "$(Agent.TempDirectory)/prod_appsettings.json" "$(Build.ArtifactStagingDirectory)"'
````
With these changes in place, run a build and then drill into the artifact to see the result:




