using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models.DTO
{
    public class ChangePasswordDTO
    {
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 6)]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 6)]
        public string NewPassword { get; set; }
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 6)]
        public string ConfirmPassword { get; set; }
    }
}
