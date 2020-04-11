![banner](./img/banner.png)

# Welcome to this workshop on application configuration with .NET Core, Azure and Azure DevOps!

### Keeping secrets out of source code has always been challenging. De-coupling sensitive information like connection strings, certificates and passwords from your development projects means you won't have accidental exposure. .Net Core now provides a rich configuration system that goes far beyond the legacy web.config appSettings. This is great for development and debugging, but how do we manage configuration when it is time to go to production? In this hands-on workshop we will first configure a local application to pull configuration from various sources and then we will move on to explore real world production deployment scenarios.

This workshop will guide you through step-by-step exercises including:

* Working with Azure App Services configuration and environment variables.
* How to leverage Managed Identities for Azure Resources to eliminate a whole class of stored secrets.
* Configure KeyVaults and Access Policies and integrate them with DevOps.
* Setting up Azure DevOps build and release pipelines and experimenting with Tasks and Variables to configure applications for production.
* Connecting Azure DevOps to Azure App Service Deployments.
* Slot deployments and maintaining different configuration versions between them.
* Configuring and obtaining secrets from pipeline Variables, shared Library Variable Groups and Secure Files.
* Transforming json configuration files during release pipelines.

Please move on to view the prerequisites in [prereqs.md](./prereqs.md)