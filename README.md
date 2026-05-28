# NexusEd

NexusEd is a student feedback system for collecting, managing, and reviewing academic feedback.

## What It Includes

- Student and administrator sign-in
- Feedback category and question management
- Student feedback capture
- Reports for reviewing submitted feedback

## Run Locally

1. Open `NexusEd.sln` in Visual Studio.
2. Restore NuGet packages.
3. Create the local database with `DatabaseSetup.sql`, or add your own local database file as `App_Data/NexusEdLocal.mdf` and update `MyConnection` in `Web.config`.
4. Run the project with IIS Express.

## Screenshots

Current UI screenshots are stored in `docs/screenshots`.

Local database files and build output are intentionally not committed. `DatabaseSetup.sql` contains sample development users and seed data for local testing.
