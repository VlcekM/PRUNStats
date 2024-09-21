# PRUNStats
A Prosperous Universe stats web app.

http://prunstats.com/

## First-time setup
1. Clone the project
2. Create the DB by running the migrations in the `PRUNStatsCommon` project.
   - The design-time DbContext factory takes its' connection string from the first argument, e.g.: `dotnet ef database update -- "INSERT YOUR CONNECTION STRING HERE"`
4. Populate the DB by fetching data from the FIO API
   - Configure the connection string "StatsContext" by using your own UserSecrets file or by using environment variables
   - If needed, also overwrite the "FIOAPIURL" setting
   - Finally, run PRUNStatsSynchronizer.exe
5. Overwrite the connection string for the StatsContext in PRUNStatsApp, again by using either your own UserSecrets file or by using environment variables
