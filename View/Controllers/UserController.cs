using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using View.Models;

namespace View.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<Driver> _userManager;
        private readonly SignInManager<Driver> _signInManager;

        public UserController(UserManager<Driver> userManager, SignInManager<Driver> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                } else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                } 
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Driver { 
                    UserName = model.Email, 
                    FirstName = model.FirstName, 
                    LastName = model.LastName 
                };

                bool isFirstUser = !_userManager.Users.ToList().Any();
                if (isFirstUser)
                {
                    user.IsManager = true;
                    user.IsActivated = true;
                }
                else
                {
                    user.IsManager = false;
                    user.IsActivated = false;
                }
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (user.IsManager)
                    {
                        await _userManager.AddClaimAsync(user, new Claim("IsManager", "true"));
                    }

                    await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Activate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                throw new Exception("No user found.");
            }
            await _userManager.AddClaimAsync(user, new Claim("IsActivated", "true"));
            user.IsActivated = true;
            await _userManager.UpdateAsync(user);
            return Ok();
        }
    }
}
