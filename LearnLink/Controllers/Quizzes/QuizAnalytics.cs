using LearnLink.Content;
using LearnLink.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace LearnLink.Controllers.Quizzes
{
    public class QuizAnalyticsController : Controller
    {
        public ActionResult Index(int id)
        {
            if (Session["UserID"] == null || Session["UserRole"] == null)
            {
                return RedirectToAction("login", "Login");
            }

            if (Session["UserRole"].ToString() != "Teacher")
            {
                return new HttpStatusCodeResult(403);
            }

            int teacherID = (int)Session["UserID"];

            Quiz quiz = null;
            List<QuizEvaluation> evaluations = new List<QuizEvaluation>();
            int totalQuestions = 0;

            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                conn.Open();

                string quizQuery = @"
                    SELECT
                        QuizID,
                        CourseID,
                        CourseName,
                        TeacherID,
                        Title,
                        Duration,
                        Description,
                        CreationDate,
                        Status
                    FROM Quiz
                    WHERE QuizID = @QuizID
                      AND TeacherID = @TeacherID";

                using (SqlCommand cmd = new SqlCommand(quizQuery, conn))
                {
                    cmd.Parameters.Add("@QuizID", System.Data.SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@TeacherID", System.Data.SqlDbType.Int).Value = teacherID;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quiz = new Quiz
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
                                    : "NotStarted"
                            };
                        }
                    }
                }

                if (quiz == null)
                {
                    return HttpNotFound("Quiz not found or you do not have permission to view this quiz.");
                }

                string questionQuery = @"
                    SELECT COUNT(*)
                    FROM QuizQuestions
                    WHERE QuizID = @QuizID";

                using (SqlCommand cmd = new SqlCommand(questionQuery, conn))
                {
                    cmd.Parameters.Add("@QuizID", System.Data.SqlDbType.Int).Value = id;

                    totalQuestions = Convert.ToInt32(cmd.ExecuteScalar());
                }

                string evaluationQuery = @"
                    SELECT
                        qe.SubmitID,
                        qe.QuizID,
                        qe.StudentID,
                        s.Name AS StudentName,
                        qe.Score,
                        qe.SubmissionTime,
                        qe.Feedback
                    FROM QuizEvaluation qe
                    INNER JOIN Student s
                        ON qe.StudentID = s.UserID
                    WHERE qe.QuizID = @QuizID
                    ORDER BY qe.SubmissionTime DESC";

                using (SqlCommand cmd = new SqlCommand(evaluationQuery, conn))
                {
                    cmd.Parameters.Add("@QuizID", System.Data.SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            evaluations.Add(new QuizEvaluation
                            {
                                SubmitID = reader["SubmitID"] != DBNull.Value
                                    ? Convert.ToInt32(reader["SubmitID"])
                                    : 0,

                                QuizID = reader["QuizID"] != DBNull.Value
                                    ? Convert.ToInt32(reader["QuizID"])
                                    : 0,

                                StudentID = reader["StudentID"] != DBNull.Value
                                    ? Convert.ToInt32(reader["StudentID"])
                                    : 0,

                                StudentName = reader["StudentName"] != DBNull.Value
                                    ? reader["StudentName"].ToString()
                                    : "Unknown Student",

                                Score = reader["Score"] != DBNull.Value
                                    ? Convert.ToInt32(reader["Score"])
                                    : 0,

                                SubmissionTime = reader["SubmissionTime"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["SubmissionTime"])
                                    : DateTime.MinValue,

                                Feedback = reader["Feedback"] != DBNull.Value
                                    ? reader["Feedback"].ToString()
                                    : ""
                            });
                        }
                    }
                }
            }

            QuizAnalytics analytics = new QuizAnalytics
            {
                Quiz = quiz,
                Evaluations = evaluations,
                TotalSubmissions = evaluations.Count,
                TotalQuestions = totalQuestions,
                AverageScore = evaluations.Count > 0
                    ? evaluations.Average(e => e.Score)
                    : 0,
                HighestScore = evaluations.Count > 0
                    ? evaluations.Max(e => e.Score)
                    : 0,
                LowestScore = evaluations.Count > 0
                    ? evaluations.Min(e => e.Score)
                    : 0
            };

            return View(analytics);
        }

        [HttpPost]
        public ActionResult SaveFeedback(int submitID, string feedback)
        {
            if (Session["UserID"] == null || Session["UserRole"] == null)
            {
                return RedirectToAction("login", "Login");
            }

            if (Session["UserRole"].ToString() != "Teacher")
            {
                return new HttpStatusCodeResult(403);
            }

            if (string.IsNullOrWhiteSpace(feedback))
            {
                TempData["FeedbackError"] = "Feedback cannot be empty.";
                return RedirectToAction("Index");
            }

            int teacherID = (int)Session["UserID"];
            int quizID = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
                {
                    conn.Open();

                    
                    string getQuizQuery = @"
                SELECT qe.QuizID
                FROM QuizEvaluation qe
                INNER JOIN Quiz q
                    ON qe.QuizID = q.QuizID
                WHERE qe.SubmitID = @SubmitID
                  AND q.TeacherID = @TeacherID";

                    using (SqlCommand cmd = new SqlCommand(getQuizQuery, conn))
                    {
                        cmd.Parameters.Add("@SubmitID", System.Data.SqlDbType.Int).Value = submitID;
                        cmd.Parameters.Add("@TeacherID", System.Data.SqlDbType.Int).Value = teacherID;

                        object result = cmd.ExecuteScalar();

                        if (result == null)
                        {
                            TempData["FeedbackError"] =
                                "Submission not found or you do not have permission to give feedback.";

                            return RedirectToAction("Index");
                        }

                        quizID = Convert.ToInt32(result);
                    }

                    string updateQuery = @"
                UPDATE QuizEvaluation
                SET Feedback = @Feedback
                WHERE SubmitID = @SubmitID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add("@Feedback", System.Data.SqlDbType.NVarChar, 500)
                            .Value = feedback.Trim();

                        cmd.Parameters.Add("@SubmitID", System.Data.SqlDbType.Int)
                            .Value = submitID;

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["FeedbackSuccess"] = "Feedback saved successfully.";

                return RedirectToAction("Index", new { id = quizID });
            }
            catch (Exception)
            {
                TempData["FeedbackError"] =
                    "Something went wrong while saving the feedback.";

                return RedirectToAction("Index", new { id = quizID });
            }
        }
    }
}