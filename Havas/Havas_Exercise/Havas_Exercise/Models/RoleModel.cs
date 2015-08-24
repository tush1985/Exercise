using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace Havas_Exercise.Models
{
    public class RoleModel
    {
        #region public property
        [Required]
        [Display(Name = "Role name")]
        public string RoleName { get; set; }

        #endregion

        //Create a role
        #region public method
        public bool CreateRole(string rolename)
        {
            try
            {
                //IF role is alerady exist return false else return true
                if (!Roles.RoleExists(rolename))
                {
                    //create a role if the role is not exist
                    Roles.CreateRole(rolename);
                    return true;
                }
                else //IF the role is exist return false               
                    return false;
            }
            catch (Exception ex)
            {
                //write the log file for the exception
                return false;
            }
        }
        
        //Get the roles for displaying
        public List<RoleModel> GetRoles()
        {
            List<RoleModel> rolemodels = new List<RoleModel>();

            try
            {
                var roles = (SimpleRoleProvider)Roles.Provider;
                string[] UserRoles = roles.GetAllRoles();


                //If Roles alerady in the database
                if (UserRoles.Length > 0)
                {
                    //Add the role to RoleModel

                    for (int i = 0; i < UserRoles.Length; i++)
                    {
                        RoleModel role = new RoleModel();
                        role.RoleName = UserRoles[i];
                        rolemodels.Add(role);
                    }
                }
            }
            catch (Exception ex)
            {
                //write the log file for the exception
            }
            return rolemodels;

        }
        #endregion

    }
}