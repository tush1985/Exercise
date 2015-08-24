using Havas_Exercise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Havas_Exercise.Controllers
{
    public class HomeController : Controller
    {
        //Admin Can see all user data
        //Dealer can see only dealer data associated with dealer username
        //[Authorize(Roles="Admin")]
        public ActionResult Index()
        {
            //By Default user is not an admin user
            Boolean IsAdmin =false;
            //Check if the user is an admin user
            if (Roles.IsUserInRole(User.Identity.Name, "Admin"))
                IsAdmin = true;
            //Create an object for the commission model
            CommissionModel Commissions = new CommissionModel();
            //Get the commission result, if an admin then retrive all data and if dealer then get the data associated with username
            var result = Commissions.GetCommission(User.Identity.Name, IsAdmin);
            //List<Havas_Exercise.Models.CommissionModel> C = result.ToList();
            return View(result);
        }
        //This is for getting a data for the selected period or by PaymentStatus
        //single post for reusable of code
        //Assigning a default value to null
        [HttpPost]   
        [ValidateAntiForgeryToken]     
        public ActionResult Index(DateTime? StartDate=null,DateTime? EndDate=null,int PaymentStatus=0)
        {
            //By default user is not an admin user
            Boolean IsAdmin = false;
            //Check if the user is logged in as admin user
            if (Roles.IsUserInRole(User.Identity.Name, "Admin"))
                IsAdmin = true;
            //Create a commission model
            CommissionModel Commissions = new CommissionModel();
            //Passing the value to the method
            var result = Commissions.GetCommission(User.Identity.Name, IsAdmin,StartDate,EndDate,PaymentStatus);
            //List<Havas_Exercise.Models.CommissionModel> C = result.ToList();
            //pass the result to view which is a Index view
            return View(result);
        }
        
        //When user click on about method
        //We can allow only admin user to allow about view
        
        [Authorize(Roles="Admin")]
        public ActionResult About()
        {
            //Logic needs to be implement for a non admin user for displaying an error that they cannot access this view
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}