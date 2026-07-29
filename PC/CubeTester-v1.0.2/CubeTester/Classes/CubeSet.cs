using System;
using System.ComponentModel.DataAnnotations;

namespace CubeTester.Classes
{
    public class CubeSet
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string SpecId { get; set; } // S206 and more
        [Required]
        public string TestCriteria { get; set; }
        [Required]
        public int ConcreteGrade { get; set; }
        [Required]
        public string ConcreteType { get; set; }
        [Required]
        public double CharacteristicStrength { get; set; }
        [Required]
        public double StdDeviation { get; set; }
        [Required]
        public string SupplierId { get; set; }
        public string Location { get; set; }
        public DateTime? CastingDate { get; set; }

        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastUpdateUser { get; set; }
        public DateTime? LastUpdate { get; set; }

        // derived
        public string ProjectCode;
    }
}
