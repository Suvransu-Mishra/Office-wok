using Microsoft.AspNetCore.Mvc;
using MVP_DashBoard.Models;
using System.Data.SqlClient;

namespace MVP_DashBoard.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Auth(/*User us*/)
        {
            //string connectionString = ConfigurationManager.ConnectionStrings[""].ToString();

            //using (SqlConnection con = new SqlConnection(connectionString))
            //{
            //    con.Open();
            //    string query = ;
            //    SqlCommand cmd = new SqlCommand(query, con);
            //    cmd.Parameters.AddWithValue("@username", us.username);
            //    cmd.Parameters.AddWithValue("@password", us.password);

            //    SqlDataReader dr = cmd.ExecuteReader();
            //    if (dr.Read())
            //    {
            //        con.Close();
            //        return RedirectToAction("Index", "Dashboard");
            //    }
            //    else
            //    {
            //        con.Close();
            //        ViewBag.ErrorMessage = "Invalid username or password";
            //        return View("Login");
            //    }
            //}
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
