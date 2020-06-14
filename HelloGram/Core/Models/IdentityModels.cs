using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelloGram.Core.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        public string Image { get; set; }

        public DateTime? Age { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength(32, ErrorMessage = "City name can't be more than 32 letters")]
        public string City { get; set; }

        [MaxLength(32, ErrorMessage = "Country name can't be more than 32 letters")]
        public string Country { get; set; }

        [MaxLength(64, ErrorMessage = "University name can't be more than 64 letters")]
        public string University { get; set; }

        [MaxLength(32, ErrorMessage = "Country name can't be more than 32 letters")]
        public string Company { get; set; }

        [MaxLength(32, ErrorMessage = "Country name can't be more than 32 letters")]
        public string Position { get; set; }

        [MaxLength(255, ErrorMessage = "Country name can't be more than 32 letters")]
        public string About { get; set; }

        public string FacebookLink { get; set; }
        
        public string TwitterLink { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public static class IdentityExtension
    {
        public static string GetName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Name");
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}