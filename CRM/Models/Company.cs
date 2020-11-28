using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class Company
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }

        //[StringLength(10, MinimumLength = 10)]
        [Required(ErrorMessage = "Please Enter NIP")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Please Enter 10 digits NIP Number")]
        public string NIP { get; set; }

        //[Range(1, 3)]
        [Required(ErrorMessage = "Please Enter Id Of Business")]
        public string BusinessId { get; set; }

        [StringLength(200, MinimumLength = 3)]
        [Required(ErrorMessage = "Please Enter Address")]
        public string Address { get; set; }

        [StringLength(200, MinimumLength = 3)]
        [Required(ErrorMessage = "Please Enter City")]
        public string City { get; set; }

        //[Range(1, 3)]
        [Required(ErrorMessage = "Please Enter Id Of User")]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        //[Range(typeof(DateTime), "1900-01-01", "2010-01-01" ,
        //ErrorMessage = "Value for {0} must be between {1} and {2}")]
        //[CustomDateRange(ErrorMessage = "wrong")]
        //[Date(ErrorMessage = "wrong")]
        public DateTime CreationDate { get; set; }
    }
}
