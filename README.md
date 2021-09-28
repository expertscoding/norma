# What is **Norma**?
**Norma** is a library to help with the task of decoupling the authorization from your logic business in your apps. It also has a very flexible and granular model to control what resources your users can use.

Its name comes from the constellation with the namesake, which at the beginning was "Norma et regula", which regulate the rules.
(https://www.constellationsofwords.com/Constellations/Norma.htm & https://en.wikipedia.org/wiki/Norma_(constellation)).

# Main Concepts in **Norma**
The main concepts in norma are:

- **Resource**: Something you want to secure. Usually Views, documents or business data (we are talking about software!).
- **Action**: What you want to do to a resource. Read it, modify it...
- **Permission**: The relation of a resource with an action. Search employees or view details of an employee. This is one of the keys concepts in **Norma**.
- **Profile**: Is the role you are assigning permissions to. For example, the read profile that usually has only permissions that read data (resources).
- **Assignment**:  Is the relation between a profile and a permission. This relation can be allowing or denying it. Most of teh time, the assigment will be positive (allowing the action with the resource) but in some cases, you want to deny it. A permission of modifying employee data can be denied to a student profile in a HHRR application.
- **Policy**: Validation to carry out to a permission. The main, and virtually the only, policy is to have an allowing assigment to a permission in any of your profiles. But this concept is for extension in special cases within an application.

So, in the basic use, **Norma** is about to Know if a user (with some profiles attached) has a permission allowing an action over a resource.

Other auxiliary concepts in **Norma** are:
- **Module**: Set of resources to group them in order to facilitate their maintenance.


If we adapt the concepts to a normal LOB web application, in this case a small application to manage the projects an employee is assigned to in a consultancy firm, you will have:
- Resources: typically, each business entity is a resource. Employees, Projects...
- Actions: Search, ReadDetail, Modify, Create anddelete are the usual actions. 
- Permissions: here is the relations between every resource with every employee. Depending on other company applications, you may lack the permissions about create, modify and delete employees that will be read directly from this other sources.
- Profile: Read and Management will be the two profiles for this app.
- Assigments: Read profile only will have assigments to permissions with Search and ReadDetail actions. Management profile will have   assigment to all the permissions. All assigments allow access to resource.
- Policies: Only one policy is needed. This Policy will check that the user has a profile that has an assigment to the permission that relates the performed action over the selected resource. This policy is built-in **Norma**.

#What norma doesn´t do

**Norma** doesn´t care about Authentication, so has no methods to know the user that is acceding to your application. You have to rely on the Authentication middleware provided by Microsoft or another third party.

#What Norma actually do

Once you have an Authenticated user, Norma validates if it is allowed to access the resource that is requested.

In .Net Core terms, is a middleware that goes after the authentication middleware.

# How to use it?
This section is for a typical .Net MVC application (it doesn´t matter if it is a Rest Api or an application with server side rendering views) that has controllers an actions within them.

##Download the nuget package

First of all, you have to download the nuget package.

`$ dotnet add package Norma`

##Using Norma in your application
In the ConfigureService method, in your startup.cs file:

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddNorma();
        }

In the Configure method:

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

Take notice that _UseNorma_ should always be called after _UseAuthenticaton_. In addition, you can call UseAuthorization to validate  standard .Net Policies, as always.

## Norma options
In the method _AddNomra_ you can configure some defaults responses from Norma.

- NoPermissionAction: What Norma will do when no permissions are found for a web request. There are two possible values for this behabiour represented in the NoPermissionsBehaviour enum: Failure and Success (default). 

- MissingRequirementAction: What Norma will do when no requirement class is found through reflection. The requeriment class is the foundational class in .Net Core Authorization system. Each Policy in **Norma** is traduced in, at least, one requirement that is loaded onto memory through reflection. The bahavior is marked by the members of the enum _MissingRequirementBehaviour_ : ThrowException (dafault) or LogObnly.

##Configure Permission repository

