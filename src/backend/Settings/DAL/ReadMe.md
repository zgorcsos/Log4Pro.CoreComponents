We love & use EF-CF approach! Use these commands in VS-PMC window for add migration, if change the modell!
This module supports the SQLite Memory databaseb too (in separated db context), for easy unit testing include testing the store logic in context layer. So allways add all migration step into both db context! (SettingContext & SettingContextSQLite)

The correct command for SettingContext:
`
Add-Migration -Name YourMigrationName -Context SettingContext -StartupProject Log4Pro.CoreComponents -OutputDir Settings/DAL/Migrations
`

The correct command for SQLiteSettingContext:
`
Add-Migration -Name YourMigrationName -Context SettingContextSQLite -StartupProject Log4Pro.CoreComponents -OutputDir Settings/DAL/Migrations/SQLite
`