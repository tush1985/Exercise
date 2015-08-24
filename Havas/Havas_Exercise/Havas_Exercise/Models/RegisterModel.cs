using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Havas_Exercise.Models
{
    public class RegisterModel
    {
         
#region public property
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }


        [Required]        
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        #endregion

    
    
    }
}