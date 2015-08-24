using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Havas_Exercise.Models
{
    public class LogOnModel
    {
        #region public property
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        #endregion

        #region private property
        private SqlCommand cmd;

        private SqlConnection cn = new SqlConnection();

        private SqlDataReader dr;

        //Assigning a connection string
        private static string ConnectionString = ConfigurationSettings.AppSettings["DefaultConnection"];

        #endregion
        #region public method
        //Authenticate the user by taking a model value from the form
        public bool Authenticate(LogOnModel model)
        {
            //Making a connection to database for checking a username and password
            try
            {
                using (cn = new SqlConnection(ConnectionString))
                {                      
                      cmd = new SqlCommand("CheckAuthentication",cn);
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.AddWithValue("@UserName", model.UserName); 
                      cmd.Parameters.AddWithValue("@PassWord", model.Password);
                      dr = cmd.ExecuteReader();
                    //if the user is a valid user return true
                      if(dr.Read() )
                            return true;
                       else//return false if the user is not a valid user
                            return false;

                }
                
            }
                //Cathe the Exception and write to a log file
            catch(Exception ex){
                //Because of the exception return false
                return false;
            }
            //Error if Something wrong
            return false;
        }
        #endregion

    }
}