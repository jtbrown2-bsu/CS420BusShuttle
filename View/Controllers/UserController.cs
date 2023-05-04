using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using View.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace View.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<Driver> _userManager;
        private readonly SignInManager<Driver> _signInManager;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<Driver> userManager, SignInManager<Driver> signInManager, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
                    _logger.LogInformation("Logging in user {email} succeeded at {time}.", model.Email, DateTime.Now);
                    return RedirectToAction("Index", "Home");
                } else
                {
                    _logger.LogError("Logging in user {email} failed at {time}.", model.Email, DateTime.Now);
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
                        _logger.LogInformation("Manager with email {email} created at {time}.", model.Email, DateTime.Now);
                    } else
                    {
                        _logger.LogInformation("Non-manager with email {email} created at {time}.", model.Email, DateTime.Now);
                    }

                    await _signInManager.SignInAsync(user, model.RememberMe);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Registering account with email {email} failed with error {error} at {time}.", model.Email, error.Description, DateTime.Now);
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
                _logger.LogError("Activating user id {id} failed at {time}.", id, DateTime.Now);
                throw new Exception("No user found.");
            }
            await _userManager.AddClaimAsync(user, new Claim("IsActivated", "true"));
            user.IsActivated = true;
            await _userManager.UpdateAsync(user);
            _logger.LogInformation("Activating user id {id} succeeded at {time}.", id, DateTime.Now);
            return Ok();
        }
    }
}
