using Havas_Exercise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Havas_Exercise.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        //When we run this application default will come here
        [HttpGet]
        public ActionResult Login()
        {
            //Default goes to a LoginView
            return View();
        }

        //when user click on login( username and password enter )
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //check id usename and password exists
                try
                {
                    //If the username and password is valid
                    if (WebSecurity.Login(model.UserName, model.Password))
                    {
                       //Redirect to Index method of Home  Controller
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        //Display the error back to the user
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                        return View(model);
                    }


                }
                //If not found
                catch (Exception ex)
                {
                    //Any Error on catch write a to a log file
                    ModelState.AddModelError("", "Please contct to support help desk");
                }
            }

            // If we got this far, something failed, redisplay form 
            return View(model);

        }


        //Getall the roles
        [HttpGet]
        public ActionResult GetRole()
        {
            //Get the all roles for creating a new user
            RoleModel role = new RoleModel();
            try
            {
                 var userroles = role.GetRoles();

                return View(userroles);
            }
            catch (Exception ex)
            {
                //Any Error on catch write a to a log file
                ModelState.AddModelError("", "Please contct to support help desk");
            }
            return View(role);

        }

        //Partial view for creating a role
        [HttpGet]
        public ActionResult _CreateRole()
        {
             return PartialView();
        }
        //For Creating a new Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRole(RoleModel model)
        {
            //Check if modelstate is valid
            if (ModelState.IsValid)
            {
                try
                {
                    RoleModel role = new RoleModel();
                    if (role.CreateRole(model.RoleName))
                    {
                        //Success for creating a new role
                      return   RedirectToAction("GetRole", "Account");
                    }
                    else
                    {
                        //Failed for creating a new role
                        ModelState.AddModelError("", "Sorry the  role alerady exists");
                    }
                }
                catch (Exception ex)
                {
                    //Exception for creating a new role
                    ModelState.AddModelError("", "Plese contact to support");
                }
            }
            
            return RedirectToAction("GetRole", "Account"); 
        }


        //when user click on register button we will return the register view
        [HttpGet]
        public ActionResult Register()
        {
            //Get all the roles and put in viewdata for diplaying to user
            RoleModel role = new RoleModel();

            var userroles = role.GetRoles();

            ViewData["UserRoles"] = userroles;
            return View();
        }

        // POST: /Account/Register
        //Registering a new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model,string Role)
        {
            //if model state is valid
            if (ModelState.IsValid)
            {
                // Attempt to register the user                
                try
                {
                    //Adding a new user
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, false);
                    //Assign a role to user
                    Roles.AddUserToRole(model.UserName, Role);
                    //Set the cookies
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    //Redirect the user to Index method of Home Controller
                    return RedirectToAction("Index", "Home");
                }
                //If Fails
                catch (System.Web.Security.MembershipCreateUserException ex)
                {
                    //Please write the error in log file
                    RoleModel role = new RoleModel();

                    var userroles = role.GetRoles();

                    ViewData["UserRoles"] = userroles;
                    ModelState.AddModelError("", "Sorry the username alerady exists");
                }

            }
            else
            {
                //if model state is not valid then debug for the error
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();
                return View(model);
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff
        //When logoff button click
        public ActionResult LogOff()
        {
            //Logot from the browser
            FormsAuthentication.SignOut();
            
            return RedirectToAction("Login", "Account");
        }

     
        

    }
}