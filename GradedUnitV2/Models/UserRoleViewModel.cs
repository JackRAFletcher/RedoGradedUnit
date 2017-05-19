using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradedUnitV2.Models
{
    public class UserViewModel
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Forename")]
        public string FirstName { get; set; }
        [Display(Name = "Surname")]
        public string Surname { get; set; }
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [Display(Name = "Lockout End Date Utc")]
        public DateTime? LockoutEndDateUtc { get; set; }
        public int AccessFailedCount { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<UserRolesViewModel> Roles { get; set; }
    }

    public class UserRolesViewModel
    {
        [Key]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class UserRoleViewModel
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class RoleViewModel
    {
        [Key]
        public string Id { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class UserAndRolesViewModel
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public List<UserRoleViewModel> UserRoles { get; set; }
    }
}