using Microsoft.Owin;
using Owin;
using WebAccess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartupAttribute(typeof(WebAccess.Startup))]
namespace WebAccess
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }




        // In this method we will create default User roles and Admin user for login   
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            webaccessEntities context2 = new webaccessEntities();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));



            // In Startup , creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("User Administration"))
            {

                // first we create Admin
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "User Administration";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "admin@vantantech.com";
                user.Email = "admin@vantantech.com";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;

                string userPWD = "sparcman";

                var chkUser = UserManager.Create(user, userPWD);

                //Add User to Role User Admin   
                if (chkUser.Succeeded)
                {

                    AspNetUserRole collection = new AspNetUserRole();
                    collection.UserId = user.Id;
                    collection.RoleId = role.Id;
                    collection.R = true;
                    collection.C = true;
                    collection.U = true;
                    collection.D = true;
                    collection.A = true;
                    context2.AspNetUserRoles.Add(collection);
                    context2.SaveChanges();
                }
            }


            // seeding role table for each access area

            if (!roleManager.RoleExists("Store Manager"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Store Manager";
                roleManager.Create(role);

            }

            if (!roleManager.RoleExists("Store Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Store Admin";
                roleManager.Create(role);

            }

            if (!roleManager.RoleExists("VTT Sales"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "VTT Sales";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("System"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "System";
                roleManager.Create(role);
            }
  
        } 
    }
}
