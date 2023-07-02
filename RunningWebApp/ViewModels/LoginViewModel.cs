using System.ComponentModel.DataAnnotations;

namespace RunningWebApp.ViewModels
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "Email Address is required")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
    }
}
