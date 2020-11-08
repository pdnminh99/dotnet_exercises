
# Author

- Name: Pham Do Nhat Minh
- StudentId: 217 2259

# Boot up project

Restore dependencies

`dotnet restore`

Install dotnet ef tools

`dotnet tool install --global dotnet-ef`

Install code gen tool

`dotnet tool install -g dotnet-aspnet-codegenerator`

# Run project as DB first

Scaffold the existing database (dbFirst.db) to models & db context.

`dotnet ef dbcontext scaffold "DataSource=dbFirst.db" Microsoft.EntityFrameworkCore.Sqlite -o Models`

# Run project as Code first

## Init & Build

Existing codes are located in Program.cs. Schema initialisation is in `Migrations` folder.
To generate Migrations from start.

`dotnet ef migrations add [SchemaName]`

Build up `codefirst.db` from Migrations

`dotnet ef database update`

## Drop & Remove

Drop `codefirst.db`

`dotnet ef database drop`

Remove existing migrations

`dotnet ef migrations remove`
