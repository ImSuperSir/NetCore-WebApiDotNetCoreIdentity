using System.ComponentModel.DataAnnotations;

namespace ImSuperSir.Security.Controllers
{
  public class UserCredentials
  {
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
  }
}