# Entity Framework Deep Dive

Demo code for my workshop "Entity Framework Deep Dive"

## Getting started

Open [`FS.EntityFramework.DeepDive.sln`](https://github.com/fschick/EntityFramework.DeepDive/tree/main/source/backend) and configure databases in [`appsettings.json`](https://github.com/fschick/EntityFramework.DeepDive/blob/main/source/backend/FS.EntityFramework.DeepDive/appsettings.json).

Start the project `FS.EntityFramework.DeepDive`. No compilation of the front-end required, a compiled version is also checked in.

## Points of Interest

Methods of `BlogController.cs` and subordinates [CRUD operations](https://github.com/fschick/EntityFramework.DeepDive/blob/main/source/backend/FS.EntityFramework.DeepDive/Controllers/BlogController.01_Crud.cs), [JSON Columns](https://github.com/fschick/EntityFramework.DeepDive/blob/main/source/backend/FS.EntityFramework.DeepDive/Controllers/BlogController.02_Json.cs), [SQL Queries](https://github.com/fschick/EntityFramework.DeepDive/blob/main/source/backend/FS.EntityFramework.DeepDive/Controllers/BlogController.03_SqlQueries.cs), [Interception](https://github.com/fschick/EntityFramework.DeepDive/blob/main/source/backend/FS.EntityFramework.DeepDive/Controllers/BlogController.04_Interception.cs), [Functions](https://github.com/fschick/EntityFramework.DeepDive/blob/main/source/backend/FS.EntityFramework.DeepDive/Controllers/BlogController.05_Functions.cs), [SQL Expressions](https://github.com/fschick/EntityFramework.DeepDive/blob/main/source/backend/FS.EntityFramework.DeepDive/Controllers/BlogController.06_SqlExpressions.cs) where you can modify and play with.

Model configuration in [`DeepDiveDbContext`](https://github.com/fschick/EntityFramework.DeepDive/blob/f8cf9260acffb2a4c0c865c226083c67002ec64c/source/backend/FS.EntityFramework.DeepDive.Repository/DbContexts/DeepDiveDbContext.cs#L144)

## License
MIT

