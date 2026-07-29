using MudBlazor;
using System.ComponentModel.DataAnnotations;
using CubeServer.Data;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace CubeServer.Models
{
    public class Project
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string PaymentCode { get; set; } = "C"; // one char only, C for cash, P for payment terms (credit)
        [Required, Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int CurSCONum { get; set; }
        [Required]
        public int Quotation { get; set; }
        [Required]
        public string ProjectName { get; set; } = "";
        public string BillToCompany { get; set; } = "";
        public string BillToAddress { get; set; } = "";
        public string SoldToCompany { get; set; } = "";
        public string SoldToAddress { get; set; } = "";
        public string ReportsRequired { get; set; } = "";
        public string ApplicantName { get; set; } = "";
        public string ApplicantDesignation { get; set; } = "";
        [Required]
        public List<string> EmailAddr { get; set; } = new List<string>();
        public List<string> EmailCC { get; set; } = new List<string>();
        [Required]
        public double PricePerCube { get; set; }
        public string CreateUser { get; set; } = "";
        public DateTime? CreateDate { get; set; }
        public string LastUpdateUser { get; set; } = "";
        public DateTime? LastUpdate { get; set; }

        // derived var
        private bool dailyReport;
        public bool DailyReport
        {
            get { return dailyReport; }
            set 
            { 
                dailyReport = value;
                BuildReportsRequired(); 
            }
        }
        private bool monthlyReport;
        public bool MonthlyReport 
        { 
            get { return monthlyReport; }
            set
            { 
                monthlyReport = value;
                BuildReportsRequired(); 
            } 
        }
        private bool statisticalReport;
        public bool StatisticalReport 
        { 
            get { return statisticalReport; }
            set
            {
                statisticalReport = true;
                BuildReportsRequired();
            }
        }
        public bool PaymentCash { get; set; }
        public bool PaymentCredit { get; set; }
        public string ProjectCode { get; set; }

        public void ReadSync()
        {
            string[] sarr = ReportsRequired.Split(Global.commaSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach(string s in sarr )
            {
                if (s == "Daily") DailyReport = true;
                else if (s == "Monthly") MonthlyReport = true;
                else if (s == "Statistical") StatisticalReport = true;
            }

            PaymentCash = PaymentCode == "C";
            PaymentCredit = PaymentCode == "P";
            ProjectCode = $"{PaymentCode}{Id:0000}";
        }

        private void BuildReportsRequired()
        {
            List<string> arr = new List<string>();
            if (DailyReport) arr.Add("Daily");
            if (MonthlyReport) arr.Add("Monthly");
            if (StatisticalReport) arr.Add("Statistical");

            ReportsRequired = string.Join(", ", arr);
        }

        public void WriteSync()
        {
            BuildReportsRequired();

            PaymentCode = PaymentCash ? "C" : "P";            
        }

        public static (string, int) SplitProjectCode(string projectCode)
        {
            if (projectCode == null || projectCode.Length == 0) return ("", 0);

            string paymentCode = projectCode.Substring(0, 1);
            int id;
            if (int.TryParse(projectCode.Substring(1), out id))
            {
                return (paymentCode, id);
            }

            return ("", 0);
        }
    }
}

