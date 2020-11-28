using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please Enter User Name")]
        [Display(Name = "Login")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
