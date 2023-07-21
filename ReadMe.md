
# Cleanception

is a Clean Architecture designed for ASP.NET Core based on .NET 7 and Full-Stack hero respository, with more features and changes



## Features and changes

- Multilevel search: Cleanception incorporates a multilevel search feature that simplifies keyword searches within the current entity and related entities. This functionality enables developers to search for specific keywords across multiple levels of the application's data structure. By providing a comprehensive search experience, users can retrieve relevant information efficiently, improving the overall usability of the application.


- Firebase support: Cleanception integrates with Firebase, This allows cms/dashboard admin to send notifications to users based on events, schedules, or user parameters, providing a highly customizable and targeted notification experience.

- Twilio support for SMS

## Getting Started
- Change the domain database connection string in ___database.json___

- Change the hangfire connection string in ___hangfire.json___, __YOU MUST CRAETE THE HANGFIRE DATABASE MANUALLY.__ 

- Run the __Host__ project and you are done !
## Migrations commands

Some important migration commands

```csharp
add-migration Init -Context Cleanception.Infrastructure.Persistence.Context.ApplicationDbContext -o Migrations\Application

update-database  -Context Cleanception.Infrastructure.Persistence.Context.ApplicationDbContext

remove-migration -Context Cleanception.Infrastructure.Persistence.Context.ApplicationDbContext
  
```

