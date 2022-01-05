using ERace_WebApp.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;


using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin; // for the .GetOwinContext() extension method

namespace ERace_WebApp.Security
{
    public class SecurityController
    {
        #region Constructor & Dependencies
        private readonly ApplicationUserManager UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        //constructor of SecurityController
        public SecurityController()
        {
            UserManager = HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        }
#endregion

        #region Employee/IDs
        /// <summary>
        /// Extract the EmployeeID (if it exists) for the supplied username
        /// </summary>
        /// <param name="userName">Logged-in user name; typically 
        /// <code>User.Identity.Name</code> from your web form.
        /// </param>
        /// <returns>Null, if no EmployeeID was found, or the ID of the employee</returns>
        public int? GetCurrentUserEmployeeId(string userName)
        {
            int? id = null;
            var request = HttpContext.Current.Request;
            if (request.IsAuthenticated)
            {
                var manager = request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var appUser = manager.Users.SingleOrDefault(x => x.UserName == userName);
                if (appUser != null)
                    id = appUser.EmployeeId;
            }
            return id;
        }
       
        #endregion
    }
}
