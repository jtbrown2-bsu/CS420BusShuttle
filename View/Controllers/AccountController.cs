using Microsoft.AspNetCore.Mvc;
using View.Models;

namespace View.Controllers;

public class AccountController : Controller
{
    public ActionResult Login()
    {
        ViewBag.IsLoginPage = true;
        return View();
    }

    [HttpPost]
    public ActionResult Login(LoginModel model)
    {
        if (ModelState.IsValid && model is { Username: "admin", Password: "password" })
        {
            return RedirectToAction("Index", "Home");
        }
        ViewBag.ErrorMessage = "Invalid username or password.";
        ViewBag.IsLoginPage = true;
            return View(model);
    }
}
