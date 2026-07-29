using System;
using System.ComponentModel.DataAnnotations;

namespace CubeTester.Classes
{
    public enum CubeTestResult
    {
        None,
        Pass,
        Fail,
        Pending
    }

    public class Cube
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public long Barcode { get; set; } // max 8 digits
        [Required]
        public string SampleRef { get; set; }
        [Required]
        public int ScoNum { get; set; }
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
        public double MeasuredStrength { get; set; } // MPa, calculated
        public double MeasuredMaxForce { get; set; } // kN, from tester
        public double MeasuredWeight { get; set; } // kg, from scale
        public double MeasuredDensity { get; set; } // kg/m3, calculated
        public int TesterId { get; set; } // tester 1 to 6
        public int TestResult { get; set; } // 0: Not tested, 1: Pass, 2: Fail, 3: Pending
        public string StatusCode { get; set; } // A to G, see notes
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
                else if (TestResult == 3) return "Pending";
                else return "";
            }

            set { }
        }

        public string BatchNum;
    }
}
