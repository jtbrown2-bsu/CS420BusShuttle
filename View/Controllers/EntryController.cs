using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
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
    private readonly ILogger<EntryController> _logger;
    private readonly UserManager<Driver> _userManager;

    public EntryController(IEntryRepository entryRepository, IStopRepository stopRepository, UserManager<Driver> userManager, IBusRepository busRepository, ILoopRepository loopRepository, ILogger<EntryController> logger)
    {
        _entryRepository = entryRepository;
        _stopRepository = stopRepository;
        _userManager = userManager;
        _busRepository = busRepository;
        _loopRepository = loopRepository;
        _logger = logger;
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

    private async Task<List<SelectListItem>> GetAvailableDrivers()
    {
        var drivers = _userManager.Users.ToList();
        var viewbagSelect = new List<SelectListItem>();
        foreach (var driver in drivers)
        {
            viewbagSelect.Add(new SelectListItem
            {
                Text = driver.FirstName + " " + driver.LastName,
                Value = driver.Id
            });
        }
        return viewbagSelect;
    }
    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> Index(string loopId, string busId, string stopId, string driverId, string day)
    {
        var loops = await GetAvailableLoops();
        loops.Insert(0, new SelectListItem
        {
            Text = "",
            Value = ""
        });
        ViewBag.AvailableLoops = loops;

        var busses = await GetAvailableBusses();
        busses.Insert(0, new SelectListItem
        {
            Text = "",
            Value = ""
        });
        ViewBag.AvailableBusses = busses;

        var stops = await GetAvailableStops();
        stops.Insert(0, new SelectListItem
        {
            Text = "",
            Value = ""
        });
        ViewBag.AvailableStops = stops;

        var drivers = await GetAvailableDrivers();
        drivers.Insert(0, new SelectListItem
        {
            Text = "",
            Value = ""
        });
        ViewBag.AvailableDrivers = drivers;

        var entries = await _entryRepository.Get();

        if (!string.IsNullOrEmpty(loopId))
        {
            entries = entries.Where(e => e.LoopId == int.Parse(loopId)).ToList();
        }

        if (!string.IsNullOrEmpty(busId))
        {
            entries = entries.Where(e => e.BusId == int.Parse(busId)).ToList();
        }

        if (!string.IsNullOrEmpty(stopId))
        {
            entries = entries.Where(e => e.StopId == int.Parse(stopId)).ToList();
        }

        if (!string.IsNullOrEmpty(driverId))
        {
            entries = entries.Where(e => e.DriverId == driverId).ToList();
        }

        if (!string.IsNullOrEmpty(day))
        {
            entries = entries.Where(e => e.Timestamp.Date.Equals(DateTime.Parse(day).Date)).ToList();
        }


        return View(entries);
    }

    [Authorize(Policy = "ActivatedOnly")]
    public async Task<IActionResult> StartDriving()
    {
        ViewBag.AvailableBusses = await GetAvailableBusses();
        ViewBag.AvailableLoops = await GetAvailableLoops();

        return View(new EntryStartViewModel());
    }

    [HttpPost]
    [Authorize(Policy = "ActivatedOnly")]
    public async Task<IActionResult> StartDriving(EntryStartViewModel model)
    {
        if (ModelState.IsValid)
        {
            return RedirectToAction("Create", new {busId = model.BusId, loopId = model.LoopId});
        }
        _logger.LogError("Failed entry start validation at {time}.", DateTime.Now);
        return View(model);
    }

    [Authorize(Policy = "ActivatedOnly")]
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
    [Authorize(Policy = "ActivatedOnly")]
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
            try
            {
                await _entryRepository.Add(entry);
                _logger.LogInformation("Entry created by {driver} on loop {loop} for stop {stop} on bus {bus} at {time}.", entry.DriverId, entry.LoopId, entry.StopId, entry.BusId, DateTime.Now);
            }
            catch
            {
                _logger.LogError("Entry failed created by {driver} on loop {loop} for stop {stop} on bus {bus} at {time}.", entry.DriverId, entry.LoopId, entry.StopId, entry.BusId, DateTime.Now);
                return NotFound();
            }
            return Ok();
        }
        _logger.LogError("Failed entry create validation at {time}.", DateTime.Now);
        return View(model);
    }

    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> Add()
    {
        ViewBag.AvailableLoops = await GetAvailableLoops();
        ViewBag.AvailableBusses = await GetAvailableBusses();
        ViewBag.AvailableStops = await GetAvailableStops();
        ViewBag.AvailableDrivers= await GetAvailableDrivers();

        var viewModel = new EntryAddViewModel
        {
        };


        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> Add(EntryAddViewModel model)
    {
        if (ModelState.IsValid)
        {
            var entry = new Entry
            {
                DriverId = model.DriverId,
                Boarded = model.Boarded,
                LeftBehind = model.LeftBehind,
                BusId = model.BusId,
                StopId = model.StopId,
                LoopId = model.LoopId,
                Timestamp = model.Date.ToDateTime(model.Time),
            };
            try
            {
                await _entryRepository.Add(entry);
                _logger.LogInformation("Entry created for loop {loop} for stop {stop} on bus {bus} at {time}.", entry.LoopId, entry.StopId, entry.BusId, DateTime.Now);
            }
            catch
            {
                _logger.LogError("Entry failed creation for loop {loop} for stop {stop} on bus {bus} at {time}.", entry.LoopId, entry.StopId, entry.BusId, DateTime.Now);
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        return View(model);
    }

    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _entryRepository.Get(id);

        if (model == null)
        {
            _logger.LogWarning("Failed to find entry with ID {id} validation at {time}.", id, DateTime.Now);
            return NotFound();
        }

        ViewBag.AvailableLoops = await GetAvailableLoops();
        ViewBag.AvailableBusses = await GetAvailableBusses();
        ViewBag.AvailableStops = await GetAvailableStops();

        var viewModel = new EntryEditViewModel
        {
            Id = model.Id,
            Boarded = model.Boarded,
            LeftBehind = model.LeftBehind,
            BusId = model.BusId,
            StopId = model.StopId,
            LoopId = model.LoopId,
            Time = TimeOnly.FromDateTime(model.Timestamp),
            Date = DateOnly.FromDateTime(model.Timestamp)
        };


        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> Edit(EntryEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var entry = new Entry
            {
                Id = model.Id,
                Boarded = model.Boarded,
                LeftBehind = model.LeftBehind,
                BusId = model.BusId,
                StopId = model.StopId,
                LoopId = model.LoopId,
                Timestamp = model.Date.ToDateTime(model.Time),
            };
            try
            {
                await _entryRepository.Update(entry);
                _logger.LogInformation("Entry {id} updated for loop {loop} for stop {stop} on bus {bus} at {time}.", entry.Id, entry.LoopId, entry.StopId, entry.BusId, DateTime.Now);
            }
            catch
            {
                _logger.LogError("Entry {id} failed creation for loop {loop} for stop {stop} on bus {bus} at {time}.", entry.Id, entry.LoopId, entry.StopId, entry.BusId, DateTime.Now);
                return NotFound();
            }
            return RedirectToAction("Index");
        }
        _logger.LogError("Failed entry edit validation at {time}.", DateTime.Now);
        return View(model);
    }

    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _entryRepository.Get(id);

        if (model == null)
        {
            _logger.LogWarning("Entry to delete not found with ID {id} at {time}.", id, DateTime.Now);
            return NotFound();
        }

        ViewBag.Time = model.Timestamp.ToString("dd MMMM yyyy HH:mm:ss");

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            await _entryRepository.Delete(id);
            _logger.LogInformation("Deleted entry with ID {id} at {time}.", id, DateTime.Now);
        }
        catch
        {
            _logger.LogError("Deleting entry with ID {id} failed at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
