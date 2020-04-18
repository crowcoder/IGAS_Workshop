# Exercise 3 - Configuration in Azure DevOps Release Pipelines

Very often we have multiple environments that our applications need to be deployed to. In this workshop we will have two - "DEV" and "Production". For simplicity we have left out other, traditional environments like QA and UAT, but the following concepts apply exactly the same.

We only want a single code base so that is why we have application configuration. Dev configuration values need to be different from Production configuration values. We are going to experiment with JSON file token replacement. At the time of release, we will substitute token values in json files with environment-specific values. We will do this via 3 methods:
1. Pipeline variables
1. Variable groups
1. Azure KeyVault values

You may recall from a previous exercise that we have 3 appsettings.json settings that are just placeholders: *\_\_ConnectionString\_\_*, *\_\_Password\_\_* and *\_\_StockQuoteAPIURL\_\_*. These are our tokens that will be replaced during the Release.

Let's get the creation of our KeyVault out of the way. A KeyVault is an Azure service that securely stores keys, secrets and certificates. In this workshop, we will only work with *secrets*. An Azure release pipeline has the capability to read from a KeyVault, making it a great choice for storing secrets that not even a developer needs to know.
