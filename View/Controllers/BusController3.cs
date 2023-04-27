using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;

public class BusController3 : Controller
{
    private readonly BusRepository _busRepositor;

    public BusController3(BusRepository busRepository)
    {
        _busRepositor = busRepository;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetAllBuses()
    {
        return View(_busRepositor.GetAllBusses());
    }

    [HttpGet]
    public IActionResult GetBusById(int id)
    {
        var bus = _busRepositor.GetBus(id);
        return View(bus);
    }

    [HttpPost]
    public IActionResult AddBus(Bus bus)
    {
        if (!ModelState.IsValid) return View(bus);

        _busRepositor.AddBus(bus);
        return RedirectToAction(nameof(Index));

    }

    [HttpPut]
    public IActionResult EditBus(int id, Bus bus)
    {
        if (!ModelState.IsValid) return View(bus);
        _busRepositor.UpdateBus(id, bus);
        return RedirectToAction(nameof(System.Index));

    }

    [HttpDelete]
    public IActionResult DeleteBus(int id)
    {
        _busRepositor.DeleteBus(id);
        return RedirectToAction(nameof(Index));
    }
}