using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Tasks4U.Models
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Имя пользователя для отображения в списке
        /// </summary>
        [StringLength(200)]
        public string DisplayName { get; set; }
    }
}
