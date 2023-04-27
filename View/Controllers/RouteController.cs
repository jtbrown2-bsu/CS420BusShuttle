using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Route = Core.Models.Route;

namespace View.Controllers;

public class RouteController : Controller
{
    private readonly RouteRepository _routeRepository;

    public RouteController(RouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult GetAllRoutes()
    {
        return View(_routeRepository.GetAllRoutes());
    }

    [HttpGet]
    public IActionResult GetRouteById(int id)
    {
        var route = _routeRepository.GetRoute(id);
        return View(route);
    }

    [HttpPost]
    public IActionResult AddRoute(Route route)
    {
        if (!ModelState.IsValid) return View(route);
        
        _routeRepository.AddRoute(route);
        return RedirectToAction(nameof(Index));

    }

    [HttpPut]
    public IActionResult EditRoute(int id, Route route)
    {
        if (!ModelState.IsValid) return View(route);
        _routeRepository.UpdateRoute(id, route);
        return RedirectToAction(nameof(System.Index));

    }

    [HttpDelete]
    public IActionResult DeleteRoute(int id)
    {
        _routeRepository.DeleteRoute(id);
        return RedirectToAction(nameof(Index));
    }
}