Before you can use **Norma**, you have to make an important decision, where to save the Permission data that **Norma** use to validate user`s access.
-  **Json**: You can have a section in the appsettings.json file to configure permissions. This is only recommended for demo purposes in this stage, because for simplification, json setup only recognizes profiles, permissions and assignments. No resources nor actions nor policies are configured.
To use this configuration you have to add the provider in _ConfigureServices_ method:

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddNorma(opt =>
            {
                opt.MissingRequirementAction = MissingRequirementBehaviour.LogOnly;
            })
            .AddNormaJson(Configuration.GetSection("profiles").Get<List<Profile>>())
            ;
        }

_AddNormaJson_ admits _Profile_ list (as they are define in _EC.Norma.Domain_ assembly.

The _Profiles_ section is configured like this:

  
```
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

As you can see, each profile has a name and a permission list. Only names are needed here. This simplifies the configuration in json format, but it has the drawback of no detailed configuration for Controllers and actions.

- **DataBase**: This is the repository intended for production usage. This provider will admit any database that has an Entity Frameowrk Core connector. The tables can be created through the dacpac file in the nuget Package.
This file is only for SqlServer. You have to adapt it for other database engines.

To use this provider add it in the _ConfigureServices_ method:

```
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddNorma(opt =>
            {
                opt.MissingRequirementAction = MissingRequirementBehaviour.LogOnly;
            })
            .AddNormaEF(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("Normadb"));
            });
        }
``` 

##Working with Norma

For each request in your MVC application, Norma will try to acquire a resource and action or, alternatively, a Permission.

- Resource: There are two ordered ways to indicate the resource a request is trying to access:
1. Attribute: If the method (the action in MVC terminology) has an _NormaResourceAttribute_ the resource will be whatever is indicated there
``` 
[NormaResource("Employees")]
```
2. Controller Name: as it is calculated through routing patterns in MVC

- Action: As with resources, for a request, action is calculated:
1. Attribute: If the method to access has a _NormaActionAttribute_, that defines the action.
```
[NormaAction("List")]
```
2. Action Name: as it is calculated through routing patterns in MVC

- Permission: Alternatively, you can assign directly a Permission name to an MVC action with the _NormaPermissionAttribute_
```
[NormaPermission("ListEmployees")]
```

The name of a permission differs from the name of actions and resources and it`s recommended that a permission has a name that let you easily identify the resource and action, but due a permission can have more than one action, it will by difficult in some cases.


With a pair resource-action or a Permission Norma searchs for the correspondent permissions in repository and contruct the .Net Core Policy that the request has to accomplish. If no Permission is found, Norma will grant or deny access trough NoPermissionAction setup.

If any permission is found, Norma search if any of the user`s profiles has that permission within the allowed ones and them calls to .Net Authorization engine to resolve if teh user fulfill the requirements.

Out of the box, Norma only checks if a user has a permission (if any of the profiles of an user has the permission) with the built-in Requirement _HasPermissionRequirement_

#Extending Norma
Currently, only with database provider is possible to create your own Requirements and RequirementHandlers (as is described in Microsoft [documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-5.0) to be used by **Norma**

You have to inherit from _NormaRequirement_ and Resource and Action are passed by **Norma**.

To join your implementation with **Norma**, you have to populate your requirement class name in Policy Table and relate your policy with an action.

But, in this version, in order to make it work, you have to include your requirement in _EC.Norma.Core_ spacename. This will be modified in future version to let you specify the namespace in Policy table.

In order to evaluate the requirement, you have to register the requirement handler as usually in .Net. 
```
services.AddTransient<IAuthorizationHandler, HasPermissionHandler>();
```

#Future features

We have a lot of plans about **Norma** and we will very pleased to hear from you about your desires about **Norma** evolution.

In the near future, we plan to:
- Support OAuth2 to extend authorization in M2M (Integrated with IdServer)
- Cache (for reflection & queries)
- Use with standard ASP.Net  Core Attributes
- Improve logging
- .NET 6.0 Support
