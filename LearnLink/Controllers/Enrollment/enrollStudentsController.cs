using LearnLink.App_Start;
using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers.Enrollment
{
    [CustomAuthorize(Roles = "Teacher")]
    public class enrollStudentsController : Controller
    {
        // GET: enrollStudents
        public ActionResult enrollStudents()
        {
            return View();
        }
        public ActionResult enrollNewStudents(
     string searchTerm,
     int pageSize = 10,
     string sortBy = "CourseID",
     string sortOrder = "DESC")
        {
            List<LearnLink.Models.Enrollment> requests =
                new List<LearnLink.Models.Enrollment>();

            int teacherId = Convert.ToInt32(Session["UserID"]);

            if (pageSize != 10 && pageSize != 100 && pageSize != 1000)
                pageSize = 10;


            string sortColumn;

            switch (sortBy)
            {
                case "CourseID":
                    sortColumn = "c.CourseID";
                    break;

                case "CourseName":
                    sortColumn = "c.CourseName";
                    break;

                case "StudentID":
                    sortColumn = "r.StudentID";
                    break;

                case "StudentName":
                    sortColumn = "s.Name";
                    break;

                case "StudentInstitution":
                    sortColumn = "s.Institution";
                    break;

                default:
                    sortColumn = "c.CourseID";
                    sortBy = "CourseID";
                    break;
            }


            if (sortOrder != "ASC" && sortOrder != "DESC")
                sortOrder = "DESC";

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = @"
            SELECT TOP (@PageSize)
                r.EnrollmentID,
                c.CourseID,
                c.CourseName,
                r.StudentID,
                s.Name,
                s.Institution,
                r.Status
            FROM Enrollment r
            INNER JOIN Courses c
                ON r.CourseID = c.CourseID
            INNER JOIN Student s
                ON r.StudentID = s.UserID
            WHERE r.TeacherID = @TeacherID
              AND r.Status = 'Requested'
        ";

    
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query += @"
                AND (
                    CAST(c.CourseID AS VARCHAR(20)) LIKE @SearchTerm
                    OR c.CourseName LIKE @SearchTerm
                    OR CAST(r.StudentID AS VARCHAR(20)) LIKE @SearchTerm
                    OR s.Name LIKE @SearchTerm
                    OR s.Institution LIKE @SearchTerm
                )
            ";
                }

            
                query += $" ORDER BY {sortColumn} {sortOrder}";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@TeacherID", System.Data.SqlDbType.Int)
                        .Value = teacherId;

                    cmd.Parameters.Add("@PageSize", System.Data.SqlDbType.Int)
                        .Value = pageSize;

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.Parameters.Add("@SearchTerm", System.Data.SqlDbType.VarChar)
                            .Value = "%" + searchTerm.Trim() + "%";
                    }

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            requests.Add(new LearnLink.Models.Enrollment
                            {
                                EnrollmentID = (int)reader["EnrollmentID"],
                                CourseID = (int)reader["CourseID"],
                                CourseName = (string)reader["CourseName"],
                                StudentID = (int)reader["StudentID"],
                                StudentName = (string)reader["Name"],
                                StudentInstitution = (string)reader["Institution"],
                                Status = (string)reader["Status"]
                            });
                        }
                    }
                }
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.PageSize = pageSize;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            return View(requests);
        }

        public ActionResult UpdateRequest(int enrollmentID, string actionType)
        {
            string status = "";

            if (actionType.ToLower() == "accept")
            {
                status = "Accepted";
            }
            else if (actionType.ToLower() == "reject")
            {
                status = "Rejected";
            }


            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                string query = "UPDATE Enrollment SET Status = @Status WHERE enrollmentID = @enrollmentID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@enrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@Status", status);

                conn.Open();
                cmd.ExecuteNonQuery();
            }


            return RedirectToAction("enrollNewStudents", "enrollStudents");
        }



        public ActionResult ViewEnrolledStudents(int courseID)
        {
            List<User> enrolledStudents = new List<User>();

            using (SqlConnection con = new SqlConnection(DBconnection.connStr))
            {
                string query = @"SELECT s.UserID, s.Name, s.Institution, s.Phone FROM student s
                    JOIN Enrollment e ON s.UserID = e.StudentID
                WHERE e.CourseID = @CourseID AND e.Status = 'Accepted'";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CourseID", courseID);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            enrolledStudents.Add(new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Name = reader["Name"].ToString(),
                                Institution = reader["Institution"].ToString(),
                                Phone = reader["Phone"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            ViewBag.CourseID = courseID; 
            return View(enrolledStudents);
        }

        public ActionResult UnenrollStudent(int studentId, int courseId,int page)
        {
            using (SqlConnection con = new SqlConnection(DBconnection.connStr))
            {
                string query = "DELETE FROM enrollment WHERE StudentID = @StudentID AND CourseID = @CourseID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            if(page==1)
                return RedirectToAction("AllEnrolledStudents");
            else
            return RedirectToAction("ViewEnrolledStudents", new { courseID = courseId });
        }

        public ActionResult AllEnrolledStudents(
     string searchTerm,
     int pageSize = 10,
     string sortBy = "CourseID",
     string sortOrder = "DESC")
        {
            List<EnrolledStudentCourse> enrolledStudentsCourses =
                new List<EnrolledStudentCourse>();

            int teacherId = Convert.ToInt32(Session["UserID"]);

            // Validate page size
            if (pageSize != 10 && pageSize != 100 && pageSize != 1000)
            {
                pageSize = 10;
            }

            // Validate sort column
            string sortColumn;

            switch (sortBy)
            {
                case "CourseID":
                    sortColumn = "c.CourseID";
                    break;

                case "CourseName":
                    sortColumn = "c.CourseName";
                    break;

                case "StudentID":
                    sortColumn = "s.UserID";
                    break;

                case "StudentName":
                    sortColumn = "s.Name";
                    break;

                default:
                    sortColumn = "c.CourseID";
                    sortBy = "CourseID";
                    break;
            }

            // Validate sort order
            if (sortOrder != "ASC" && sortOrder != "DESC")
            {
                sortOrder = "DESC";
            }

            using (SqlConnection con = new SqlConnection(DBconnection.connStr))
            {
                string query = @"
            SELECT TOP (@PageSize)
                c.CourseID,
                c.CourseName,
                s.UserID AS StudentID,
                s.Name AS StudentName,
                s.Institution,
                s.Phone
            FROM Enrollment e
            INNER JOIN Courses c
                ON e.CourseID = c.CourseID
            INNER JOIN Student s
                ON e.StudentID = s.UserID
            WHERE c.TeacherID = @TeacherID
            AND e.Status = 'Accepted'
        ";

                // Search
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query += @"
                AND (
                    CAST(c.CourseID AS VARCHAR(20)) LIKE @SearchTerm
                    OR c.CourseName LIKE @SearchTerm
                    OR CAST(s.UserID AS VARCHAR(20)) LIKE @SearchTerm
                    OR s.Name LIKE @SearchTerm
                    OR s.Institution LIKE @SearchTerm
                    OR s.Phone LIKE @SearchTerm
                )
            ";
                }

                // Sorting
                query += $" ORDER BY {sortColumn} {sortOrder}";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@TeacherID", System.Data.SqlDbType.Int)
                        .Value = teacherId;

                    cmd.Parameters.Add("@PageSize", System.Data.SqlDbType.Int)
                        .Value = pageSize;

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.Parameters.Add("@SearchTerm", System.Data.SqlDbType.VarChar)
                            .Value = searchTerm.Trim() + "%";
                    }

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            enrolledStudentsCourses.Add(new EnrolledStudentCourse
                            {
                                CourseID = Convert.ToInt32(reader["CourseID"]),
                                CourseName = reader["CourseName"].ToString(),
                                StudentID = Convert.ToInt32(reader["StudentID"]),
                                StudentName = reader["StudentName"].ToString(),
                                Institution = reader["Institution"].ToString(),
                                Phone = reader["Phone"].ToString()
                            });
                        }
                    }
                }
            }

            // Keep selected values after search/filter
            ViewBag.SearchTerm = searchTerm;
            ViewBag.PageSize = pageSize;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            return View(enrolledStudentsCourses);
        }


    }
}
