using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "Please Enter Surname")]
        public string Surname { get; set; }

        [RegularExpression(@"^(\d{9})$", ErrorMessage = "Please Enter Right Phone Number")]
        [Required(ErrorMessage = "Please Enter Phone Number")]
        public string Phone { get; set; }

        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@""]+(\.[^<>()[\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please Enter Right Email")]
        [Required(ErrorMessage = "Please Enter Email")]
        public string Email { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "Please Enter Position")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Please Enter Id Of Business")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please Enter Id Of User")]
        public int UserId { get; set; }

        public int IsDeleted { get; set; }
    }
}
