using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;

public class AccountController : Controller
{
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        if (username == "admin" && password == "password")
        {
            return RedirectToAction("Index", "Home");
        }
        ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
    }
}
