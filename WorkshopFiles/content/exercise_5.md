# Exercise 5 - Secure Files (30 min)

Exercise 3 introduced you to Variable Groups which are variables stored globally within the DevOps Library. Another feature of the Library is Secure Files, which allow you to securely store arbitrary files. They cannot be downloaded nor viewed, they are only available to pipelines and they are deleted from the pipeline temporary folder immediately after use. The primary purpose is to store [signing certificates, Apple Provisioning Profiles, Android Keystore files, and SSH keys](https://docs.microsoft.com/en-us/azure/devops/pipelines/library/secure-files?view=azure-devops), but they can also be extremely handy for storing configuration secrets needed by your applications.

Additionally, if developer access to the Library and the Azure App Service are restricted, this is a great way to utilize secrets that the developer does not have access to.

The basic idea is to store a json file containing configuration settings in the DevOps Library and copy it into your publish folder during a Release. If, in the start up of your .NET Core application you do an Add() of a json file by the same name as your secure file, your application will pull the settings into Configuration the same as if you put your settings into appsettings.json or any other provider. Doing this during the Release instead of the Build keeps the file out of the build artifact and thus makes it easier to restrict access.

You may have noticed that in [program.cs](/Program.cs) there is a line of code that adds a json file named "prod_appsettings.json" to Configuration. This was designated optional but now we are going to make use of it.

#### Extend the Release to Bring in prod_appsettings.json







