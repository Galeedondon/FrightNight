using System.ComponentModel.DataAnnotations;
using AntDesign;
using Microsoft.AspNetCore.Components;

namespace MyAntDesignApp.Models
{
    public class AuthRequest
    {
        [Required]
        public string Account { get; set; }
        [Required]
        public string Password { get; set; }
        
        public string CheckPassword { get; set; }
        public bool RememberMe { get; set; } = true;
    }
}