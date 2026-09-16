using LearnLink.App_Start;
using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers.Quizzes
{
    [CustomAuthorize(Roles = "Student")]
    public class StudentViewQuizzesController : Controller
    {
        public ActionResult ViewQuizzes(string searchTerm, string filter = "all", string sort = "newest")
        {
            List<Quiz> quizzes = new List<Quiz>();

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                try
                {
                    conn.Open();

                    string queryQuizzes = @"
                SELECT 
                    q.QuizID,
                    q.CourseID,
                    q.CourseName,
                    q.TeacherID,
                    t.Name AS TeacherName,
                    q.Title,
                    q.Duration,
                    q.Description,
                    q.CreationDate,
                    q.Status,
                    CASE 
                        WHEN qe.Score IS NOT NULL THEN qe.Score
                        ELSE -1
                    END AS Score
                FROM Quiz q
                INNER JOIN Enrollment e
                    ON q.CourseID = e.CourseID
                LEFT JOIN Teacher t
                    ON q.TeacherID = t.UserID
                LEFT JOIN QuizEvaluation qe
                    ON q.QuizID = qe.QuizID
                    AND qe.StudentID = @StudentID
                WHERE e.StudentID = @StudentID
            ";

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        queryQuizzes += @"
                    AND (
                        q.Title LIKE @SearchTerm
                        OR q.CourseName LIKE @SearchTerm
                        OR CAST(q.CourseID AS VARCHAR(20)) LIKE @SearchTerm
                        OR CAST(q.TeacherID AS VARCHAR(20)) LIKE @SearchTerm
                        OR t.Name LIKE @SearchTerm
                        OR CONVERT(VARCHAR(30), q.CreationDate, 100) LIKE @SearchTerm
                    )
                ";
                    }

                    if (filter == "completed")
                    {
                        queryQuizzes += " AND qe.Score IS NOT NULL ";
                    }
                    else if (filter == "available")
                    {
                        queryQuizzes += " AND q.Status = 'Started' AND qe.Score IS NULL ";
                    }
                    else if (filter == "notcompleted")
                    {
                        queryQuizzes += " AND qe.Score IS NULL ";
                    }
                    else if (filter == "unavailable")
                    {
                        queryQuizzes += " AND q.Status <> 'Started' AND qe.Score IS NULL ";
                    }

                    if (sort == "oldest")
                    {
                        queryQuizzes += " ORDER BY q.CreationDate ASC ";
                    }
                    else if (sort == "name_asc")
                    {
                        queryQuizzes += " ORDER BY q.Title ASC ";
                    }
                    else if (sort == "name_desc")
                    {
                        queryQuizzes += " ORDER BY q.Title DESC ";
                    }
                    else if (sort == "course_asc")
                    {
                        queryQuizzes += " ORDER BY q.CourseName ASC ";
                    }
                    else if (sort == "course_desc")
                    {
                        queryQuizzes += " ORDER BY q.CourseName DESC ";
                    }
                    else
                    {
                        queryQuizzes += " ORDER BY q.CreationDate DESC ";
                    }

                    using (SqlCommand cmd = new SqlCommand(queryQuizzes, conn))
                    {
                        cmd.Parameters.Add("@StudentID", System.Data.SqlDbType.Int)
                            .Value = (int)Session["UserID"];

                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            cmd.Parameters.Add("@SearchTerm", System.Data.SqlDbType.VarChar)
                                .Value = "%" + searchTerm.Trim() + "%";
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                quizzes.Add(new Quiz
                                {
                                    QuizID = reader["QuizID"] != DBNull.Value
                                        ? Convert.ToInt32(reader["QuizID"])
                                        : 0,

                                    CourseID = reader["CourseID"] != DBNull.Value
                                        ? Convert.ToInt32(reader["CourseID"])
                                        : 0,

                                    CourseName = reader["CourseName"] != DBNull.Value
                                        ? reader["CourseName"].ToString()
                                        : "No Course Name",

                                    TeacherID = reader["TeacherID"] != DBNull.Value
                                        ? Convert.ToInt32(reader["TeacherID"])
                                        : 0,

                                    TeacherName = reader["TeacherName"] != DBNull.Value
                                        ? reader["TeacherName"].ToString()
                                        : "Unknown Teacher",

                                    Title = reader["Title"] != DBNull.Value
                                        ? reader["Title"].ToString()
                                        : "No Title",

                                    Duration = reader["Duration"] != DBNull.Value
                                        ? Convert.ToInt32(reader["Duration"])
                                        : 0,

                                    Description = reader["Description"] != DBNull.Value
                                        ? reader["Description"].ToString()
                                        : "No Description",

                                    CreationDate = reader["CreationDate"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["CreationDate"])
                                        : DateTime.MinValue,

                                    Status = reader["Status"] != DBNull.Value
                                        ? reader["Status"].ToString()
                                        : "No Status",

                                    Score = reader["Score"] != DBNull.Value
                                        ? Convert.ToInt32(reader["Score"])
                                        : -1
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write(
                        "<script>alert('An error occurred while fetching quizzes. Please try again. "
                        + ex.Message +
                        "');</script>"
                    );
                }
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Filter = filter;
            ViewBag.Sort = sort;

            return View(quizzes);
        }

        public ActionResult StartQuiz(int id)
        {
            if (Session["QuizStartTime"] != null)
            {
                DateTime startTime = (DateTime)Session["QuizStartTime"];
                int duration = (int)Session["QuizDuration"];
                DateTime endTime = startTime.AddMinutes(duration);
                DateTime currentTime = DateTime.Now;

                if (currentTime > endTime)
                {
                    using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
                    {
                        conn.Open();
                        string query = "INSERT INTO QuizEvaluation (StudentID, QuizID, Score, SubmissionTime) VALUES (@StudentID, @QuizID, @Score, @SubmissionTime)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@StudentID", (int)Session["UserID"]);
                            cmd.Parameters.AddWithValue("@QuizID", (int)Session["QuizID"]);
                            cmd.Parameters.AddWithValue("@Score", 0);
                            cmd.Parameters.AddWithValue("@SubmissionTime", currentTime);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Session["QuizID"] = null;
                }
            }

            if (Session["QuizID"] != null && (int)Session["QuizID"] != id)
            {
                TempData["AlertMessage"] = "You cannot start another quiz while you are currently taking one.";
                return RedirectToAction("ViewQuizzes");
            }

            Quiz quiz = null;
            List<QuizQuestion> questions = new List<QuizQuestion>();
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                conn.Open();
                string queryQuiz = "SELECT * FROM Quiz WHERE QuizID = @QuizID";
                using (SqlCommand cmd = new SqlCommand(queryQuiz, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quiz = new Quiz
                            {
                                QuizID = (int)reader["QuizID"],
                                Title = reader["Title"].ToString(),
                                Duration = (int)reader["Duration"]
                            };
                        }
                    }
                }

                string queryQuestions = "SELECT * FROM QuizQuestions WHERE QuizID = @QuizID";
                using (SqlCommand cmd = new SqlCommand(queryQuestions, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(new QuizQuestion
                            {
                                QuizID = (int)reader["QuizID"],
                                QuestionID = (int)reader["QuestionID"],
                                Question = reader["Question"].ToString(),
                                OptionA = reader["OptionA"].ToString(),
                                OptionB = reader["OptionB"].ToString(),
                                OptionC = reader["OptionC"].ToString(),
                                OptionD = reader["OptionD"].ToString(),
                                CorrectOption = reader["CorrectOption"].ToString()
                            });
                        }
                    }
                }
            }


            if (Session["QuizID"] == null
                || (int)Session["QuizID"] != quiz.QuizID
                || Session["QuizStartTime"] == null
                || Session["QuizEndtime"] == null)
            {
                var start = DateTime.Now;
                Session["QuizStartTime"] = start;
                Session["QuizDuration"] = quiz.Duration;
                Session["QuizEndtime"] = start.AddMinutes(quiz.Duration);
                Session["QuizID"] = quiz.QuizID;
            }

            return View(questions);
        }

        [HttpPost]
        public ActionResult SubmitQuiz(FormCollection form)
        {
            int quizID = (int)Session["QuizID"];
            DateTime startTime = (DateTime)Session["QuizStartTime"];
            int duration = (int)Session["QuizDuration"];
            DateTime endTime = startTime.AddMinutes(duration);
            DateTime currentTime = DateTime.Now;

            if (currentTime > endTime)
            {
                using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
                {
                    conn.Open();
                    string query = "INSERT INTO QuizEvaluation (StudentID, QuizID, Score, SubmissionTime) VALUES (@StudentID, @QuizID, @Score, @SubmissionTime)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", (int)Session["UserID"]);
                        cmd.Parameters.AddWithValue("@QuizID", quizID);
                        cmd.Parameters.AddWithValue("@Score", 0);
                        cmd.Parameters.AddWithValue("@SubmissionTime", currentTime);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["AlertMessage"] = "Submission Time is exceeded";
                Session["QuizID"] = null;
                return RedirectToAction("ViewQuizzes");
            }

            int score = 0;
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                conn.Open();

                Dictionary<int, string> correctAnswers = new Dictionary<int, string>();
                string queryCorrectAnswers = "SELECT QuestionID, CorrectOption FROM QuizQuestions WHERE QuizID = @QuizID";
                using (SqlCommand cmd = new SqlCommand(queryCorrectAnswers, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizID", quizID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            correctAnswers.Add((int)reader["QuestionID"], reader["CorrectOption"].ToString());
                        }
                    }
                }

                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("question_"))
                    {
                        int questionID = int.Parse(key.Split('_')[1]);
                        string selectedOption = form[key];

                        if (correctAnswers.ContainsKey(questionID) && correctAnswers[questionID] == selectedOption)
                        {
                            score++;
                        }
                    }
                }

                string query = "INSERT INTO QuizEvaluation (StudentID, QuizID, Score, SubmissionTime) VALUES (@StudentID, @QuizID, @Score, @SubmissionTime)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", (int)Session["UserID"]);
                    cmd.Parameters.AddWithValue("@QuizID", quizID);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@SubmissionTime", currentTime);
                    cmd.ExecuteNonQuery();
                }
            }
            Session["QuizID"] = null;
            TempData["AlertMessage"] = "Submission submitted successfully";
            return RedirectToAction("ViewQuizzes");
        }

        [HttpGet]
        public ActionResult GetFeedback(int quizID)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "Login");
            }

            int studentID = (int)Session["UserID"];

            string feedback = null;

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                conn.Open();

                string query = @"
            SELECT TOP 1 Feedback
            FROM QuizEvaluation
            WHERE QuizID = @QuizID
              AND StudentID = @StudentID
            ORDER BY SubmissionTime DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@QuizID", System.Data.SqlDbType.Int)
                        .Value = quizID;

                    cmd.Parameters.Add("@StudentID", System.Data.SqlDbType.Int)
                        .Value = studentID;

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        feedback = result.ToString();
                    }
                }
            }

            TempData["Feedback"] = feedback;
            TempData["FeedbackQuizID"] = quizID;

            return RedirectToAction("ViewQuizzes");
        }
    }
}
