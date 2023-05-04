using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using View.Models;

namespace View.Controllers;
[Authorize(Policy = "ManagerOnly")]
public class LoopController : Controller
{
    private readonly ILoopRepository _loopRepository;
    private readonly IStopRepository _stopRepository;
    private readonly IEntryRepository _entryRepository;
    private readonly ILogger<LoopController> _logger;

    public LoopController(ILoopRepository loopRepository, IStopRepository stopRepository, IEntryRepository entryRepository, ILogger<LoopController> logger)
    {
        _loopRepository = loopRepository;
        _stopRepository = stopRepository;
        _entryRepository = entryRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string loopId, string filterByVisits)
    {
        var stops = await _stopRepository.Get();
        var loops = await _loopRepository.Get();
        var entries = await _entryRepository.Get();
        var stopsForMap = new List<Stop>();

        var loopIds = loops.Select(l => l.Id);
        
        List<SelectListItem> loopSelectList = new SelectList(loops, "Id", "Name").ToList();
        loopSelectList.Insert(0, new SelectListItem
        {
            Text = "",
            Value = "",
            Selected = true
        });
        ViewBag.AvailableLoops = loopSelectList;

        stopsForMap = stops;

        if (!string.IsNullOrEmpty(loopId) && loopIds.Contains(int.Parse(loopId)))
        {
            var loopToFilterTo = loops.Where(l => l.Id == int.Parse(loopId)).First();

            stopsForMap = loopToFilterTo.Routes.Select(r => r.Stop).ToList();

            loopSelectList.Where(l => l.Value == loopId).First().Selected = true;
        }

        if (!string.IsNullOrEmpty(filterByVisits))
        {
            if(entries.Count < 1)
            {
                stopsForMap = new List<Stop>();
            } else
            {
                var stopsWithManyPeople = entries.OrderByDescending(e => e.Boarded).Select(e => e.Stop).Take(5).ToList();
                stopsForMap = stopsWithManyPeople.Join(stopsForMap, s => s.Id, s2 => s2.Id, (s, s2) => s).ToList();
            }

        }

        ViewBag.StopsForMap = stopsForMap;

        return View(loops);
    }

    public IActionResult Create()
    {
        return View(new LoopViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(LoopViewModel model)
    {
        if (ModelState.IsValid)
        {
            var loop = new Loop
            {
                Name = model.Name
            };
            await _loopRepository.Add(loop);
            _logger.LogInformation("Created new loop with ID {id}, Name {name} at {time}.", model.Id, model.Name, DateTime.Now);
            return RedirectToAction("Index");
        }
        _logger.LogError("Failed loop creation validation at {time}.", DateTime.Now);
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _loopRepository.Get(id);

        if (model == null)
        {
            _logger.LogWarning("Loop to update not found with ID {id} at {time}.", id, DateTime.Now);
            return NotFound();
        }

        var viewModel = new LoopViewModel
        {
            Id = model.Id,
            Name = model.Name,
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LoopViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var loop = new Loop
                {
                    Id = model.Id,
                    Name = model.Name
                };
                await _loopRepository.Update(loop);
                _logger.LogInformation("Updated loop with ID {id} at {time}.", model.Id, DateTime.Now);
            }
            catch
            {
                _logger.LogError("Updating loop with ID {id} failed at {time}.", model.Id, DateTime.Now);
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var model = await _loopRepository.Get(id);

        if (model == null)
        {
            _logger.LogWarning("Loop to delete not found with ID {id} at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            await _loopRepository.Delete(id);
            _logger.LogInformation("Deleted bus with ID {id} at {time}.", id, DateTime.Now);
        }
        catch
        {
            _logger.LogError("Deleting bus with ID {id} failed at {time}.", id, DateTime.Now);
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
