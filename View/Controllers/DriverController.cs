using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;

public class DriverController : Controller
{
    private readonly DriverRepository _driverRepository;

    public DriverController(DriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public IActionResult DriverCreate()
    {
        ViewData["Drivers"] = _driverRepository.GetAllDrivers();
        return View();
    }

    [HttpPost]
    public IActionResult DriverCreate(Driver model)
    {
        if (ModelState.IsValid)
        {
            _driverRepository.AddDriver(model);
            ViewData["Drivers"] = _driverRepository.GetAllDrivers();
            return View();
        }

        ViewData["Drivers"] = _driverRepository.GetAllDrivers();
        return View(model);
    }

    public IActionResult EditDriver(int id)
    {
        var model = _driverRepository.GetDriver(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult EditDriver(int id, Driver driver)
    {
        if (ModelState.IsValid)
        {
            _driverRepository.UpdateDriver(id, driver);
            ViewData["Drivers"] = _driverRepository.GetAllDrivers();
            return View("DriverCreate");
        }

        ViewData["Drivers"] = _driverRepository.GetAllDrivers();
        return View(driver);
    }

    [HttpPost]
    public IActionResult DeleteDriver(int id)
    {
        _driverRepository.DeleteDriver(id);
        ViewData["Drivers"] = _driverRepository.GetAllDrivers();
        return View("DriverCreate");
    }
}