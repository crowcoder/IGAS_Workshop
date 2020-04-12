# Exercise #1, Exploring the sample application (30 min.)

The IGAS (I've Got a Secret) application is a ASP.NET API. There are no web pages, just a single endpoint that returns JSON. The code and structure of the application is kept as simple as possible to concentrate on how to use configuration settings as opposed to how to create an API in .NET Core. The JSON payload is simply going to echo configuration values so we can observe the behavior as we edit the application.

If you are familiar with the `dotnet` cli, you might notice the source code differs from what you get when you create a new application: `dotnet new webapi`. The difference is intentional - some of what is normally abstracted has been explicitly written for educational purposes and is the reason the term "Manually" appears in the method name.

### A little bit about .NET Core Configuration
.NET Core is a major overhaul to the configuration system previously used by .NET Framework applications. When I say "configuration", I'm talking about data that cannot or is not practical to be set at development time. Commonly configured values include URL's, file paths, passwords, etc. - things that have test values during debugging but production values when running live. It has often been a challenge to store configuration such that it is not exposed. .NET Core provides various ways to keep these potentially sensitive values out of source control repositories.

> [More Information on .NET Core Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1)

In .NET Core, configuration can come from nearly any conceivable source, know as Providers. - JSON, XML and INI files, Azure Keyvault, command line arguments, environment variables, User Secrets, and the list goes on. You can even implement your own configuration provider if none of the existing ones meet your needs.

Let's begin exploring configuration by adding a setting to the JSON file in the sample project.

Open up `appsettings.Development.json` and add a new string entry: `"FromAppSettings" :"I came from appsettings."`
The entire file should look like this when you are done:

```
    {
      "FromAppSettings" :"I came from appsettings."
    }
```
Next, edit the controller to return this configuration value.
1. In the Controllers folder, open the file [ConfigurationController.cs](../../Controllers/ConfigurationController.cs).
1. Edit the `Get()` action to return the new configuration setting in adddition to the Test value:

```
    [HttpGet]
    public IEnumerable<string> Get()
    {
        string appSettingValue =_config.GetValue<string>("FromAppSettings") ?? "NOT SET";
        
        return new string[] { 
            "Test - Hello World!",
            appSettingValue
         };
    }
```
3. Run the application and invoke a GET request to the controller just like you did in the prerequisites. 

You should now see the new configuration value in the body of the response:

`["Test - Hello World!","I came from appsettings."]`

> OK, but what happened and what makes that work?

Let's dive a little deeper in the Configuration Providers. This material provides the base of understanding for all the work we will do from here on out, including Azure.

In the root of the project, there is `Program.cs`. If you think this file looks like a console application, that's because it is. .NET Core applications are console applications that happen to get hosted by something like Internet Information Services. The following code is executed at startup, we mostly just care about the numbered lines. Please note that this code is an edited version of the ASP .NET Core source code. The default project template abstracts this away but it is valuable for demonstration:

```
        public static IWebHostBuilder CreateDefaultBuilderManually(string[] args)
        {
            var builder = new WebHostBuilder();

            builder.UseContentRoot(Directory.GetCurrentDirectory());

            if (args != null)
            {
1.               builder.UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build());
            }

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

2.               config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
3.                     .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment())
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly != null)
                    {
4.                      config.AddUserSecrets(appAssembly, optional: true);
                    }
                }

5.              config.AddEnvironmentVariables();

                if (args != null)
                {
6.                  config.AddCommandLine(args);
                }
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
            }).
            UseDefaultServiceProvider((context, options) =>
            {
                options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
            });

            ConfigureWebDefaultsManually(builder);

            return builder;
        }
```

Much of this code is not important in the context of what we are learning about configuration but I have numbered the relevant lines. 
Notice this "thing" called **config**. Think of this as the configuration system for .NET Core. Looking at lines 2 and 3, we see `AddJsonFile`. This code instructs the runtime to go read the contents of these files and pull the information into the overall configuration environment. With that happening on start up, we can explain the code we wrote in the controller:

`string appSettingValue =_config.GetValue<string>("FromAppSettings") ?? "NOT SET";`

Again, `_config` represents the .NET Core configuration system. We are asking it to get a value from configuration with the key "FromAppSettings". If it is not found then use the value "NOT SET".

The important thing to notice here is that we do not write code that says "get the value of FromAppSettings from the appsettings.Development.json file". We just ask `_config` to get the value **regardless of what configuration provider injested it**.

To drive that point home let's demonstrate that concept and another. The other is how configuration providers override eachother. In other words, *order matters*. As the code execution progresses from line 1, to 2 to 3 etc., the various providers will override any previously injested values by the same name.

To demonstrate, lets run the application and pass it a command line variable that has the same key as the value we added to appSettings.Development.json.

Open a terminal to the root of the project and invoke the dotnet cli command to build and run the application, including a command line parameter by the same name as the one we already used, but with a different value so we can observe the behavior.

`dotnet run --FromAppSettings "I'm actually from the command line argument"`

Wait for it to start up and then invoke the GET request:

Notice how the command line argument overrode the appSetting? That is because, in startup, `config.AddCommandLine(args);` is called after `config.AddJson(...`. 

> Important:

We have just learned how the configuration system injests settings from various sources and bundles them up in one "location". This is a very powerful techinque to use. It allows us to put configuration anywhere we want during development without needing to have the same configuration provider being used in production. For example, you can put a URL in a json file, but in production you can put it in an environment variable and your code does not have to change.

We have also just learned the first example of providing configuration without storing it where it would be accidentally checked in to source control. While I personally have not had much use for passing command line arguments to .NET Core applications, it is definitely an option and it does not require persistence in any project file.

## User Secrets
> [User Secrets documentation](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows)

As a conscientious developer who is careful to keep configuration secrets out of source code, User Secrets is probably your best friend. User Secrets is a built-in way to store configuration in a file outside of your project. This file is stored in your profile folder and is as secure as anything under your profile. And being outside the project folder, there is no chance of adding to source control. It consists of a json file named `secrets.json`, within a folder typically named with a GUID to avoid collisions between applications. The options for enabling User Secrets varies by your IDE, lets do it with the `dotnet` cli so it works the same for everyone:

1. Open the IGAS.csproj file to get a look at the contents before we make changes:
```
    <Project Sdk="Microsoft.NET.Sdk.Web">
    <PropertyGroup>
        <TargetFramework>netcoreapp3.1</TargetFramework>
    </PropertyGroup>
    </Project>
```

2. Open a terminal in the folder containing the IGAS.csproj file.
3. Run the following command to initialize User Secrets: `dotnet user-secrets init`
4. The output of the command will show the generated GUID.
5. Go back and compare the .csproj file. Notice the addition of UserSecretsId:

```<?xml version="1.0" encoding="utf-8"?>
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <UserSecretsId>f1a8764b-0472-4566-9fe7-84b6c5f93843</UserSecretsId>
  </PropertyGroup>
</Project>
```

6. Add a secret, again with the dotnet cli: `dotnet user-secrets set "MyUserSecret" "ABCXYZ"`
7. In a text editor, inspect the json file that was created by the `set` command. On Windows go to %APDATA%\Microsoft\UserSecrets\<generated guid>\secrets.json . The contents of the file will be:

```
    {
       MyUserSecret": "ABCXYZ"
    }
```

## Completing the API
Lets pull in a value from each of the registered providers and wire up our application to respond with them. The only one we have left is Environment Variables. For this example, we'll just create one in the terminal but you can use any technique you'd like. Just remember that system level variables are read at the start of a process so you may need to restart your terminal or IDE after you create one.
 
1. Choose any existing environment variable from your system. In a powershell terminal run `Set-Location ENV:`
2. This will allow you to list the environment variables as if they were files in a folder. Now run `dir`
3. Choose any variable you'd like to add to the API's output. I'm going to choose the `TMP` variable.
4. Go back to ConfigurationController.cs and delete everything in the `Get()` method.
5. Now use one of the code snippets by typing `getall` then tab.

Your `Get()` method should look like this:


> Extra Credit

Experiment with configuration by commenting out one or more of the providers that are added in `program.cs`. What happens when you do?

Research and inject additional prodivers like the [Memory configuration provider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#memory-configuration-provider), the [Key-per-file configuration provider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#key-per-file-configuration-provider), the [Azure Keyvault configuration provider](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-3.1) and more.

Continue to [Exercise 2](./exercise_2.md)