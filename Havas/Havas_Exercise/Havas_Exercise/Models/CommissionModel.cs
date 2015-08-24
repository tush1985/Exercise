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
    public class CommissionModel
    {
       //Set the property of the Commission Model
       #region public property
        public int CommissionID { get; set; }
        public int CustomerID { get; set; }
        public int DealerID { get; set; }        
        public string DealerName { get; set; }
        public int ProductID { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]        
        public decimal CommissionAmount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
       #endregion

       #region Private Property
        private SqlCommand cmd;

        private SqlConnection cn = new SqlConnection();

        private SqlDataReader dr;

        private static string ConnectionString ;
      #endregion
      #region Default Constructor
        //get the value of the connection string for web.config
        public CommissionModel()
       {
          ConnectionString = ConfigurationManager.AppSettings["Connection"];
       }
      #endregion

        #region public method
        //Get the commission 
        //username: user's login name
        //isadmin : if the user is an admin user or not
        //startdate: if user wants to get the report for a specific date, put a startdate
        //enddate: if user wants to get the report for a specific date, put a enddate
        //status: get the report based on status value(Created=1 , Verified =2,Rejected=3,Awaiting Payment=4,Paid=5,Refunded =6)

        public List<CommissionModel> GetCommission(string username,bool isadmin, DateTime? startdate=null,DateTime? enddate=null,int status=0)
        {
            try
            {
                //Create a List for storing a Commission data
                List<CommissionModel> CommissionReport = new List<CommissionModel>();
                using (cn = new SqlConnection(ConnectionString))
                {
                    //open the connection
                    cn.Open();
                    cmd = new SqlCommand("GetCommissionReport", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IsAdmin", isadmin);
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@StartDate", startdate);
                    cmd.Parameters.AddWithValue("@EndDate", enddate);
                    cmd.Parameters.AddWithValue("@status",status);

                    dr = cmd.ExecuteReader();
                    //if readwer has rows
                    if (dr.HasRows)                   
                    {
                        //read all the value
                        while(dr.Read())
                        {
                            //create the commission object
                            CommissionModel commission = new CommissionModel();
                            commission.CommissionID = Convert.ToInt32(dr["CommissionId"]);
                            commission.CustomerID = Convert.ToInt32(dr["CustomerId"]);
                            commission.DealerID =  Convert.ToInt32(dr["DealerId"]);
                            commission.DealerName = dr["Name"].ToString();
                            commission.ProductID =  Convert.ToInt32(dr["ProductId"]);                            
                            commission.CommissionAmount = Convert.ToDecimal(dr["CommissionAmount"])  ;
                            //Switch case for displaying a payment status
                            switch (Convert.ToUInt32(dr["PaymentStatusId"] ))
	                        {
	                            case 1:
		                            commission.PaymentStatus ="Created";
		                           break;
                                case 2:
                                   commission.PaymentStatus ="Verified";
                                   break;
                                case 3:
                                    commission.PaymentStatus ="Rejected";
                                    break;
                                case 4:
                                    commission.PaymentStatus ="Awaiting Payment";
                                    break;
                                case 5:
                                    commission.PaymentStatus ="Paid";
                                    break;
	                            case 6:
                                    commission.PaymentStatus ="Refunded";		                        
                                    break;
	                        }
                            commission.CreatedDate =  Convert.ToDateTime(dr["CreatedDate"]);
                            commission.ModifiedDate =  Convert.ToDateTime(dr["ModifiedDate"]);
                            //Add the commission object to list
                            CommissionReport.Add(commission) ;
                       }
                   }
                }
                //close the connection
                cn.Close();
                //return the Commission List
                return CommissionReport;
            }
            catch(Exception ex)
            {
                //if any error close the connection
                cn.Close();
                //Please log the error
                return null;
            }

        }

        #endregion
    }
}