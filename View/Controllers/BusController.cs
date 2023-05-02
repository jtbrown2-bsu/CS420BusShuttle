using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;

public class BusController : Controller
{
    private readonly BusRepository _busRepository;

    public BusController(BusRepository busRepository)
    {
        _busRepository = busRepository;
    }

    public async Task<IActionResult> Index()
    {
        var buses = await _busRepository.Get();
        return View(buses);
    }

    public IActionResult BusCreate()
    {
        ViewData["Buses"] = _busRepository.GetAllBusses();
        return View();
    }

    [HttpPost]
    public IActionResult BusCreate(Bus model)
    {
        if (ModelState.IsValid)
        {
            _busRepository.AddBus(model);
            ViewData["Buses"] = _busRepository.GetAllBusses();
            return View();
        }

        ViewData["Buses"] = _busRepository.GetAllBusses();
        return View(model);
    }

    public IActionResult EditBus(int id)
    {
        var model = _busRepository.GetBus(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult EditBus(int id, Bus bus)
    {
        if (ModelState.IsValid)
        {
            _busRepository.UpdateBus(id, bus);
            ViewData["Buses"] = _busRepository.GetAllBusses();
            return View("BusCreate");
        }

        ViewData["Buses"] = _busRepository.GetAllBusses();
        return View(bus);
    }

    [HttpPost]
    public IActionResult DeleteBus(int id)
    {
        _busRepository.DeleteBus(id);
        ViewData["Buses"] = _busRepository.GetAllBusses();
        return View("BusCreate");
    }
}
