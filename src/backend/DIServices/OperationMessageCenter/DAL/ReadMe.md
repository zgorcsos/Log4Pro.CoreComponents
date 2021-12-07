We love & use EF-CF approach! Use these commands in VS-PMC window for add migration, if change the modell!
This module supports the SQLite Memory databaseb too (in separated db context), for easy unit testing include testing the store logic in context layer. So allways add all migration step into both db context! (OperationMessageCenterContext & OperationMessageCenterContextSQLite)

The correct command for OperationMessageCenterContext:
`
Add-Migration -Name YourMigrationName -Context OperationMessageCenterContext -StartupProject Log4Pro.CoreComponents -OutputDir DIServices/OperationMessageCenter/DAL/Migrations
`

The correct command for OperationMessageCenterContextSQLite:
`
Add-Migration -Name YourMigrationName -Context OperationMessageCenterContextSQLite -StartupProject Log4Pro.CoreComponents -OutputDir DIServices/OperationMessageCenter/DAL/Migrations/SQLite
`