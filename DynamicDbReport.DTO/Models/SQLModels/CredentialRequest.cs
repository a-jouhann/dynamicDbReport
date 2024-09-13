using System.ComponentModel.DataAnnotations;
using DynamicDbReport.DTO.Models.Public;

namespace DynamicDbReport.DTO.Models.SQLModels;

public class CredentialRequest
{
    [Required]
    public EngineName Engine { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is required")]
    [MinLength(1, ErrorMessage = "{0} Min length is: 1")]
    public string ServerAddress { get; set; }

    public string DBPort { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is required")]
    [MinLength(2, ErrorMessage = "{0} Min length is: 2")]
    public string Username { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is required")]
    [MinLength(2, ErrorMessage = "{0} Min length is: 2")]
    public string Password { get; set; }

    public string DbName { get; set; }

    public string DBVersion { get; set; }
}
