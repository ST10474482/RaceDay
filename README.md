RaceDay API

RaceDay is an ASP.NET Core Web API for managing race events, participant registrations, organizers, categories, and race results. It uses Entity Framework Core and SQL Server for data storage.

Features
User registration and login
Participant and Organizer roles
Profile management
Event and category management
Event enrollment
Race result management
Session-based access control
Technologies
C# and ASP.NET Core
Entity Framework Core
SQL Server LocalDB
Swagger / OpenAPI
Getting Started
Clone or download the repository.
Configure the DefaultConnection connection string in appsettings.json to point to your SQL Server database.
Run the application from Visual Studio or with dotnet run.
Open /swagger in your browser to test the API endpoints.
API Routes
/api/Auth — Registration, login, and logout
/api/Profile — User profile management
/api/Organizers — Organizer profiles
/api/Categories — Race categories
/api/Events — Race events
/api/Enrollments — Participant enrollments
/api/Results — Race results
Database

The application uses SQL Server through Entity Framework Core. Ensure the connection string is configured correctly before running the API.
