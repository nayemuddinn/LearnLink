using LearnLink.Content;
using LearnLink.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LearnLink.Controllers
{
    public class CreateQuizController : Controller
    {
      
        public ActionResult CreateQuiz()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateQuiz(Quiz quiz)
        {
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO Quiz (CourseID, TeacherID, Title, Description, CreationDate, CourseName) SELECT @CourseID,@TeacherID,@Title,@Description,@CreationDate,@CourseName
                    c.CourseName FROM Courses c WHERE c.CourseID = @CourseID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CourseID", quiz.CourseID);
                        cmd.Parameters.AddWithValue("@CourseName", quiz.CourseName);
                        cmd.Parameters.AddWithValue("@TeacherID", Session["UserID"]);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Response.Write("<script>alert('Quiz created successfully!');</script>");
                        }
                        else
                        {
                            Response.Write("<script>alert('Course ID not found. Please check the Course ID and try again.');</script>");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('An error occurred. Please try again.');</script>");
                   // Response.Write("<script>alert('An error occurred. Please try again.');</script>");
                }
            }
            return View();
        }



        public ActionResult UploadQuiz()
        {
            return View();
        }

     [HttpPost]
        public ActionResult UploadQuiz(QuizQuestion quizQuestion)
        {
            using (SqlConnection conn = new SqlConnection(DBconnection.connStr))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO QuizQuestions (QuizID, Question, OptionA, OptionB, OptionC, OptionD, CorrectOption) " +
                                   "VALUES (@QuizID, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD, @CorrectOption)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QuizID", quizQuestion.QuizID);
                        cmd.Parameters.AddWithValue("@QuestionText", quizQuestion.Question);
                        cmd.Parameters.AddWithValue("@OptionA", quizQuestion.OptionA);
                        cmd.Parameters.AddWithValue("@OptionB", quizQuestion.OptionB);
                        cmd.Parameters.AddWithValue("@OptionC", quizQuestion.OptionC);
                        cmd.Parameters.AddWithValue("@OptionD", quizQuestion.OptionD);
                        cmd.Parameters.AddWithValue("@CorrectOption", quizQuestion.CorrectOption);

                        cmd.ExecuteNonQuery();
                        ViewBag.Message = "Question uploaded successfully!";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "An error occurred: " + ex.Message;
                }
            }
            return View();
        }
    }
}