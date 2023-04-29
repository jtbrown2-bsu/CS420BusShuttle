using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;

public class StopController : Controller
{
   
    private readonly StopRepository _stopRepository;

    public StopController(StopRepository stopRepository)
    {
        _stopRepository = stopRepository;
    }
    public IActionResult StopCreate()
    {
        ViewData["Stops"] = _stopRepository.GetAllStops();
        return View();
    }

    [HttpPost]
    public IActionResult StopCreate(Stop stop)
    {
        if (ModelState.IsValid)
        {
            _stopRepository.AddStop(stop);
            ViewData["Stops"] = _stopRepository.GetAllStops();
            return View();
        }

        ViewData["Stops"] = _stopRepository.GetAllStops();
        return View(stop);


    }

    public IActionResult EditStop(int id)
    {
        var model = _stopRepository.GetStop(id);

        if (model == null)
        {
            return NotFound();

        }

        return View(model);
    }

    [HttpPost]
    public IActionResult EditStop(int id,Stop stop)
    {
        if (ModelState.IsValid)
        {
            _stopRepository.UpdateStop(id, stop);
            ViewData["Loops"] = _stopRepository.GetAllStops();
            return View("StopCreate");
        }

        ViewData["Stops"] = _stopRepository.GetAllStops();
        return View(stop);
      }

    [HttpPost]
    public IActionResult DeleteBus(int id)
    {
        _stopRepository.DeleteStop(id);
        ViewData["Stops"] = _stopRepository.GetAllStops();
        return View("StopCreate");
    }
}