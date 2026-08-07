using CubeServer.Data;
using Microsoft.AspNetCore.Hosting.Server;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace CubeServer.Models
{
    public enum CubeTestResult
    {
        None,
        Pass,
        Fail,
        Pending
    }

    public enum CubeViewOption
    {
        Untested,
        Tested,
        All
    }

    public class Cube
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public long Barcode { get; set; } // max 8 digits
        [Required]
        public string SampleRef { get; set; } = "";
        [Required]
        public int ScoNum { get; set; }
        [Required]
        public int BatchId { get; set; }
        public double MeasuredDimX1 { get; set; } // from line
        public double MeasuredDimX2 { get; set; } // from line
        public double MeasuredDimX3 { get; set; } // from line
        public double MeasuredDimX4 { get; set; } // from line
        public double MeasuredDimX5 { get; set; } // from line
        public double MeasuredDimX6 { get; set; } // from line
        public double MeasuredDimY1 { get; set; } // from line
        public double MeasuredDimY2 { get; set; } // from line
        public double MeasuredDimY3 { get; set; } // from line
        public double MeasuredDimY4 { get; set; } // from line
        public double MeasuredDimY5 { get; set; } // from line
        public double MeasuredDimY6 { get; set; } // from line
        public double AvgDimension { get; set; } // calculated from 12 measurements
        double _MeasuredMaxForce;
        public double MeasuredMaxForce 
        { 
            get
            {
                return _MeasuredMaxForce;
            }
            set
            {
                if (value > 0)
                {
                    _MeasuredMaxForce = value;
                    if (Dimension > 0)
                    {
                        _MeasuredStrength = _MeasuredMaxForce / Util.Sqr(Dimension);
                    }
                    ActualTestDate = DateTime.Now;
                }
                else
                {
                    _MeasuredMaxForce = 0.0;
                    _MeasuredStrength = 0.0;
                    ActualTestDate = null;
                }
            } 
        } // N, from tester

        double _MeasuredStrength;
        public double MeasuredStrength 
        {
            get
            {
                return _MeasuredStrength;
            }

            set
            {
                if (value > 0)
                {
                    _MeasuredStrength = value;
                    if (Dimension > 0)
                    {
                        _MeasuredMaxForce = _MeasuredStrength * Util.Sqr(Dimension);
                    }
                    ActualTestDate = DateTime.Now;
                }
                else
                {
                    _MeasuredMaxForce = 0.0;
                    _MeasuredStrength = 0.0;
                    ActualTestDate = null;
                }
            }
        } // MPa, calculated
        public double MeasuredWeight { get; set; } // kg
        public double MeasuredDensity { get; set; } // kg/m3, calculated
        public int TesterId { get; set; } // tester 1 to 6
        public int TestResult { get; set; } // 0: Not tested, 1: Pass, 2: Fail, 3: Void
        public string StatusCode { get; set; } = ""; // A to G, see notes
        public DateTime? ActualTestDate { get; set; }
        public bool Uploaded { get; set; } // 0: Not uploaded, 1: Uploaded to server
        public DateTime? UploadTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastUpdateUser { get; set; }
        public DateTime? LastUpdate { get; set; }

        // derived
        public string TestResultStr
        { get
            {
                if (TestResult == 1) return "Pass";
                else if (TestResult == 2) return "Fail";
                else if (TestResult == 3) return "Void";
                else return "Not Tested";
            }

            set 
            {
                if (value == "Pass") TestResult = 1;
                else if (value == "Fail") TestResult = 2;
                else if (value == "Void") TestResult = 3;
                else TestResult = 0;
            }
        }

        public string BarcodeStr
        {
            get 
            {
                return Barcode.ToString("D8");                  
            }
            set
            {
                long bc;
                if (value != null && long.TryParse(value, out bc))
                {
                    Barcode = bc;
                }
                else Barcode = 0;
            }
        }

        public string BatchNum;

        // From Batch
        public double Dimension;
        // From CubeSet
        public double ConcreteGrade;

    }
}
