/*
using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using View.Models;

namespace View.Controllers;

public class DriverController2 : Controller
{
    private readonly DriverRepository _driverRepository;

    public DriverController2(DriverRepository driverRepository)
    {
        _driverRepository = driverRepository; 
    }
    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult DriverTable()
    {
        
        return View();
    }

    [HttpGet]
    public IActionResult GetDriverId(int id)
    {
        var driver = _driverRepository.GetDriver(id);
        if (driver == null)
        {
            return NotFound();
        }

        return View(driver);
    }

    [HttpPost]
    public IActionResult DriverCreate( Driver driver)
    {
        if (ModelState.IsValid)
        {
            _driverRepository.AddDriver(driver);
            return (RedirectToAction(nameof(Index)));
        }
        var driverViewModel = new DriverModel
        {
            FirstName = driver.FirstName,
            Id = driver.Id,
            LastName = driver.LastName
        };

        return View(driverViewModel);
    }

    [HttpPut]
    public IActionResult EditDriver(int id, Driver driver)
    {
        if (ModelState.IsValid)
        {
            _driverRepository.UpdateDriver(id, driver);
            return RedirectToAction(nameof(Index));
        }

        var driverViewModel = new DriverModel
        {
            FirstName = driver.FirstName,
            Id = driver.Id,
            LastName = driver.LastName
        };
        return View(driverViewModel);
    }
    
    [HttpDelete]
    public IActionResult DeleteConfirmed(int id)
    {
        _driverRepository.DeleteDriver(id);
        return RedirectToAction(nameof(Index));
    }
    
}
*/