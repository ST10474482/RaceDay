RaceDay Event Management System

RaceDay is an event management system designed to manage race events, participant enrolments, organisers, categories, and race results. The system is developed using C#, ASP.NET Core Web API, Entity Framework Core, and Microsoft SQL Server.

The project is divided into two parts: system planning and database design, followed by application development and API implementation.

Part 1: System Planning and Database Design

Part 1 focuses on planning the RaceDay system and designing its database before application development begins.

Planning Documents

The following planning documents were created:

Entity Relationship Diagram (ERD): Defines the database entities, relationships, and data structure.
API Endpoint Plan: Outlines the API routes and operations required by the system.
SQL Server Database Script: Defines the database structure and tables required to store application data.
System Roles: Defines the permissions and responsibilities of Organisers and Participants.
System Roles and Responsibilities
Organiser

An Organiser is responsible for managing race events and related information.

Organisers can:

Create, edit, and delete events.
Manage event categories.
View participant enrolments.
Capture and manage participant results.
Manage organiser information.
Participant

A Participant can register for events and manage their personal information.

Participants can:

Create an account and log in.
Browse available race events.
View event details.
Enrol in events.
View personal enrolments.
View personal race results.
Update personal profile information.
Part 2: Application Development and API Implementation

Part 2 focuses on implementing the planned RaceDay system as a functional ASP.NET Core Web API.

Technologies Used
C# — Application programming language.
ASP.NET Core Web API — Backend API development.
Entity Framework Core — Database access and object-relational mapping.
Microsoft SQL Server / LocalDB — Relational database storage.
Swagger / OpenAPI — API documentation and endpoint testing.
Visual Studio / .NET CLI — Application development and execution.
System Features

The application provides the following functionality:

User registration, login, and logout.
Role-based access for Organisers and Participants.
User profile management.
Organiser profile management.
Race event and category management.
Participant event enrolments.
Participant enrolment viewing.
Race result capture and retrieval.
Database integration through Entity Framework Core.
API documentation and testing through Swagger.
API Endpoints

The API is organized into the following route groups:

Endpoint	Description
/api/Auth	User registration, login, and logout
/api/Profile	Participant profile management
/api/Organizers	Organiser information management
/api/Categories	Race category management
/api/Events	Race event management
/api/Enrollments	Participant enrolment management
/api/Results	Race result management

The available HTTP methods, request parameters, and response formats can be inspected through Swagger/OpenAPI.

Database Implementation

The system uses Microsoft SQL Server with Entity Framework Core to store and manage application data.

The database supports the information required for:

User accounts and roles.
Organiser profiles.
Race categories.
Race events.
Participant enrolments.
Race results.

The database connection is configured through the DefaultConnection setting in appsettings.json. Entity Framework Core migrations can be used to apply database schema changes.

Getting Started
Prerequisites

Ensure the following are installed:

.NET SDK compatible with the project.
Visual Studio or another .NET-compatible development environment.
Microsoft SQL Server or SQL Server LocalDB.
Git, if cloning the repository.
1. Clone the Repository
bash
git clone <repository-url>
cd RaceDay


Replace <repository-url> with the actual repository URL.

2. Configure the Database

Update the DefaultConnection connection string in appsettings.json to point to your SQL Server database.

3. Restore Dependencies
bash
dotnet restore

4. Apply Database Migrations

If the project uses Entity Framework Core migrations, run:

bash
dotnet ef database update

5. Run the Application
bash
dotnet run


Alternatively, run the project through Visual Studio.

6. Access Swagger

Open the Swagger UI using the application URL and port displayed in the console:

text
https://localhost:<port>/swagger


Swagger can be used to explore and test the available API endpoints.

Conclusion

The RaceDay Event Management System combines system planning, database design, and backend application development into a single project.

Part 1 establishes the system requirements, user roles, database structure, and API plan. Part 2 implements the planned functionality using ASP.NET Core Web API, Entity Framework Core, and SQL Server.

Together, these parts provide the foundation for managing race events, participant enrolments, organisers, categories, and race results through a structured and maintainable system.
