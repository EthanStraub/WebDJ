﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.IO;
using System.Web;
using WebDjProject.Models;
using WebDjProject.Controllers;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(WebDjProject.Startup))]
namespace WebDjProject
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
            HomeController controller = new HomeController();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);


                //Here we create a Admin super user who will maintain the website
                var user = new ApplicationUser();
                user.UserName = "ethan";
                user.Email = "ethan.straub@gmail.com";

                string userPWD = "Carbuncle#9";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating Registered User role    
            if (!roleManager.RoleExists("RegisteredUser"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "RegisteredUser";
                roleManager.Create(role);

            }

            // creating Creating Premium User role    
            if (!roleManager.RoleExists("PremiumUser"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "PremiumUser";
                roleManager.Create(role);

            }
        }
    }
}
