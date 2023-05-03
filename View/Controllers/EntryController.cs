using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using View.Models;

namespace View.Controllers;
[Authorize]
public class EntryController : Controller
{
    private readonly IEntryRepository _entryRepository;
    private readonly IStopRepository _stopRepository;
    private readonly IBusRepository _busRepository;
    private readonly ILoopRepository _loopRepository;
    private readonly UserManager<Driver> _userManager;

    public EntryController(IEntryRepository entryRepository, IStopRepository stopRepository, UserManager<Driver> userManager, IBusRepository busRepository, ILoopRepository loopRepository)
    {
        _entryRepository = entryRepository;
        _stopRepository = stopRepository;
        _userManager = userManager;
        _busRepository = busRepository;
        _loopRepository = loopRepository;
    }

    private async Task<List<SelectListItem>> GetAvailableBusses()
    {
        var busses = await _busRepository.Get();
        var viewbagSelect = new List<SelectListItem>();
        foreach (var bus in busses)
        {
            viewbagSelect.Add(new SelectListItem
            {
                Text = bus.BusNumber.ToString(),
                Value = bus.Id.ToString()
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

    private async Task<List<SelectListItem>> GetAvailableStops(int loopId)
    {
        var loop = await _loopRepository.Get(loopId);
        var viewbagSelect = new List<SelectListItem>();
        foreach (var route in loop.Routes)
        {
            viewbagSelect.Add(new SelectListItem
            {
                Text = route.Stop.Name,
                Value = route.Stop.Id.ToString()
            });
        }
        return viewbagSelect;
    }

    public async Task<IActionResult> Index()
    {
        var entries = await _entryRepository.Get();
        return View(entries);
    }

    public async Task<IActionResult> StartDriving()
    {
        ViewBag.AvailableBusses = await GetAvailableBusses();
        ViewBag.AvailableLoops = await GetAvailableLoops();

        return View(new EntryStartViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> StartDriving(EntryStartViewModel model)
    {
        if (ModelState.IsValid)
        {
            return RedirectToAction("Create", new {busId = model.BusId, loopId = model.LoopId});
        }

        return View(model);
    }

    public async Task<IActionResult> Create(int busId, int loopId)
    {
        ViewBag.AvailableStops = await GetAvailableStops(loopId);

        return View(new EntryViewModel
        {
            BusId = busId,
            LoopId = loopId
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(EntryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            var routeId = (await _loopRepository.Get(model.LoopId)).Routes.Where(r => r.StopId == model.StopId).First().Id;
            var entry = new Entry
            {
                DriverId = userId,
                BusId = model.BusId,
                LoopId = model.LoopId,
                StopId = model.StopId,
                RouteId = routeId,
                Boarded = model.Boarded,
                LeftBehind = model.LeftBehind,
                Timestamp = DateTime.Now
            };
            await _entryRepository.Add(entry);
            return Ok();
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _entryRepository.Get(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Entry model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _entryRepository.Update(model);
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
        var model = await _entryRepository.Get(id);

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
            await _entryRepository.Delete(id);
        }
        catch
        {
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
