# NexusEd

NexusEd is an ASP.NET Web Forms student feedback system for collecting, managing, and reviewing academic feedback.

## What it includes

- Student and administrator sign-in flow
- Feedback category and question management
- Student feedback capture
- Reporting pages for reviewing submitted feedback

## Tech Stack

- ASP.NET Web Forms
- C#
- SQL Server LocalDB
- Entity Framework 6
- .NET Framework 4.7.2

## Run Locally

1. Open `NexusEd.sln` in Visual Studio.
2. Restore NuGet packages.
3. Add your own local database as `App_Data/NexusEdLocal.mdf` or update `MyConnection` in `Web.config`.
4. Run the project with IIS Express.

Local database files and credentials are intentionally not committed.
