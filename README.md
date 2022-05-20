# What is **Norma**?

**Norma** is a .NET 6 library to help with decoupling the authorization from your logic business in your applications, in a centralized way because it has multi-application support. It also has a very flexible and granular model to control what resources your users can use.

Its name comes from the constellation with the namesake, which at the beginning was "Norma et regula", which regulate the rules.
(https://www.constellationsofwords.com/Constellations/Norma.htm & https://en.wikipedia.org/wiki/Norma_(constellation)).

# Main Concepts in **Norma**

The main concepts in Norma are:

- **Application**: just this, is an application registered with a code capable to use norma as the source of its authorization.
- **Resource**: Something you want to secure. Usually, views, documents, or business data (we are talking about software!).
- **Action**: What you want to do with a resource. Read it, modify it...
- **Permission**: The relation between a resource and an action. To search employees, to view details of an employee, etc. This is one of the key concepts in **Norma**.
- **Profile**: The role you are assigning permissions to. For example, the reader profile usually has only permissions to read data (resources).
- **Assignment**:  Is the relation between a profile and a permission.
- **Requirement**: Validation to carry out to permission. The main, and virtually the only, requirement is to have an allowing assignment to permission in any of your profiles. But this concept is for extension in special cases within an application.

So, in the basic use, **Norma** is about knowing if a user (with some profiles attached) has permission to allow an action over a resource.

Other auxiliary concepts in **Norma** are:

- **Module**: a set of resources grouped to facilitate their maintenance. A module is always dependent on an Application.


If we take norma concepts into a normal LOB web application, as a small application to manage employee's asignemets to projects in a consultancy firm, for example, we will have:
- Resources: typically, each business entity is a resource. Employees, Projects...
- Actions: Search, ReadDetail, Modify, Create and Delete are typical actions.
- Permissions: the relations between every resource with every employee. Depending on other company applications, you may lack the permissions to create, modify and delete employees that will be read directly from these other sources.
- Profile: Reader and Manager would be two valid profiles for this app.
- Assignments: Reader profile only will have assignments to permissions with Search and ReadDetail actions. Manager profile will have an assignment to all the permissions. All assignments allow access to a resource.
- Requirements: Only one requirement is needed. This Requirement will check that the user has a profile that has an assignment to the permission that relates to the performed action over the selected resource. This requirement is built-in into **Norma**.

# What Norma doesn't do

**Norma** doesn't care about Authentication, so has no methods to know what user is accessing your application. You have to rely on the Authentication middleware provided by Microsoft or any other third party.

# What Norma actually do

Once you have an Authenticated user, Norma validates if it is allowed to access the resource that is requested.

In .NET terms, is a middleware that goes after the authentication middleware.

# How to use it?

This section is for a typical .NET MVC application (it doesn't matter if it is a Rest API or an application with server-side rendering views) that has controllers and actions within them.

## Download the nuget package

First of all, you have to download the nuget package.

`$ dotnet add package Norma`

## Configuring Norma in your application

In the ConfigureService method, in your Startup.cs or Program.cs file add the following registration:

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNorma(options => options.ApplicationKey = "ApPK3y");
        }
```

Replace the ApplicationKey value with the one generated for your Application definition.
In the Configure method:

```csharp
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env))
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseNorma();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
```

Notice how `UseNorma` should always be called after `UseAuthenticaton`. In addition, you can call `UseAuthorization` to validate standard .NET Policies, as always.

## Norma options

In the method `AddNorma` you can configure some default responses from Norma.

- **NoPermissionAction**: What Norma will do when no permissions are found for a web request. There are two possible values for this behavior represented in the NoPermissionsBehaviour enum: Failure and Success (default).

- **MissingRequirementAction**: What Norma will do when no requirement class is found through reflection. The requirement class is the foundational class in .NET Authorization system. Each Policy in **Norma** is traduced in, at least, one requirement that is loaded onto memory through reflection. The behavior is marked by the members of the enum `MissingRequirementBehaviour`: ThrowException (default) or LogOnly.

## Configure Permission repository

We provide two flavors repositories where to save the Permission data that **Norma** uses to validate the user's access:

- **JSON**: You can have a section in the appsettings.json file to configure permissions. This is only recommended for demo purposes because of its simplicity. It has some limitations: JSON setup only recognizes profiles, permissions, and assignments. No resources, actions, or policies are configured.
To use this configuration you have to add the provider in the `ConfigureServices` method:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddNorma(options =>
        {
            options.MissingRequirementAction = MissingRequirementBehaviour.LogOnly;
        }
    ).AddNormaJson(Configuration.GetSection("profiles").Get<List<Profile>>());
}
```

