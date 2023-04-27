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
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult GetAllStops()
    {
        return View(_stopRepository.GetAllStops());
    }

    [HttpGet]
    public IActionResult GetBusById(int id)
    {
        var stop = _stopRepository.GetStop(id);
        return View(stop);
    }

    [HttpPost]
    public IActionResult AddStop(Stop stop)
    {
        if (!ModelState.IsValid) return View(stop);
        
        _stopRepository.AddStop(stop);
        return RedirectToAction(nameof(Index));

    }

    [HttpPut]
    public IActionResult EditBus(int id,Stop stop)
    {
        if (!ModelState.IsValid) return View(stop);
        _stopRepository.UpdateStop(id, stop);
        return RedirectToAction(nameof(System.Index));

    }

    [HttpDelete]
    public IActionResult DeleteBus(int id)
    {
        _stopRepository.DeleteStop(id);
        return RedirectToAction(nameof(Index));
    }
}