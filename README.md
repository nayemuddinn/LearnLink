# LearnLink

![ASP.NET MVC](https://img.shields.io/badge/ASP.NET%20MVC-5.2-blue?style=flat-square)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-informational?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL%20Server-MSSQL-red?style=flat-square)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

A role-based Learning Management System for course delivery, enrollment, and assessment, built on ASP.NET MVC 5 and SQL Server.

[Overview](#overview) · [Features](#features) · [Tech Stack](#tech-stack) · [Architecture](#architecture) · [Database Schema](#database-schema) · [API Reference](#api-reference) · [Getting Started](#getting-started) · [Configuration](#configuration) · [Security](#security) · [Testing](#testing) · [Contributing](#contributing) 

---

## Overview

LearnLink is a full-stack Learning Management System that gives teachers a single place to create courses, publish materials, run quizzes, and track student performance, while giving students a self-service flow for discovering courses, submitting coursework, and monitoring their own progress.

The application is organized around two account roles — Teacher and Student — enforced end to end through custom authorization attributes, server-side validation, and role-scoped database queries.

Key characteristics:

- Role-based access control enforced on every protected action
- Email-verified registration and token-based password recovery
- File-backed course material storage (PDF, images, documents) held as binary data in SQL Server
- Timed, auto-graded multiple-choice quizzes with a one-active-attempt rule
- ADO.NET data access using parameterized queries throughout

---

## Features

### Teacher

| Feature | Description |
|---|---|
| Account management | Register, log in, edit profile, change credentials |
| Course management | Create, update, and delete courses; view enrollments |
| Materials | Upload, list, download, and remove course files |
| Enrollment control | Approve or reject enrollment requests; remove enrolled students |
| Quizzes | Create quizzes, add multiple-choice questions, toggle active status |
| Analytics | View submission counts, average scores, and pass/fail rates |
| Feedback | Provide feedback on individual quiz submissions |
| Dashboard | Course, student, and quiz summary at a glance |

### Student

| Feature | Description |
|---|---|
| Account management | Register, log in, edit profile, change credentials |
| Course discovery | Browse and search available courses |
| Enrollment | Request enrollment, track status, unenroll |
| Materials | Download materials for approved courses |
| Quizzes | Take timed quizzes, limited to one active attempt at a time |
| Results | View scores and teacher feedback |
| Dashboard | Enrolled courses, pending quizzes, and progress summary |
| Password recovery | Reset password via emailed link |

### Platform-wide

| Feature | Description |
|---|---|
| Email verification | Required before first login |
| Session management | Cookie-backed session with optional persistent login |
| Password security | Bcrypt hashing; minimum length, digit, and special-character rules |
| SQL injection protection | Parameterized queries on all data access paths |
| Authorization | Custom `CustomAuthorize` attribute for role-scoped controller actions |
| Error handling | Centralized exception handling with user-facing feedback |

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET MVC 5.2 |
| Runtime | .NET Framework 4.7.2 |
| Language | C# |
| Database | Microsoft SQL Server (Express or higher) |
| Data access | ADO.NET (`System.Data.SqlClient`), parameterized queries |
| Views | Razor (`.cshtml`) |
| Frontend | HTML5, CSS3, Bootstrap 5.x, jQuery 3.7.0 |
| Email | SMTP via Brevo |
| Local server | IIS Express |

---

## Architecture

### Repository Structure

```
LearnLink/
├── Controllers/
│   ├── Account/          Authentication, registration, credential recovery
│   ├── Courses/          Course creation and management
│   ├── Enrollment/       Enrollment requests and approval
│   ├── Quizzes/          Quiz creation, attempts, and analytics
│   ├── Teachers/         Teacher dashboard and profile
│   ├── Students/         Student dashboard and profile
│   ├── Feedback/         Feedback submission and review
│   ├── Misc/             Home, dashboard routing, assessment utilities
│   ├── CookieAuthenticationFilter.cs
│   └── CustomAuthorizeAttribute.cs
├── Models/                 POCO data models (User, Course, Enrollment, Quiz, ...)
├── Views/                   Razor templates, organized by controller
├── Content/                 Stylesheets, DB connection, email service, static assets
├── Scripts/                 Bootstrap, jQuery, and client-side validation
├── App_Start/                Route, filter, and bundle configuration
├── Database/schema.sql       Database initialization script
├── Web.config                 Application configuration
└── LearnLink.csproj
```

### Request Lifecycle

```
Browser
  -> Routing (RouteConfig.cs)
  -> Controller action (CustomAuthorizeAttribute enforces role access)
  -> Business logic and validation
  -> ADO.NET data access (SqlConnection / SqlCommand / SqlDataReader)
  -> SQL Server
  -> Model populated from query results
  -> Razor view rendering (HTML + Bootstrap)
  -> Browser
```

---

## Database Schema

### Entity Overview

```
student / teacher              Enrollment                     Courses
  UserID (PK)          <----   StudentID/TeacherID (FK) ---->   CourseID (PK)
  Email (unique)                CourseID (FK)                    TeacherID (FK)
  Phone (unique)                 Status                           courseMaterials (FK: CourseID)
  IsVerified
                                 Quiz (FK: CourseID, TeacherID)
                                    |
                                    v
                               QuizQuestions (FK: QuizID, cascade delete)
                                    |
                                    v
                               QuizEvaluation (FK: QuizID, StudentID)
```


```
## API Reference

All endpoints are ASP.NET MVC controller actions. Unless noted, responses are HTML views or redirects rather than JSON.

### Authentication and account

| Method | Route | Description | Access |
|---|---|---|---|
| POST | `/Login/Login` | Authenticate and start a session | Public |
| POST | `/Registration/reg` | Register as teacher or student | Public |
| GET | `/Logout/Logout` | End the current session | Authenticated |
| POST | `/forgotPassword/forgotPassword` | Send a password reset email | Public |
| POST | `/changeCredentials/changeCredentials` | Change password | Authenticated |

### Courses

| Method | Route | Description | Access |
|---|---|---|---|
| GET | `/AllCourse/AllCourse` | List and search all courses | Public |
| GET / POST | `/CreateCourse/CreateCourse` | Display form / create a course | Teacher |
| GET | `/ManageCourse/ManageCourse` | List a teacher's own courses | Teacher |
| POST | `/ManageCourse/UpdateCourse` | Update a course | Teacher |
| POST | `/ManageCourse/DeleteCourse` | Delete a course | Teacher |

### Course materials

| Method | Route | Description | Access |
|---|---|---|---|
| GET | `/CourseMaterials/CourseMaterials` | List materials for a course | Authenticated |
| POST | `/CourseMaterials/UploadMaterial` | Upload a file (PDF, image, document) | Teacher |
| GET | `/CourseMaterials/DownloadMaterial` | Download a file | Authenticated |
| POST | `/CourseMaterials/DeleteMaterial` | Delete a file | Teacher |

### Enrollment

| Method | Route | Description | Access |
|---|---|---|---|
| GET / POST | `/enrollCourses/EnrollCourse` | Display form / request enrollment | Student |
| GET | `/StudentEnrolledCourses/ViewEnrolledCourses` | List approved courses | Student |
| POST | `/StudentEnrolledCourses/Unenroll` | Withdraw from a course | Student |
| GET | `/enrollStudents/enrollNewStudents` | List pending enrollment requests | Teacher |
| POST | `/enrollStudents/ApproveEnrollment` | Approve a request | Teacher |
| POST | `/enrollStudents/RejectEnrollment` | Reject a request | Teacher |
| POST | `/enrollStudents/RemoveStudent` | Remove an enrolled student | Teacher |

### Quizzes

| Method | Route | Description | Access |
|---|---|---|---|
| GET / POST | `/CreateQuiz/CreateQuiz` | Display form / create a quiz | Teacher |
| GET / POST | `/CreateQuiz/UploadQuiz` | Display form / add a question | Teacher |
| GET | `/TeacherViewQuizzes/ViewQuizzes` | List quizzes created by the teacher | Teacher |
| GET | `/StudentViewQuizzes/ViewQuizzes` | List quizzes available to the student | Student |
| GET | `/StudentViewQuizzes/StartQuiz` | Start an attempt | Student |
| POST | `/StudentViewQuizzes/SubmitQuiz` | Submit answers for auto-grading | Student |
| GET | `/QuizAnalytics/index` | View submission count, average score, pass rate | Teacher |
| POST | `/QuizAnalytics/ProvideFeedback` | Attach feedback to a submission | Teacher |

### Dashboards, profiles, and feedback

| Method | Route | Description | Access |
|---|---|---|---|
| GET | `/TeacherDashboard/Dashboard` | Teacher summary view | Teacher |
| GET | `/StudentDashboard/Dashboard` | Student summary view | Student |
| GET / POST | `/EditTeacherProfile/editteacherProfile` | View / update teacher profile | Teacher |
| GET / POST | `/EditStudentProfile/editStudentProfile` | View / update student profile | Student |
| GET / POST | `/giveFeedback/giveFeedback` | Display form / submit feedback | Authenticated |
| GET | `/seeFeedback/seeFeedback` | View submitted feedback | Teacher |

### Business rules enforced server-side

- An enrollment request cannot be submitted twice for the same course.
- A quiz can be attempted once per student; resubmission is blocked.
- Only one quiz may be active for a student at a time.
- Deleting a quiz cascades to its questions (`ON DELETE CASCADE`).
- Quiz answers are graded automatically on submission; scores and timestamps are recorded in `QuizEvaluation`.

---

## Getting Started

### Prerequisites

- Visual Studio 2022 or later, with the ASP.NET and web development workload
- .NET Framework 4.7.2 or later
- SQL Server Express or Developer Edition
- SQL Server Management Studio (SSMS)
- Git

### Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/nayemuddinn/LearnLink.git
   cd LearnLink
   ```

2. **Create the database**

   Open `Database/schema.sql` in SSMS and execute it. This creates the `student`, `teacher`, `Courses`, `Enrollment`, `Quiz`, `QuizQuestions`, `QuizEvaluation`, and `courseMaterials` tables with their constraints.

3. **Open the solution**

   Open `LearnLink.sln` in Visual Studio and allow it to restore NuGet packages.

4. **Configure the connection string**

   In `Web.config`:

   ```xml
   <connectionStrings>
     <add name="LearnLinkDb"
          connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=learnlink;Integrated Security=True;TrustServerCertificate=True;"
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

   Alternatively, set the `LEARNLINK_CONNSTR` environment variable. The application checks `Web.config` first and falls back to the environment variable.

5. **Build and run**

   Build the solution (`Ctrl+Shift+B`) and start it with IIS Express (`F5`). The app opens at `https://localhost:<port>/`.

6. **Create an account**

   Register as a teacher or student, verify the account through the emailed link, then log in.

---

## Configuration

| Setting | Location | Purpose |
|---|---|---|
| `LearnLinkDb` | `Web.config` connection strings | SQL Server connection |
| `LEARNLINK_CONNSTR` | Environment variable | Overrides the connection string when set |
| `SmtpUser` / `SmtpPass` / `MailFrom` | `Web.config` app settings | Outbound email for verification and password reset |

---

## Security

- Role-based authorization enforced per action via `CustomAuthorize(Roles = "...")`
- Passwords hashed with bcrypt; minimum length, digit, and special-character requirements
- All SQL access uses parameterized queries (`SqlCommand.Parameters`)
- Session state keyed by `UserID`, `UserRole`, and `Email`, with optional persistent login cookies
- Server-side validation for email format, phone format, and password complexity
- HTML-encoded output and anti-forgery tokens on forms

---

## Testing

The application has been manually verified against the following scenarios:

- Registration, email verification, and duplicate-account rejection for both roles
- Login with correct and incorrect credentials, session persistence, and logout
- Course creation, update, and deletion
- Material upload, download, and deletion across supported file types
- Enrollment request, approval, rejection, and unenrollment
- Quiz creation, question authoring, timed attempts, auto-grading, and analytics
- Feedback submission and review
- Profile updates and password reset

---

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes with clear messages
4. Open a pull request describing the change

Bug reports should include reproduction steps and expected vs. actual behavior. Feature requests should include the motivating use case.

---

