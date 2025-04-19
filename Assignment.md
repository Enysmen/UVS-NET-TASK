# Build a set of services to get and set employees

## Task

This task is to create a simple console app to `get` and `set` employee data using ORM (e.g. Entity Framework).

For more information
 * https://en.wikipedia.org/wiki/Object%E2%80%93relational_mapping
 * https://learn.microsoft.com/en-us/ef/

The app should be built in .NET 6.0

e.g.

* dotnet run set-employee --employeeId 5 --employeeName Steve --employeeSalary 123
* dotnet run get-employee --employeeId 5

## Note

If you are facing any difficulties with this task, feel free to finish task as best as you can, just state the issues faced in your submission.

## Database

There are tools included to set up and tear down a postgres database using docker:

 * Setup database: ./setUpDatabase.ps1
 * Database schema: ./DatabaseSchema/dbSchema.sql

If you choose to use a different database, that is fine, just include details in your submission.

## Interface parameters

These are the methods that the console app must support, along with the arguments that are required.

### get-employee

 1. --employeeId, INT, Mandatory

### set-employee

 1. --employeeId, INT, Mandatory
 2. --employeeName, STRING Mandatory
 3. --employeeSalary, INT Mandatory

## Testing your submissions

 * The script ./verifySubmission.ps1 can verify that functionality is complete
 
