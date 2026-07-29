using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeTester.Classes
{
    public class Batch
    {
        // primary key is ScoNum, Id
        public int ScoNum { get; set; }

        public int Id { get; set; }
        public int CubeSetId { get; set; }

        int _TestAge;
        [Required]
        public int TestAge
        {
            get
            {
                return _TestAge;
            }
            set
            {
                _TestAge = value;
                if (CastingDate != null)
                {
                    TargetTestDate = ((DateTime)CastingDate).AddDays(_TestAge);
                }
            }
        }
        public DateTime? TargetTestDate { get; set; } // calculated
        [Required, Range(100, 150, ErrorMessage = "Dimension must be 100mm or 150mm.")]
        public double Dimension { get; set; } // 100 or 150
        [Required]
        public int WitnessNum { get; set; }
        public double AvgStrength { get; set; } // for cubes under batch
        public double RollingAvgStrength { get; set; } // for cubes under batch
        public char CriterionA { get; set; } // for cubes under batch
        public char CriterionB { get; set; } // for cubes under batch
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastUpdateUser { get; set; }
        public DateTime? LastUpdate { get; set; }

        // derived
        public string BatchNum { get; set; }

        public int AgeAtTest { get; set; }

        // From CubeSet
        public DateTime? CastingDate { get; set; }

    }
}
