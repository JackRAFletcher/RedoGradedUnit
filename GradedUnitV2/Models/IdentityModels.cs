using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
//jack fletcher 19/5/17 
namespace GradedUnitV2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //additional user attributes 
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string MobileNumber { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {

            if (!context.Users.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var userStore = new UserStore<ApplicationUser>(context);

                //_____________________________________________________________________
                //populating the Role table
                if (!roleManager.RoleExists(RoleNames.ROLE_ADMINISTRATOR))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_ADMINISTRATOR));
                }

                if (!roleManager.RoleExists(RoleNames.ROLE_STAFF))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_STAFF));
                }

                if (!roleManager.RoleExists(RoleNames.ROLE_CUSTOMER))
                {

                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_CUSTOMER));
                }

                //_________________________________________________________________

                string userName = "admin@admin.com";
                string password = "123456";

                //  var passwordHash = new PasswordHasher();
                //  password = passwordHash.HashPassword(password);

                //create Admin user and role

                var user = userManager.FindByName(userName);
                if (user == null)
                {

                    var newUser = new ApplicationUser()
                    {
                        FirstName = "Administrator",
                        Surname = "Admin",
                        MobileNumber = "01316671900",
                        PhoneNumber = "0141454637",
                        UserName = userName,
                        Email = userName,
                        EmailConfirmed = true,

                    };

                    userManager.Create(newUser, password);
                    userManager.AddToRole(newUser.Id, RoleNames.ROLE_ADMINISTRATOR);
                }
                //___________________________________________________________________________________
                var user2 = userManager.FindByName("staff@test.com");
                if (user2 == null)
                {

                    var newUser2 = new ApplicationUser()
                    {
                        FirstName = "Joe",
                        Surname = "Black",
                        MobileNumber = "01316671900",
                        PhoneNumber = "0141454637",
                        UserName = "staff@test.com",
                        Email = "staff@test.com",
                        EmailConfirmed = true,

                    };

                    userManager.Create(newUser2, "staff123");
                    userManager.AddToRole(newUser2.Id, RoleNames.ROLE_STAFF);
                }

                //__________________________________________________________________
                var user3 = userManager.FindByName("customer@test2.com");
                if (user3 == null)
                {

                    var newUser3 = new ApplicationUser()
                    {
                        FirstName = "Suzzie",
                        Surname = "White",
                        MobileNumber = "01316671900",
                        PhoneNumber = "0141454637",
                        UserName = "customer@test2.com",
                        Email = "customer@test2.com",
                        EmailConfirmed = true,

                    };

                    userManager.Create(newUser3, "customer123");
                    userManager.AddToRole(newUser3.Id, RoleNames.ROLE_CUSTOMER);
                }




            }
            //______________________________________________________

            base.Seed(context);
            context.SaveChanges();
        }



    }
}