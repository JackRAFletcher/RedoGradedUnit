using GradedUnitV2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GradedUnitV2.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        // Controllers

        // GET: /Admin/
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {

            List<UserViewModel> userViewModelList = new List<UserViewModel>();
            var result = UserManager.Users.OrderBy(x => x.UserName).ToList();

            foreach (var item in result)
            {
                UserViewModel userViewModel = new UserViewModel();
                userViewModel.FirstName = item.FirstName;
                userViewModel.Surname = item.Surname;
                userViewModel.MobileNumber = item.MobileNumber;
                userViewModel.UserName = item.UserName;
                userViewModel.Email = item.Email;
                userViewModel.LockoutEndDateUtc = item.LockoutEndDateUtc;

                userViewModelList.Add(userViewModel);
            }

            return View(userViewModelList);

        }


        // Users *****************************

        // GET: /Admin/Edit/Create 
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            UserViewModel userViewModel = new UserViewModel();

            ViewBag.Roles = GetAllRolesAsSelectList();

            return View(userViewModel);
        }


        // PUT: /Admin/Create
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(UserViewModel paramUserViewModel)
        {
            try
            {
                if (paramUserViewModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var FirstName = paramUserViewModel.FirstName.Trim();
                var Surname = paramUserViewModel.Surname.Trim();
                var MobileNumber = paramUserViewModel.MobileNumber.Trim();
                var Email = paramUserViewModel.Email.Trim();
                var UserName = paramUserViewModel.Email.Trim();
                var Password = paramUserViewModel.Password.Trim();


                if (FirstName == "")
                {
                    throw new Exception("No First Name");
                }
                if (MobileNumber == "")
                {
                    throw new Exception("No MobileNumber");
                }
                if (Surname == "")
                {
                    throw new Exception("No Surname");
                }
                if (Email == "")
                {
                    throw new Exception("No Email");
                }

                if (Password == "")
                {
                    throw new Exception("No Password");
                }

                // UserName is LowerCase of the Email
                UserName = Email.ToLower();

                // Create user

                var objNewAdminUser = new ApplicationUser { UserName = UserName, Email = Email, FirstName = FirstName, Surname = Surname };
                var AdminUserCreateResult = UserManager.Create(objNewAdminUser, Password);

                if (AdminUserCreateResult.Succeeded == true)
                {
                    string strNewRole = Convert.ToString(Request.Form["Roles"]);

                    if (strNewRole != "0")
                    {
                        // Put user in role
                        UserManager.AddToRole(objNewAdminUser.Id, strNewRole);
                    }

                    return Redirect("~/Admin");
                }
                else
                {
                    ViewBag.Roles = GetAllRolesAsSelectList();
                    ModelState.AddModelError(string.Empty,
                        "Error: Failed to create the user. Check password requirements.");
                    return View(paramUserViewModel);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Roles = GetAllRolesAsSelectList();
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("Create");
            }
        }


        // GET: /Admin/Edit/TestUser 
        [Authorize(Roles = "Administrator")]

        public ActionResult EditUser(string UserName)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserViewModel objUserViewModel = GetUser(UserName);
            if (objUserViewModel == null)
            {
                return HttpNotFound();
            }
            return View(objUserViewModel);
        }


        // PUT: /Admin/EditUser
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult EditUser(UserViewModel paramUserViewModel)
        {
            try
            {
                if (paramUserViewModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                UserViewModel objUserViewModel = UpdateUserViewModel(paramUserViewModel);

                if (objUserViewModel == null)
                {
                    return HttpNotFound();
                }

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(paramUserViewModel.UserName));
            }
        }


        // DELETE: /Admin/DeleteUser
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteUser(string UserName)
        {
            try
            {
                if (UserName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (UserName.ToLower() == this.User.Identity.Name.ToLower())
                {
                    ModelState.AddModelError(
                        string.Empty, "Error: Cannot delete the current user");

                    return View("EditUser");
                }

                UserViewModel objUserViewModel = GetUser(UserName);

                if (objUserViewModel == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    DeleteUser(objUserViewModel);
                }

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(UserName));
            }
        }


        // GET: /Admin/EditRoles/TestUser 
        [Authorize(Roles = "Administrator")]
        public ActionResult EditRoles(string UserName)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserName = UserName.ToLower();

            // Check that we have an actual user
            UserViewModel objUserViewModel = GetUser(UserName);

            if (objUserViewModel == null)
            {
                return HttpNotFound();
            }

            UserAndRolesViewModel objUserAndRolesViewModel =
                GetUserAndRoles(UserName);

            return View(objUserAndRolesViewModel);
        }

        // PUT: /Admin/EditRoles/TestUser 
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult EditRoles(UserAndRolesViewModel paramUserAndRolesViewModel)
        {
            try
            {
                if (paramUserAndRolesViewModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string UserName = paramUserAndRolesViewModel.UserName;
                string strNewRole = Convert.ToString(Request.Form["AddRole"]);

                if (strNewRole != "No Roles Found")
                {
                    // Go get the User
                    ApplicationUser user = UserManager.FindByName(UserName);

                    // Put user in role
                    UserManager.AddToRole(user.Id, strNewRole);
                }

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesViewModel objUserAndRolesViewModel =
                    GetUserAndRoles(UserName);

                return View(objUserAndRolesViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditRoles");
            }
        }


        // DELETE: /Admin/DeleteRole?UserName="TestUser&RoleName=Administrator
        [Authorize(Roles = "Administrator")]

        public ActionResult DeleteRole(string UserName, string RoleName)
        {
            try
            {
                if ((UserName == null) || (RoleName == null))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                UserName = UserName.ToLower();

                // Check that we have an actual user
                UserViewModel objExpandedUserDTO = GetUser(UserName);

                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }

                if (UserName.ToLower() ==
                    this.User.Identity.Name.ToLower() && RoleName == "Administrator")
                {
                    ModelState.AddModelError(string.Empty,
                        "Error: Cannot delete Administrator Role for the current user");
                }

                // Go get the User
                ApplicationUser user = UserManager.FindByName(UserName);
                // Remove User from role
                UserManager.RemoveFromRoles(user.Id, RoleName);
                UserManager.Update(user);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                return RedirectToAction("EditRoles", new { UserName = UserName });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesViewModel objUserAndRolesViewModel =
                    GetUserAndRoles(UserName);

                return View("EditRoles", objUserAndRolesViewModel);
            }
        }


        // Roles *****************************

        // GET: /Admin/ViewAllRoles
        [Authorize(Roles = "Administrator")]

        public ActionResult ViewAllRoles()
        {
            var roleManager =
                new RoleManager<IdentityRole>
                (
                    new RoleStore<IdentityRole>(new ApplicationDbContext())
                    );

            List<RoleViewModel> colRoleViewModel = (from objRole in roleManager.Roles
                                                    select new RoleViewModel
                                                    {
                                                        Id = objRole.Id,
                                                        RoleName = objRole.Name
                                                    }).ToList();

            return View(colRoleViewModel);
        }


        // GET: /Admin/AddRole
        [Authorize(Roles = "Administrator")]

        public ActionResult AddRole()
        {
            RoleViewModel objRoleViewModel = new RoleViewModel();

            return View(objRoleViewModel);
        }


        // PUT: /Admin/AddRole
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult AddRole(RoleViewModel paramRoleViewModel)
        {
            try
            {
                if (paramRoleViewModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var RoleName = paramRoleViewModel.RoleName.Trim();

                if (RoleName == "")
                {
                    throw new Exception("No RoleName");
                }

                // Create Role
                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext())
                        );

                if (!roleManager.RoleExists(RoleName))
                {
                    roleManager.Create(new IdentityRole(RoleName));
                }

                return Redirect("~/Admin/ViewAllRoles");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("AddRole");
            }
        }


        // DELETE: /Admin/DeleteUserRole?RoleName=TestRole
        [Authorize(Roles = "Administrator")]

        public ActionResult DeleteUserRole(string RoleName)
        {
            try
            {
                if (RoleName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (RoleName.ToLower() == "administrator")
                {
                    throw new Exception(String.Format("Cannot delete {0} Role.", RoleName));
                }

                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));

                var UsersInRole = roleManager.FindByName(RoleName).Users.Count();
                if (UsersInRole > 0)
                {
                    throw new Exception(
                        String.Format(
                            "Canot delete {0} Role because it still has users.",
                            RoleName)
                            );
                }

                var objRoleToDelete = (from objRole in roleManager.Roles
                                       where objRole.Name == RoleName
                                       select objRole).FirstOrDefault();
                if (objRoleToDelete != null)
                {
                    roleManager.Delete(objRoleToDelete);
                }
                else
                {
                    throw new Exception(
                        String.Format(
                            "Canot delete {0} Role does not exist.",
                            RoleName)
                            );
                }

                List<RoleViewModel> colRoleDTO = (from objRole in roleManager.Roles
                                                  select new RoleViewModel
                                                  {
                                                      Id = objRole.Id,
                                                      RoleName = objRole.Name
                                                  }).ToList();

                return View("ViewAllRoles", colRoleDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));

                List<RoleViewModel> colRoleViewModel = (from objRole in roleManager.Roles
                                                        select new RoleViewModel
                                                        {
                                                            Id = objRole.Id,
                                                            RoleName = objRole.Name
                                                        }).ToList();

                return View("ViewAllRoles", colRoleViewModel);
            }
        }


        // Utility

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }



        private List<SelectListItem> GetAllRolesAsSelectList()
        {
            List<SelectListItem> SelectRoleListItems = new List<SelectListItem>();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            var colRoleSelectList = roleManager.Roles.OrderBy(x => x.Name).ToList();

            SelectRoleListItems.Add(
                new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });

            foreach (var item in colRoleSelectList)
            {
                SelectRoleListItems.Add(
                    new SelectListItem
                    {
                        Text = item.Name.ToString(),
                        Value = item.Name.ToString()
                    });
            }

            return SelectRoleListItems;
        }



        private UserViewModel GetUser(string paramUserName)
        {
            UserViewModel objUserViewModel = new UserViewModel();

            var result = UserManager.FindByName(paramUserName);

            // If we could not find the user, throw an exception
            if (result == null) throw new Exception("Could not find the User");
            objUserViewModel.FirstName = result.FirstName;
            objUserViewModel.Surname = result.Surname;
            objUserViewModel.MobileNumber = result.MobileNumber;
            objUserViewModel.UserName = result.UserName;
            objUserViewModel.Email = result.Email;
            objUserViewModel.LockoutEndDateUtc = result.LockoutEndDateUtc;
            objUserViewModel.AccessFailedCount = result.AccessFailedCount;
            objUserViewModel.PhoneNumber = result.PhoneNumber;

            return objUserViewModel;
        }



        private UserViewModel UpdateUserViewModel(UserViewModel paramUserViewModel)
        {
            ApplicationUser result =
                UserManager.FindByName(paramUserViewModel.UserName);

            // If we could not find the user, throw an exception
            if (result == null)
            {
                throw new Exception("Could not find the User");
            }

            result.Email = paramUserViewModel.Email;

            // Lets check if the account needs to be unlocked
            if (UserManager.IsLockedOut(result.Id))
            {
                // Unlock user
                UserManager.ResetAccessFailedCountAsync(result.Id);
            }

            UserManager.Update(result);

            // Was a password sent across?
            if (!string.IsNullOrEmpty(paramUserViewModel.Password))
            {
                // Remove current password
                var removePassword = UserManager.RemovePassword(result.Id);
                if (removePassword.Succeeded)
                {
                    // Add new password
                    var AddPassword =
                        UserManager.AddPassword(
                            result.Id,
                            paramUserViewModel.Password
                            );

                    if (AddPassword.Errors.Count() > 0)
                    {
                        throw new Exception(AddPassword.Errors.FirstOrDefault());
                    }
                }
            }

            return paramUserViewModel;
        }



        private void DeleteUser(UserViewModel paramUserViewModel)
        {
            ApplicationUser user =
                UserManager.FindByName(paramUserViewModel.UserName);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
            UserManager.Update(user);
            UserManager.Delete(user);
        }



        private UserAndRolesViewModel GetUserAndRoles(string UserName)
        {
            // Go get the User
            ApplicationUser user = UserManager.FindByName(UserName);

            List<UserRoleViewModel> objUserRoleViewModel =
                (from objRole in UserManager.GetRoles(user.Id)
                 select new UserRoleViewModel
                 {
                     RoleName = objRole,
                     UserName = UserName
                 }).ToList();

            if (objUserRoleViewModel.Count() == 0)
            {
                objUserRoleViewModel.Add(new UserRoleViewModel { RoleName = "No Roles Found" });
            }

            ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

            // Create UserRolesAndPermissionsDTO
            UserAndRolesViewModel objUserAndRolesDTO =
                new UserAndRolesViewModel();
            objUserAndRolesDTO.UserName = UserName;
            objUserAndRolesDTO.UserRoles = objUserRoleViewModel;
            return objUserAndRolesDTO;
        }



        private List<string> RolesUserIsNotIn(string UserName)
        {
            // Get roles the user is not in
            var colAllRoles = RoleManager.Roles.Select(x => x.Name).ToList();

            // Go get the roles for an individual
            ApplicationUser user = UserManager.FindByName(UserName);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            var colRolesForUser = UserManager.GetRoles(user.Id).ToList();
            var colRolesUserInNotIn = (from objRole in colAllRoles
                                       where !colRolesForUser.Contains(objRole)
                                       select objRole).ToList();

            if (colRolesUserInNotIn.Count() == 0)
            {
                colRolesUserInNotIn.Add("No Roles Found");
            }

            return colRolesUserInNotIn;
        }

    }
}