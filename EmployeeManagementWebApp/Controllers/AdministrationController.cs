using EmployeeManagementWebApp.Models;
using EmployeeManagementWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagementWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AdministrationController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id : {userId} cannot be found";
                return View("NotFound");
            }
            else
            {
                var existingUserClaims = await _userManager.GetClaimsAsync(user);

                var model = new UserClaimsViewModel
                {
                    UserId = userId
                };


                foreach (Claim claim in ClaimsStore.AllClaims)
                {
                    UserClaim userClaim = new UserClaim
                    {
                        ClaimType = claim.Type
                    };

                    //If the user has the claim, set IsSelected property to true, so the checkbox next to the claim is checked on the UI
                    if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                    {
                        userClaim.IsSelected = true;
                    }
                    model.Claims.Add(userClaim);
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id : {model.UserId} cannot be found";
                return View("NotFound");
            }
            else
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var result = await _userManager.RemoveClaimsAsync(user, claims);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot remove user existing claims");
                    return View(model);
                }


                //result = await _userManager.AddClaimsAsync(user, ClaimsStore.AllClaims.Where(claim => claim.Type.Equals(model.Claims.Where(c => c.IsSelected).Select(claim => claim.ClaimType))));

                result = await _userManager.AddClaimsAsync(user, model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));  //by pragim

                //foreach (var claim in model.Claims)
                //{
                //    if (claim.IsSelected)
                //    {
                //        Claim selectedClaim = ClaimsStore.AllClaims.FirstOrDefault(c => c.Type == claim.ClaimType);
                //        result = await _userManager.AddClaimAsync(user, selectedClaim);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add selected claims to user");
                    return View(model);
                }
                //    }
                //}

                return RedirectToAction("EditUser", new { Id = model.UserId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id : {userId} cannot be found";
                return View("NotFound");
            }
            else
            {
                var model = new List<UserRolesViewModel>();
                //IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
                foreach (var role in _roleManager.Roles)
                {
                    var userRolesViewModel = new UserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    };

                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRolesViewModel.IsSelected = true;
                    }
                    else
                    {
                        userRolesViewModel.IsSelected = false;
                    }
                    model.Add(userRolesViewModel);
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            #region Either this
            //for (int i = 0; i < model.Count; i++)
            //{
            //    var role = await _roleManager.FindByIdAsync(model[i].RoleId);

            //    IdentityResult result = null;

            //    if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
            //    {
            //        result = await _userManager.AddToRoleAsync(user, role.Name);
            //    }
            //    else if (!model[i].IsSelected && (await _userManager.IsInRoleAsync(user, role.Name)))
            //    {
            //        result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            //    }
            //    else
            //    {
            //        continue;
            //    }

            //    if (result.Succeeded)
            //    {
            //        if (i < model.Count - 1)
            //            continue;
            //        else
            //            return RedirectToAction("EditUser", new { Id = userId });
            //    }
            //}
            #endregion

            #region Or this
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing role");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, model.Where(userRole => userRole.IsSelected).Select(role => role.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            #endregion
            return RedirectToAction("EditUser", new { Id = userId });
        }


        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await _roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"Error deleting role {ex}");
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are user in this role, please remove the user from the role and then try to delete";
                    return View("Error");
                }

            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("ListUsers");
            }

        }

        //[AllowAnonymous]
        [HttpGet]
        [Authorize(Policy = "CreateRolePolicy")]
        public IActionResult CreateRole()
        {
            return View();
        }

        //[AllowAnonymous]
        [HttpPost]
        [Authorize(Policy = "CreateRolePolicy")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel createRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                IdentityRole identityRole = new IdentityRole
                {
                    Name = createRoleViewModel.RoleName
                };

                // Saves the role in the underlying AspNetRoles table
                IdentityResult result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(createRoleViewModel);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string Id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {Id}  cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
            };

            IList<ApplicationUser> usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            foreach (var user in usersInRole)
            {
                model.Users.Add(user.UserName);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id}  cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < model.Count - 1)
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {Id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles

            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
