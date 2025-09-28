
## Overview

**Learn Link** is a modern **Learning Management System (LMS)** built with **ASP.NET MVC** and **MSSQL**, designed to streamline collaboration between students and teachers.  
It provides a centralized platform for **course creation, material management, quizzes, and performance tracking**, making it easier for educators to deliver content and for learners to engage with their studies.  

By combining robust features with an intuitive interface, **Learn Link** helps modernize education, making online learning more **efficient, interactive, and accessible** for all users.  


##  Key Features

### Teacher Capabilities
- **User Management** – Register, login, and access a personalized dashboard.
- **Course Management** – Create, update, or delete courses with ease.
- **Material Upload** – Add, update, and remove course materials (Image, PDF, doc files).
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

## Screenshots

| Login | Register |
|------|----------|
| <img width="600" height="480" alt="login" src="https://github.com/user-attachments/assets/8a642409-de72-4d64-858e-4daed426c3b5" /> | <img width="600" height="480" alt="reg" src="https://github.com/user-attachments/assets/8bfa6e45-42a3-49bf-9168-03996b2ec62d" /> |

| Teacher Dashboard | Student Dashboard |
|-------------------|-------------------|
| <img width="600" height="480" alt="Dashboard_Teacher" src="https://github.com/user-attachments/assets/8e96f945-bce8-4db7-9c01-c91fcbce3fbc" /> | <img width="600" height="480" alt="Dashboard_student" src="https://github.com/user-attachments/assets/99be5743-d3b4-4722-bbd3-84bde61262cc" /> |




---

## Prerequisites

- Visual Studio (with “ASP.NET and web development” workload)  
- .NET Framework / .NET SDK version required by the project  
- SQL Server (Express or higher) + SQL Server Management Studio (SSMS)  
- Git  

---

## Installation & Setup

###  Clone the repository

git clone https://github.com/your-username/learn-link.git
cd learn-link

### Database Setup

[Run the SQL script here](./database/schema.sql)


### Run the Project

1. **Open the Solution**  
   - Launch **Visual Studio**.  
   - Open the `LearnLink.sln` file from the cloned repository.  

2. **Restore NuGet Packages**  
   - Visual Studio will usually restore packages automatically.  
   - If not, go to:  
     `Tools > NuGet Package Manager > Manage NuGet Packages for Solution`  
     and click **Restore**.  

3. **Configure the Connection String**  
   - Open `Web.config`.  
   - Update the connection string under `<connectionStrings>` if necessary to match your SQL Server instance:  
     ```xml
     <connectionStrings>
       <add name="DefaultConnection"
            connectionString="Data Source=.;Initial Catalog=LearnLink;Integrated Security=True;MultipleActiveResultSets=True"
            providerName="System.Data.SqlClient" />
     </connectionStrings>
     ```
   - Replace `Data Source=.` with your SQL Server name if it’s different.  

4. **Build the Solution**  
   - From the top menu, select `Build > Build Solution` (or press `Ctrl+Shift+B`).  

5. **Run the Application**  
   - Press **F5** to run with IIS Express, or use the dropdown to select your preferred IIS/local server.  
   - The project should open in your default browser at a URL like:  
     ```
     https://localhost:xxxx/
     ```



