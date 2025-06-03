using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Model.DTO
{
    public class LoginRequestDTO
    {
        public string userName_email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
