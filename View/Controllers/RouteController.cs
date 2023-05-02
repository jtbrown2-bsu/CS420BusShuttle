using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;

public class RouteController : Controller
{
    private readonly RouteRepository _routeRepository;

    public RouteController(RouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public async Task<IActionResult> Index()
    {
        var routes = await _routeRepository.Get();
        return View(routes);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Core.Models.Route model)
    {
        if (ModelState.IsValid)
        {
            await _routeRepository.Add(model);
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _routeRepository.Get(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Core.Models.Route model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _routeRepository.Update(model);
            }
            catch
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var model = await _routeRepository.Get(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            await _routeRepository.Delete(id);
        }
        catch
        {
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
