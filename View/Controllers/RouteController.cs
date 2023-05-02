using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using View.Models;

namespace View.Controllers;

public class RouteController : Controller
{
    private readonly IRouteRepository _routeRepository;
    private readonly IStopRepository _stopRepository;

    public RouteController(IRouteRepository routeRepository, IStopRepository stopRepository)
    {
        _routeRepository = routeRepository;
        _stopRepository = stopRepository;
    }

    public async Task<IActionResult> Index()
    {
        var routes = await _routeRepository.Get();
        return View(routes);
    }

    public async Task<IActionResult> Create()
    {
        var stops = await _stopRepository.Get();
        var viewModel = new RouteViewModel();
        foreach(var stop in stops)
        {
            viewModel.AvailableStops.Add(new SelectListItem
            {
                Text = stop.Name,
                Value = stop.Id.ToString()
            });
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RouteViewModel model)
    {
        if (ModelState.IsValid)
        {
            var route = new Core.Models.Route
            {
                Order = model.Order,
                StopId = model.SelectedStopId,
            };
            await _routeRepository.Add(route);
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
