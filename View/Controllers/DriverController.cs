using Microsoft.AspNetCore.Mvc;
using View.Models;

namespace View.Controllers
{
    public class DriverController : Controller
    {
        private static List<DriverModel> _drivers = new List<DriverModel>();

        public IActionResult DriverCreate()
        {
            ViewData["Drivers"] = _drivers;
            return View();
        }

        [HttpPost]
        public IActionResult DriverCreate(DriverModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = _drivers.Count + 1;
                _drivers.Add(model);
                ViewData["Drivers"] = _drivers;
                return View();
            }

            ViewData["Drivers"] = _drivers;
            return View(model);
        }
        
        public IActionResult EditDriver(int id)
        {
            var model = _drivers.FirstOrDefault(b => b.Id == id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        

        [HttpPost]
        public IActionResult EditDriver(DriverModel model)
        {
            if (ModelState.IsValid)
            {
                var driver = _drivers.FirstOrDefault(b => b.Id == model.Id);

                if (driver == null)
                {
                    return NotFound();
                }

                driver.FirstName = model.FirstName;
                driver.LastName = model.LastName;
                ViewData["Drivers"] = _drivers;

                return View("DriverCreate");
            }

            ViewData["Drivers"] = _drivers;
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteDriver(int id)
        {
            var driver = _drivers.FirstOrDefault(b => b.Id == id);

            if (driver == null)
            {
                return NotFound();
            }

            _drivers.Remove(driver);

            ViewData["Drivers"] = _drivers;
            return View("DriverCreate");
        }
    }
}