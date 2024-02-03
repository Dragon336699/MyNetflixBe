using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NetflixBe.Models
{
    public class ApplicationUserModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}

