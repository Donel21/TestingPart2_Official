# TestingPart2_Official


RaceDay – Event Management API (Part 2)

  Introduction
RaceDay is a robust RESTful Web API built with ASP.NET Core (.NET 8) designed to streamline the management of sports events, categories, and participant enrollments. This project represents Part 2 of the Programming 2B assignment (ST10434784). It focuses on the data layer, relational database integration, role-based security, and unit testing.

The API provides secure endpoints for two distinct types of users:
Organisers: Can create and manage sporting events, set up race categories, and record participant results.Participants:** Can browse events, enroll in specific race categories, and view their personal race results.

 Core Functionality

Authentication & Authorization (JWT):** Secure login and registration system using JSON Web Tokens (JWT) and BCrypt password hashing. Role-based access control ensures participants cannot create events, and organisers cannot enroll.
Role Management: A standard registration endpoint creates *Participant* accounts, while a secured `/register-organiser` endpoint requires a secret key to create *Organiser* accounts.
Event & Category Management (CRUD): Organisers have full control to create, update, and delete their events and the corresponding categories (e.g., 10km Run, 40km Cycle).
Enrollment System: Participants can enroll in available categories, with built-in constraints preventing duplicate enrollments.
Result Tracking: Organisers can log finish times and positions for participants, which participants can then view via their personal profile.
Unit Testing: The solution includes an `xUnit` testing project (`testing_endPoints`) to validate core logic and authentication flows.

Technologies & Tools Used

Framework: ASP.NET Core Web API (.NET 8)
Database:SQL Server (LocalDB) / Entity Framework Core (Code-First Migrations)
Security: JWT Bearer Authentication, BCrypt.Net-Next
Documentation: Swagger / OpenAPI
Testing: xUnit, In-Memory SQLite

 How to Run the Project

1. Clone the repository and open `TestingPart2_Official.sln` in Visual Studio 2022.
2. Ensure the `appsettings.json` connection string points to your local SQL Server instance.
3. Open the **Package Manager Console** and run the database migrations:
   ```powershell
   Update-Database
   ```
4. (Optional) Run the provided `race_day_progA1.sql` script in SQL Server Management Studio to seed initial test data.
5. Press *F5 to launch the application. Swagger UI will automatically open in your browser, allowing you to test the API endpoints.

 Project Structure
*   `Controllers/` - API endpoints handling HTTP requests (Auth, Events, Enrollments, etc.)
*   `Models/` - Database entities (User, Event, Category, Enrollment, Result, Route)
*   `Data/` - Entity Framework `ApplicationDbContext` and configurations
*   `DTOs/` - Data Transfer Objects for clean request/response formatting
*   `Helpers/` - JWT Token generation logic
*   `testing_endPoints/` - xUnit Unit Testing Project
 Video Demonstration
A brief video walking through the API functionality, Swagger UI, and role-based security is available here: 
*(Paste your YouTube link here)*
