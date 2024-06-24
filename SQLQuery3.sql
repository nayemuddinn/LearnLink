CREATE TABLE Course (
    CourseID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    CourseName VARCHAR(255) NOT NULL,
    TeacherID INT NOT NULL,
    CONSTRAINT FK_Teacher_Course FOREIGN KEY (TeacherID) REFERENCES Teacher(UserID),
    CONSTRAINT Unique_Course UNIQUE (CourseName, TeacherID)
);

CREATE TABLE StudentCourse (
    StudentID INT NOT NULL,
    CourseID INT NOT NULL,
    CONSTRAINT PK_StudentCourse PRIMARY KEY (StudentID, CourseID),
    CONSTRAINT FK_StudentCourse_Student FOREIGN KEY (StudentID) REFERENCES Student(UserID),
    CONSTRAINT FK_StudentCourse_Course FOREIGN KEY (CourseID) REFERENCES Course(CourseID)
);
