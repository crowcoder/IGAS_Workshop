# Exercise 5 - Secure Files (30 min)

Exercise 3 introduced you to Variable Groups which are variables stored globally within the DevOps Library. Another feature of the Library is Secure Files, which allow you to securely store arbitrary files. They cannot be downloaded nor viewed, they are only available to pipelines and they are deleted from the pipeline temporary folder immediately after use. The primary purpose is to store [signing certificates, Apple Provisioning Profiles, Android Keystore files, and SSH keys](https://docs.microsoft.com/en-us/azure/devops/pipelines/library/secure-files?view=azure-devops), but they can also be extremely handy for storing configuration secrets needed by your applications.

Additionally, if developer access to the Library and the Azure App Service are restricted, this is a great way to utilize secrets that the developer does not have access to.

The basic idea is:
1. Store a json file of configuration in the DevOps Library
1. In the start up of your .NET Core application, do an Add() of a json file by the same name.
1. For non-production environments, use configuration values that are stored in User Secrets, appsettings.Development.json, or wherever you prefer.
1. In the Release pipeline, copy the secure file into the publish directory so it deploys along with your application. You could also do this in the build pipeline, but makes restricting user access more work. By copying the file during the Release it is not part of the build artifact that is genarally available to developers that manage builds.


