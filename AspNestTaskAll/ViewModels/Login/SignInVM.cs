using System.ComponentModel.DataAnnotations;


namespace AspNestTaskAll.ViewModels.Login
{
    public class SignInVM
    {
        [Required]
        public string UserNameOrEmail { get; set; }
        [Required,DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
