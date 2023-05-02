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

    private async Task<List<SelectListItem>> GetAvailableStops()
    {
        var stops = await _stopRepository.Get();
        var viewbagSelect = new List<SelectListItem>();
        foreach (var stop in stops)
        {
            viewbagSelect.Add(new SelectListItem
            {
                Text = stop.Name,
                Value = stop.Id.ToString()
            });
        }
        return viewbagSelect;
    }

    public async Task<IActionResult> Index()
    {
        var routes = await _routeRepository.Get();
        return View(routes);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.AvailableStops = await GetAvailableStops();
        
        return View(new RouteViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(RouteViewModel model)
    {
        if (ModelState.IsValid)
        {
            var route = new Core.Models.Route
            {
                Order = model.Order,
                StopId = model.StopId,
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

        ViewBag.AvailableStops = await GetAvailableStops();

        var viewModel = new RouteViewModel
        {
            Id = model.Id,
            Order = model.Order,
            StopId = model.StopId
        };


        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(RouteViewModel model)
    {
        if (ModelState.IsValid)
        {
            var route = new Core.Models.Route
            {
                Id = model.Id,
                Order = model.Order,
                StopId = model.StopId,
            };
            try
            {
                await _routeRepository.Update(route);
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
