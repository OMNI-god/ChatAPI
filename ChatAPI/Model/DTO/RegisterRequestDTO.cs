using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Model.DTO
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "User name is required.")]
        public string userName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required.")]
        public string email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        public string password { get; set; }
        public string[] roles { get; set; }
    }
}
