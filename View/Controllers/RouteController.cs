using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using View.Models;

namespace View.Controllers;

public class RouteController : Controller
{
    private readonly IRouteRepository _routeRepository;
    private readonly IStopRepository _stopRepository;
    private readonly ILoopRepository _loopRepository;

    public RouteController(IRouteRepository routeRepository, IStopRepository stopRepository, ILoopRepository loopRepository)
    {
        _routeRepository = routeRepository;
        _stopRepository = stopRepository;
        _loopRepository = loopRepository;
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

    private async Task<List<SelectListItem>> GetAvailableLoops()
    {
        var loops = await _loopRepository.Get();
        var viewbagSelect = new List<SelectListItem>();
        foreach (var loop in loops)
        {
            viewbagSelect.Add(new SelectListItem
            {
                Text = loop.Name,
                Value = loop.Id.ToString()
            });
        }
        return viewbagSelect;
    }

    public async Task<IActionResult> Index(string loopId)
    {
        var loops = await GetAvailableLoops();
        var loopIds = loops.Select(l => l.Value);
        ViewBag.AvailableLoops = loops;
            
        var routes = await _routeRepository.Get();
        if (!loopIds.Contains(loopId))
        {
            var defaultId = "0";
            if(loopIds.Any())
            {
                defaultId = loopIds.ToList().First();
            }
            
            return View(routes.Where(r => r.LoopId == int.Parse(defaultId)).OrderBy(r => r.Order));
        }
        loops.Where(l => l.Value == loopId).First().Selected = true;
        ViewBag.AvailableLoops = loops;
        return View(routes.Where(r => r.LoopId == int.Parse(loopId)).OrderBy(r => r.Order));
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.AvailableStops = await GetAvailableStops();
        ViewBag.AvailableLoops = await GetAvailableLoops();

        return View(new RouteViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(RouteViewModel model)
    {
        if (ModelState.IsValid)
        {
            var routeCount = (await _loopRepository.Get(model.LoopId)).Routes.Count;
            var route = new Core.Models.Route
            {
                Order = routeCount + 1,
                StopId = model.StopId,
                LoopId = model.LoopId
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

        var viewModel = new RouteEditViewModel
        {
            Id = model.Id,
            Order = model.Order,
            StopId = model.StopId,
            LoopId = model.LoopId
        };


        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(RouteEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var route = new Core.Models.Route
            {
                Id = model.Id,
                Order = model.Order,
                StopId = model.StopId,
                LoopId = model.LoopId
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

    [HttpPost, ActionName("SwapOrders")]
    public async Task<IActionResult> SwapOrders(int currentId, int updatedId)
    {
        await _routeRepository.SwapOrders(currentId, updatedId);
        return Ok();
    }
}
