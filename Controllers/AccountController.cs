using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Hostel2._0.Models;
using Hostel2._0.Models.ViewModels;
using System.Threading.Tasks;
using System;

namespace Hostel2._0.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("Account/Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("Account/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Try to find user by email first
                var user = await _userManager.FindByEmailAsync(model.Login);
                
                // If not found by email, try to find by username
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.Login);
                }

                if (user != null)
                {
                    var userName = user.UserName ?? throw new InvalidOperationException("User name cannot be null");
                    var result = await _signInManager.PasswordSignInAsync(
                        userName, 
                        model.Password, 
                        model.RememberMe, 
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        // Redirect to the dashboard index which will handle role-based routing
                        return RedirectToAction("Index", "Dashboard");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your username/email and password.");
            }

            return View(model);
        }

        private async Task<IActionResult> RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return await Task.FromResult(Redirect(returnUrl));
            }

            return await Task.FromResult(RedirectToAction("Index", "Dashboard"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

#if DEBUG
        [HttpGet]
        [Route("Account/AdminStatus")]
        public async Task<IActionResult> AdminStatus()
        {
            var adminEmail = "mohebullah.cse@gmail.com";
            var user = await _userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                return Content("Admin user not found.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            return Content($"Admin user found.\nEmailConfirmed: {user.EmailConfirmed}\nRoles: {string.Join(", ", roles)}");
        }
#endif
    }
} 