`AddNormaJson` receives a `Profile` list (as they are defined in `EC.Norma.Domain` assembly.

An example of the _Profiles_ section for the appsettings.json file is configured like this:

```json
"profiles": [
    {
      "name": "Administrator",
      "Permissions": [ "ListarS", "ConsultarS", "EditarS" ]
    },
    {
      "name": "User",
      "Permissions": [ "ListarS", "ConsultarS" ]
    }
  ]
```

As you can see, each profile has a name and a permission list. Only names are needed here. This simplifies the configuration in JSON format, but it has the drawback of no detailed configuration for Controllers and Actions.

- **Database**: This is the repository intended for production usage. This provider will admit any database that has an Entity Framework Core connector. You'll find a dacpac file within the nuget Package to generate the appropriate schema in SqlServer only. You have to adapt it for other database engines.

To use this provider add it in the `ConfigureServices` method:

```csharp
public void ConfigureServices(IServiceCollection services)
{

    services.AddNorma(options =>
    {
        options.MissingRequirementAction = MissingRequirementBehaviour.LogOnly;
    })
    .AddNormaEF(options =>
        options.UseSqlServer(Configuration.GetConnectionString("Normadb"));
    });
}
```

## Working with Norma

You have different options to model the authorization in your application:

- By default, Norma will try to guess the resource and action from the MVC Controller and Action, then, will look for permission in the repository that matches its names.
- Using Norma attributes at Controller and/or Action level: In case you want to define an Action or Resource name not suitable for your class, you can use `NormaResourceAttribute` or `NormaActionAttribute` to define the names desired. i.e.:

```csharp
[NormaResource("Employees")]
public class Empleados : Controller
{
    [NormaAction("List")]
    public List<Employee> Get() { ... }
}
```

- Using Permission attribute: at the method level within a controller, you can use `NormaPermissionAttribute` to precisely defined the permission name to look for in the repository. i.e

```csharp
public class Employees : Controller
{
    [NormaPermission("GetEmployeeDetails")]
    public List<Employee> Get(int id) { ... }
}
```

The name of permission differs from the name of actions and resources and it`s recommended that permission has a name that let you easily identify the resource and action, but due permission can have more than one action, it will be difficult in some cases.

With a resource-action pair or a Permission, Norma search for the corresponding permissions in the repository and construct the .Net Core Policy that the request must achieve. If no Permission is found, Norma will grant or deny access based on the `NoPermissionAction` property settled during setup.

If no permission is found, **Norma** searches if any of the user's profiles have that permission within the allowed ones and then calls to the .NET Authorization engine to resolve if the user fulfills the requirements.

## Requirements

**Norma** has two built-in requirements that can be combined to fulfill most of the cases in authorization scenarios:

- HasPermission: Its handler just checks that any role of the user matches with any of the profiles assigned to the Permission being evaluated.
- IsAdmin: the handler checks that the user has the role Administrator. This value is configurable through Startup options with `NormaOptions.AdministratorRoleName`.

Requirements in **Norma** can be grouped into the entity PriorityGroup where we can assign an ordered priority to one or more requirements.
These are evaluated with the AND logical operator for all within the same priority group, then OR logical operator is used to evaluate between different PriorityGroups.

For example, we can define a top priority group with the IsAdmin requirement, then add another priority group with HasPermissions to evaluate the remaining profiles.
If an Admin accesses the application, the top requirement will be evaluated, and nothing else, don't take care if it has other roles.
If a not Admin user accesses the application, the IsAdmin check will fail, but then **Norma** continues checking other requirements looking for the profile of the action among its roles.

# Extending Norma

Currently, only with a database provider is possible to create your Requirements and RequirementHandlers (as is described in Microsoft [documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies)) to be used by **Norma**

To create your customized requirement you have to create two classes,
one that inherits from `NormaRequirement` and another that inherits from  `AuthorizationHandler<TRequirement>` and implements the method `HandleRequirementAsync`. Resource and Action are filled by **Norma** when needed.
> Currently, all customs requirements should be defined in the namespace `EC.Norma.Core`. This will be configurable in future versions.

i.e.

```csharp
public class HasLegalAgeRequirement : NormaRequirement { ... }

public class HasLegalAgeHandler : AuthorizationHandler<HasLegalAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasLegalAgeRequirement requirement)
    { /* Your logic implementation */ }
}
```

Finally, to **Norma** can evaluate your requirement, you have to register the requirement handler as usually in the Startup .NET code.

```csharp
services.AddTransient<IAuthorizationHandler, HasLegalAgeRequirement>();
```

In order to **Norma** can be aware of your implementation, you have to populate the Requirements table with your requirement class name, and relate your requirement with an action.

# Future features

We have many ideas for **Norma** next steps, but we are also looking forward to receive comments and ideas from you all.


In the near future, we plan to:

- Support OAuth2 to extend authorization in M2M (Integrated with Identity Server)
- Use with standard ASP.Net Core Attributes
