
## Overview

**Learn Link** is a web-based **Learning Management System (LMS)** designed to facilitate seamless collaboration between **students and teachers**.  
Built with **ASP.NET MVC** and **MSSQL**, it provides a centralized platform for **course creation, material management, assessments, and performance tracking**.  

The platform aims to modernize education by making online learning **efficient, interactive, and accessible** for both educators and learners.

##  Key Features

### Teacher Capabilities
- **User Management** – Register, login, and access a personalized dashboard.
- **Course Management** – Create, update, or delete courses with ease.
- **Material Upload** – Add, update, and remove course materials.
- **Quiz Creation** – Create timed quizzes with configurable settings.
- **Student Control** – Approve/reject enrollment requests, remove students when necessary.
- **Assessment & Tracking** – View student scores, evaluate performance, and track progress.

###  Student Capabilities
- **User Management** – Register, login, and access a personalized dashboard.
- **Course Enrollment** – Enroll in available courses or send requests by name/code.
- **Access Resources** – View and download course content once approved.
- **Participate in Quizzes** – Take quizzes (one active quiz at a time).
- **Performance Tracking** – View quiz scores and monitor progress.

---

## Tech Stack

| Layer            | Technology |
|------------------|------------|
| **Frontend**     | ASP.NET MVC Views (Razor) |
| **Backend**      | ASP.NET (C#) |
| **Architecture** | MVC (Model-View-Controller) |
| **Database**     | MSSQL |
| **Version Control** | Git & GitHub |
| **Deployment**   | Localhost / Server-based Hosting |

---



_Here are some images showing this app's features and UI_

## 📸 Screenshots

| Login & Register | Dashboard |
|------------------|-----------|
| ![log-reg](https://github.com/user-attachments/assets/5ce30ff0-85f4-43ce-89b9-9f2773a2de12) |  |


---

## Prerequisites

- Visual Studio (with “ASP.NET and web development” workload)  
- .NET Framework / .NET SDK version required by the project  
- SQL Server (Express or higher) + SQL Server Management Studio (SSMS)  
- Git  

---

## Installation & Setup

### 1. Clone the repository
```bash
git clone https://github.com/your-username/learn-link.git
cd learn-link

### Database Setup

Run the SQL script located at [`/database/schema.sql`](./database/schema.sql):

